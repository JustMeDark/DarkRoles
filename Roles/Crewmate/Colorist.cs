using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Crewmate;
public sealed class Colorist : RoleBase
{
    public static OptionItem OptionAbilityUses;
    private int AbilityUses;
    public static int UsesLeft;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Colorist),
            player => new Colorist(player),
            CustomRoles.Colorist,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            20100,
            null,
            "co",
            "#8f0d56"
        );

    public Colorist(PlayerControl player) : base(RoleInfo, player)
    {
        AbilityUses = UsesLeft = OptionAbilityUses.GetInt();
    }

    enum OptionName
    {
        ColoristAbilityUses,
    }

    public static void SetupOptionItem()
    {
        OptionAbilityUses = IntegerOptionItem.Create(RoleInfo, 10, OptionName.ColoristAbilityUses, new(0, 99, 1), 3, false)
            .SetValueFormat(OptionFormat.Times);
    }

    public override void OnStartMeeting()
    {
        if (UsesLeft > 0) UsesLeft--;
    }
}