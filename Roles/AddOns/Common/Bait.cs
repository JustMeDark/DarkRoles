using System.Collections.Generic;
using UnityEngine;
using TownOfHost.Attributes;
using TownOfHost.Roles.Core;
using static TownOfHost.Options;

namespace TownOfHost.Roles.AddOns.Common
{
    public static class Bait
    {
        private static readonly int Id = 80900;
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Bait);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "Bait");
        private static List<byte> playerIdList = new();
        private static OptionItem OptionAssignOnlyToCrewmate;
        public static bool AssignOnlyToCrewmate;

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.Addons, CustomRoles.Watcher);
            AddOnsAssignData.Create(Id + 10, CustomRoles.Watcher, true, false, false);
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

        public static void OnMurderPlayerAsTarget(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (target.Is(CustomRoles.Bait) && !info.IsSuicide)
                _ = new LateTask(() => killer.CmdReportDeadBody(target.Data), 0.15f, "Bait Self Report");
        }
    }
}