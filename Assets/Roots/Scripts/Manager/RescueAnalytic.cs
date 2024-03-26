using Firebase.Analytics;

public static class RescueAnalytic
{
    #region key

    private const string EVENT_UNLOCK_SKIN_HERO = "unlock_skin_hero";
    private const string EVENT_CLAIM_DAILREWARD = "daily_reward_claim";
    private const string EVENT_CLAIM_DAILREWARD_BY_REWARD = "daily_reward_claim_by_ad_reward";
    private const string EVENT_LEVEL_COMPLETE = "level_complete";
    private const string EVENT_LEVEL_FAILD = "level_faild";
    private const string EVENT_LEVEL_START = "level_start";
    private const string EVENT_FIRST_LEVEL_START = "level_first_start";
    private const string EVENT_LEVEL_SKIP = "level_skip";
    private const string EVENT_LEVEL_SKIP_HARDMODE = "level_skip_hardmode";
    private const string EVENT_AD_BANNER_REQUEST = "ad_banner_request";
    private const string EVENT_AD_BANNER_IMPRESSION = "ad_banner_impression";
    private const string EVENT_AD_INTERSTITIAL_REQUEST = "ad_interstitial_request";
    private const string EVENT_AD_INTERSTITIAL_IMPRESSION = "ad_interstitial_impression";
    private const string EVENT_AD_REWARD_REQUEST = "ad_reward_request";
    private const string EVENT_AD_REWARD_IMPRESSION = "ad_reward_impression";
    private const string EVENT_BUILD_CASTLE = "build_castle";
    private const string EVENT_ACHIEVEMENT = "achievement";
    private const string EVENT_CLAIM_SKIN = "process_claim_skin";
    private const string EVENT_ALL_HERO_UNLOCKED = "all_hero_unlocked";
    private const string EVENT_ALL_CASTLE_BUILDED = "all_castle_builded";
    private const string EVENT_DOUBLE_GOLD_UNLOCKED = "double_gold_unlocked";
    private const string EVENT_HARD_LEVEL_COMPLETE = "hard_level_complete";
    private const string EVENT_HARD_LEVEL_FAILD = "hard_level_faild";
    private const string EVENT_HARD_LEVEL_START = "hard_level_start";
    private const string EVENT_SHARE_FB = "share_fb";

    private const string PARAM_EASTER_REWARD = "easter_reward";
    private const string PARAM_LEVEL_INDEX = "level_index";
    private const string PARAM_EVENT_LEVEL_INDEX = "level_index";
    private const string PARAM_SKIN_HERO = "skin_hero";
    private const string PARAM_CURRENT_CURRENCY = "current_currency";
    private const string PARAM_EVENT_CURRENT_CURRENCY = "current_currency";
    private const string PARAM_DAY_DAILYREWARD = "day_dailyreward";
    private const string PARAM_DAY_EVENTDAILYREWARD = "day_dailyreward";
    private const string PARAM_AD_LOCATION = "ad_location";
    private const string PARAM_SKIP_LOCATION = "skip_location";
    private const string PARAM_CASTLE_ID = "castle_id";
    private const string PARAM_CASTLE_STAR = "castle_star";
    private const string PARAM_ACHIEVEMENT_NAME = "achievement_name";
    private const string PARAM_ACHIEVEMENT_LEVEL = "achievement_level";

    #endregion

