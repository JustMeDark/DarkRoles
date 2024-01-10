using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using DarkRoles.Modules;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using DarkRoles.Roles.Crewmate;

namespace DarkRoles.Roles.Impostor
{
    public class Karen : RoleBase, IImpostor
    {
        public Dictionary<byte, bool> HasUsed;

        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(typeof(Karen),
                player => new Karen(player),
                CustomRoles.Karen,
                () => RoleTypes.Impostor,
                CustomRoleTypes.Impostor,
                22000, null, "ka");

        public Karen(PlayerControl player) : base(RoleInfo, player)
        {
            HasUsed[player.PlayerId] = false;
        }

        public override (byte? votedForId, int? numVotes, bool doVote) ModifyVote(byte voterId, byte sourceVotedForId, bool isIntentional)
        {
            var (votedForId, numVotes, doVote) = base.ModifyVote(voterId, sourceVotedForId, isIntentional);
            var baseVote = (votedForId, numVotes, doVote);
            if (!HasUsed[Player.PlayerId])
            {
                if (voterId != Player.PlayerId || sourceVotedForId == Player.PlayerId || sourceVotedForId >= 253 || !Player.IsAlive())
                    return baseVote;
                Utils.GetPlayerById(sourceVotedForId).SetRealKiller(Player);
                MeetingVoteManager.Instance.ClearAndExile(Player.PlayerId, sourceVotedForId);
                HasUsed[Player.PlayerId] = true;
            }
            return (votedForId, numVotes, false);
        }
    }
}
