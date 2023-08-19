using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CoreScripts;
using HarmonyLib;
using Hazel;

using DarkRoles.Roles.Core;
using static DarkRoles.Translator;

namespace DarkRoles
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    class ChatCommands
    {
        public static List<string> ChatHistory = new();
        private static Dictionary<CustomRoles, string> roleCommands;

        public static bool Prefix(ChatController __instance)
        {
            if (__instance.freeChatField.textArea.text == "") return false;
            __instance.timeSinceLastMessage = 3f;
            var text = __instance.freeChatField.textArea.text;
            if (ChatHistory.Count == 0 || ChatHistory[^1] != text) ChatHistory.Add(text);
            ChatControllerUpdatePatch.CurrentHistorySelection = ChatHistory.Count;
            string[] args = text.Split(' ');
            string subArgs = "";
            var canceled = false;
            var cancelVal = "";
            Main.isChatCommand = true;
            Logger.Info(text, "SendChat");
            switch (args[0])
            {
                case "/dump":
                    canceled = true;
                    Utils.DumpLog();
                    break;
                case "/v":
                case "/version":
                    canceled = true;
                    string version_text = "";
                    foreach (var kvp in Main.playerVersion.OrderBy(pair => pair.Key))
                    {
                        version_text += $"{kvp.Key}:{Utils.GetPlayerById(kvp.Key)?.Data?.PlayerName}:{kvp.Value.forkId}/{kvp.Value.version}({kvp.Value.tag})\n";
                    }
                    if (version_text != "") HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, version_text);
                    break;
                default:
                    Main.isChatCommand = false;
                    break;
            }
            if (AmongUsClient.Instance.AmHost)
            {
                Main.isChatCommand = true;
                switch (args[0])
                {
                    case "/win":
                    case "/winner":
                        canceled = true;
                        Utils.SendMessage("Winner: " + string.Join(",", Main.winnerList.Select(b => Main.AllPlayerNames[b])));
                        break;

                    case "/l":
                    case "/lastresult":
                        canceled = true;
                        Utils.ShowLastResult();
                        break;

                    case "/kl":
                    case "/killlog":
                        canceled = true;
                        Utils.ShowKillLog();
                        break;

                    case "/rn":
                    case "/rename":
                        canceled = true;
                        Main.nickName = args.Length > 1 ? Main.nickName = args[1] : "";
                        break;
                    case "/r":
                    case "/role":
                        canceled = true;
                        subArgs = text.Remove(0, 2);
                        SendRolesInfo(subArgs, 255);
                        break;
                    case "/hn":
                    case "/hidename":
                        canceled = true;
                        Main.HideName.Value = args.Length > 1 ? args.Skip(1).Join(delimiter: " ") : Main.HideName.DefaultValue.ToString();
                        GameStartManagerPatch.HideName.text = Main.HideName.Value;
                        break;

                    case "/n":
                    case "/now":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "r":
                            case "roles":
                                Utils.ShowActiveRoles();
                                break;
                            default:
                                Utils.ShowActiveSettings();
                                break;
                        }
                        break;

                    case "/dis":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "crewmate":
                                GameManager.Instance.enabled = false;
                                GameManager.Instance.RpcEndGame(GameOverReason.HumansDisconnect, false);
                                break;

                            case "impostor":
                                GameManager.Instance.enabled = false;
                                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                                break;

                            default:
                                __instance.AddChat(PlayerControl.LocalPlayer, "crewmate | impostor");
                                cancelVal = "/dis";
                                break;
                        }
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Admin, 0);
                        break;

                    case "/h":
                    case "/help":
                        canceled = true;
                        subArgs = args.Length < 2 ? "" : args[1];
                        switch (subArgs)
                        {
                            case "roles":
                                subArgs = args.Length < 3 ? "" : args[2];
                                GetRolesInfo(subArgs);
                                break;

                            case "a":
                            case "addons":
                                subArgs = args.Length < 3 ? "" : args[2];
                                switch (subArgs)
                                {
                                    case "lastimpostor":
                                    case "limp":
                                        Utils.SendMessage(Utils.GetRoleName(CustomRoles.LastImpostor) + GetString("LastImpostorInfoLong"));
                                        break;

                                    default:
                                        Utils.SendMessage($"{GetString("Command.h_args")}:\n lastimpostor(limp)");
                                        break;
                                }
                                break;

                            case "m":
                            case "modes":
                                subArgs = args.Length < 3 ? "" : args[2];
                                switch (subArgs)
                                {
                                    case "hideandseek":
                                    case "has":
                                        Utils.SendMessage(GetString("HideAndSeekInfo"));
                                        break;
                                    case "syncbuttonmode":
                                    case "sbm":
                                        Utils.SendMessage(GetString("SyncButtonModeInfo"));
                                        break;

                                    case "randommapsmode":
                                    case "rmm":
                                        Utils.SendMessage(GetString("RandomMapsModeInfo"));
                                        break;

                                    default:
                                        Utils.SendMessage($"{GetString("Command.h_args")}:\n hideandseek(has), syncbuttonmode(sbm), randommapsmode(rmm)");
                                        break;
                                }
                                break;

                            case "n":
                            case "now":
                                Utils.ShowActiveSettingsHelp();
                                break;

                            default:
                                Utils.ShowHelp();
                                break;
                        }
                        break;

                    case "/m":
                    case "/myrole":
                        canceled = true;
                        var role = PlayerControl.LocalPlayer.GetCustomRole();
                        if (GameStates.IsInGame)
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString(role.ToString()) + PlayerControl.LocalPlayer.GetRoleInfo(true));
                        break;

                    case "/t":
                    case "/template":
                        canceled = true;
                        if (args.Length > 1) TemplateManager.SendTemplate(args[1]);
                        else HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{GetString("ForExample")}:\n{args[0]} test");
                        break;

                    case "/mw":
                    case "/messagewait":
                        canceled = true;
                        if (args.Length > 1 && int.TryParse(args[1], out int sec))
                        {
                            Main.MessageWait.Value = sec;
                            Utils.SendMessage(string.Format(GetString("Message.SetToSeconds"), sec), 0);
                        }
                        else Utils.SendMessage($"{GetString("Message.MessageWaitHelp")}\n{GetString("ForExample")}:\n{args[0]} 3", 0);
                        break;

                    case "/say":
                        canceled = true;
                        if (args.Length > 1)
                            Utils.SendMessage(args.Skip(1).Join(delimiter: " "), title: $"<color=#ff0000>{GetString("MessageFromTheHost")}</color>");
                        break;

                    case "/exile":
                        canceled = true;
                        if (args.Length < 2 || !int.TryParse(args[1], out int id)) break;
                        Utils.GetPlayerById(id)?.RpcExileV2();
                        break;

                    case "/kill":
                        canceled = true;
                        if (args.Length < 2 || !int.TryParse(args[1], out int id2)) break;
                        Utils.GetPlayerById(id2)?.RpcMurderPlayer(Utils.GetPlayerById(id2));
                        break;

                    case "/up":
                        canceled = true;
                        subArgs = text.Remove(0, 3);

                        if (args[1] is null)
                            Utils.SendMessage("Please provide a role to select!");
                        else
                            Utils.SendMessage("Role has been selected!");
                        break;

                    default:
                        Main.isChatCommand = false;
                        break;
                }
            }
            if (canceled)
            {
                Logger.Info("Command Canceled", "ChatCommand");
                __instance.freeChatField.textArea.Clear();
                __instance.freeChatField.textArea.SetText(cancelVal);
            }
            return !canceled;
        }

        public static void GetRolesInfo(string role)
        {
            // 初回のみ処理
            if (roleCommands == null)
            {
#pragma warning disable IDE0028  // Dictionary初期化の簡素化をしない
                roleCommands = new Dictionary<CustomRoles, string>();

                // GM
                roleCommands.Add(CustomRoles.GM, "gm");

                // Impostor役職
                roleCommands.Add((CustomRoles)(-1), $"== {GetString("Impostor")} ==");  // 区切り用
                ConcatCommands(CustomRoleTypes.Impostor);

                // Madmate役職
                roleCommands.Add((CustomRoles)(-2), $"== {GetString("Madmate")} ==");  // 区切り用
                ConcatCommands(CustomRoleTypes.Madmate);
                roleCommands.Add(CustomRoles.SKMadmate, "sm");

                // Crewmate役職
                roleCommands.Add((CustomRoles)(-3), $"== {GetString("Crewmate")} ==");  // 区切り用
                ConcatCommands(CustomRoleTypes.Crewmate);

                // Neutral役職
                roleCommands.Add((CustomRoles)(-4), $"== {GetString("Neutral")} ==");  // 区切り用
                ConcatCommands(CustomRoleTypes.Neutral);

                // 属性
                roleCommands.Add((CustomRoles)(-5), $"== {GetString("Addons")} ==");  // 区切り用
                roleCommands.Add(CustomRoles.Lovers, "lo");
                roleCommands.Add(CustomRoles.Watcher, "wat");
                roleCommands.Add(CustomRoles.Workhorse, "wh");
                roleCommands.Add(CustomRoles.Bait, "ba");

                // HAS
                roleCommands.Add((CustomRoles)(-6), $"== {GetString("HideAndSeek")} ==");  // 区切り用
                roleCommands.Add(CustomRoles.HASFox, "hfo");
                roleCommands.Add(CustomRoles.HASTroll, "htr");
#pragma warning restore IDE0028
            }

            var msg = "";
            var rolemsg = $"{GetString("Command.h_args")}";
            foreach (var r in roleCommands)
            {
                var roleName = r.Key.ToString();
                var roleShort = r.Value;

                if (String.Compare(role, roleName, true) == 0 || String.Compare(role, roleShort, true) == 0)
                {
                    Utils.SendMessage(GetString(roleName) + GetString($"{roleName}InfoLong"));
                    return;
                }

                var roleText = $"{roleName.ToLower()}({roleShort.ToLower()}), ";
                if ((int)r.Key < 0)
                {
                    msg += rolemsg + "\n" + roleShort + "\n";
                    rolemsg = "";
                }
                else if ((rolemsg.Length + roleText.Length) > 40)
                {
                    msg += rolemsg + "\n";
                    rolemsg = roleText;
                }
                else
                {
                    rolemsg += roleText;
                }
            }
            msg += rolemsg;
            Utils.SendMessage(msg);
        }
        private static void ConcatCommands(CustomRoleTypes roleType)
        {
            var roles = CustomRoleManager.AllRolesInfo.Values.Where(role => role.CustomRoleType == roleType);
            foreach (var role in roles)
            {
                if (role.ChatCommand is null) continue;

                roleCommands[role.RoleName] = role.ChatCommand;
            }
        }
        public static void OnReceiveChat(PlayerControl player, string text)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            string[] args = text.Split(' ');
            string subArgs = "";
            switch (args[0])
            {
                case "/l":
                case "/lastresult":
                    Utils.ShowLastResult(player.PlayerId);
                    break;

                case "/kl":
                case "/killlog":
                    Utils.ShowKillLog(player.PlayerId);
                    break;

                case "/n":
                case "/now":
                    subArgs = args.Length < 2 ? "" : args[1];
                    switch (subArgs)
                    {
                        case "r":
                        case "roles":
                            Utils.ShowActiveRoles(player.PlayerId);
                            break;

                        default:
                            Utils.ShowActiveSettings(player.PlayerId);
                            break;
                    }
                    break;

                case "/h":
                case "/help":
                    subArgs = args.Length < 2 ? "" : args[1];
                    switch (subArgs)
                    {
                        case "n":
                        case "now":
                            Utils.ShowActiveSettingsHelp(player.PlayerId);
                            break;
                    }
                    break;

                case "/m":
                case "/myrole":
                    var role = player.GetCustomRole();
                    if (GameStates.IsInGame)
                        Utils.SendMessage(GetString(role.ToString()) + player.GetRoleInfo(true), player.PlayerId);
                    break;

                case "/t":
                case "/template":
                    if (args.Length > 1) TemplateManager.SendTemplate(args[1], player.PlayerId);
                    else Utils.SendMessage($"{GetString("ForExample")}:\n{args[0]} test", player.PlayerId);
                    break;

                default:
                    break;
            }
        }

        public static void SendRolesInfo(string role, byte playerId)
        {
            role = role.Trim().ToLower();
            if (role.StartsWith("/r")) role.Replace("/r", string.Empty);
            if (role.StartsWith("/up")) role.Replace("/up", string.Empty);
            if (role.EndsWith("\r\n")) role.Replace("\r\n", string.Empty);
            if (role.EndsWith("\n")) role.Replace("\n", string.Empty);

            if (role == "" || role == string.Empty)
            {
                Utils.ShowActiveRoles(playerId);
                return;
            }

            role = FixRoleNameInput(role).ToLower().Trim().Replace(" ", string.Empty);

            foreach (CustomRoles rl in Enum.GetValues(typeof(CustomRoles)))
            {
                if (rl.IsVanilla()) continue;
                var roleName = GetString(rl.ToString());
                if (role == roleName.ToLower().Trim().TrimStart('*').Replace(" ", string.Empty))
                {
                   
                    var sb = new StringBuilder();
                    sb.Append(roleName + GetString($"{rl}InfoLong"));
                    if (Options.CustomRoleSpawnChances.ContainsKey(rl))
                    {
                        //Utils.ShowChildrenSettings(Options.CustomRoleSpawnChances[rl], ref sb, command: true);
                        var txt = sb.ToString();
                        sb.Clear().Append(txt.RemoveHtmlTags());
                    }
                    Utils.SendMessage(sb.ToString(), playerId);
                    return;
                }
            }
            //if (isUp) Utils.SendMessage(GetString("Message.YTPlanCanNotFindRoleThePlayerEnter"), playerId);
            //else Utils.SendMessage(GetString("Message.CanNotFindRoleThePlayerEnter"), playerId);
            return;
        }

        public static string FixRoleNameInput(string text)
        {
            text = text.Replace("着", "者").Trim().ToLower();
            return text switch
            {
                "管理員" or "管理" or "gm" => GetString("GM"),
                "賞金獵人" or "赏金" => GetString("BountyHunter"),
                "自爆兵" or "自爆" => GetString("Bomber"),
                "邪惡的追踪者" or "邪恶追踪者" or "追踪" => GetString("EvilTracker"),
                "煙花商人" or "烟花" => GetString("FireWorks"),
                "夢魘" or "夜魇" => GetString("Mare"),
                "詭雷" => GetString("BoobyTrap"),
                "黑手黨" or "黑手" => GetString("Mafia"),
                "嗜血殺手" or "嗜血" => GetString("SerialKiller"),
                "千面鬼" or "千面" => GetString("ShapeMaster"),
                "狂妄殺手" or "狂妄" => GetString("Sans"),
                "殺戮機器" or "杀戮" or "机器" or "杀戮兵器" => GetString("Minimalism"),
                "蝕時者" or "蚀时" or "偷时" => GetString("TimeThief"),
                "狙擊手" or "狙击" => GetString("Sniper"),
                "傀儡師" or "傀儡" => GetString("Puppeteer"),
                "殭屍" or "丧尸" => GetString("Zombie"),
                "吸血鬼" or "吸血" => GetString("Vampire"),
                "術士" => GetString("Warlock"),
                "駭客" or "黑客" => GetString("Hacker"),
                "刺客" or "忍者" => GetString("Assassin"),
                "礦工" => GetString("Miner"),
                "逃逸者" or "逃逸" => GetString("Escapee"),
                "女巫" => GetString("Witch"),
                "監視者" or "监管" => GetString("AntiAdminer"),
                "清道夫" or "清道" => GetString("Scavenger"),
                "窺視者" or "窥视" => GetString("Watcher"),
                "誘餌" or "大奖" or "头奖" => GetString("Bait"),
                "擺爛人" or "摆烂" => GetString("Needy"),
                "獨裁者" or "独裁" => GetString("Dictator"),
                "法醫" => GetString("Doctor"),
                "偵探" => GetString("Detective"),
                "幸運兒" or "幸运" => GetString("Luckey"),
                "大明星" or "明星" => GetString("SuperStar"),
                "網紅" => GetString("CyberStar"),
                "俠客" => GetString("SwordsMan"),
                "正義賭怪" or "正义的赌怪" or "好赌" or "正义赌" => GetString("NiceGuesser"),
                "邪惡賭怪" or "邪恶的赌怪" or "坏赌" or "恶赌" or "邪恶赌" or "赌怪" => GetString("EvilGuesser"),
                "市長" or "逝长" => GetString("Mayor"),
                "被害妄想症" or "被害妄想" or "被迫害妄想症" or "被害" or "妄想" or "妄想症" => GetString("Paranoia"),
                "愚者" or "愚" => GetString("Psychic"),
                "修理大师" or "修理" or "维修" => GetString("SabotageMaster"),
                "警長" => GetString("Sheriff"),
                "告密者" or "告密" => GetString("Snitch"),
                "增速者" or "增速" => GetString("SpeedBooster"),
                "時間操控者" or "时间操控人" or "时间操控" => GetString("TimeManager"),
                "陷阱師" or "陷阱" or "小奖" => GetString("Trapper"),
                "傳送師" or "传送" => GetString("Transporter"),
                "縱火犯" or "纵火" => GetString("Arsonist"),
                "處刑人" or "处刑" => GetString("Executioner"),
                "小丑" or "丑皇" => GetString("Jester"),
                "投機者" or "投机" => GetString("Opportunist"),
                "馬里奧" or "马力欧" => GetString("Mario"),
                "恐怖分子" or "恐怖" => GetString("Terrorist"),
                "豺狼" or "蓝狼" or "狼" => GetString("Jackal"),
                "神" or "上帝" => GetString("God"),
                "情人" or "愛人" or "链子" or "老婆" or "老公" => GetString("Lovers"),
                "絕境者" or "绝境" => GetString("LastImpostor"),
                "閃電俠" or "闪电" => GetString("Flashman"),
                "靈媒" => GetString("Seer"),
                "破平者" or "破平" => GetString("Brakar"),
                "執燈人" or "执灯" or "灯人" => GetString("Lighter"),
                "膽小" or "胆小" => GetString("Oblivious"),
                "迷惑者" or "迷幻" => GetString("Bewilder"),
                "sun" => GetString("Sunglasses"),
                "蠢蛋" or "蠢狗" or "傻逼" => GetString("Fool"),
                "冤罪師" or "冤罪" => GetString("Innocent"),
                "資本家" or "资本主义" or "资本" => GetString("Capitalism"),
                "老兵" => GetString("Veteran"),
                "加班狂" or "加班" => GetString("Workhorse"),
                "復仇者" or "复仇" => GetString("Avanger"),
                "鵜鶘" => GetString("Pelican"),
                "保鏢" => GetString("Bodyguard"),
                "up" or "up主" => GetString("Youtuber"),
                "利己主義者" or "利己主义" or "利己" => GetString("Egoist"),
                "贗品商" or "赝品" => GetString("Counterfeiter"),
                "擲雷兵" or "掷雷" or "闪光弹" => GetString("Grenadier"),
                "竊票者" or "偷票" or "偷票者" or "窃票师" or "窃票" => GetString("TicketsStealer"),
                "教父" => GetString("Gangster"),
                "革命家" or "革命" => GetString("Revolutionist"),
                "fff團" or "fff" or "fff团" => GetString("FFF"),
                "清理工" or "清潔工" or "清洁工" or "清理" or "清洁" => GetString("Cleaner"),
                "醫生" => GetString("Medicaler"),
                "占卜師" or "占卜" => GetString("Divinator"),
                "雙重人格" or "双重" or "双人格" or "人格" => GetString("DualPersonality"),
                "玩家" => GetString("Gamer"),
                "情報販子" or "情报" or "贩子" => GetString("Messenger"),
                "球狀閃電" or "球闪" or "球状" => GetString("BallLightning"),
                "潛藏者" or "潜藏" => GetString("DarkHide"),
                "貪婪者" or "贪婪" => GetString("Greedier"),
                "工作狂" or "工作" => GetString("Workaholic"),
                "呪狼" or "咒狼" => GetString("CursedWolf"),
                "寶箱怪" or "宝箱" => GetString("Mimic"),
                "集票者" or "集票" or "寄票" or "机票" => GetString("Collector"),
                "活死人" or "活死" => GetString("Glitch"),
                "奪魂者" or "多混" or "夺魂" => GetString("ImperiusCurse"),
                "自爆卡車" or "自爆" or "卡车" => GetString("Provocateur"),
                "快槍手" or "快枪" => GetString("QuickShooter"),
                "隱蔽者" or "隐蔽" or "小黑人" => GetString("Concealer"),
                "抹除者" or "抹除" => GetString("Eraser"),
                "肢解者" or "肢解" => GetString("OverKiller"),
                "劊子手" or "侩子手" or "柜子手" => GetString("Hangman"),
                "陽光開朗大男孩" or "阳光" or "开朗" or "大男孩" or "阳光开朗" or "开朗大男孩" or "阳光大男孩" => GetString("Sunnyboy"),
                "法官" or "审判" => GetString("Judge"),
                "入殮師" or "入检师" or "入殓" => GetString("Mortician"),
                "通靈師" or "通灵" => GetString("Mediumshiper"),
                "吟游詩人" or "诗人" => GetString("Bard"),
                "隱匿者" or "隐匿" or "隐身" or "隐身人" or "印尼" => GetString("Swooper"),
                "船鬼" => GetString("Crewpostor"),
                "嗜血騎士" or "血骑" or "骑士" or "bk" => GetString("BloodKnight"),
                "賭徒" => GetString("Totocalcio"),
                "分散机" => GetString("Disperser"),
                "和平之鸽" or "和平之鴿" or "和平的鸽子" or "和平" => GetString("DovesOfNeace"),
                "持槍" or "持械" or "手长" => GetString("Reach"),
                "monarch" => GetString("Monarch"),
                _ => text,
            };
        }
    }



    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    class ChatUpdatePatch
    {
        public static bool DoBlockChat = false;
        public static void Postfix(ChatController __instance)
        {
            if (!AmongUsClient.Instance.AmHost || Main.MessagesToSend.Count < 1 || (Main.MessagesToSend[0].Item2 == byte.MaxValue && Main.MessageWait.Value > __instance.timeSinceLastMessage)) return;
            if (DoBlockChat) return;
            var player = Main.AllAlivePlayerControls.OrderBy(x => x.PlayerId).FirstOrDefault();
            if (player == null) return;
            (string msg, byte sendTo, string title) = Main.MessagesToSend[0];
            Main.MessagesToSend.RemoveAt(0);
            int clientId = sendTo == byte.MaxValue ? -1 : Utils.GetPlayerById(sendTo).GetClientId();
            var name = player.Data.PlayerName;
            if (clientId == -1)
            {
                player.SetName(title);
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, msg);
                player.SetName(name);
            }
            var writer = CustomRpcSender.Create("MessagesToSend", SendOption.None);
            writer.StartMessage(clientId);
            writer.StartRpc(player.NetId, (byte)RpcCalls.SetName)
                .Write(title)
                .EndRpc();
            writer.StartRpc(player.NetId, (byte)RpcCalls.SendChat)
                .Write(msg)
                .EndRpc();
            writer.StartRpc(player.NetId, (byte)RpcCalls.SetName)
                .Write(player.Data.PlayerName)
                .EndRpc();
            writer.EndMessage();
            writer.SendMessage();
            __instance.timeSinceLastMessage = 0f;
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    class AddChatPatch
    {
        public static void Postfix(string chatText)
        {
            switch (chatText)
            {
                default:
                    break;
            }
            if (!AmongUsClient.Instance.AmHost) return;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
    class RpcSendChatPatch
    {
        public static bool Prefix(PlayerControl __instance, string chatText, ref bool __result)
        {
            if (string.IsNullOrWhiteSpace(chatText))
            {
                __result = false;
                return false;
            }
            int return_count = PlayerControl.LocalPlayer.name.Count(x => x == '\n');
            chatText = new StringBuilder(chatText).Insert(0, "\n", return_count).ToString();
            if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText);
            if (chatText.Contains("who", StringComparison.OrdinalIgnoreCase))
                DestroyableSingleton<Telemetry>.Instance.SendWho();
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte)RpcCalls.SendChat, SendOption.None);
            messageWriter.Write(chatText);
            messageWriter.EndMessage();
            __result = true;
            return false;
        }
    }
}