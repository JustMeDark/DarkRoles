using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Vanilla;

public sealed class Impostor : RoleBase, IImpostor
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.CreateForVanilla(
            typeof(Impostor),
            player => new Impostor(player),
            RoleTypes.Impostor
        );
    public Impostor(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    { }
}