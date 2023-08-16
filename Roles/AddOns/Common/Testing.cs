using System.Collections.Generic;
using UnityEngine;
using TownOfHost.Attributes;
using TownOfHost.Roles.Core;
using static TownOfHost.Options;

namespace TownOfHost.Roles.AddOns.Common
{
    public static class Testing
    {
        private static readonly int Id = 80200;
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Test);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "Ｗ");
        private static List<byte> playerIdList = new();

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.Addons, CustomRoles.Test);
            AddOnsAssignData.Create(Id + 10, CustomRoles.Test, true, true, true);
        }
        [GameModuleInitializer]
        public static void Init()
        {
            playerIdList = new();
        }
        public static void Add(byte playerId)
        {
            playerIdList.Add(playerId);
        }
        public static bool IsEnable => playerIdList.Count > 0;
        public static bool IsThisRole(byte playerId) => playerIdList.Contains(playerId);

    }
}