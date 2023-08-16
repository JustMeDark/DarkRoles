using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using Epic.OnlineServices;
using Hazel;
using TMPro;
using TownOfHost.Roles.Core;
using UnityEngine;

namespace TownOfHost.Roles.Neutral
{
    public sealed class Pelican : RoleBase
    {

        private static OptionItem OptionKillCooldown;
        public static OptionItem OptionCanVent;
        private static OptionItem OptionHasImpostorVision;
        private static float KillCooldown;
        public static bool CanVent;
        private static bool HasImpostorVision;
        private static Dictionary<byte, List<byte>> eatenList = new();
        private static List<byte> playerIdList = new();
        private static readonly Dictionary<byte, float> originalSpeed = new();

        public static readonly SimpleRoleInfo RoleInfo =
         SimpleRoleInfo.Create(
             typeof(Pelican),
             player => new Pelican(player),
             CustomRoles.Pelican,
             () => RoleTypes.Crewmate,
             CustomRoleTypes.Neutral,
             60000,
             SetupOptionItem,
             "pel",
             "#7edeab"
         );

        public Pelican(PlayerControl player) : base(RoleInfo, player, () => HasTask.False)
        {
            KillCooldown = OptionKillCooldown.GetFloat();
            CanVent = OptionCanVent.GetBool();
            HasImpostorVision = OptionHasImpostorVision.GetBool();
        }

        public override void Add()
        {
            foreach (var ar in Main.AllPlayerControls)
                playerIdList.Add(ar.PlayerId);
        }

