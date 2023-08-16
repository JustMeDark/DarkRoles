using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AmongUs.GameOptions;

using TownOfHost.Roles.Core;
using TownOfHost.Roles.Core.Interfaces;
using static TownOfHost.Translator;
using TownOfHost.Modules;

namespace TownOfHost.Roles.Impostor
{
    public sealed class Karen : RoleBase, IImpostor
    {

        public static bool HasVoted;

        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(Karen),
                player => new Karen(player),
                CustomRoles.Karen,
                () => RoleTypes.Impostor,
                CustomRoleTypes.Impostor,
                1300,
                SetupOptionItem,
                "ka",
                introSound: () => GetIntroSound(RoleTypes.Shapeshifter)
            );
        public Karen(PlayerControl player)
        : base(
            RoleInfo,
            player
        )
        {
        }

        public bool CanBeLastImpostor { get; } = true;

        private static void SetupOptionItem()
        {

        }

        public override (byte? votedForId, int? numVotes, bool doVote) OnVote(byte voterId, byte sourceVotedForId)
        {
            var (votedForId, numVotes, doVote) = base.OnVote(voterId, sourceVotedForId);
            var baseVote = (votedForId, numVotes, doVote);
            if (!HasVoted)
            {
                if (voterId != Player.PlayerId || sourceVotedForId == Player.PlayerId || sourceVotedForId >= 253 || !Player.IsAlive())
                {
                    return baseVote;
                }
                Utils.GetPlayerById(sourceVotedForId).SetRealKiller(Player);
                MeetingVoteManager.Instance.ClearAndExile(Player.PlayerId, sourceVotedForId);
                HasVoted = true;
            }
            return (votedForId, numVotes, false);
        }
    }
}
