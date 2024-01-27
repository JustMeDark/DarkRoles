using System.Collections.Generic;
using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using DarkRoles.Roles.Impostor;
using Hazel;
using TMPro;
using UnityEngine;

namespace DarkRoles.Roles.Neutral
{
    public sealed class Agent47 : RoleBase, IKiller
    {
        private static Dictionary<byte, bool> MarkedPlayers = [];
        private static OptionItem OptionMarkCooldown;
        private static int Marked;

        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(Agent47),
                player => new Agent47(player),
                CustomRoles.Agent47,
                () => RoleTypes.Impostor,
                CustomRoleTypes.Neutral,
                10900,
                SetupOptionItem,
                "47",
                "#823026",
                true,
                countType: CountTypes.Agent47,
                assignInfo: new RoleAssignInfo(CustomRoles.Agent47, CustomRoleTypes.Neutral) { AssignCountRule = new(1, 1, 1) });

        public Agent47(PlayerControl player) : base(RoleInfo, player, () => HasTask.False) => MarkedPlayers.Clear();

        public override void Add()
        {
            foreach (var ar in Main.AllPlayerControls)
                MarkedPlayers.Add(ar.PlayerId, false);
            Marked = 0;
        }
        private static void SetupOptionItem() => OptionMarkCooldown = FloatOptionItem.Create(RoleInfo, 10, "47MarkCooldown", new(2.5f, 180f, 2.5f), 25f, false).SetValueFormat(OptionFormat.Seconds);
        public float CalculateKillCooldown() => OptionMarkCooldown.GetFloat();
        public bool CanUseSabotageButton() => false;
        public bool CanUseImpostorVentButton() => false;
        public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(true);
        public override void OnMurderPlayerAsTarget(MurderInfo info) => MarkedPlayers.Clear();

        private void SendRPC()
        {
            using var sender = CreateSender(CustomRPC.SetMarkedPlayers);
            sender.Writer.Write(Marked);
        }
        public override void ReceiveRPC(MessageReader reader, CustomRPC rpcType)
        {
            if (rpcType != CustomRPC.SetMarkedPlayers) return;
            Marked = reader.ReadInt32();
        }

        public void OnCheckMurderAsKiller(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            killer.SetKillCooldown(OptionMarkCooldown.GetFloat());
            if (MarkedPlayers[target.PlayerId] == false && target is not null)
            {
                MarkedPlayers[target.PlayerId] = true;
                Marked++;
                SendRPC();
            }
            info.DoKill = false;
            return;
        }

        public override void OnStartMeeting()
        {
            foreach (var pc in Main.AllAlivePlayerControls)
                if (pc.PlayerId != Player.PlayerId)
                    if (MarkedPlayers.TryGetValue(pc.PlayerId, out var isMarked) && isMarked)
                    {
                        pc.SetRealKiller(Player);
                        pc.RpcMurderPlayer(pc);
                        var state = PlayerState.GetByPlayerId(pc.PlayerId);
                        state.DeathReason = CustomDeathReason.Hit;
                        state.SetDead();
                        MarkedPlayers.Clear();
                        Marked = MarkedPlayers.Count;
                        SendRPC();
                    }
            return;
        }

        public override string GetProgressText(bool comms = false) => Utils.ColorString(Color.gray, $"({Marked})");
    }
}