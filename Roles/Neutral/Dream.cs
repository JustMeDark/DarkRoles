using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using UnityEngine;

namespace DarkRoles.Roles.Neutral;
public sealed class Dream : RoleBase
{
    private static Options.OverrideTasksData tasks;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Dream),
            player => new Dream(player),
            CustomRoles.Dream,
            () => RoleTypes.Engineer,
            CustomRoleTypes.Neutral,
            50000,
            SetupOptionItem,
            "dr",
            "#16ba00"
        );

    public Dream(PlayerControl player) : base(RoleInfo, player, () => HasTask.ForRecompute) { }

    public static void SetupOptionItem()
    {
        tasks = Options.OverrideTasksData.Create(RoleInfo, 20);
    }

    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.EngineerCooldown = 0;
        AURoleOptions.EngineerInVentMaxTime = 0;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (IsTaskFinished)
        {
            CustomWinnerHolder.ShiftWinnerAndSetWinner(CustomWinner.Dream);
            CustomWinnerHolder.WinnerIds.Add(Player.PlayerId);
        }
    }

    public override bool OnCompleteTask()
    {
        if (IsTaskFinished)
        {
            CustomWinnerHolder.ShiftWinnerAndSetWinner(CustomWinner.Dream);
            CustomWinnerHolder.WinnerIds.Add(Player.PlayerId);
            return true;
        }
        return false;
    }
}