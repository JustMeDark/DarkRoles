using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class Bait : RoleBase
{
    public static List<byte> BaitAlive = new();
    public static OptionItem OptionBaitReveal;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Bait),
            player => new Bait(player),
            CustomRoles.Bait,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            20100,
            SetupOptionItem,
            "ba",
            "#00f7ff"
        );
    public Bait(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
    }

    public enum Options
    {
        RevealBait
    }

    private static void SetupOptionItem()
    {
        OptionBaitReveal = BooleanOptionItem.Create(RoleInfo, 10, Options.RevealBait, true, false);
    }

    public override void OnStartMeeting()
    {
        if((MeetingStates.FirstMeeting && OptionBaitReveal.GetBool()) && CustomRoles.Bait.RoleExist())
        {
            foreach (var pc in Main.AllAlivePlayerControls.Where(x => x.Is(CustomRoles.Bait)))
                BaitAlive.Add(pc.PlayerId);
            List<string> baitAliveList = new();
            foreach (var whId in BaitAlive)
                baitAliveList.Add(Main.AllPlayerNames[whId]);
            Utils.SendMessage("The bait is " + baitAliveList);
        }
    }

    public override void OnMurderPlayerAsTarget(MurderInfo info)
    {
        var (killer, target) = info.AttemptTuple;
        if (target.Is(CustomRoles.Bait) && !info.IsSuicide)
            _ = new LateTask(() => killer.CmdReportDeadBody(target.Data), 0.15f, "Bait Self Report");
    }
}