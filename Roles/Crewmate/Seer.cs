using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Crewmate;
public sealed class Seer : RoleBase, IKillFlashSeeable
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Seer),
            player => new Seer(player),
            CustomRoles.Seer,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            1600,
            null,
            "se",
            "#61b26c"
        );
    public Seer(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    { }
}