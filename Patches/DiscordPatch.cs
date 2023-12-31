using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data;
using Discord;
using HarmonyLib;

namespace DarkRoles.Patches
{
    // Originally from Town of Us Rewritten, by Det
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    public class DiscordRPC
    {
        private static string lobbycode = "";
        private static string region = "";
        public static void Prefix([HarmonyArgument(0)] Activity activity)
        {
            var details = $"Dark Roles v{Main.version}";
            activity.Details = details;
            activity.Name = "Dark Roles Reloaded";

            try
            {
                if (activity.State != "In Menus")
                {
                    if (!DataManager.Settings.Gameplay.StreamerMode)
                    {
                        int maxSize = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                        if (GameStates.IsLobby)
                        {
                            lobbycode = GameStartManager.Instance.GameRoomNameCode.text;
                            region = ServerManager.Instance.CurrentRegion.Name;
                            if (region == "North America") region = "NA";
                            if (region == "Europe") region = "EU";
                            if (region == "Asia") region = "AS";
                        }

                        if (lobbycode != "" && region != "")
                        {
                            details = $"Dark Roles - {lobbycode} ({region})";
                        }

                        activity.Details = details;
                    }
                    else
                    {
                        details = $"Dark Roles v{Main.version}";
                    }
                }
            }

            catch (ArgumentException ex)
            {
                Logger.Error("Error in updating discord rpc", "DiscordPatch");
                Logger.Exception(ex, "DiscordPatch");
                details = $"Dark Roles v{Main.version}";
                activity.Details = details;
            }
        }
    }
}