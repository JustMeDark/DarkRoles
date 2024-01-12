using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using DarkRoles.Roles.Impostor;

namespace DarkRoles.Roles.Crewmate;
public class Visionary : RoleBase, IKiller
{
    public static List<PlayerControl> SeenList = [];
    public static List<PlayerControl> OldList = [];
    public static OptionItem VisionCooldown;
    public static OptionItem VisionChance;
    public PlayerControl player;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(typeof(Visionary),
            player => new Visionary(player),
            CustomRoles.Visionary,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Crewmate,
            2400, SetupOptionItem,
            "vis", "#19ff30", true);

    public Visionary(PlayerControl player) : base(RoleInfo, player) { this.player = player; SeenList.Clear(); }

    public static void SetupOptionItem()
    {
        VisionCooldown = FloatOptionItem.Create(RoleInfo, 2410, "VisionaryCooldown", new(0f, 100f, 2.5f), 22.5f, false);
        VisionChance = IntegerOptionItem.Create(RoleInfo, 2420, "VisionaryChance", new(0, 100, 5), 40, false).SetValueFormat(OptionFormat.Percent);
    }
    public bool CanUseSabotageButton() => false;
    public bool CanUseKillButton() => true;
    public float CalculateKillCooldown() => VisionCooldown.GetFloat();

    public override void OnStartMeeting()
    {
        var rd = IRandom.Instance;
        foreach (var pc in SeenList)
            if (rd.Next(0, 100) > VisionChance.GetInt())
                Utils.SendMessage($"<b>Your vision tells you that {pc.name}'s role is {pc.GetCustomRole()}!</b>", Player.PlayerId);
            else
                Utils.SendMessage("Sorry looks like your vision faled. Womp Womp.", Player.PlayerId);
    }

    public override void AfterMeetingTasks()
    {
        foreach (var pc in SeenList)
            OldList.Add(pc);
        SeenList.Clear();
    }

    public void OnCheckMurderAsKiller(MurderInfo info)
    {
        var (killer, target) = info.AttemptTuple;
        killer.SetKillCooldown(VisionCooldown.GetFloat());
        if (!SeenList.Contains(target) && target != null)
        {
            SeenList.Add(target);
            info.DoKill = false;
        }
    }
}
