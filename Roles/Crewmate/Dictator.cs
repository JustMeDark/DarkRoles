using System.Collections.Generic;
using AmongUs.GameOptions;

using DarkRoles.Modules;
using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class Dictator : RoleBase
{
    public static Dictionary<byte, int> AbilityUses = new();
    public static OptionItem CaptainAbilityUses;
    public static OptionItem CaptainDies;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Dictator),
            player => new Dictator(player),
            CustomRoles.Dictator,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            1100,
            SetupCustomOption,
            "cap",
            "#d72bF0");

    public Dictator(PlayerControl player) : base(RoleInfo, player)
    {
        AbilityUses[player.PlayerId] = CaptainAbilityUses.GetInt();
    }
    public static void SetupCustomOption()
    {
        CaptainAbilityUses = IntegerOptionItem.Create(RoleInfo, 1101, "CaptainAbilityUses", new(0, 99, 1), 3, false)
           .SetValueFormat(OptionFormat.Times);
        CaptainDies = BooleanOptionItem.Create(RoleInfo, 1102, "CaptainCaptainDies", true, false);
    }
    public override (byte? votedForId, int? numVotes, bool doVote) ModifyVote(byte voterId, byte sourceVotedForId, bool isIntentional)
    {
        var (votedForId, numVotes, doVote) = base.ModifyVote(voterId, sourceVotedForId, isIntentional);
        var baseVote = (votedForId, numVotes, doVote);
        if (AbilityUses[Player.PlayerId] is > 0)
        {
            if (voterId != Player.PlayerId || sourceVotedForId == Player.PlayerId || sourceVotedForId >= 253 || !Player.IsAlive())
                return baseVote;
            Utils.GetPlayerById(sourceVotedForId).SetRealKiller(Player);
            MeetingVoteManager.Instance.ClearAndExile(Player.PlayerId, sourceVotedForId);
            AbilityUses[Player.PlayerId]--;
        }
        if (AbilityUses[Player.PlayerId] == 1)
            MeetingHudPatch.TryAddAfterMeetingDeathPlayers(CustomDeathReason.Suicide, Player.PlayerId);
        return (votedForId, numVotes, false);
    }
}
