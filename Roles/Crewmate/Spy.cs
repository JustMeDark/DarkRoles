using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using AmongUs.GameOptions;
using TownOfHost.Roles.Core;

namespace TownOfHost.Roles.Crewmate;
public sealed class Spy : RoleBase
{
    //public static OptionItem OptionBaitReveal;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Bait),
            player => new Spy(player),
            CustomRoles.Spy,
            () => RoleTypes.Engineer,
            CustomRoleTypes.Crewmate,
            20100,
            SetupOptionItem,
            "ba",
            "#00f7ff"
        );
    public Spy(PlayerControl player) : base(RoleInfo, player)
    {
    }

    private static void SetupOptionItem()
    {
        //OptionBaitReveal = BooleanOptionItem.Create(10, "Reveal Bait", true, TabGroup.CrewmateRoles, false);
    }
}