        public static void SendRPC(byte targetId = byte.MaxValue)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetEatenPlayer, SendOption.Reliable, -1);
            writer.Write(targetId);
            if (targetId != byte.MaxValue)
            {
                writer.Write(eatenList[targetId].Count);
                foreach (var el in eatenList[targetId])
                    writer.Write(el);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void ReceiveRPC(MessageReader reader, CustomRPC rpcType)
        {
            byte playerId = reader.ReadByte();
            if (playerId == byte.MaxValue)
            {
                eatenList.Clear();
            }
            else
            {
                int eatenNum = reader.ReadInt32();
                eatenList.Remove(playerId);
                List<byte> list = new();
                for (int i = 0; i < eatenNum; i++)
                    list.Add(reader.ReadByte());
                eatenList.Add(playerId, list);
            }
        }

        public static bool IsEaten(PlayerControl pc, byte id) => eatenList.ContainsKey(pc.PlayerId) && eatenList[pc.PlayerId].Contains(id);
        public static bool IsEaten(byte id)
        {
            foreach (var el in eatenList)
                if (el.Value.Contains(id))
                    return true;
            return false;
        }

        public static Vector2 GetBlackRoomPS()
        {
            return Main.NormalOptions.MapId switch
            {
                0 => new(-27f, 3.3f), // The Skeld
                1 => new(-11.4f, 8.2f), // MIRA HQ
                2 => new(42.6f, -19.9f), // Polus
                4 => new(-16.8f, -6.2f), // Airship
                _ => throw new System.NotImplementedException(),
            };
        }

        public static bool IsEnable => playerIdList.Count > 0;

        private static void SyncEatenList(byte playerId)
        {
            SendRPC(byte.MaxValue);
            foreach (var el in eatenList)
                SendRPC(el.Key);
        }

        public static bool CanEat(PlayerControl pc, byte id)
        {
            if (!pc.Is(CustomRoles.Pelican) || GameStates.IsMeeting) return false;
            var target = Utils.GetPlayerById(id);
            return target != null && target.IsAlive() && !target.inVent && !target.Is(CustomRoles.GM) && !IsEaten(pc, id) && !IsEaten(id);
        }

        public static string GetProgressText(byte playerId)
        {
            var player = Utils.GetPlayerById(playerId);
            if (player == null) return "Invalid";
            var eatenNum = 0;
            if (eatenList.ContainsKey(playerId))
                eatenNum = eatenList[playerId].Count;
            return Utils.ColorString(eatenNum < 1 ? Color.gray : Utils.GetRoleColor(CustomRoles.Pelican), $"({eatenNum})");
        }
        public static void EatPlayer(PlayerControl pc, PlayerControl target)
        {
            if (pc == null || target == null || !CanEat(pc, target.PlayerId)) return;
            if (!eatenList.ContainsKey(pc.PlayerId)) eatenList.Add(pc.PlayerId, new());
            eatenList[pc.PlayerId].Add(target.PlayerId);

            SyncEatenList(pc.PlayerId);

            originalSpeed.Remove(target.PlayerId);
            originalSpeed.Add(target.PlayerId, Main.AllPlayerSpeed[target.PlayerId]);

            Utils.TP(target.NetTransform, GetBlackRoomPS());
            Main.AllPlayerSpeed[target.PlayerId] = 0.5f;
            ReportDeadBodyPatch.CanReport[target.PlayerId] = false;
            target.MarkDirtySettings();

            Utils.NotifyRoles(SpecifySeer: pc);
            Utils.NotifyRoles(SpecifySeer: target);
            Logger.Info($"{pc.GetRealName()} 吞掉了 {target.GetRealName()}", "Pelican");
        }

        public static void OnReportDeadBody()
        {
            foreach (var pc in eatenList)
            {
                foreach (var tar in pc.Value)
                {
                    var target = Utils.GetPlayerById(tar);
                    var killer = Utils.GetPlayerById(pc.Key);
                    if (killer == null || target == null) continue;
                    Main.AllPlayerSpeed[tar] = Main.AllPlayerSpeed[tar] - 0.5f + originalSpeed[tar];
                    ReportDeadBodyPatch.CanReport[tar] = true;
                    target.RpcExileV2();
                    target.SetRealKiller(killer);
                    PlayerState.GetByPlayerId(target.PlayerId).DeathReason = CustomDeathReason.Bite;
                    Main.PlayerStates[tar].SetDead();
                    Logger.Info($"{killer.GetRealName()} 消化了 {target.GetRealName()}", "Pelican");
                }
            }
            eatenList.Clear();
            SyncEatenList(byte.MaxValue);
        }

        public static void OnPelicanDied(byte pc)
        {
            if (!eatenList.ContainsKey(pc)) return;
            foreach (var tar in eatenList[pc])
            {
                var target = Utils.GetPlayerById(tar);
                var palyer = Utils.GetPlayerById(pc);
                if (palyer == null || target == null) continue;
                Utils.TP(target.NetTransform, palyer.GetTruePosition());
                Main.AllPlayerSpeed[tar] = Main.AllPlayerSpeed[tar] - 0.5f + originalSpeed[tar];
                ReportDeadBodyPatch.CanReport[tar] = true;
                target.MarkDirtySettings();
                RPC.PlaySoundRPC(tar, Sounds.TaskComplete);
                Utils.NotifyRoles(SpecifySeer: target);
                Logger.Info($"{Utils.GetPlayerById(pc).GetRealName()} 吐出了 {target.GetRealName()}", "Pelican");
            }
            eatenList.Remove(pc);
            SyncEatenList(pc);
        }

        private static int Count = 0;
        public override void OnFixedUpdate(PlayerControl player)
        {
            if (!GameStates.IsInTask)
            {
                if (eatenList.Count > 0)
                {
                    eatenList.Clear();
                    SyncEatenList(byte.MaxValue);
                }
                return;
            }

            if (!IsEnable) return; Count--; if (Count > 0) return; Count = 30;

            foreach (var pc in eatenList)
            {
                foreach (var tar in pc.Value)
                {
                    var target = Utils.GetPlayerById(tar);
                    if (target == null) continue;
                    var pos = GetBlackRoomPS();
                    var dis = Vector2.Distance(pos, target.GetTruePosition());
                    if (dis < 1f) continue;
                    Utils.TP(target.NetTransform, pos);
                    Utils.NotifyRoles(SpecifySeer: target);
                }
            }
        }

            public float CalculateKillCooldown() => KillCooldown;
        public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(HasImpostorVision);

        private static void SetupOptionItem()
        {
            OptionKillCooldown = FloatOptionItem.Create(RoleInfo, 10, GeneralOption.KillCooldown, new(2.5f, 180f, 2.5f), 20f, false)
                 .SetValueFormat(OptionFormat.Seconds);
            OptionCanVent = BooleanOptionItem.Create(RoleInfo, 11, GeneralOption.CanVent, true, false);
            OptionHasImpostorVision = BooleanOptionItem.Create(RoleInfo, 13, GeneralOption.ImpostorVision, true, false);
        }
    }
}
