using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Crewmate;
public sealed class Spy : RoleBase, IKillFlashSeeable, IDeathReasonSeeable
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
            null,
            "ba",
            "#00f7ff"
        );
    public Spy(PlayerControl player) : base(RoleInfo, player)
    {
    }

    public bool CheckKillFlash(MurderInfo info) => true;
    public bool CheckSeeDeathReason(PlayerControl seen) => true;
}