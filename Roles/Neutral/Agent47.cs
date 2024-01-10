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
        private static List<PlayerControl> MarkedPlayers = [];
        private static OptionItem OptionMarkCooldown;
        private static int Marked = 0; //we dont need a dictionary because theres only 1 47 (yes moe i know youd comment on that)

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

        public override void Add() => Marked = MarkedPlayers.Count;
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
            if (!MarkedPlayers.Contains(target) && target != null)
            {
                MarkedPlayers.Add(target);
                Marked++;
                SendRPC();
            }
            info.DoKill = false;
        }

        public override void OnStartMeeting()
        {
            foreach (var pc in MarkedPlayers)
                if (pc != null && pc.PlayerId != Player.PlayerId)
                {
                    pc.SetRealKiller(Player);
                    pc.RpcMurderPlayer(pc);
                    var state = PlayerState.GetByPlayerId(pc.PlayerId);
                    state.DeathReason = CustomDeathReason.Hit;
                    state.SetDead();
                    MarkedPlayers.Clear();
                    Marked = 0;
                    SendRPC();
                }
        }

        public override string GetProgressText(bool comms = false) => Utils.ColorString(Color.gray, $"({Marked})");
    }
}