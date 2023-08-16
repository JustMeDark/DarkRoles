using AmongUs.GameOptions;

using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Vanilla;

public sealed class Engineer : RoleBase
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.CreateForVanilla(
            typeof(Engineer),
            player => new Engineer(player),
            RoleTypes.Engineer,
            "#8cffff"
        );
    public Engineer(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    { }

    public override string GetAbilityButtonText() => StringNames.VentAbility.ToString();
}