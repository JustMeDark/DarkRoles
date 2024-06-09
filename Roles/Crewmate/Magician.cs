using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using TheDarkRoles.Roles.Core;
using static TheDarkRoles.Translator;

namespace TheDarkRoles.Roles.Crewmate
{
    public class Magician : RoleBase
    {
        public static Dictionary<byte, bool> HasVented = new();
        public static OptionItem VentCooldown;

        public static readonly SimpleRoleInfo RoleInfo =
           SimpleRoleInfo.Create(typeof(Spy),
               player => new Magician(player),
               CustomRoles.Magician,
               () => RoleTypes.Engineer,
               CustomRoleTypes.Crewmate,
               12000, SetupOptionItem,
               "ma", "#E500F8");

        public Magician(PlayerControl player) : base(RoleInfo, player) { }

        private static void SetupOptionItem()
        {
            VentCooldown = FloatOptionItem.Create(RoleInfo, 12001, "MagicianVentCooldown", new(0f, 90f, 2.5f), 10f, false)
            .SetValueFormat(OptionFormat.Seconds);
        }

        public override void ApplyGameOptions(IGameOptions opt)
        {
            AURoleOptions.EngineerInVentMaxTime = 1;
            AURoleOptions.EngineerCooldown = VentCooldown.GetFloat();
        }

        public static void VentButtonText(HudManager __instance) => __instance.AbilityButton.OverrideText(GetString("MagicianVent"));

        public static void OnExitVent(PlayerControl pc) => pc.RpcRandomVentTeleport();
    }
}
