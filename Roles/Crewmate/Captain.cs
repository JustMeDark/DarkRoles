using AmongUs.GameOptions;

using DarkRoles.Modules;
using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class Captain : RoleBase
{
    private static OptionItem OptionAllowedVotes;
    private static float RemainingVotes;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Captain),
            player => new Captain(player),
            CustomRoles.Captain,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            20900,
            SetupOptionItem,
            "cap",
            "#df9b00",
            true,
             introSound: () => GetIntroSound(RoleTypes.Crewmate)
        );
    public Captain(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        RemainingVotes = OptionAllowedVotes.GetFloat();
    }

    private static void SetupOptionItem()
    {
        OptionAllowedVotes = FloatOptionItem.Create(RoleInfo, 10, OptionName.CaptainAllowedVotes, new(1f, 99f, 1f), 1f, false).SetValueFormat(OptionFormat.Votes);
    }
        
    public override (byte? votedForId, int? numVotes, bool doVote) OnVote(byte voterId, byte sourceVotedForId)
    {
        var (votedForId, numVotes, doVote) = base.OnVote(voterId, sourceVotedForId);
        var baseVote = (votedForId, numVotes, doVote);
        if (RemainingVotes is > 0)
        {
            if (voterId != Player.PlayerId || sourceVotedForId == Player.PlayerId || sourceVotedForId >= 253 || !Player.IsAlive())
            {
                return baseVote;
            }
            //MeetingHudPatch.TryAddAfterMeetingDeathPlayers(CustomDeathReason.Suicide, Player.PlayerId);
            Utils.GetPlayerById(sourceVotedForId).SetRealKiller(Player);
            MeetingVoteManager.Instance.ClearAndExile(Player.PlayerId, sourceVotedForId);
            RemainingVotes--;
        }
        return (votedForId, numVotes, false);
    }

    enum OptionName
    {
        CaptainAllowedVotes,
    }
}
