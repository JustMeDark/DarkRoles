using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Vanilla;

public sealed class Shapeshifter : RoleBase, IImpostor, IKiller, ISidekickable
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.CreateForVanilla(
            typeof(Shapeshifter),
            player => new Shapeshifter(player),
            RoleTypes.Shapeshifter,
            canMakeMadmate: true
        );
    public Shapeshifter(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    { }
}
