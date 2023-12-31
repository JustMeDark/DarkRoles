using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Crewmate
{
    public class Spy : RoleBase, IKillFlashSeeable, IDeathReasonSeeable
    {
        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(typeof(Spy),
                player => new Spy(player),
                CustomRoles.Spy,
                () => RoleTypes.Engineer,
                CustomRoleTypes.Crewmate,
                2000, SetupOptionItem,
                "sp", "#9CCDFF");

        public static OptionItem VentCooldown;
        public static OptionItem VentDuration;

        public Spy(PlayerControl player) : base(RoleInfo, player) { }

        private static void SetupOptionItem()
        {
            VentCooldown = FloatOptionItem.Create(RoleInfo, 2001, "SpyVentCooldown", new(0f, 90f, 2.5f), 10f, false)
            .SetValueFormat(OptionFormat.Seconds);
            VentDuration = FloatOptionItem.Create(RoleInfo, 2002, "SpyVentDuration", new(0f, 90f, 2.5f), 15f, false)
            .SetValueFormat(OptionFormat.Seconds);
        }

        public override void ApplyGameOptions(IGameOptions opt)
        {
            AURoleOptions.EngineerCooldown = VentCooldown.GetFloat();
            AURoleOptions.EngineerInVentMaxTime = VentDuration.GetFloat();
            opt.SetBool(BoolOptionNames.AnonymousVotes, true);
        }
    }
}
