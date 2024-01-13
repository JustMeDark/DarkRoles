using System.Collections.Generic;
using System.Linq;
using DarkRoles.Roles.Core;
using static DarkRoles.Options;

namespace DarkRoles.Roles.AddOns.Common
{
    public class Wise
    {
        public static string Mark = "(Wise)", Colour = "#4e9bed";
        public static void SetupCustomOption()
        {
            SetupRoleOptions(100600, TabGroup.Addons, CustomRoles.Wise);
            AddOnsAssignData.Create(100610, CustomRoles.Wise, true, true, true);
        }

        public static void OnFirstMeeting(PlayerControl pc)
        {
            var random = IRandom.Instance;
            List<PlayerControl> targetPlayers = [.. Main.AllAlivePlayerControls.ToArray()];
            if (targetPlayers.Count >= 1)
            {
                var target = targetPlayers[random.Next(0, targetPlayers.Count)];
                if (target != null && target != pc)
                    Utils.SendMessage($"You get a feeling theres a {target.GetCustomRole()} in the lobby.", pc.PlayerId);
            }
        }
    }
}
