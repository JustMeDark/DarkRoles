using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Neutral
{
    public sealed class Jackal : RoleBase, IKiller, ISchrodingerCatOwner
    {
        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(Jackal),
                player => new Jackal(player),
                CustomRoles.Jackal,
                () => RoleTypes.Impostor,
                CustomRoleTypes.Neutral,
                10300,
                SetupOptionItem,
                "jac",
                "#00b4eb",
                true,
                countType: CountTypes.Jackal,
                assignInfo: new RoleAssignInfo(CustomRoles.Jackal, CustomRoleTypes.Neutral)
                {
                    AssignCountRule = new(1, 1, 1)
                }
            );
        public Jackal(PlayerControl player)
        : base(
            RoleInfo,
            player,
            () => HasTask.False
        )
        {
            KillCooldown = OptionKillCooldown.GetFloat();
            CanVent = OptionCanVent.GetBool();
            CanUseSabotage = OptionCanUseSabotage.GetBool();
            HasImpostorVision = OptionHasImpostorVision.GetBool();
        }

        private static OptionItem OptionKillCooldown;
        public static OptionItem OptionCanVent;
        public static OptionItem OptionCanUseSabotage;
        private static OptionItem OptionHasImpostorVision;
        private static float KillCooldown;
        public static bool CanVent;
        public static bool CanUseSabotage;
        private static bool HasImpostorVision;

        public SchrodingerCat.TeamType SchrodingerCatChangeTo => SchrodingerCat.TeamType.Jackal;

        private static void SetupOptionItem()
        {
            OptionKillCooldown = FloatOptionItem.Create(RoleInfo, 10301, GeneralOption.KillCooldown, new(2.5f, 180f, 2.5f), 30f, false)
                .SetValueFormat(OptionFormat.Seconds);
            OptionCanVent = BooleanOptionItem.Create(RoleInfo, 10302, GeneralOption.CanVent, true, false);
            OptionCanUseSabotage = BooleanOptionItem.Create(RoleInfo, 10303, GeneralOption.CanUseSabotage, false, false);
            OptionHasImpostorVision = BooleanOptionItem.Create(RoleInfo, 10304, GeneralOption.ImpostorVision, true, false);
        }
        public float CalculateKillCooldown() => KillCooldown;
        public bool CanUseSabotageButton() => CanUseSabotage;
        public bool CanUseImpostorVentButton() => CanVent;
        public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(HasImpostorVision);
        public void ApplySchrodingerCatOptions(IGameOptions option) => ApplyGameOptions(option);
    }
}