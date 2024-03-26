#pragma warning disable 649
using System;
using System.Collections.Generic;
using UnityEngine;
using Worldreaver.Root.Attribute;

/// <summary>
/// golbal game config
/// </summary>
public class Config : ScriptableObject
{
    private static Config instance;
    private static Config Instance => instance ? instance : (instance = Resources.Load<Config>(Constants.CONFIG));

    [SerializeField] private int maxLevelCanReach; // 0 -> maxLevelCanReach - 1
    [SerializeField] private int maxLevelWithOutTutotial;
    [SerializeField] private int levelFragment = 5;

    [Space] [SerializeField] private string linkGroupFacebook;

    [Header("android")] [SerializeField] private string admobAndroidAppId = "ca-app-pub-8566745611252640~7605802011";

    [SerializeField] private string admobAndroidBannerId = "ca-app-pub-8566745611252640/1972231762";
    [SerializeField] private string admobAndroidIntertitialId = "ca-app-pub-8566745611252640/7031086948";
    [SerializeField] private string admobAndroidRewardedId = "ca-app-pub-8566745611252640/7507946707";
    [Header("ios")] [SerializeField] private string admobIosAppId = "ca-app-pub-8566745611252640~6570418919";
    [SerializeField] private string admobIosBannerId = "ca-app-pub-8566745611252640/5236870538";
    [SerializeField] private string admobIosIntertitialId = "ca-app-pub-8566745611252640/2631173907";
    [SerializeField] private string admobIosRewardedId = "ca-app-pub-8566745611252640/8168664945";

    [Space] [SerializeField] private int[] costPurchaseByStars;

    [Space] [SerializeField] private int[] rateConfigs;

    [Space] [SerializeField] private int valueIncreateWatchAdsWinLevel = 5;

    [Space] [SerializeField] private List<int> levelSkips;

    [Header("event")] [SerializeField] private int dayStartEvent;
    [SerializeField] private int monthStartEvent;
    [SerializeField] private int yearStartEvent;

    [SerializeField] private int dayEndEvent;
    [SerializeField] private int monthEndEvent;
    [SerializeField] private int yearEndEvent;
    [SerializeField] private int levelShowCutscene1;
    [SerializeField] private int levelShowCutscene4;
    [SerializeField] private int levelShowCutsceneBefore5;
    [SerializeField] private int levelShowCutscene5;
    [SerializeField] private int levelShowCutscene6;
    [SerializeField] private int levelShowCutscene7;
    [SerializeField] private int levelShowCutscene8;
    [SerializeField] private int levelShowCutscene9;
    [SerializeField] private int maxIdRoomItem;
    [SerializeField] private int maxRoomCanReach;
    [Space] [SerializeField] private bool isDebug;


    #region public api
    public static int MaxLevelWithOutTutotial => Instance.maxLevelWithOutTutotial;

    public static int MaxLevelCanReach => Instance.maxLevelCanReach;
    public static int LevelFragment => Instance.levelFragment;

    public static string AdmobAppId
    {
        get
        {
#if UNITY_ANDROID
            return Instance.admobAndroidAppId;
#elif UNITY_IOS
            return Instance.admobIosAppId;
#else
            return "";
#endif
        }
    }

    public static string AdmobBannerId
    {
        get
        {
#if UNITY_ANDROID
            return Instance.admobAndroidBannerId;
#elif UNITY_IOS
            return Instance.admobIosBannerId;
#else
            return "";
#endif
        }
    }

    public static string AdmobInterstitialId
    {
        get
        {
#if UNITY_ANDROID
            return Instance.admobAndroidIntertitialId;
#elif UNITY_IOS
            return Instance.admobIosIntertitialId;
#else
            return "";
#endif
        }
    }

    public static string AdmobRewardedId
    {
        get
        {
#if UNITY_ANDROID
            return Instance.admobAndroidRewardedId;
#elif UNITY_IOS
            return Instance.admobIosRewardedId;
#else
            return "";
#endif
        }
    }

    public static int[] RateConfigs => instance.rateConfigs;
    public static int ValueIncreateWatchAdsWinLevel => instance.valueIncreateWatchAdsWinLevel;
    public static List<int> LevelSkips => instance.levelSkips;
    public static int[] CostPurchaseByStars => instance.costPurchaseByStars;
    public static int DayStartEvent => Instance.dayStartEvent;
    public static int DayEndEvent => Instance.dayEndEvent;
    public static int MonthStartEvent => Instance.monthStartEvent;
    public static int MonthEndEvent => Instance.monthEndEvent;
    public static int YearStartEvent => Instance.yearStartEvent;
    public static int YearEndEvent => Instance.yearEndEvent;
    public static int LevelShowCutscene1 => Instance.levelShowCutscene1;
    public static int LevelShowCutscene4 => Instance.levelShowCutscene4;
    public static int LevelShowCutsceneBefore5 => Instance.levelShowCutsceneBefore5;
    public static int LevelShowCutscene5 => Instance.levelShowCutscene5;
    public static int LevelShowCutscene6 => Instance.levelShowCutscene6;
    public static int LevelShowCutscene7 => Instance.levelShowCutscene7;

    public static int LevelShowCutscene8 => Instance.levelShowCutscene8;
    public static int LevelShowCutscene9 => Instance.levelShowCutscene9;

    public static int MaxIdRoomItem => Instance.maxIdRoomItem;
    public static int MaxRoomCanReach => Instance.maxRoomCanReach;
    public static bool IsDebug => Instance.isDebug;

    public string LinkGroupFacebook => linkGroupFacebook;
    #endregion
}
