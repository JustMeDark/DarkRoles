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
    public class Flash
    {
        public static OptionItem OptionFlashSpeed;
        public static void SetupCustomOption()
        {
            SetupRoleOptions(100300, TabGroup.Addons, CustomRoles.Flash);
            AddOnsAssignData.Create(100310, CustomRoles.Flash, true, true, true);
            OptionFlashSpeed = FloatOptionItem.Create(100320, "FlashSpeed", new(0.5f, 5.0f, 0.25f), 2.5f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Flash]);
        }

        public static void DoSpeed(PlayerControl player)
        {
            Main.AllPlayerSpeed[player.PlayerId] = OptionFlashSpeed.GetFloat();
        }
    }
}