    #region Analytic

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventLevelStart(int levelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{levelIndex}";
        Parameter[] paramLevelStart =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_LEVEL_INDEX, levelIndex),
            new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_LEVEL_START, paramLevelStart);
        if (!Data.GetFirstStartLevel(levelIndex) && !Config.IsDebug)
        {
            FirebaseAnalytics.LogEvent(EVENT_FIRST_LEVEL_START, paramLevelStart);
            Data.SetFirstStartLevel(levelIndex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventLevelComplete(int levelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{levelIndex}";
        Parameter[] paramLevelComplete =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_LEVEL_INDEX, levelIndex),
            new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_LEVEL_COMPLETE, paramLevelComplete);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventLevelFaild(int levelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{levelIndex}";
        Parameter[] paramLevelComplete =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_LEVEL_INDEX, levelIndex),
            new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_LEVEL_FAILD, paramLevelComplete);
    }


    /// <summary>
    /// 
    /// </summary>
    public static void LogEventAdBannerRequest()
    {
        if (!Utils.IsMobile) return;
        FirebaseAnalytics.LogEvent(EVENT_AD_BANNER_REQUEST);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="location">location click</param>
    /// <param name="currency">current currency</param>
    /// <param name="level">current level</param>
    public static void LogEventAdBannerImpression(int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_AD_LOCATION, ""), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_AD_BANNER_IMPRESSION, param);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void LogEventAdInterstitialRequest()
    {
        if (!Utils.IsMobile) return;
        FirebaseAnalytics.LogEvent(EVENT_AD_INTERSTITIAL_REQUEST);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventAdInterstitialImpression(int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_AD_LOCATION, ""), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_AD_INTERSTITIAL_IMPRESSION, param);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void LogEventAdRewardRequest()
    {
        if (!Utils.IsMobile) return;
        FirebaseAnalytics.LogEvent(EVENT_AD_REWARD_REQUEST);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void LogEventAdRewardImpression(int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_AD_LOCATION, ""), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_AD_REWARD_IMPRESSION, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameSkinHero"></param>
    /// <param name="currency">current currency</param>
    /// <param name="level">current level</param>
    public static void LogEventUnlockSkinHero(string nameSkinHero, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_SKIN_HERO, nameSkinHero), new Parameter(PARAM_LEVEL_INDEX, level),
            new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };
        FirebaseAnalytics.LogEvent(EVENT_UNLOCK_SKIN_HERO, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventClaimDailyReward(int day, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_DAY_DAILYREWARD, day), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_CLAIM_DAILREWARD, param);
    }

    public static void LogEventClaimEventDailyReward(int day, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_DAY_EVENTDAILYREWARD, day), new Parameter(PARAM_EVENT_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_EVENT_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_CLAIM_DAILREWARD, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventClaimDailyRewardByAdReward(int day, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_DAY_DAILYREWARD, day), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_CLAIM_DAILREWARD_BY_REWARD, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventLevelSkip(int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_SKIP_LOCATION, ""), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_LEVEL_SKIP, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventLevelSkipHardMode(int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_SKIP_LOCATION, ""), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_LEVEL_SKIP_HARDMODE, param);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    /// <param name="castleId"></param>
    /// <param name="castleStar"></param>
    public static void LogEventBuildCastle(int currency, int level, int castleId, int castleStar)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_CURRENT_CURRENCY, currency), new Parameter(PARAM_LEVEL_INDEX, level),
            new Parameter(PARAM_CASTLE_ID, castleId),
            new Parameter(PARAM_CASTLE_STAR, castleStar),
        };
        FirebaseAnalytics.LogEvent(EVENT_BUILD_CASTLE, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelAchievement"></param>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    /// <param name="nameAchievement"></param>
    public static void LogEventAchievement(string nameAchievement, int levelAchievement, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_ACHIEVEMENT_NAME, nameAchievement),
            new Parameter(PARAM_ACHIEVEMENT_LEVEL, levelAchievement),
            new Parameter(PARAM_CURRENT_CURRENCY, currency), new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_ACHIEVEMENT, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameSkin"></param>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventProcessClaimSkin(string nameSkin, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_SKIN_HERO, nameSkin), new Parameter(PARAM_CURRENT_CURRENCY, currency),
            new Parameter(PARAM_LEVEL_INDEX, level),
        };
        FirebaseAnalytics.LogEvent(EVENT_CLAIM_SKIN, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventAllHeroUnlocked(int currency, int level)
    {
        if (!Utils.IsMobile || Data.AllHeroUnlocked) return;

        Data.AllHeroUnlocked = true;
        Parameter[] param =
            { new Parameter(PARAM_CURRENT_CURRENCY, currency), new Parameter(PARAM_LEVEL_INDEX, level), };

        FirebaseAnalytics.LogEvent(EVENT_ALL_HERO_UNLOCKED, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventAllCastleBuilded(int currency, int level)
    {
        if (!Utils.IsMobile || Data.AllCastleBuilded) return;

        Data.AllCastleBuilded = true;
        Parameter[] param =
            { new Parameter(PARAM_CURRENT_CURRENCY, currency), new Parameter(PARAM_LEVEL_INDEX, level), };

        FirebaseAnalytics.LogEvent(EVENT_ALL_CASTLE_BUILDED, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventDoubleGoldPurchase(int currency, int level)
    {
        if (!Utils.IsMobile || Data.DoubleGold) return;

        Parameter[] param =
            { new Parameter(PARAM_CURRENT_CURRENCY, currency), new Parameter(PARAM_LEVEL_INDEX, level), };
        FirebaseAnalytics.LogEvent(EVENT_DOUBLE_GOLD_UNLOCKED, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentLevelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventHardLevelStart(int currentLevelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{currentLevelIndex}";
        Parameter[] paramLevelStart =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_HARD_LEVEL_START, paramLevelStart);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentLevelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventHardLevelComplete(int currentLevelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{currentLevelIndex}";
        Parameter[] paramLevelComplete =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_HARD_LEVEL_COMPLETE, paramLevelComplete);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentLevelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventHardLevelFaild(int currentLevelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{currentLevelIndex}";
        Parameter[] paramLevelComplete =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_HARD_LEVEL_FAILD, paramLevelComplete);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameReward"></param>
    /// <param name="currency"></param>
    /// <param name="level"></param>
    public static void LogEventClaimEasterReward(string nameReward, int currency, int level)
    {
        if (!Utils.IsMobile) return;
        Parameter[] param =
        {
            new Parameter(PARAM_EASTER_REWARD, nameReward), new Parameter(PARAM_LEVEL_INDEX, level),
            new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };
        FirebaseAnalytics.LogEvent(EVENT_UNLOCK_SKIN_HERO, param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <param name="currency"></param>
    public static void LogEventShareFb(int levelIndex, int currency)
    {
        if (!Utils.IsMobile) return;
        var name = $"level_{levelIndex}";
        Parameter[] paramLevelStart =
        {
            new Parameter(FirebaseAnalytics.ParameterLevelName, name), new Parameter(PARAM_LEVEL_INDEX, levelIndex),
            new Parameter(PARAM_CURRENT_CURRENCY, currency),
        };

        FirebaseAnalytics.LogEvent(EVENT_SHARE_FB, paramLevelStart);
    }

    #endregion
}