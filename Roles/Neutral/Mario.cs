using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using UnityEngine;

namespace DarkRoles.Roles.Neutral;
public sealed class Mario : RoleBase
{
    public static OptionItem OptionVentAmount;
    private int VentAmount;
    private int UsedVents;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Mario),
            player => new Mario(player),
            CustomRoles.Mario,
            () => RoleTypes.Engineer,
            CustomRoleTypes.Neutral,
            50000,
            SetupOptionItem,
            "ma",
            "#f54702"
        );

    public Mario(PlayerControl player) : base(RoleInfo, player)
    {
        VentAmount = OptionVentAmount.GetInt();
        UsedVents = 0;
    }

    enum OptionName
    {
        MarioVentAmount,
    }

    public static void SetupOptionItem()
    {
        OptionVentAmount = IntegerOptionItem.Create(RoleInfo, 10, OptionName.MarioVentAmount, new(0, 100, 5), 50, false)
            .SetValueFormat(OptionFormat.Times);
    }

    public override bool OnEnterVent(PlayerPhysics physics, int ventId)
    {
        if(UsedVents < VentAmount)
        {
            UsedVents++;
        }
        if(UsedVents == VentAmount)
        {
            CustomWinnerHolder.ShiftWinnerAndSetWinner(CustomWinner.Mario);
            CustomWinnerHolder.WinnerIds.Add(Player.PlayerId);
            return true;
        }
        return false;
    }

    public override void ApplyGameOptions(IGameOptions opt)
    {
        var vent = FloatOptionNames.EngineerCooldown;
        opt.SetFloat(vent, 0.0f);
    }

    public override string GetProgressText(bool comms = false) => Utils.ColorString(Color.red, $"({VentAmount - UsedVents})");
}