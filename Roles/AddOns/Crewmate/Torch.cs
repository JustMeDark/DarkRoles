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
    public class Torch
    {
        public static void SetupCustomOption()
        {
            SetupRoleOptions(100500, TabGroup.Addons, CustomRoles.Torch);
            AddOnsAssignData.Create(100510, CustomRoles.Torch, true, true, true);
        }
    }
}
