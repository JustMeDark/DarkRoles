using AmongUs.GameOptions;
using DarkRoles.Modules;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Impostor
{
    public class Grappler : RoleBase, IImpostor
    {
        public static OptionItem FailChance, GrappleCooldown, ShiftDuration;

        public static readonly SimpleRoleInfo RoleInfo =
           SimpleRoleInfo.Create(typeof(Grappler),
               player => new Grappler(player),
               CustomRoles.Grappler,
               () => RoleTypes.Shapeshifter,
               CustomRoleTypes.Impostor,
               21800, SetupOptionItem, "gr");

        public Grappler(PlayerControl player) : base(RoleInfo, player) { }

        private static void SetupOptionItem()
        {
            FailChance = IntegerOptionItem.Create(RoleInfo, 21801, "GrapplerFailChance", new(0, 100, 5), 15, false)
            .SetValueFormat(OptionFormat.Percent);
            GrappleCooldown = FloatOptionItem.Create(RoleInfo, 21802, "GrapplerGrappleCooldown", new(0f, 90f, 2.5f), 10f, false)
            .SetValueFormat(OptionFormat.Seconds);
            ShiftDuration = FloatOptionItem.Create(RoleInfo, 21803, "GrapplerShiftDuration", new(0f, 90f, 2.5f), 30f, false)
            .SetValueFormat(OptionFormat.Seconds);
        }

        public override void OnShapeshift(PlayerControl player, PlayerControl target, bool shapeshifting)
        {
            if (shapeshifting)
            {
                NameNotifyManager.Notify(target, Translator.GetString("Grappling"));
                _ = new LateTask(() =>
                    {
                        if (player.IsAlive() && target.IsAlive() && !player.inVent && !target.inVent)
                            target.RpcTeleport(player.transform.position);
                        else
                            NameNotifyManager.Notify(player, Translator.GetString("GrapplingFailed"));
                    }, 1.5f, "Grappler TP");
            }
        }
    }
}
