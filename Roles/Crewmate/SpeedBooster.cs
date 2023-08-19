using System.Linq;
using System.Collections.Generic;

using AmongUs.GameOptions;

using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class SpeedBooster : RoleBase
{
    private static OptionItem OptionUpSpeed;
    private static float FlashSpeedAmonut;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(SpeedBooster),
            player => new SpeedBooster(player),
            CustomRoles.SpeedBooster,
            () => RoleTypes.Crewmate,
            CustomRoleTypes.Crewmate,
            20600,
            SetupOptionItem,
            "fl",
            "#00ffff"
        );
    public SpeedBooster(PlayerControl player)
    : base(
        RoleInfo,
        player
    )
    {
        FlashSpeedAmonut = OptionUpSpeed.GetFloat();
    }

    enum OptionName
    {
        FlashSpeed,
    }

    private static void SetupOptionItem()
    {
        OptionUpSpeed = FloatOptionItem.Create(RoleInfo, 10, OptionName.FlashSpeed, new(1.0f, 3.5f, 0.25f), 2.0f, false);
    }



    public override void OnFixedUpdate(PlayerControl player)
    {
        Main.AllPlayerSpeed[player.PlayerId] = FlashSpeedAmonut;
    }
}