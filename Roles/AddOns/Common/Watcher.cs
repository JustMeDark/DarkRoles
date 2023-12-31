using System.Collections.Generic;
using UnityEngine;
using DarkRoles.Attributes;
using DarkRoles.Roles.Core;
using static DarkRoles.Options;

namespace DarkRoles.Roles.AddOns.Common
{
    public static class Watcher
    {
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Watcher);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "Ｗ");
        private static List<byte> playerIdList = new();

        public static void SetupCustomOption()
        {
            SetupRoleOptions(100100, TabGroup.Addons, CustomRoles.Watcher);
            AddOnsAssignData.Create(100101, CustomRoles.Watcher, true, true, true);
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