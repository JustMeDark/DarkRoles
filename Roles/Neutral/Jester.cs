using AmongUs.GameOptions;

using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Neutral;
public sealed class Jester : RoleBase
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Jester),
            player => new Jester(player),
            CustomRoles.Jester,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Neutral,
            10400,
            null,
            "je",
            "#ec62a5"
        );
    public Jester(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
    }
    public override void OnExileWrapUp(GameData.PlayerInfo exiled, ref bool DecidedWinner)
    {
        if (!AmongUsClient.Instance.AmHost || Player.PlayerId != exiled.PlayerId) return;

        CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Jester);
        CustomWinnerHolder.WinnerIds.Add(exiled.PlayerId);
        DecidedWinner = true;
    }
}