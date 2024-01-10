using System.Collections.Generic;
using UnityEngine;
using DarkRoles.Attributes;
using DarkRoles.Roles.Core;
using static DarkRoles.Options;

namespace DarkRoles.Roles.AddOns.Common
{
    public class Watcher
    {
        private static List<byte> playerIdList = [];

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