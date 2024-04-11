using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using static DarkRoles.Options;

namespace DarkRoles.Roles.Madmate;
public sealed class MadJester : RoleBase, IKillFlashSeeable
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(MadJester),
            player => new  MadJester(player),
            CustomRoles.MadJester,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Madmate,
            30300,
            null,
            "mj",
            introSound: () => GetIntroSound(RoleTypes.Impostor),
            assignInfo: new RoleAssignInfo(CustomRoles.MadJester, CustomRoleTypes.Madmate)
            {
                AssignCountRule = new(1, 1, 1)
            }
        );
    public MadJester(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        FieldCanSeeKillFlash = MadmateCanSeeKillFlash.GetBool();
    }
   
    private static bool FieldCanSeeKillFlash;

    public override void OnExileWrapUp(GameData.PlayerInfo exiled, ref bool DecidedWinner)
    {
        if (!AmongUsClient.Instance.AmHost || Player.PlayerId != exiled.PlayerId) return;

        CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Impostor);
        CustomWinnerHolder.WinnerIds.Add(exiled.PlayerId);
        CustomWinnerHolder.SetWinnerOrAdditonalWinner(CustomWinner.Impostor);
        DecidedWinner = true;
    }

    public bool CheckKillFlash(MurderInfo info) => FieldCanSeeKillFlash;
}