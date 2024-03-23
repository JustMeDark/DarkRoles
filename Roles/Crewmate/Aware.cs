using System.Collections.Generic;
using AmongUs.GameOptions;

using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class Aware : RoleBase
{
    public Dictionary<byte, byte> Target = [];
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Aware),
            player => new Aware(player),
            CustomRoles.Aware,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            2500,
            null,
            "aw",
            "#85ffda"
        );
    public Aware(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
    }

    public override void Add()
    {
        Target.Clear();
    }

    public override bool OnCheckMurderAsTarget(MurderInfo info)
    {
        var (killer, target) = info.AttemptTuple;
        if (target.Is(CustomRoles.Aware) && Target[target.PlayerId] == killer.PlayerId)
        {
            target.RpcMurderPlayerV2(killer);
            var state = PlayerState.GetByPlayerId(killer.PlayerId);
            state.DeathReason = CustomDeathReason.CaughtLacking;
            state.SetDead();
            Target.Clear();
            return false;
        }
        else
            return true;
    }

    public override bool CheckVoteAsVoter(PlayerControl votedFor)
    {
        Target[Player.PlayerId] = votedFor.PlayerId;
        Utils.SendMessage($"You will be protected from {votedFor.name} this round.", Player.PlayerId, "Protection set!", true);
        return true;
    }

    public override void OnStartMeeting() => Target.Clear();
} 