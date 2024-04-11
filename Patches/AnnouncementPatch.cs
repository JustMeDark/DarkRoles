using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data.Player;
using AmongUs.Data;
using Assets.InnerNet;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace DarkRoles.Patches
{
    public class ModNews
    {
        public int Number;
        public int BeforeNumber;
        public string Title;
        public string SubTitle;
        public string ShortTitle;
        public string Text;
        public string Date;

        public Announcement ToAnnouncement()
        {
            var result = new Announcement
            {
                Number = Number,
                Title = Title,
                SubTitle = SubTitle,
                ShortTitle = ShortTitle,
                Text = Text,
                Language = (uint)DataManager.Settings.Language.CurrentLanguage,
                Date = Date,
                Id = "ModNews"
            };

            return result;
        }
    }
    [HarmonyPatch]
    public class ModNewsHistory
    {
        public static List<ModNews> AllModNews = new();

        // When creating new news, you can not delete old news 
        public static void Init()
        {
            if (TranslationController.Instance.currentLanguage.languageID == SupportedLangs.English)
            {
                {
                    // Dark Roles 1.0.5
                    var news = new ModNews
                    {
                        Number = 100001,
                        Title = "Dark Roles 1.0.5!",
                        SubTitle = "\r★★ The new madmates! ★★",
                        ShortTitle = "Dark Roles v1.0.5",
                        Text = "<size=150%>Welcome to Dark Roles v1.0.5!</size>\n\n<size=125%>Support for Among Us 2024.3.5</size>\n"
                            + "\n【Base】\n - Base on TOH\r\n"
                            //+ "\n【Added】\n - Bunch of new roles\n - New custom tags for players\n - An indoor pool\r\n"
                            + "\n【Changes】\n - Madmates have been redone\n\r",
                            //+ "\n【Fixes】\n - /r no longer only sends to host\n - Troll is no longer an impostor\n - Troll randomization is fixed\n\r",
                        Date = "2024-04-10T00:00:00Z"
                    };
                    AllModNews.Add(news);
                }
                {
                    // Dark Roles 1.0.0 Release
                    var news = new ModNews
                    {
                        Number = 100000,
                        Title = "The Awaited Release!",
                        SubTitle = "\r★★ Finally released! ★★",
                        ShortTitle = "Dark Roles v1.0.0",
                        Text = "<size=150%>Welcome to Dark Roles v1.0.0!</size>\n\n<size=125%>Support for Among Us 2024.3.5</size>\n"
                            + "\n【Base】\n - Base on TOH\r\n"
                            + "\n【Added】\n - Bunch of new roles\n - New custom tags for players\n - An indoor pool\r\n"
                            + "\n【Changes】\n - Mod is refreshed\n\r"
                            + "\n【Fixes】\n - /r no longer only sends to host\n - Troll is no longer an impostor\n - Troll randomization is fixed\n\r",
                        Date = "2024-04-10T00:00:00Z"
                    };
                    AllModNews.Add(news);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
        public static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] ref Il2CppReferenceArray<Announcement> aRange)
        {
            if (!AllModNews.Any())
            {
                Init();
                AllModNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });
            }

            List<Announcement> FinalAllNews = new();
            AllModNews.Do(n => FinalAllNews.Add(n.ToAnnouncement()));
            foreach (var news in aRange)
            {
                if (!AllModNews.Any(x => x.Number == news.Number))
                    FinalAllNews.Add(news);
            }
            FinalAllNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

            aRange = new(FinalAllNews.Count);
            for (int i = 0; i < FinalAllNews.Count; i++)
                aRange[i] = FinalAllNews[i];

            return true;
        }
    }
}
