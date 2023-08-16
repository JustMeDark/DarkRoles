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
    public sealed class Manager : RoleBase, IImpostor
    {

        private static OptionItem OptionAdditionalVote;
        public static bool HasVoted;
        public static int AdditionalVote;

        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(Manager),
                player => new Manager(player),
                CustomRoles.Manager,
                () => RoleTypes.Shapeshifter,
                CustomRoleTypes.Impostor,
                1300,
                SetupOptionItem,
                "ma",
                introSound: () => GetIntroSound(RoleTypes.Shapeshifter)
            );
        public Manager(PlayerControl player)
        : base(
            RoleInfo,
            player
        )
        {
            AdditionalVote = OptionAdditionalVote.GetInt();
        }

        public bool CanBeLastImpostor { get; } = true;

        private static void SetupOptionItem()
        {
            OptionAdditionalVote = IntegerOptionItem.Create(RoleInfo, 10, OptionName.ManagerAdditionalVote, new(1, 99, 1), 1, false)
           .SetValueFormat(OptionFormat.Votes);
        }

        public override (byte? votedForId, int? numVotes, bool doVote) OnVote(byte voterId, byte sourceVotedForId)
        {
            var (votedForId, numVotes, doVote) = base.OnVote(voterId, sourceVotedForId);
            if (voterId == Player.PlayerId)
            {
                numVotes = AdditionalVote + 1;
            }
            return (votedForId, numVotes, doVote);
        }

        enum OptionName
        {
            ManagerAdditionalVote,
        }
    }
}
