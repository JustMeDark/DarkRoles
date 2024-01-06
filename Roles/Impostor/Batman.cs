using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using DarkRoles.Roles.Crewmate;

namespace DarkRoles.Roles.Impostor
{
    public class Batman : RoleBase, IImpostor
    {
        public static OptionItem DarkSpeed, KillCooldown;
        public float originalSpeed, originalKillCooldown;
        public bool CanKill;

        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(typeof(Batman),
                player => new Batman(player),
                CustomRoles.Batman,
                () => RoleTypes.Impostor,
                CustomRoleTypes.Impostor,
                21900, SetupOptionItem, "bm");

        public Batman(PlayerControl player) : base(RoleInfo, player)
        {
            originalSpeed = AURoleOptions.PlayerSpeedMod;
            originalKillCooldown = AURoleOptions.KillCooldown;
        }

        private static void SetupOptionItem()
        {
            DarkSpeed = FloatOptionItem.Create(RoleInfo, 21901, "BatmanSpeed", new(0f, 5f, 0.25f), 2.5f, false);
            KillCooldown = FloatOptionItem.Create(RoleInfo, 21902, "BatmanKillCooldown", new(0f, 180f, 2.5f), 7.5f, false);
        }

        public override void OnFixedUpdate(PlayerControl player) => Main.AllPlayerSpeed[player.PlayerId] = Utils.IsActive(SystemTypes.Electrical) ? DarkSpeed.GetFloat() : originalSpeed;

        public float CalculateKillCooldown() => CanKill ? KillCooldown.GetFloat() : Options.DefaultKillCooldown;

        public bool CanUseKillButton() => true;

        public override bool OnSabotage(PlayerControl player, SystemTypes systemType)
        {
            CanKill = systemType == SystemTypes.Electrical;
            return true;
        }
    }
}
