using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using TheDarkRoles.Roles.Core.Interfaces;
using TheDarkRoles.Roles.Core;
using static TheDarkRoles.Options;

namespace TheDarkRoles.Roles.Madmate
{
    public sealed class MadJester : RoleBase, IKillFlashSeeable
    {
        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(MadJester),
                player => new MadJester(player),
                CustomRoles.MadJester,
                () => RoleTypes.Crewmate,
                CustomRoleTypes.Madmate,
                14000,
                null,
                "mj",
                introSound: () => GetIntroSound(RoleTypes.Impostor)
            );
        public MadJester(PlayerControl player)
        : base(
            RoleInfo,
            player
        )
        {
            FieldCanSeeKillFlash = MadmateCanSeeKillFlash.GetBool();
        }

        private static bool FieldCanSeeKillFlash;

        public override void OnFixedUpdate(PlayerControl player)
        {
            foreach (var pc in Main.AllAlivePlayerControls)
            {
                if (pc.Is(CustomRoleTypes.Impostor))
                {
                    NameColorManager.Add(PlayerControl.LocalPlayer.PlayerId, pc.PlayerId, "#FF0000");
                }
            }
        }

        public override void OnExileWrapUp(GameData.PlayerInfo exiled, ref bool DecidedWinner)
        {
            if (!AmongUsClient.Instance.AmHost || Player.PlayerId != exiled.PlayerId) return;

            CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Impostor);
            CustomWinnerHolder.WinnerIds.Add(exiled.PlayerId);
            CustomWinnerHolder.SetWinnerOrAdditonalWinner(CustomWinner.Impostor);
            DecidedWinner = true;
        }

        public bool CheckKillFlash(MurderInfo info) => FieldCanSeeKillFlash;
    }
}