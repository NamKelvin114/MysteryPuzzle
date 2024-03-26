using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Pancake;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Utils
{
    public const int BASE_COIN = 100;
    private const string GAME_KEY = "gamee.prison";
    public const string COIN_KEY = GAME_KEY + ".coin";
    public const string CURRENT_LEVEL_KEY = "current_level_play";
    public const string CURRENT_HARD_LEVEL_KEY = "current_hard_level_play";
    public const string CURRENT_JIGSAW_LEVEL_KEY = "current_jigsaw_level_play";
    public const string MAX_LEVEL_KEY = "max_level_play";
    public const string IS_JIGSAW_MODE = "is_jigsaw_mode";
    public const string KEY_DAILY_REWARD = GAME_KEY + ".KEY_DAILY_REWARD";
    public const string KEY_EVENT_DAILY_REWARD = GAME_KEY + ".KEY_EVENT_DAILY_REWARD_SUMMER";
    public const string KEY_CURRENT_DAILY_GIFT = GAME_KEY + ".KEY_CURRENT_DAILY_GIFT";
    public const string KEY_CURRENT_EVENT_DAILY_GIFT = GAME_KEY + ".KEY_CURRENT_EVENT_DAILY_GIFT_SUMMER";
    public const string KEY_PLAYER_SKIN = GAME_KEY + ".player.skin";
    public const string KEY_HERO_SELECTED = GAME_KEY + ".hero.selected";
    public const string KEY_SKIN_NORMAL = GAME_KEY + ".skin.hero.normal";
    public const string KEY_SKIN_SWORD = GAME_KEY + ".skin.hero.sword";
    public const string Task_To_Load = " Task_To_Load";
    public const string Process_Task = " Process_Task";
    public const string GiftRewardAmount = " Gift_Reward_Amount";
    public const string DoneAllMission = "Done_All_Mission";
    public const string TaskMap = " Task_Map";
    public const string GiftOrder = "Gift_Order";
    public const string KEY_FIRST_OPEN_IN_DAY = "first_open_in_day";
    public const string KEY_BG = "key_background";


    public const string MAX_HARD_LEVEL_KEY = "max_hard_level_play";
    public const string IS_FIRST_TIME_PLAY = "is_first_time_play";
    public const string SHOW_CUTSCENE1 = "is_show_cutscene1";
    public const string SHOW_CUTSCENE3 = "is_show_cutscene3";
    public const string SHOW_CUTSCENE4 = "is_show_cutscene4";
    public const string SHOW_CUTSCENEBEFORE5 = "is_show_cutscenebefore5";
    public const string SHOW_CUTSCENE5 = "is_show_cutscene5";
    public const string SHOW_CUTSCENE6 = "is_show_cutscene6";
    public const string SHOW_CUTSCENE7 = "is_show_cutscene7";
    public const string SHOW_CUTSCENE8 = "is_show_cutscene8";
    public const string SHOW_CUTSCENE9 = "is_show_cutscene9";



    public const string TAG_STICKBARRIE = "StickBarrie";
    public const string TAG_LAVA = "Trap_Lava";
    public const string TAG_ICE_WATER = "Ice";
    public const string TAG_GAS = "Trap_Gas";
    public const string TAG_WIN = "Tag_Win";
    public const string TAG_STONE = "Tag_Stone";
    public const string TAG_CHEST = "Chest";
    public const string TAG_ITEM = "Item";
    public const string TAG_WALL_BOTTOM = "Wall_Bottom";
    public const string TAG_SWORD = "Sword";
    public const string TAG_DUMBBELL = "Dumbbell";
    public static int CoinReward => Data.DoubleGold ? BASE_COIN * 2 : BASE_COIN;

    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);
        set
        {
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value);
            Playfab.UpdateScoreLevelLB();
            Playfab.UpdateScoreLevelCountryLB();
        }
    }

    public static int CurrentHardLevel
    {
        get => PlayerPrefs.GetInt(CURRENT_HARD_LEVEL_KEY, 0);
        set => PlayerPrefs.SetInt(CURRENT_HARD_LEVEL_KEY, value);
    }

    public static int CurrentJigsawLevel
    {
        get => PlayerPrefs.GetInt(CURRENT_JIGSAW_LEVEL_KEY, 0);
        set => PlayerPrefs.SetInt(CURRENT_JIGSAW_LEVEL_KEY, value);
    }

    public static string CurrentBackGround
    {
        get => PlayerPrefs.GetString(KEY_BG, "");
        set => PlayerPrefs.SetString(KEY_BG, value);
    }

    public static int MaxLevel
    {
        get => PlayerPrefs.GetInt(MAX_LEVEL_KEY, 0);
        set => PlayerPrefs.SetInt(MAX_LEVEL_KEY, value);
    }

    public static int GetTaskProcess(ETaskType taskType)
    {
        var getTaskProcesss = PlayerPrefs.GetInt(taskType.ToString(), 0);
        return getTaskProcesss;
    }

    public static void SetTaskProcess(ETaskType taskType, int process)
    {
        Observer.CheckToActiveNotiBtnTask?.Invoke();
        PlayerPrefs.SetInt(taskType.ToString(), process);
    }

    public static string GetMapTaskProcess()
    {
        var getTaskProcesss = PlayerPrefs.GetString(TaskMap, "");
        return getTaskProcesss;
    }

    // public static void SetMapTaskProcess(ETaskMap taskMap)
    // {
    //     PlayerPrefs.SetString(TaskMap, taskMap.ToString());
    // }
    public static int ProcessTask
    {
        get => PlayerPrefs.GetInt(Process_Task, 0);
        set => PlayerPrefs.SetInt(Process_Task, value);
    }

    public static int OrderGift
    {
        get => PlayerPrefs.GetInt(GiftOrder, 0);
        set => PlayerPrefs.SetInt(GiftOrder, value);
    }

    public static int GiftTaskRewardAmount
    {
        get => PlayerPrefs.GetInt(GiftRewardAmount, 0);
        set => PlayerPrefs.SetInt(GiftRewardAmount, value);
    }

    public static bool DebugDoneAllMission
    {
        get => PlayerPrefs.GetInt(DoneAllMission, 0) == 1;
        set => PlayerPrefs.SetInt(DoneAllMission, value ? 1 : 0);
    }

    public static bool DoAllTask { get; set; } = false;
    public static bool IsDebugDaily { get; set; } = false;
    public static bool IsTurnOnAds { get; set; } = !Config.IsDebug;

    public static string TaskToLoad
    {
        get => PlayerPrefs.GetString(Task_To_Load, "");
        set => PlayerPrefs.SetString(Task_To_Load, value);
    }

    public static int MaxHardLevel
    {
        get => PlayerPrefs.GetInt(MAX_HARD_LEVEL_KEY, 0);
        set => PlayerPrefs.SetInt(MAX_HARD_LEVEL_KEY, value);
    }

    public static bool IsJigsawMode
    {
        get => PlayerPrefs.GetInt(IS_JIGSAW_MODE, 0) == 1;
        set => PlayerPrefs.SetInt(IS_JIGSAW_MODE, value ? 1 : 0);
    }

    public static int currentCoin = 0;
    public static bool pauseUpdateFetchIcon = false;
    public static int roomId;

    public static void UpdateCoin(int amount)
    {
        Observer.SaveCurrencyTotal?.Invoke();
        currentCoin += amount;
        Observer.CurrencyTotalChanged?.Invoke(true);
        Observer.ShowHideShopWarning?.Invoke();
    }

    public static int roomItemId;
    public static bool showNewWorld;
    public static bool isHardMode = false;
    public static string tempCountryCode = "";

    public const string CACHE_LEVEL_INDEX = "cache_level_index";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="level"></param>
    private static bool GetBool(string key, bool defaultValue = false)
    {
        var value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value > 0;
    }

    public static void SetBool(string id, bool value)
    {
        PlayerPrefs.SetInt(id, value ? 1 : 0);
    }

    public static bool IsFirstTimePLay
    {
        get => GetBool(IS_FIRST_TIME_PLAY, true);
        set => SetBool(IS_FIRST_TIME_PLAY, value);
    }

    public static bool IsShowCutscene1
    {
        get => GetBool(SHOW_CUTSCENE1, false);
        set => SetBool(SHOW_CUTSCENE1, value);
    }

    public static bool IsShowCutscene4
    {
        get => GetBool(SHOW_CUTSCENE4, false);
        set => SetBool(SHOW_CUTSCENE4, value);
    }

    public static bool IsShowCutsceneBefore5
    {
        get => GetBool(SHOW_CUTSCENEBEFORE5, false);
        set => SetBool(SHOW_CUTSCENEBEFORE5, value);
    }

    public static bool IsShowCutscene5
    {
        get => GetBool(SHOW_CUTSCENE5, false);
        set => SetBool(SHOW_CUTSCENE5, value);
    }

    public static bool IsShowCutscene6
    {
        get => GetBool(SHOW_CUTSCENE6, false);
        set => SetBool(SHOW_CUTSCENE6, value);
    }

    public static bool IsShowCutscene7
    {
        get => GetBool(SHOW_CUTSCENE7, false);
        set => SetBool(SHOW_CUTSCENE7, value);
    }
    public static bool IsShowCutscene8
    {
        get => GetBool(SHOW_CUTSCENE8, false);
        set => SetBool(SHOW_CUTSCENE8, value);
    }
    public static bool IsShowCutscene9
    {
        get => GetBool(SHOW_CUTSCENE9, false);
        set => SetBool(SHOW_CUTSCENE9, value);
    }

    public static void SetCacheLevelIndex(int index, int level)
    {
        PlayerPrefs.SetInt($"{CACHE_LEVEL_INDEX}_{index}", level);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int GetCacheLevelIndex(int index)
    {
        return PlayerPrefs.GetInt($"{CACHE_LEVEL_INDEX}_{index}", 0);
    }

    public static void LoadGameData()
    {
        curDailyGift = PlayerPrefs.GetInt(KEY_CURRENT_DAILY_GIFT, 1);
        if (curDailyGift > 7)
        {
            curDailyGift = 1;
        }

        if (Data.DateTimeStart == "")
        {
            Data.DateTimeStart = DateTime.Now.ToString();
        }

        curEventDailyGift = PlayerPrefs.GetInt(KEY_CURRENT_EVENT_DAILY_GIFT, 1);
        // curEventDailyGift= Data.TotalDays+1;
    }

    #region Daily reward
    public static bool IsClaimReward()
    {
        if (IsDebugDaily)
        {
            return false;
        }

        string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        return _key.Equals(SReward());
    }

    public static string SReward()
    {
        return PlayerPrefs.GetString(KEY_DAILY_REWARD, "");
    }

    public static void HasClaimReward()
    {
        string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        PlayerPrefs.SetString(KEY_DAILY_REWARD, _key);
        // PlayerPrefs.Save();
    }


    public static string curShirtDay7Gift = "KEY_CURRENT_DAY7_GIFT";

    public static void SaveNameOfDay7Gift(string name)
    {
        PlayerPrefs.SetString(curShirtDay7Gift, name);
    }

    public static string GetNameOfDay7Gift()
    {
        if (PlayerPrefs.HasKey(curShirtDay7Gift))
        {
            return PlayerPrefs.GetString(curShirtDay7Gift);
        }

        return null;
    }

    public static int curDailyGift;
    public static bool cantakegiftdaily;

    public static void SaveDailyGift()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_DAILY_GIFT, curDailyGift);
        //  PlayerPrefs.Save();
    }
    #endregion

    #region Valentine Daily reward
    public static bool IsFirstOpenInDay()
    {
        string key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        return !key.Equals(GetFirstOpenKey());
    }

    public static void SetFirstOpenKey()
    {
        string key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        PlayerPrefs.SetString(KEY_FIRST_OPEN_IN_DAY, key);
    }

    public static string GetFirstOpenKey()
    {
        return PlayerPrefs.GetString(KEY_FIRST_OPEN_IN_DAY, null);
    }

    public static bool IsClaimEventReward()
    {
        string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        return _key.Equals(SEventReward());
    }

    public static string SEventReward()
    {
        return PlayerPrefs.GetString(KEY_EVENT_DAILY_REWARD, "");
    }

    public static void HasClaimEventReward()
    {
        string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        PlayerPrefs.SetString(KEY_EVENT_DAILY_REWARD, _key);
        // PlayerPrefs.Save();
    }


    public static int curEventDailyGift;
    public static bool canTakeEventGiftDaily;

    public static void SaveEventDailyGift()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_EVENT_DAILY_GIFT, curEventDailyGift);
        //  PlayerPrefs.Save();
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool IsMobile => Application.platform == RuntimePlatform.Android ||
        Application.platform == RuntimePlatform.IPhonePlayer;

    public static bool CheckInternetConnection()
    {
#if UNITY_EDITOR
        return true;
#endif
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int ChangeScale(Vector2 self, Vector2 target)
    {
        //unit default facing right
        if (self.x < target.x)
        {
            return 1;
        }

        return -1;
    }

    public static string FormatTime(TimeSpan distance)
    {
        if (distance.Days > 0)
        {
            return string.Format("{0}d {1}h", distance.Days, distance.Hours);
        }
        else
        {
            return string.Format("{0:00}:{1:00}:{2:00}", distance.Hours, distance.Minutes, distance.Seconds);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void GotoStore()
    {
        var url = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            //url = "market://details?id=" + Application.identifier;
            url = "https://play.google.com/store/apps/details?id=com.gamee.pinmaster.adventure.savethegirl&hl=en_US";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //url = "itms-apps://itunes.apple.com/app/id" + Application.identifier;
            url = "https://apps.apple.com/us/app/id1538267929";
        }

#if UNITY_EDITOR
        if (string.IsNullOrEmpty(url))
        {
            //url = "market://details?id=" + Application.identifier;
            url = "https://play.google.com/store/apps/details?id=com.gamee.pinmaster.adventure.savethegirl&hl=en_US";
        }
#endif

        Application.OpenURL(url);
    }

    public static CountryCodeEF SplitCountry(string nameDisPlay)
    {
        if (nameDisPlay == null || nameDisPlay == "") return new CountryCodeEF("", "US");
        var str = nameDisPlay.Split('_');
        if (str.Length == 1)
        {
            return new CountryCodeEF(str[0], "VN");
        }

        var name = str[0];
        var code = str[1];
        return new CountryCodeEF(name, code);
    }

    public static string SplitLongString(string nameDisPlay, int maxLength, string charS = "...")
    {
        if (nameDisPlay.Length > maxLength)
        {
            nameDisPlay = nameDisPlay.Substring(0, maxLength) + charS;
        }

        return nameDisPlay;
    }

    public static string getCodeLeaderBoardCountry(string nameDisPlay, string StatisticName)
    {
        var split = SplitCountry(nameDisPlay);
        return StatisticName + "_" + split.countryCodes;
    }
}
