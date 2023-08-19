using AmongUs.GameOptions;
using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class Torch : RoleBase
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Torch),
            player => new Torch(player),
            CustomRoles.Torch,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            20100,
            null,
            "to",
            "#eee5be"
        );
    public Torch(PlayerControl player) : base(RoleInfo, player) { }


    public override void ApplyGameOptions(IGameOptions opt)
    {
        var crewLightMod = FloatOptionNames.CrewLightMod;
        opt.SetFloat(crewLightMod, 5.0f);
    }
}