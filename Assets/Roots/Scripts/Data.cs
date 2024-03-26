using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Data
{
    private const string SOUND_STATE_KEY = "sound_state";

    public static bool UserSound
    {
        get => GetBool(SOUND_STATE_KEY, true);
        set => SetBool(SOUND_STATE_KEY, value);
    }


    private const string MUSIC_STATE_KEY = "music_state";

    public static bool UserMusic
    {
        get => GetBool(MUSIC_STATE_KEY, true);
        set => SetBool(MUSIC_STATE_KEY, value);
    }

    private const string FIRST_START_LEVEL = "first_start_level_";

    public static void SetFirstStartLevel(int level)
    {
        SetBool(FIRST_START_LEVEL + level.ToString(), true);
    }

    public static bool GetFirstStartLevel(int level)
    {
        return
            GetBool(FIRST_START_LEVEL + level.ToString(), false);
    }


    private const string VIBRATE_STATE_KEY = "vibrate_state";

    public static bool UserVibrate
    {
        get => GetBool(VIBRATE_STATE_KEY, false);
        set => SetBool(VIBRATE_STATE_KEY, value);
    }


    private const string CheckPopupTaskDay = "check_popuptask_day";

    public static int PopupTaskDay
    {
        get => PlayerPrefs.GetInt(CheckPopupTaskDay, 0);
        set => PlayerPrefs.SetInt(CheckPopupTaskDay, value);
    }


    private const string CheckPopupTaskMonth = "check_popuptask_month";

    public static int PopupTaskMonth
    {
        get => PlayerPrefs.GetInt(CheckPopupTaskMonth, 0);
        set => PlayerPrefs.SetInt(CheckPopupTaskMonth, value);
    }


    private const string CheckPopupTaskYear = "check_popuptask_year";

    public static int PopupTaskYear
    {
        get => PlayerPrefs.GetInt(CheckPopupTaskYear, 0);
        set => PlayerPrefs.SetInt(CheckPopupTaskYear, value);
    }

    public static string IdCheckUnlocked = "";

    public static bool IsUnlocked
    {
        get => GetBool(IdCheckUnlocked, false);
        set => SetBool(IdCheckUnlocked, value);
    }

    public static string IdCollected = "";

    public static bool IsCollected
    {
        get => GetBool(IdCollected, false);
        set => SetBool(IdCollected, value);
    }

    private const string RATE_STATE_KEY = "rate_us";

    public static bool UserRated
    {
        get => GetBool(RATE_STATE_KEY, false);
        set => SetBool(RATE_STATE_KEY, value);
    }


    private const string COUNT_SHOW_RATE = "count_show_rate";

    public static int CountShowRate
    {
        get => PlayerPrefs.GetInt(COUNT_SHOW_RATE, 0);
        set => PlayerPrefs.SetInt(COUNT_SHOW_RATE, value);
    }


    private const string LAST_LEVEL_SHOW_RATE = "last_level_show_rate";

    public static int LastLevelShowRate
    {
        get => PlayerPrefs.GetInt(LAST_LEVEL_SHOW_RATE, 0);
        set => PlayerPrefs.SetInt(LAST_LEVEL_SHOW_RATE, value);
    }


    private const string REMOVE_ADS_STATE = "remove_ads";

    public static bool RemoveAds
    {
        get => GetBool(REMOVE_ADS_STATE, false);
        set => SetBool(REMOVE_ADS_STATE, value);
    }


    private const string UNLOCKED_HERO_STATE = "unlock_hero";

    public static bool UnlockedHero
    {
        get => GetBool(UNLOCKED_HERO_STATE, false);
        set => SetBool(UNLOCKED_HERO_STATE, value);
    }

    private const string TASK_LIST = "task_list";

    public static string TaskList
    {
        get => GetString(TASK_LIST, "");
        set => SetString(TASK_LIST, value);
    }

    private const string TASK_LIST_PROGRESS = "task_list_progress";

    public static string TaskListProgress
    {
        get => GetString(TASK_LIST_PROGRESS, "");
        set => SetString(TASK_LIST_PROGRESS, value);
    }


    private const string VIP_STATE = "user_vip";

    public static bool UserVip
    {
        get => GetBool(VIP_STATE, false);
        set => SetBool(VIP_STATE, value);
    }


    private const string WIN_PROGRESS = "win_progress";

    public static float WinProgress
    {
        get => PlayerPrefs.GetFloat(WIN_PROGRESS, 0f);
        set => PlayerPrefs.SetFloat(WIN_PROGRESS, value);
    }


    private const string ALL_HERO_UNLOCKED = "all_hero_unlocked";

    public static bool AllHeroUnlocked
    {
        get => GetBool(ALL_HERO_UNLOCKED, false);
        set => SetBool(ALL_HERO_UNLOCKED, value);
    }


    private const string ALL_CASTLE_BUILDED = "all_castle_builded";

    public static bool AllCastleBuilded
    {
        get => GetBool(ALL_CASTLE_BUILDED, false);
        set => SetBool(ALL_CASTLE_BUILDED, value);
    }


    private const string DONT_SHOW_UPDATE = "dont_show_update";

    public static bool DontShowUpdateAgain
    {
        get => GetBool(DONT_SHOW_UPDATE, false);
        set => SetBool(DONT_SHOW_UPDATE, value);
    }

    public static bool DontShowUpdate = false;


    private const string CASTLE_CURRENT_MAP = "castle_current_map";

    public static int CastleCurrentMap
    {
        get => PlayerPrefs.GetInt(CASTLE_CURRENT_MAP, 0);
        set => PlayerPrefs.SetInt(CASTLE_CURRENT_MAP, value);
    }


    private const string UNLOCKED_DOUBLE_GOLD = "double_gold";

    public static bool DoubleGold
    {
        get => GetBool(UNLOCKED_DOUBLE_GOLD, false);
        set => SetBool(UNLOCKED_DOUBLE_GOLD, value);
    }


    private const string WEEK_NUMBER = "week_number";

    public static int WeekNumber
    {
        get => PlayerPrefs.GetInt(WEEK_NUMBER, 0);
        set => PlayerPrefs.SetInt(WEEK_NUMBER, value);
    }


    private const string USER_NAME = "user_name";

    public static string UserName
    {
        get => PlayerPrefs.GetString(USER_NAME, "");
        set => PlayerPrefs.SetString(USER_NAME, value);
    }


    private const string USER_COUNTRY_CODE = "user_country_code";

    public static string UserCountryCode
    {
        get => PlayerPrefs.GetString(USER_COUNTRY_CODE, "");
        set => PlayerPrefs.SetString(USER_COUNTRY_CODE, value);
    }

    private const string SHOW_NOTI_HARDMODE = "noti_hardmode";

    public static bool NotiHardMode
    {
        get => GetBool(SHOW_NOTI_HARDMODE, false);
        set => SetBool(SHOW_NOTI_HARDMODE, value);
    }

    private const string CUTSCENE = "cut_scene{0}";
    public static bool GetStateCutScene(int index) => GetBool(string.Format(CUTSCENE, index), false);
    public static void SetStateCutScene(int index, bool value) => SetBool(string.Format(CUTSCENE, index), value);

    public const string CURRENT_WORLD_KEY = "current_room";
    public const string CURRENT_ITEM_ID = "current_ID";
    public const string CURRENT_ROOM_ID = "current_roonm_ID";
    public const string CURRENT_MENU_WORLD_KEY = "current_menu_room";
    public const string CURRENT_LEVEL_MAP_ITEM = "current_level_map_item";

    public static int CurrentWorld
    {
        get => PlayerPrefs.GetInt(CURRENT_WORLD_KEY, 0);
        set => PlayerPrefs.SetInt(CURRENT_WORLD_KEY, value);
    }

    public static int CurrentItemID
    {
        get => PlayerPrefs.GetInt(CURRENT_ITEM_ID, 0);
        set => PlayerPrefs.SetInt(CURRENT_ITEM_ID, value);
    }

    public static int CurrentRoomID
    {
        get => PlayerPrefs.GetInt(CURRENT_ROOM_ID, 0);
        set => PlayerPrefs.SetInt(CURRENT_ROOM_ID, value);
    }

    public static int CurrentMenuWorld
    {
        get => PlayerPrefs.GetInt(CURRENT_MENU_WORLD_KEY, 0);
        set => PlayerPrefs.SetInt(CURRENT_MENU_WORLD_KEY, value);
    }

    public static int CurrentLevelMapItem
    {
        get => PlayerPrefs.GetInt(CURRENT_LEVEL_MAP_ITEM, 0);
        set => PlayerPrefs.SetInt(CURRENT_LEVEL_MAP_ITEM, value);
    }

    private const string CUSTOM_ID = "custom_id";

    private const string CURRENT_EGG = "current_egg";

    public static int CurrentEgg
    {
        get => PlayerPrefs.GetInt(CURRENT_EGG, 0);
        set => PlayerPrefs.SetInt(CURRENT_EGG, value);
    }

    private const string REWARD_EASTER = "reward_easter";

    public static bool GetStatusRewardEaster(int id)
    {
        return GetBool($"{REWARD_EASTER}_{id}");
    }

    public static void SetStatusRewardEaster(int id, bool state)
    {
        SetBool($"{REWARD_EASTER}_{id}", state);
    }

    private const string REWARD_TOP100_EASTER = "reward_top100_easter";

    public static bool RewardTop100Easter
    {
        get => GetBool(REWARD_TOP100_EASTER, false);
        set => SetBool(REWARD_TOP100_EASTER, value);
    }

    public static bool IsEventEasterStarted()
    {
        return false;
        var start = new DateTime(Config.YearStartEvent, Config.MonthStartEvent, Config.DayStartEvent);
        var end = new DateTime(Config.YearEndEvent, Config.MonthEndEvent, Config.DayEndEvent);
        return easterEventFlag && DateTime.UtcNow >= start && DateTime.UtcNow <= end;
    }

    public static bool CheckConditionRewardEventEaster()
    {
        if (!GetStatusRewardEaster(0) && CurrentEgg >= 50 || !GetStatusRewardEaster(1) && CurrentEgg >= 100 ||
            !GetStatusRewardEaster(2) && CurrentEgg >= 150 || !GetStatusRewardEaster(3) && CurrentEgg >= 300 ||
            !GetStatusRewardEaster(4) && CurrentEgg >= 500)
        {
            return true;
        }

        return false;
    }

    public static string CustomId
    {
        get
        {
            var id = PlayerPrefs.GetString(CUSTOM_ID, "");
            if (string.IsNullOrEmpty(id))
            {
                var deviceId = "";
                try
                {
                    deviceId = SystemInfo.deviceUniqueIdentifier;
                }
                catch (Exception e)
                {
                    deviceId = $"{Random.Range(100, 10000)}";
                }

                deviceId += Guid.NewGuid().ToString();
                PlayerPrefs.SetString(CUSTOM_ID, deviceId);
                id = deviceId;
            }

            return id;
        }
        set => PlayerPrefs.SetString(CUSTOM_ID, value);
    }

    private const string PLAYER_ID = "playerid";

    public static string PlayerId
    {
        get => PlayerPrefs.GetString(PLAYER_ID);
        set => PlayerPrefs.SetString(PLAYER_ID, value);
    }

    private const string DATA_VERSION = "data_version";

    public static int DataVersion
    {
        get => PlayerPrefs.GetInt(DATA_VERSION, 0);
        set => PlayerPrefs.SetInt(DATA_VERSION, value);
    }

    private const string COUNT_PLAY_LEVEL = "count_play_level";

    public static int CountPlayLevel
    {
        get => PlayerPrefs.GetInt(COUNT_PLAY_LEVEL, 0);
        set => PlayerPrefs.SetInt(COUNT_PLAY_LEVEL, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool GetBool(string key, bool defaultValue = false)
    {
        var value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public static void SetBool(string id, bool value)
    {
        PlayerPrefs.SetInt(id, value ? 1 : 0);
    }

    #region data

    public static int currentHero;
    public static int currentPrincess;
    public const string CURRENTHERO = "currenthero";
    public const string CURRENTPRINCESS = "currentprincess";

    public const string SAVEHERO = "savehero";
    public const string SAVEITEM = "saveitem";

    public const string SAVECASTLE = "savecastle";
    public const string SAVE_PRINCESS = "save_princess";

    public const string TOTALCOIN = "totalcoin";

    public const string TOTALGOLDMEDAL = "totalgoldmedal_summer";
    // public const string DATETIMESTART="datetimestart";

    public static int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
    public static void SetInt(string id, int value) => PlayerPrefs.SetInt(id, value);

    public static string DateTimeStart
    {
        get => GetString(Constants.DATE_TIME_START, "");
        set => SetString(Constants.DATE_TIME_START, value);
    }

    public static int TotalDays => (int)(DateTime.Now - DateTime.Parse(DateTimeStart)).TotalDays;

    // public static int TotalDays = 6;
    public static int TotalGoldMedal
    {
        get => GetInt(Constants.TOTAL_GOLD_MEDAL, 0);
        set
        {
            SetInt(Constants.TOTAL_GOLD_MEDAL, value);
            Playfab.UpdateScore(value, PlayfabConstants.EVENT_STATISTIC_NAME);
        }
    }

    #region

    public static int levelshowinterads = 2;
    public static int timeinteradshowdelay = 2;
    public static int interadshowcountinnewapp = 2;
    public static int timeshowinteradslosedelay = 2;
    public static bool isinterads = true;

    #endregion


    public static bool isTimeValentine = true;

    public static TimeSpan
        TimeToRescueParty => new DateTime(2022, 7, 10, 0, 0, 0) - DateTime.Now;

    public const string CURRENT_PET = "current_pet";
    public const string CURRENT_PET_LEVEL = "current_pet_level";

    public static int firsttime = 0;
    public static int levelpassshowad = 5;
    public static int delayshowAds = 2;
    public static string currentVersion = "";
    public static string updateDescription = "";
    public static bool turnOnCrossAds = false;
    public static bool easterEventFlag = true;
    public static float timedelayShowAds = 30;
    public const string FIRSTTIME = "firsttime";
    public static string bannedUser = "yenmoc";
    public static bool turnOnUpdate = false;
    public static int currentPet = -1;
    public static int petLevel = 1;
    public static int idEasterEgg = -1;
    public static int percentAdmob1 = 100;
    public static int percentAdmob2 = 0;

    //public static string appId = "";
    public static string bannerId = "";
    public static string intertitialId = "";
    public static string rewardedId = "";

    #region playfab

    public static int timeCacheLeaderboard = 600;
    public static DateTime oldTimeRequestLeaderboard = DateTime.Now;
    public static DateTime oldTimeRequestRankEaster = DateTime.Now;

    private const string LEADERBOARD_CACHE_TIME_KEY = "lb_cache_request";
    private const string RANK_EASTER_CACHE_TIME_KEY = "easter_cache_request";
    public static string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
    public static void SetString(string id, string value) => PlayerPrefs.SetString(id, value);

    public static string DateTimeStartRescueParty
    {
        get => GetString(Constants.DATE_TIME_START_RESCUE_PARTY, "");
        set => SetString(Constants.DATE_TIME_START_RESCUE_PARTY, value);
    }

    public static DateTime OldTimeRequestLeaderboard
    {
        get
        {
            var time = PlayerPrefs.GetString(LEADERBOARD_CACHE_TIME_KEY, DateTime.Now.ToString());
            DateTime.TryParse(time, out oldTimeRequestLeaderboard);
            return oldTimeRequestLeaderboard;
        }
        set
        {
            oldTimeRequestLeaderboard = value;
            PlayerPrefs.SetString(LEADERBOARD_CACHE_TIME_KEY, value.ToString());
        }
    }

    public static DateTime OldTimeRequestRankEaster
    {
        get
        {
            var time = PlayerPrefs.GetString(RANK_EASTER_CACHE_TIME_KEY, DateTime.Now.ToString());
            DateTime.TryParse(time, out oldTimeRequestRankEaster);
            return oldTimeRequestRankEaster;
        }
        set
        {
            oldTimeRequestRankEaster = value;
            PlayerPrefs.SetString(RANK_EASTER_CACHE_TIME_KEY, value.ToString());
        }
    }

    #endregion

    public static DateTime oldTimeShowAds = DateTime.Now;
    public static DateTime oldTimeShowAdsLose = DateTime.Now;

    #endregion
}

[Serializable]
public class Country
{
    public string businessName;
    public string businessWebsite;
    public string city;
    public string continent;
    public string country;
    public string countryCode;
    public string ipName;
    public string ipType;
    public string isp;
    public string lat;
    public string lon;
    public string org;
    public string query;
    public string region;
    public string status;
}