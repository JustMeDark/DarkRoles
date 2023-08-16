using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AmongUs.GameOptions;

using TownOfHost.Roles.Core;
using TownOfHost.Roles.Core.Interfaces;
using static TownOfHost.Translator;

namespace TownOfHost.Roles.Neutral
{
    public sealed class Poisoner : RoleBase, IKiller
    {
        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(Poisoner),
                player => new Poisoner(player),
                CustomRoles.Poisoner,
                () => RoleTypes.Impostor,
                CustomRoleTypes.Neutral,
                1300,
                SetupOptionItem,
                "poi",
                "#653782",
                introSound: () => GetIntroSound(RoleTypes.Shapeshifter)
            );
        public Poisoner(PlayerControl player)
        : base(
            RoleInfo,
            player
        )
        {
            KillDelay = OptionKillDelay.GetFloat();

            PoisonedPlayers.Clear();
        }

        static OptionItem OptionKillDelay;
        enum OptionName
        {
            VampireKillDelay
        }

        static float KillDelay;

        public bool CanBeLastImpostor { get; } = false;
        Dictionary<byte, float> PoisonedPlayers = new(14);

        private static void SetupOptionItem()
        {
            OptionKillDelay = FloatOptionItem.Create(RoleInfo, 10, OptionName.VampireKillDelay, new(1f, 1000f, 1f), 10f, false)
                .SetValueFormat(OptionFormat.Seconds);
        }
        public void OnCheckMurderAsKiller(MurderInfo info)
        {
            if (!info.CanKill) return; //キル出来ない相手には無効
            var (killer, target) = info.AttemptTuple;

            if (target.Is(CustomRoles.Bait)) return;
            if (info.IsFakeSuicide) return;

            //誰かに噛まれていなければ登録
            if (!PoisonedPlayers.ContainsKey(target.PlayerId))
            {
                killer.SetKillCooldown();
                PoisonedPlayers.Add(target.PlayerId, 0f);
            }
            info.DoKill = false;
        }
        public override void OnFixedUpdate(PlayerControl _)
        {
            if (!AmongUsClient.Instance.AmHost || !GameStates.IsInTask) return;

            foreach (var (targetId, timer) in PoisonedPlayers.ToArray())
            {
                if (timer >= KillDelay)
                {
                    var target = Utils.GetPlayerById(targetId);
                    KillPoisoned(target);
                    PoisonedPlayers.Remove(targetId);
                }
                else
                {
                    PoisonedPlayers[targetId] += Time.fixedDeltaTime;
                }
            }
        }
        public override void OnReportDeadBody(PlayerControl _, GameData.PlayerInfo __)
        {
            foreach (var targetId in PoisonedPlayers.Keys)
            {
                var target = Utils.GetPlayerById(targetId);
                KillPoisoned(target, true);
            }
            PoisonedPlayers.Clear();
        }
        public bool OverrideKillButtonText(out string text)
        {
            text = GetString("PoisonerBiteButtonText");
            return true;
        }

        private void KillPoisoned(PlayerControl target, bool isButton = false)
        {
            var poisoner = Player;
            if (target.IsAlive())
            {
                PlayerState.GetByPlayerId(target.PlayerId).DeathReason = CustomDeathReason.Poisoned;
                target.SetRealKiller(poisoner);
                CustomRoleManager.OnCheckMurder(
                    poisoner, target,
                    target, target
                );
                Logger.Info($"Poisonerに噛まれている{target.name}を自爆させました。", "Poisoner.KillBitten");
                if (!isButton && poisoner.IsAlive())
                {
                    RPC.PlaySoundRPC(poisoner.PlayerId, Sounds.KillSound);
                }
            }
            else
            {
                Logger.Info($"Poisonerに噛まれている{target.name}はすでに死んでいました。", "Poisoner.KillBitten");
            }
        }
    }
}
