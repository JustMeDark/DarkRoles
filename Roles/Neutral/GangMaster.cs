using AmongUs.GameOptions;

using TownOfHost.Roles.Core;
using TownOfHost.Roles.Core.Interfaces;

namespace TownOfHost.Roles.Neutral;
public sealed class GangMaster : RoleBase, IImpostor
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(GangMaster),
            player => new GangMaster(player),
            CustomRoles.GangMaster,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Neutral,
            1600,
            null,
            "gama",
            "#6e69ff"
        );
    public GangMaster(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    { }
    public bool CanUseKillButton()
    {
        if (PlayerState.AllPlayerStates == null) return false;
        //マフィアを除いた生きているインポスターの人数  Number of Living Impostors excluding mafia
        int livingGangNum = 0;
        foreach (var pc in Main.AllAlivePlayerControls)
        {
            var role = pc.GetCustomRole();
            if (role != CustomRoles.GangMaster && role.IsNeutral()) livingGangNum++;
        }

        return livingGangNum <= 0;
    }
}