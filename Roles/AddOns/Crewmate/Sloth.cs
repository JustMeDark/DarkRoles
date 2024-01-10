using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRoles.Roles.AddOns.Common;
using DarkRoles.Roles.Core;
using static DarkRoles.Options;

namespace DarkRoles.Roles.AddOns.Crewmate
{
    public class Sloth
    {
        public static OptionItem OptionSlothSpeed;
        public static void SetupCustomOption()
        {
            SetupRoleOptions(100400, TabGroup.Addons, CustomRoles.Sloth);
            AddOnsAssignData.Create(100415, CustomRoles.Sloth, true, true, true);
            OptionSlothSpeed = FloatOptionItem.Create(100425, "SlothSpeed", new(0.25f, 1.25f, 0.05f), 0.85f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Sloth]);
        }

        public static void DoSpeed(PlayerControl player)
        {
            Main.AllPlayerSpeed[player.PlayerId] = OptionSlothSpeed.GetFloat();
        }
    }
}
