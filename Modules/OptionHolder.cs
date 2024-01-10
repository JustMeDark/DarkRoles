using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

using DarkRoles.Modules;
using DarkRoles.Roles;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.AddOns.Common;
using DarkRoles.Roles.AddOns.Impostor;
using DarkRoles.Roles.AddOns.Crewmate;

namespace DarkRoles
{
    [Flags]
    public enum CustomGameMode
    {
        Standard = 0x01,
        HideAndSeek = 0x02,
        All = int.MaxValue
    }

    [HarmonyPatch]
    public static class Options
    {
        static Task taskOptionsLoad;
        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.Initialize)), HarmonyPostfix]
        public static void OptionsLoadStart()
        {
            Logger.Info("Options.Load Start", "Options");
            taskOptionsLoad = Task.Run(Load);
        }
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPostfix]
        public static void WaitOptionsLoad()
        {
            taskOptionsLoad.Wait();
            Logger.Info("Options.Load End", "Options");
        }

        // プリセット
        private static readonly string[] presets =
        {
            Main.Preset1.Value, Main.Preset2.Value, Main.Preset3.Value,
            Main.Preset4.Value, Main.Preset5.Value
        };

        // ゲームモード
        public static OptionItem GameMode;
        public static CustomGameMode CurrentGameMode
            => GameMode.CurrentValue == 0 ? CustomGameMode.Standard : CustomGameMode.HideAndSeek;

        public static readonly string[] gameModes =
        {
            "Standard", "HideAndSeek",
        };

        // MapActive
        public static bool IsActiveSkeld => AddedTheSkeld.GetBool() || Main.NormalOptions.MapId == 0;
        public static bool IsActiveMiraHQ => AddedMiraHQ.GetBool() || Main.NormalOptions.MapId == 1;
        public static bool IsActivePolus => AddedPolus.GetBool() || Main.NormalOptions.MapId == 2;
        public static bool IsActiveAirship => AddedTheAirShip.GetBool() || Main.NormalOptions.MapId == 4;
        public static bool IsActiveFungle => AddedTheFungle.GetBool() || Main.NormalOptions.MapId == 5;

        // 役職数・確率
        public static Dictionary<CustomRoles, OptionItem> CustomRoleCounts;
        public static Dictionary<CustomRoles, IntegerOptionItem> CustomRoleSpawnChances;
        public static readonly string[] rates =
        {
            "Rate0",  "Rate5",  "Rate10", "Rate20", "Rate30", "Rate40",
            "Rate50", "Rate60", "Rate70", "Rate80", "Rate90", "Rate100",
        };

        // 各役職の詳細設定
        public static OptionItem EnableGM;
        public static float DefaultKillCooldown = Main.NormalOptions?.KillCooldown ?? 20;
        public static OptionItem DefaultShapeshiftCooldown;
        public static OptionItem CanMakeMadmateCount;
        public static OptionItem MadmateCanFixLightsOut; // TODO:mii-47 マッド役職統一
        public static OptionItem MadmateCanFixComms;
        public static OptionItem MadmateHasImpostorVision;
        public static OptionItem MadmateCanSeeKillFlash;
        public static OptionItem MadmateCanSeeOtherVotes;
        public static OptionItem MadmateCanSeeDeathReason;
        public static OptionItem MadmateRevengeCrewmate;
        public static OptionItem MadmateVentCooldown;
        public static OptionItem MadmateVentMaxTime;

        public static OptionItem KillFlashDuration;

        // HideAndSeek
        public static OptionItem AllowCloseDoors;
        public static OptionItem KillDelay;
        // public static OptionItem IgnoreCosmetics;
        public static OptionItem IgnoreVent;
        public static float HideAndSeekKillDelayTimer = 0f;

        // タスク無効化
        public static OptionItem DisableTasks;
        public static OptionItem DisableSwipeCard;
        public static OptionItem DisableSubmitScan;
        public static OptionItem DisableUnlockSafe;
        public static OptionItem DisableUploadData;
        public static OptionItem DisableStartReactor;
        public static OptionItem DisableResetBreaker;

        //デバイスブロック
        public static OptionItem DisableDevices;
        public static OptionItem DisableSkeldDevices;
        public static OptionItem DisableSkeldAdmin;
        public static OptionItem DisableSkeldCamera;
        public static OptionItem DisableMiraHQDevices;
        public static OptionItem DisableMiraHQAdmin;
        public static OptionItem DisableMiraHQDoorLog;
        public static OptionItem DisablePolusDevices;
        public static OptionItem DisablePolusAdmin;
        public static OptionItem DisablePolusCamera;
        public static OptionItem DisablePolusVital;
        public static OptionItem DisableAirshipDevices;
        public static OptionItem DisableAirshipCockpitAdmin;
        public static OptionItem DisableAirshipRecordsAdmin;
        public static OptionItem DisableAirshipCamera;
        public static OptionItem DisableAirshipVital;
        public static OptionItem DisableFungleDevices;
        public static OptionItem DisableFungleVital;
        public static OptionItem DisableDevicesIgnoreConditions;
        public static OptionItem DisableDevicesIgnoreImpostors;
        public static OptionItem DisableDevicesIgnoreMadmates;
        public static OptionItem DisableDevicesIgnoreNeutrals;
        public static OptionItem DisableDevicesIgnoreCrewmates;
        public static OptionItem DisableDevicesIgnoreAfterAnyoneDied;

        // ランダムマップ
        public static OptionItem RandomMapsMode;
        public static OptionItem AddedTheSkeld;
        public static OptionItem AddedMiraHQ;
        public static OptionItem AddedPolus;
        public static OptionItem AddedTheAirShip;
        public static OptionItem AddedTheFungle;
        // public static OptionItem AddedDleks;

        // ランダムスポーン
        public static OptionItem EnableRandomSpawn;
        //Skeld
        public static OptionItem RandomSpawnSkeld;
        public static OptionItem RandomSpawnSkeldCafeteria;
        public static OptionItem RandomSpawnSkeldWeapons;
        public static OptionItem RandomSpawnSkeldLifeSupp;
        public static OptionItem RandomSpawnSkeldNav;
        public static OptionItem RandomSpawnSkeldShields;
        public static OptionItem RandomSpawnSkeldComms;
        public static OptionItem RandomSpawnSkeldStorage;
        public static OptionItem RandomSpawnSkeldAdmin;
        public static OptionItem RandomSpawnSkeldElectrical;
        public static OptionItem RandomSpawnSkeldLowerEngine;
        public static OptionItem RandomSpawnSkeldUpperEngine;
        public static OptionItem RandomSpawnSkeldSecurity;
        public static OptionItem RandomSpawnSkeldReactor;
        public static OptionItem RandomSpawnSkeldMedBay;
        //Mira
        public static OptionItem RandomSpawnMira;
        public static OptionItem RandomSpawnMiraCafeteria;
        public static OptionItem RandomSpawnMiraBalcony;
        public static OptionItem RandomSpawnMiraStorage;
        public static OptionItem RandomSpawnMiraJunction;
        public static OptionItem RandomSpawnMiraComms;
        public static OptionItem RandomSpawnMiraMedBay;
        public static OptionItem RandomSpawnMiraLockerRoom;
        public static OptionItem RandomSpawnMiraDecontamination;
        public static OptionItem RandomSpawnMiraLaboratory;
        public static OptionItem RandomSpawnMiraReactor;
        public static OptionItem RandomSpawnMiraLaunchpad;
        public static OptionItem RandomSpawnMiraAdmin;
        public static OptionItem RandomSpawnMiraOffice;
        public static OptionItem RandomSpawnMiraGreenhouse;
        //Polus
        public static OptionItem RandomSpawnPolus;
        public static OptionItem RandomSpawnPolusOfficeLeft;
        public static OptionItem RandomSpawnPolusOfficeRight;
        public static OptionItem RandomSpawnPolusAdmin;
        public static OptionItem RandomSpawnPolusComms;
        public static OptionItem RandomSpawnPolusWeapons;
        public static OptionItem RandomSpawnPolusBoilerRoom;
        public static OptionItem RandomSpawnPolusLifeSupp;
        public static OptionItem RandomSpawnPolusElectrical;
        public static OptionItem RandomSpawnPolusSecurity;
        public static OptionItem RandomSpawnPolusDropship;
        public static OptionItem RandomSpawnPolusStorage;
        public static OptionItem RandomSpawnPolusRocket;
        public static OptionItem RandomSpawnPolusLaboratory;
        public static OptionItem RandomSpawnPolusToilet;
        public static OptionItem RandomSpawnPolusSpecimens;
        //AIrShip
        public static OptionItem RandomSpawnAirship;
        public static OptionItem RandomSpawnAirshipBrig;
        public static OptionItem RandomSpawnAirshipEngine;
        public static OptionItem RandomSpawnAirshipKitchen;
        public static OptionItem RandomSpawnAirshipCargoBay;
        public static OptionItem RandomSpawnAirshipRecords;
        public static OptionItem RandomSpawnAirshipMainHall;
        public static OptionItem RandomSpawnAirshipNapRoom;
        public static OptionItem RandomSpawnAirshipMeetingRoom;
        public static OptionItem RandomSpawnAirshipGapRoom;
        public static OptionItem RandomSpawnAirshipVaultRoom;
        public static OptionItem RandomSpawnAirshipComms;
        public static OptionItem RandomSpawnAirshipCockpit;
        public static OptionItem RandomSpawnAirshipArmory;
        public static OptionItem RandomSpawnAirshipViewingDeck;
        public static OptionItem RandomSpawnAirshipSecurity;
        public static OptionItem RandomSpawnAirshipElectrical;
        public static OptionItem RandomSpawnAirshipMedical;
        public static OptionItem RandomSpawnAirshipToilet;
        public static OptionItem RandomSpawnAirshipShowers;
        //Fungle
        public static OptionItem RandomSpawnFungle;
        public static OptionItem RandomSpawnFungleKitchen;
        public static OptionItem RandomSpawnFungleBeach;
        public static OptionItem RandomSpawnFungleCafeteria;
        public static OptionItem RandomSpawnFungleRecRoom;
        public static OptionItem RandomSpawnFungleBonfire;
        public static OptionItem RandomSpawnFungleDropship;
        public static OptionItem RandomSpawnFungleStorage;
        public static OptionItem RandomSpawnFungleMeetingRoom;
        public static OptionItem RandomSpawnFungleSleepingQuarters;
        public static OptionItem RandomSpawnFungleLaboratory;
        public static OptionItem RandomSpawnFungleGreenhouse;
        public static OptionItem RandomSpawnFungleReactor;
        public static OptionItem RandomSpawnFungleJungleTop;
        public static OptionItem RandomSpawnFungleJungleBottom;
        public static OptionItem RandomSpawnFungleLookout;
        public static OptionItem RandomSpawnFungleMiningPit;
        public static OptionItem RandomSpawnFungleHighlands;
        public static OptionItem RandomSpawnFungleUpperEngine;
        public static OptionItem RandomSpawnFunglePrecipice;
        public static OptionItem RandomSpawnFungleComms;

        // 投票モード
        public static OptionItem VoteMode;
        public static OptionItem WhenSkipVote;
        public static OptionItem WhenSkipVoteIgnoreFirstMeeting;
        public static OptionItem WhenSkipVoteIgnoreNoDeadBody;
        public static OptionItem WhenSkipVoteIgnoreEmergency;
        public static OptionItem WhenNonVote;
        public static OptionItem WhenTie;
        public static readonly string[] voteModes =
        {
            "Default", "Suicide", "SelfVote", "Skip"
        };
        public static readonly string[] tieModes =
        {
            "TieMode.Default", "TieMode.All", "TieMode.Random"
        };
        public static VoteMode GetWhenSkipVote() => (VoteMode)WhenSkipVote.GetValue();
        public static VoteMode GetWhenNonVote() => (VoteMode)WhenNonVote.GetValue();

        // ボタン回数
        public static OptionItem SyncButtonMode;
        public static OptionItem SyncedButtonCount;
        public static int UsedButtonCount = 0;

        // 全員生存時の会議時間
        public static OptionItem AllAliveMeeting;
        public static OptionItem AllAliveMeetingTime;

        // 追加の緊急ボタンクールダウン
        public static OptionItem AdditionalEmergencyCooldown;
        public static OptionItem AdditionalEmergencyCooldownThreshold;
        public static OptionItem AdditionalEmergencyCooldownTime;

        //転落死
        public static OptionItem LadderDeath;
        public static OptionItem LadderDeathChance;

        // 通常モードでかくれんぼ
        public static bool IsStandardHAS => StandardHAS.GetBool() && CurrentGameMode == CustomGameMode.Standard;
        public static OptionItem StandardHAS;
        public static OptionItem StandardHASWaitingTime;

        // リアクターの時間制御
        public static OptionItem SabotageTimeControl;
        public static OptionItem PolusReactorTimeLimit;
        public static OptionItem AirshipReactorTimeLimit;

        // サボタージュのクールダウン変更
        public static OptionItem ModifySabotageCooldown;
        public static OptionItem SabotageCooldown;

        // 停電の特殊設定
        public static OptionItem LightsOutSpecialSettings;
        public static OptionItem DisableAirshipViewingDeckLightsPanel;
        public static OptionItem DisableAirshipGapRoomLightsPanel;
        public static OptionItem DisableAirshipCargoLightsPanel;
        public static OptionItem BlockDisturbancesToSwitches;

        // マップ改造
        public static OptionItem MapModification;
        public static OptionItem AirShipVariableElectrical;
        public static OptionItem DisableAirshipMovingPlatform;
        public static OptionItem ResetDoorsEveryTurns;
        public static OptionItem DoorsResetMode;
        public static OptionItem DisableFungleSporeTrigger;

        // その他
        public static OptionItem FixFirstKillCooldown;
        public static OptionItem DisableTaskWin;
        public static OptionItem GhostCanSeeOtherRoles;
        public static OptionItem GhostCanSeeOtherTasks;
        public static OptionItem GhostCanSeeOtherVotes;
        public static OptionItem GhostCanSeeDeathReason;
        public static OptionItem GhostIgnoreTasks;
        public static OptionItem CommsCamouflage;

        // プリセット対象外
        public static OptionItem NoGameEnd;
        public static OptionItem AutoDisplayLastResult;
        public static OptionItem AutoDisplayKillLog;
        public static OptionItem SuffixMode;
        public static OptionItem HideGameSettings;
        public static OptionItem ColorNameMode;
        public static OptionItem ChangeNameToRoleInfo;
        public static OptionItem RoleAssigningAlgorithm;

        public static OptionItem ApplyDenyNameList;
        public static OptionItem KickPlayerFriendCodeNotExist;
        public static OptionItem ApplyBanList;

        public static readonly string[] suffixModes =
        {
            "SuffixMode.None",
            "SuffixMode.Version",
            "SuffixMode.Streaming",
            "SuffixMode.Recording",
            "SuffixMode.RoomHost",
            "SuffixMode.OriginalName"
        };
        public static readonly string[] RoleAssigningAlgorithms =
        {
            "RoleAssigningAlgorithm.Default",
            "RoleAssigningAlgorithm.NetRandom",
            "RoleAssigningAlgorithm.HashRandom",
            "RoleAssigningAlgorithm.Xorshift",
            "RoleAssigningAlgorithm.MersenneTwister",
        };
        public static SuffixModes GetSuffixMode()
        {
            return (SuffixModes)SuffixMode.GetValue();
        }

        public static int SnitchExposeTaskLeft = 1;

        public static bool IsLoaded = false;
        public static int GetRoleCount(CustomRoles role)
        {
            return GetRoleChance(role) == 0 ? 0 : CustomRoleCounts.TryGetValue(role, out var option) ? option.GetInt() : 0;
        }

        public static int GetRoleChance(CustomRoles role)
        {
            return CustomRoleSpawnChances.TryGetValue(role, out var option) ? option.GetInt() : 0;
        }
        public static void Load()
        {
            if (IsLoaded) return;
            OptionSaver.Initialize();

            // プリセット
            _ = PresetOptionItem.Create(0, TabGroup.MainSettings)
                .SetColor(new Color32(204, 204, 0, 255))
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);

            // ゲームモード
            GameMode = StringOptionItem.Create(1, "GameMode", gameModes, 0, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);

            #region 役職・詳細設定
            CustomRoleCounts = new();
            CustomRoleSpawnChances = new();

            var sortedRoleInfo = CustomRoleManager.AllRolesInfo.Values.OrderBy(role => role.ConfigId);
            // GM
            EnableGM = BooleanOptionItem.Create(110, "GM", false, TabGroup.MainSettings, false)
                .SetColor(Utils.GetRoleColor(CustomRoles.GM))
                .SetHeader(true)
                .SetGameMode(CustomGameMode.Standard);

            RoleAssignManager.SetupOptionItem();
            // Impostor
            sortedRoleInfo.Where(role => role.CustomRoleType == CustomRoleTypes.Impostor).Do(info =>
            {
                SetupRoleOptions(info);
                info.OptionCreator?.Invoke();
            });

            DefaultShapeshiftCooldown = FloatOptionItem.Create(111, "DefaultShapeshiftCooldown", new(5f, 999f, 5f), 15f, TabGroup.ImpostorRoles, false)
                .SetHeader(true)
                .SetValueFormat(OptionFormat.Seconds);
            CanMakeMadmateCount = IntegerOptionItem.Create(112, "CanMakeMadmateCount", new(0, 15, 1), 0, TabGroup.ImpostorRoles, false)
                .SetValueFormat(OptionFormat.Times);

            // Madmate, Crewmate, Neutral
            sortedRoleInfo.Where(role => role.CustomRoleType != CustomRoleTypes.Impostor).Do(info =>
            {
                SetupRoleOptions(info);
                info.OptionCreator?.Invoke();
            });
            // Madmate Common Options
            MadmateCanFixLightsOut = BooleanOptionItem.Create(113, "MadmateCanFixLightsOut", false, TabGroup.ImpostorRoles, false)
                .SetHeader(true);
            MadmateCanFixComms = BooleanOptionItem.Create(114, "MadmateCanFixComms", false, TabGroup.ImpostorRoles, false);
            MadmateHasImpostorVision = BooleanOptionItem.Create(115, "MadmateHasImpostorVision", false, TabGroup.ImpostorRoles, false);
            MadmateCanSeeKillFlash = BooleanOptionItem.Create(116, "MadmateCanSeeKillFlash", false, TabGroup.ImpostorRoles, false);
            MadmateCanSeeOtherVotes = BooleanOptionItem.Create(117, "MadmateCanSeeOtherVotes", false, TabGroup.ImpostorRoles, false);
            MadmateCanSeeDeathReason = BooleanOptionItem.Create(118, "MadmateCanSeeDeathReason", false, TabGroup.ImpostorRoles, false);
            MadmateRevengeCrewmate = BooleanOptionItem.Create(119, "MadmateExileCrewmate", false, TabGroup.ImpostorRoles, false);
            MadmateVentCooldown = FloatOptionItem.Create(120, "MadmateVentCooldown", new(0f, 180f, 5f), 0f, TabGroup.ImpostorRoles, false)
                .SetValueFormat(OptionFormat.Seconds);
            MadmateVentMaxTime = FloatOptionItem.Create(121, "MadmateVentMaxTime", new(0f, 180f, 5f), 0f, TabGroup.ImpostorRoles, false)
                .SetValueFormat(OptionFormat.Seconds);

            // Add-Ons
            SetupRoleOptions(122, TabGroup.Addons, CustomRoles.Lovers, assignCountRule: new(2, 2, 2));
            LastImpostor.SetupCustomOption();
            Watcher.SetupCustomOption();
            Sloth.SetupCustomOption();
            Flash.SetupCustomOption();
            Torch.SetupCustomOption();
            Workhorse.SetupCustomOption();
            #endregion

            KillFlashDuration = FloatOptionItem.Create(2, "KillFlashDuration", new(0.1f, 0.45f, 0.05f), 0.3f, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetValueFormat(OptionFormat.Seconds)
                .SetGameMode(CustomGameMode.Standard);

            // HideAndSeek
            SetupRoleOptions(3, TabGroup.MainSettings, CustomRoles.HASFox, customGameMode: CustomGameMode.HideAndSeek);
            SetupRoleOptions(4, TabGroup.MainSettings, CustomRoles.HASTroll, customGameMode: CustomGameMode.HideAndSeek);
            AllowCloseDoors = BooleanOptionItem.Create(5, "AllowCloseDoors", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.HideAndSeek);
            KillDelay = FloatOptionItem.Create(6, "HideAndSeekWaitingTime", new(0f, 180f, 5f), 10f, TabGroup.MainSettings, false)
                .SetValueFormat(OptionFormat.Seconds)
                .SetGameMode(CustomGameMode.HideAndSeek);
            //IgnoreCosmetics = CustomOption.Create(101002, Color.white, "IgnoreCosmetics", false)
            //    .SetGameMode(CustomGameMode.HideAndSeek);
            IgnoreVent = BooleanOptionItem.Create(7, "IgnoreVent", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.HideAndSeek);

            // リアクターの時間制御
            SabotageTimeControl = BooleanOptionItem.Create(8, "SabotageTimeControl", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.Standard);
            PolusReactorTimeLimit = FloatOptionItem.Create(9, "PolusReactorTimeLimit", new(1f, 60f, 1f), 30f, TabGroup.MainSettings, false).SetParent(SabotageTimeControl)
                .SetValueFormat(OptionFormat.Seconds)
                .SetGameMode(CustomGameMode.Standard);
            AirshipReactorTimeLimit = FloatOptionItem.Create(10, "AirshipReactorTimeLimit", new(1f, 90f, 1f), 60f, TabGroup.MainSettings, false).SetParent(SabotageTimeControl)
                .SetValueFormat(OptionFormat.Seconds)
                .SetGameMode(CustomGameMode.Standard);

            // サボタージュのクールダウン変更
            ModifySabotageCooldown = BooleanOptionItem.Create(11, "ModifySabotageCooldown", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.Standard);
            SabotageCooldown = FloatOptionItem.Create(12, "SabotageCooldown", new(1f, 60f, 1f), 30f, TabGroup.MainSettings, false).SetParent(ModifySabotageCooldown)
                .SetValueFormat(OptionFormat.Seconds)
                .SetGameMode(CustomGameMode.Standard);

            // 停電の特殊設定
            LightsOutSpecialSettings = BooleanOptionItem.Create(13, "LightsOutSpecialSettings", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipViewingDeckLightsPanel = BooleanOptionItem.Create(14, "DisableAirshipViewingDeckLightsPanel", false, TabGroup.MainSettings, false).SetParent(LightsOutSpecialSettings)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipGapRoomLightsPanel = BooleanOptionItem.Create(15, "DisableAirshipGapRoomLightsPanel", false, TabGroup.MainSettings, false).SetParent(LightsOutSpecialSettings)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipCargoLightsPanel = BooleanOptionItem.Create(16, "DisableAirshipCargoLightsPanel", false, TabGroup.MainSettings, false).SetParent(LightsOutSpecialSettings)
                .SetGameMode(CustomGameMode.Standard);
            BlockDisturbancesToSwitches = BooleanOptionItem.Create(17, "BlockDisturbancesToSwitches", false, TabGroup.MainSettings, false).SetParent(LightsOutSpecialSettings)
                .SetGameMode(CustomGameMode.Standard);

            // マップ改造
            MapModification = BooleanOptionItem.Create(18, "MapModification", false, TabGroup.MainSettings, false)
                .SetHeader(true);
            AirShipVariableElectrical = BooleanOptionItem.Create(19, "AirShipVariableElectrical", false, TabGroup.MainSettings, false).SetParent(MapModification);
            DisableAirshipMovingPlatform = BooleanOptionItem.Create(20, "DisableAirshipMovingPlatform", false, TabGroup.MainSettings, false).SetParent(MapModification);
            ResetDoorsEveryTurns = BooleanOptionItem.Create(21, "ResetDoorsEveryTurns", false, TabGroup.MainSettings, false).SetParent(MapModification);
            DoorsResetMode = StringOptionItem.Create(22, "DoorsResetMode", EnumHelper.GetAllNames<DoorsReset.ResetMode>(), 0, TabGroup.MainSettings, false).SetParent(ResetDoorsEveryTurns);
            DisableFungleSporeTrigger = BooleanOptionItem.Create(23, "DisableFungleSporeTrigger", false, TabGroup.MainSettings, false).SetParent(MapModification);

            // タスク無効化
            DisableTasks = BooleanOptionItem.Create(24, "DisableTasks", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);
            DisableSwipeCard = BooleanOptionItem.Create(25, "DisableSwipeCardTask", false, TabGroup.MainSettings, false).SetParent(DisableTasks)
                .SetGameMode(CustomGameMode.All);
            DisableSubmitScan = BooleanOptionItem.Create(26, "DisableSubmitScanTask", false, TabGroup.MainSettings, false).SetParent(DisableTasks)
                .SetGameMode(CustomGameMode.All);
            DisableUnlockSafe = BooleanOptionItem.Create(27, "DisableUnlockSafeTask", false, TabGroup.MainSettings, false).SetParent(DisableTasks)
                .SetGameMode(CustomGameMode.All);
            DisableUploadData = BooleanOptionItem.Create(28, "DisableUploadDataTask", false, TabGroup.MainSettings, false).SetParent(DisableTasks)
                .SetGameMode(CustomGameMode.All);
            DisableStartReactor = BooleanOptionItem.Create(29, "DisableStartReactorTask", false, TabGroup.MainSettings, false).SetParent(DisableTasks)
                .SetGameMode(CustomGameMode.All);
            DisableResetBreaker = BooleanOptionItem.Create(30, "DisableResetBreakerTask", false, TabGroup.MainSettings, false).SetParent(DisableTasks)
                .SetGameMode(CustomGameMode.All);

            //デバイス無効化
            DisableDevices = BooleanOptionItem.Create(31, "DisableDevices", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.Standard);
            DisableSkeldDevices = BooleanOptionItem.Create(32, "DisableSkeldDevices", false, TabGroup.MainSettings, false).SetParent(DisableDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableSkeldAdmin = BooleanOptionItem.Create(33, "DisableSkeldAdmin", false, TabGroup.MainSettings, false).SetParent(DisableSkeldDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableSkeldCamera = BooleanOptionItem.Create(34, "DisableSkeldCamera", false, TabGroup.MainSettings, false).SetParent(DisableSkeldDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableMiraHQDevices = BooleanOptionItem.Create(35, "DisableMiraHQDevices", false, TabGroup.MainSettings, false).SetParent(DisableDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableMiraHQAdmin = BooleanOptionItem.Create(36, "DisableMiraHQAdmin", false, TabGroup.MainSettings, false).SetParent(DisableMiraHQDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableMiraHQDoorLog = BooleanOptionItem.Create(37, "DisableMiraHQDoorLog", false, TabGroup.MainSettings, false).SetParent(DisableMiraHQDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisablePolusDevices = BooleanOptionItem.Create(38, "DisablePolusDevices", false, TabGroup.MainSettings, false).SetParent(DisableDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisablePolusAdmin = BooleanOptionItem.Create(39, "DisablePolusAdmin", false, TabGroup.MainSettings, false).SetParent(DisablePolusDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisablePolusCamera = BooleanOptionItem.Create(40, "DisablePolusCamera", false, TabGroup.MainSettings, false).SetParent(DisablePolusDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisablePolusVital = BooleanOptionItem.Create(41, "DisablePolusVital", false, TabGroup.MainSettings, false).SetParent(DisablePolusDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipDevices = BooleanOptionItem.Create(42, "DisableAirshipDevices", false, TabGroup.MainSettings, false).SetParent(DisableDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipCockpitAdmin = BooleanOptionItem.Create(43, "DisableAirshipCockpitAdmin", false, TabGroup.MainSettings, false).SetParent(DisableAirshipDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipRecordsAdmin = BooleanOptionItem.Create(44, "DisableAirshipRecordsAdmin", false, TabGroup.MainSettings, false).SetParent(DisableAirshipDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipCamera = BooleanOptionItem.Create(45, "DisableAirshipCamera", false, TabGroup.MainSettings, false).SetParent(DisableAirshipDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableAirshipVital = BooleanOptionItem.Create(46, "DisableAirshipVital", false, TabGroup.MainSettings, false).SetParent(DisableAirshipDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableFungleDevices = BooleanOptionItem.Create(47, "DisableFungleDevices", false, TabGroup.MainSettings, false).SetParent(DisableDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableFungleVital = BooleanOptionItem.Create(48, "DisableFungleVital", false, TabGroup.MainSettings, false).SetParent(DisableFungleDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableDevicesIgnoreConditions = BooleanOptionItem.Create(49, "IgnoreConditions", false, TabGroup.MainSettings, false).SetParent(DisableDevices)
                .SetGameMode(CustomGameMode.Standard);
            DisableDevicesIgnoreImpostors = BooleanOptionItem.Create(50, "IgnoreImpostors", false, TabGroup.MainSettings, false).SetParent(DisableDevicesIgnoreConditions)
                .SetGameMode(CustomGameMode.Standard);
            DisableDevicesIgnoreMadmates = BooleanOptionItem.Create(51, "IgnoreMadmates", false, TabGroup.MainSettings, false).SetParent(DisableDevicesIgnoreConditions)
                .SetGameMode(CustomGameMode.Standard);
            DisableDevicesIgnoreNeutrals = BooleanOptionItem.Create(52, "IgnoreNeutrals", false, TabGroup.MainSettings, false).SetParent(DisableDevicesIgnoreConditions)
                .SetGameMode(CustomGameMode.Standard);
            DisableDevicesIgnoreCrewmates = BooleanOptionItem.Create(53, "IgnoreCrewmates", false, TabGroup.MainSettings, false).SetParent(DisableDevicesIgnoreConditions)
                .SetGameMode(CustomGameMode.Standard);
            DisableDevicesIgnoreAfterAnyoneDied = BooleanOptionItem.Create(54, "IgnoreAfterAnyoneDied", false, TabGroup.MainSettings, false).SetParent(DisableDevicesIgnoreConditions)
                .SetGameMode(CustomGameMode.Standard);

            // ランダムマップ
            RandomMapsMode = BooleanOptionItem.Create(55, "RandomMapsMode", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);
            AddedTheSkeld = BooleanOptionItem.Create(56, "AddedTheSkeld", false, TabGroup.MainSettings, false).SetParent(RandomMapsMode)
                .SetGameMode(CustomGameMode.All);
            AddedMiraHQ = BooleanOptionItem.Create(57, "AddedMIRAHQ", false, TabGroup.MainSettings, false).SetParent(RandomMapsMode)
                .SetGameMode(CustomGameMode.All);
            AddedPolus = BooleanOptionItem.Create(58, "AddedPolus", false, TabGroup.MainSettings, false).SetParent(RandomMapsMode)
                .SetGameMode(CustomGameMode.All);
            AddedTheAirShip = BooleanOptionItem.Create(59, "AddedTheAirShip", false, TabGroup.MainSettings, false).SetParent(RandomMapsMode)
                .SetGameMode(CustomGameMode.All);
            AddedTheFungle = BooleanOptionItem.Create(60, "AddedTheFungle", false, TabGroup.MainSettings, false).SetParent(RandomMapsMode);

            // ランダムスポーン
            EnableRandomSpawn = BooleanOptionItem.Create(61, "RandomSpawn", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);
            RandomSpawn.SetupCustomOption();

            // ボタン回数同期
            SyncButtonMode = BooleanOptionItem.Create(62, "SyncButtonMode", false, TabGroup.MainSettings, false)
                    .SetHeader(true)
                    .SetGameMode(CustomGameMode.Standard);
            SyncedButtonCount = IntegerOptionItem.Create(63, "SyncedButtonCount", new(0, 100, 1), 10, TabGroup.MainSettings, false).SetParent(SyncButtonMode)
                .SetValueFormat(OptionFormat.Times)
                .SetGameMode(CustomGameMode.Standard);

            // 投票モード
            VoteMode = BooleanOptionItem.Create(64, "VoteMode", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.Standard);
            WhenSkipVote = StringOptionItem.Create(65, "WhenSkipVote", voteModes[0..3], 0, TabGroup.MainSettings, false).SetParent(VoteMode)
                .SetGameMode(CustomGameMode.Standard);
            WhenSkipVoteIgnoreFirstMeeting = BooleanOptionItem.Create(66, "WhenSkipVoteIgnoreFirstMeeting", false, TabGroup.MainSettings, false).SetParent(WhenSkipVote)
                .SetGameMode(CustomGameMode.Standard);
            WhenSkipVoteIgnoreNoDeadBody = BooleanOptionItem.Create(67, "WhenSkipVoteIgnoreNoDeadBody", false, TabGroup.MainSettings, false).SetParent(WhenSkipVote)
                .SetGameMode(CustomGameMode.Standard);
            WhenSkipVoteIgnoreEmergency = BooleanOptionItem.Create(68, "WhenSkipVoteIgnoreEmergency", false, TabGroup.MainSettings, false).SetParent(WhenSkipVote)
                .SetGameMode(CustomGameMode.Standard);
            WhenNonVote = StringOptionItem.Create(69, "WhenNonVote", voteModes, 0, TabGroup.MainSettings, false).SetParent(VoteMode)
                .SetGameMode(CustomGameMode.Standard);
            WhenTie = StringOptionItem.Create(70, "WhenTie", tieModes, 0, TabGroup.MainSettings, false).SetParent(VoteMode)
                .SetGameMode(CustomGameMode.Standard);

            // 全員生存時の会議時間
            AllAliveMeeting = BooleanOptionItem.Create(71, "AllAliveMeeting", false, TabGroup.MainSettings, false);
            AllAliveMeetingTime = FloatOptionItem.Create(72, "AllAliveMeetingTime", new(1f, 300f, 1f), 10f, TabGroup.MainSettings, false).SetParent(AllAliveMeeting)
                .SetValueFormat(OptionFormat.Seconds);

            // 生存人数ごとの緊急会議
            AdditionalEmergencyCooldown = BooleanOptionItem.Create(73, "AdditionalEmergencyCooldown", false, TabGroup.MainSettings, false);
            AdditionalEmergencyCooldownThreshold = IntegerOptionItem.Create(74, "AdditionalEmergencyCooldownThreshold", new(1, 15, 1), 1, TabGroup.MainSettings, false).SetParent(AdditionalEmergencyCooldown)
                .SetValueFormat(OptionFormat.Players);
            AdditionalEmergencyCooldownTime = FloatOptionItem.Create(75, "AdditionalEmergencyCooldownTime", new(1f, 60f, 1f), 1f, TabGroup.MainSettings, false).SetParent(AdditionalEmergencyCooldown)
                .SetValueFormat(OptionFormat.Seconds);

            // 転落死
            LadderDeath = BooleanOptionItem.Create(76, "LadderDeath", false, TabGroup.MainSettings, false)
                .SetHeader(true);
            LadderDeathChance = StringOptionItem.Create(77, "LadderDeathChance", rates[1..], 0, TabGroup.MainSettings, false).SetParent(LadderDeath);

            // 通常モードでかくれんぼ用
            StandardHAS = BooleanOptionItem.Create(78, "StandardHAS", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.Standard);
            StandardHASWaitingTime = FloatOptionItem.Create(89, "StandardHASWaitingTime", new(0f, 180f, 2.5f), 10f, TabGroup.MainSettings, false).SetParent(StandardHAS)
                .SetValueFormat(OptionFormat.Seconds)
                .SetGameMode(CustomGameMode.Standard);

            // その他
            FixFirstKillCooldown = BooleanOptionItem.Create(90, "FixFirstKillCooldown", false, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);
            DisableTaskWin = BooleanOptionItem.Create(91, "DisableTaskWin", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            NoGameEnd = BooleanOptionItem.Create(92, "NoGameEnd", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            GhostCanSeeOtherRoles = BooleanOptionItem.Create(93, "GhostCanSeeOtherRoles", true, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            GhostCanSeeOtherTasks = BooleanOptionItem.Create(94, "GhostCanSeeOtherTasks", true, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            GhostCanSeeOtherVotes = BooleanOptionItem.Create(95, "GhostCanSeeOtherVotes", true, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            GhostCanSeeDeathReason = BooleanOptionItem.Create(96, "GhostCanSeeDeathReason", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            GhostIgnoreTasks = BooleanOptionItem.Create(97, "GhostIgnoreTasks", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            CommsCamouflage = BooleanOptionItem.Create(98, "CommsCamouflage", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);

            // プリセット対象外
            AutoDisplayLastResult = BooleanOptionItem.Create(99, "AutoDisplayLastResult", true, TabGroup.MainSettings, false)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);
            AutoDisplayKillLog = BooleanOptionItem.Create(100, "AutoDisplayKillLog", true, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            SuffixMode = StringOptionItem.Create(101, "SuffixMode", suffixModes, 0, TabGroup.MainSettings, true)
                .SetGameMode(CustomGameMode.All);
            HideGameSettings = BooleanOptionItem.Create(102, "HideGameSettings", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            ColorNameMode = BooleanOptionItem.Create(103, "ColorNameMode", false, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            ChangeNameToRoleInfo = BooleanOptionItem.Create(104, "ChangeNameToRoleInfo", true, TabGroup.MainSettings, false)
                .SetGameMode(CustomGameMode.All);
            RoleAssigningAlgorithm = StringOptionItem.Create(105, "RoleAssigningAlgorithm", RoleAssigningAlgorithms, 0, TabGroup.MainSettings, true)
                .SetGameMode(CustomGameMode.All)
                .RegisterUpdateValueEvent(
                    (object obj, OptionItem.UpdateValueEventArgs args) => IRandom.SetInstanceById(args.CurrentValue));

            ApplyDenyNameList = BooleanOptionItem.Create(106, "ApplyDenyNameList", true, TabGroup.MainSettings, true)
                .SetHeader(true)
                .SetGameMode(CustomGameMode.All);
            KickPlayerFriendCodeNotExist = BooleanOptionItem.Create(107, "KickPlayerFriendCodeNotExist", false, TabGroup.MainSettings, true)
                .SetGameMode(CustomGameMode.All);
            ApplyBanList = BooleanOptionItem.Create(108, "ApplyBanList", true, TabGroup.MainSettings, true)
                .SetGameMode(CustomGameMode.All);

            DebugModeManager.SetupCustomOption();

            OptionSaver.Load();

            IsLoaded = true;
        }

        public static void SetupRoleOptions(SimpleRoleInfo info) =>
            SetupRoleOptions(info.ConfigId, info.Tab, info.RoleName, info.AssignInfo.AssignCountRule);
        public static void SetupRoleOptions(int id, TabGroup tab, CustomRoles role, IntegerValueRule assignCountRule = null, CustomGameMode customGameMode = CustomGameMode.Standard)
        {
            if (role.IsVanilla()) return;
            assignCountRule ??= new(1, 15, 1);

            var spawnOption = IntegerOptionItem.Create(id, role.ToString(), new(0, 100, 10), 0, tab, false)
                .SetColor(Utils.GetRoleColor(role))
                .SetValueFormat(OptionFormat.Percent)
                .SetHeader(true)
                .SetGameMode(customGameMode) as IntegerOptionItem;
            var countOption = IntegerOptionItem.Create(id + 1, "Maximum", assignCountRule, assignCountRule.Step, tab, false)
                .SetParent(spawnOption)
                .SetValueFormat(OptionFormat.Players)
                .SetGameMode(customGameMode);

            CustomRoleSpawnChances.Add(role, spawnOption);
            CustomRoleCounts.Add(role, countOption);
        }
        public class OverrideTasksData
        {
            public static Dictionary<CustomRoles, OverrideTasksData> AllData = new();
            public CustomRoles Role { get; private set; }
            public int IdStart { get; private set; }
            public OptionItem doOverride;
            public OptionItem assignCommonTasks;
            public OptionItem numLongTasks;
            public OptionItem numShortTasks;

            public OverrideTasksData(int idStart, TabGroup tab, CustomRoles role)
            {
                this.IdStart = idStart;
                this.Role = role;
                Dictionary<string, string> replacementDic = new() { { "%role%", Utils.GetRoleName(role) } };
                doOverride = BooleanOptionItem.Create(idStart++, "doOverride", false, tab, false).SetParent(CustomRoleSpawnChances[role])
                    .SetValueFormat(OptionFormat.None);
                doOverride.ReplacementDictionary = replacementDic;
                assignCommonTasks = BooleanOptionItem.Create(idStart++, "assignCommonTasks", true, tab, false).SetParent(doOverride)
                    .SetValueFormat(OptionFormat.None);
                assignCommonTasks.ReplacementDictionary = replacementDic;
                numLongTasks = IntegerOptionItem.Create(idStart++, "roleLongTasksNum", new(0, 99, 1), 3, tab, false).SetParent(doOverride)
                    .SetValueFormat(OptionFormat.Pieces);
                numLongTasks.ReplacementDictionary = replacementDic;
                numShortTasks = IntegerOptionItem.Create(idStart++, "roleShortTasksNum", new(0, 99, 1), 3, tab, false).SetParent(doOverride)
                    .SetValueFormat(OptionFormat.Pieces);
                numShortTasks.ReplacementDictionary = replacementDic;

                if (!AllData.ContainsKey(role)) AllData.Add(role, this);
                else Logger.Warn("重複したCustomRolesを対象とするOverrideTasksDataが作成されました", "OverrideTasksData");
            }
            public static OverrideTasksData Create(int idStart, TabGroup tab, CustomRoles role)
            {
                return new OverrideTasksData(idStart, tab, role);
            }
            public static OverrideTasksData Create(SimpleRoleInfo roleInfo, int idOffset)
            {
                return new OverrideTasksData(roleInfo.ConfigId + idOffset, roleInfo.Tab, roleInfo.RoleName);
            }
        }
    }
}