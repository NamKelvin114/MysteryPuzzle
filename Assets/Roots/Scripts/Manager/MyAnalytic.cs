using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityTimer;

public class MyAnalytic : Singleton<MyAnalytic>
{
    private void Start()
    {
        if (this != Instance)
        {
            return;
        }

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                    InitalizeFirebase();
                    FetchDataAsync();
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
    }


    public bool IsFetchComplete { get; set; }
    private const string LEVEL_DELAY_SHOW_ADS = "LEVEL_DELAY_SHOW_AD";
    private const string TIME_DELAY_SHOW_AD = "TIME_DELAY_SHOW_AD";
    private const string FIRST_OPEN_DELAY_SHOW_AD = "FIRST_OPEN_DELAY_SHOW_AD";
    private const string CURRENT_VERSION_ANDROID = "CURRENT_VERSION_ANDROID";
    private const string CURRENT_VERSION_IOS = "CURRENT_VERSION_IOS";

    private const string ANDROID_UPDATE_DESCRIPTION = "ANDROID_UPDATE_DESCRIPTION";
    private const string IOS_UPDATE_DESCRIPTION = "IOS_UPDATE_DESCRIPTION";
    private const string TURN_ON_CROSS_ADS = "TURN_ON_CROSS_ADS";
    private const string TURNON_ADS = "TURNON_ADS";
    private const string ONLY_USE_ADMOB = "ONLY_USE_ADMOB";
    private const string ONLY_USE_ADMOB_IOS = "ONLY_USE_ADMOB_IOS";
    private const string ONLY_USE_MAX = "ONLY_USE_MAX";
    private const string ONLY_USE_MAX_IOS = "ONLY_USE_MAX_IOS";
    private const string BANNED_USER = "BANNED_USER";
    private const string TURN_ON_NOTIFICATION_UPDATE = "TURN_ON_NOTIFICATION_UPDATE";
    private const string TURN_ON_EASTER = "TURN_ON_EASTER";
    private const string TIME_CACHE_LEADERBOARD = "TIME_CACHE_LEADERBOARD";
    private const string PERCENT_ADS_IOS = "PERCENT_ADS_IOS";

    private const string PERCENT_ADS = "PERCENT_ADS";

    //private const string ANDROID_APP_ID = "ANDROID_APP_ID";
    private const string ANDROID_BANNER_ID = "ANDROID_BANNER_ID";
    private const string ANDROID_INTERTITIAL_ID = "ANDROID_INTERTITIAL_ID";
    private const string ANDROID_REWARDED_ID = "ANDROID_REWARDED_ID";

    //private const string IOS_APP_ID = "IOS_APP_ID";
    private const string IOS_BANNER_ID = "IOS_BANNER_ID";
    private const string IOS_INTERTITIAL_ID = "IOS_INTERTITIAL_ID";
    private const string IOS_REWARDED_ID = "IOS_REWARDED_ID";

    #region

    private const string LEVEL_SHOW_INTER_ADS = "LEVEL_SHOW_INTER_ADS";
    private const string IS_TIME_VALENTINE = "IS_TIME_VALENTINE";
    private const string TIME_INTER_AD_SHOW_DELAY = "TIME_INTER_AD_SHOW_DELAY";
    private const string INTER_AD_SHOW_COUNT_IN_NEW_APP = "INTER_AD_SHOW_COUNT_IN_NEW_APP";
    private const string IS_INTER_ADS_LOSE = "IS_INTER_ADS_LOSE";
    private const string TIME_SHOW_INTER_ADS_LOSE_DELAY = "TIME_SHOW_INTER_ADS_LOSE_DELAY";

    #endregion

    private void InitalizeFirebase()
    {
        _defaults.Add(INTER_AD_SHOW_COUNT_IN_NEW_APP, 1);

        _defaults.Add(IS_TIME_VALENTINE, false);
        _defaults.Add(LEVEL_SHOW_INTER_ADS, 1);
        _defaults.Add(TIME_INTER_AD_SHOW_DELAY, 1);
        _defaults.Add(IS_INTER_ADS_LOSE, "true");
        _defaults.Add(TIME_SHOW_INTER_ADS_LOSE_DELAY, 1);

        _defaults.Add(LEVEL_DELAY_SHOW_ADS, 2);
        _defaults.Add(FIRST_OPEN_DELAY_SHOW_AD, 5);
        _defaults.Add(TIME_DELAY_SHOW_AD, 30);
        _defaults.Add(ONLY_USE_ADMOB, "true");
        _defaults.Add(ONLY_USE_ADMOB_IOS, "true");
        _defaults.Add(PERCENT_ADS, "100_0"); // %admob1_%admob2
        _defaults.Add(PERCENT_ADS_IOS, "100_0"); // %admob1_%admob2

        //_defaults.Add(ANDROID_APP_ID, "ca-app-pub-8566745611252640~1453413163");
        _defaults.Add(ANDROID_BANNER_ID, "ca-app-pub-8566745611252640/5145246163");
        _defaults.Add(ANDROID_INTERTITIAL_ID, "ca-app-pub-8566745611252640/7827249828");
        _defaults.Add(ANDROID_REWARDED_ID, "ca-app-pub-8566745611252640/6266756146");

        //_defaults.Add(IOS_APP_ID, "ca-app-pub-8566745611252640~9748567129");
        _defaults.Add(IOS_BANNER_ID, "ca-app-pub-8566745611252640/4496240442");
        _defaults.Add(IOS_INTERTITIAL_ID, "ca-app-pub-8566745611252640/9556995431");
        _defaults.Add(IOS_REWARDED_ID, "ca-app-pub-8566745611252640/2972880566");

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(_defaults);
    }

    public Task FetchDataAsync()
    {
        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWith(FetchComplete);
    }

    private readonly Dictionary<string, object> _defaults = new Dictionary<string, object>();

    private void FetchComplete(Task fetchTask)
    {
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        Debug.Log("CHECK 1");
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:

                        Debug.LogError("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.LogError("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.LogError("Latest Fetch call still pending.");
                break;
        }

        if (fetchTask.IsCanceled)
        {
        }
        else if (fetchTask.IsFaulted)
        {
        }
        else if (fetchTask.IsCompleted)
        {
        }

        Debug.Log("CHECK TimeValentine");
        Data.isTimeValentine = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(IS_TIME_VALENTINE)
            .BooleanValue;
        Debug.Log("CHECK 2");
        Data.levelshowinterads = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(LEVEL_SHOW_INTER_ADS).StringValue);
        Debug.Log("CHECK 3");
        Data.timeinteradshowdelay = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(TIME_INTER_AD_SHOW_DELAY).StringValue);
        Debug.Log("CHECK 4");
        Data.timeshowinteradslosedelay = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(TIME_SHOW_INTER_ADS_LOSE_DELAY).StringValue);
        Debug.Log("CHECK 5");
        Data.isinterads = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(IS_INTER_ADS_LOSE)
            .BooleanValue;
        Debug.Log("CHECK 1");
        Data.interadshowcountinnewapp = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(INTER_AD_SHOW_COUNT_IN_NEW_APP).StringValue);
        Debug.Log("CHECK 6");
        Data.delayshowAds = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(LEVEL_DELAY_SHOW_ADS).StringValue);
        Debug.Log("LEVEL_DELAY_SHOW_ADS: " + Data.delayshowAds);
        Data.timedelayShowAds = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(TIME_DELAY_SHOW_AD).StringValue);
        Debug.Log("TIME_DELAY_SHOW_AD :" + Data.timedelayShowAds);
        Data.levelpassshowad = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(FIRST_OPEN_DELAY_SHOW_AD).StringValue);
        Debug.Log("FIRST_OPEN_DELAY_SHOW_AD :" + Data.levelpassshowad);
        Data.bannedUser = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(BANNED_USER).StringValue;
        Debug.Log("CHECK 10");
        Data.turnOnUpdate = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
            .GetValue(TURN_ON_NOTIFICATION_UPDATE).BooleanValue;
        Debug.Log("CHECK 11");
        Data.turnOnCrossAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(TURN_ON_CROSS_ADS)
            .BooleanValue;
        Debug.Log("CHECK 12");
        Data.easterEventFlag = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(TURN_ON_EASTER)
            .BooleanValue;
        Debug.Log("CHECK 13");


        try
        {
            Data.timeCacheLeaderboard = int.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue(TIME_CACHE_LEADERBOARD).StringValue);
            Debug.Log("----------------------" + Data.timeCacheLeaderboard);
            Debug.Log("---------------");
            Debug.Log("---------------");
            Debug.Log("---------------");
            Debug.Log("----------------------" + Data.levelshowinterads);
            Debug.Log("----------------------" + Data.timeinteradshowdelay);
            Debug.Log("----------------------" + Data.timeshowinteradslosedelay);
            Debug.Log("----------------------" + Data.isinterads);
            Debug.Log("----------------------" + Data.interadshowcountinnewapp);
            if (!string.IsNullOrEmpty(Data.bannedUser))
            {
                BridgeData.Instance.BannedUsers = Data.bannedUser.Split('_').ToList();
            }
        }
        catch (System.Exception)
        {
            Data.timeCacheLeaderboard = 600;
            BridgeData.Instance.BannedUsers = new List<string>();
        }

        if (Utils.IsMobile)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Data.currentVersion = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                    .GetValue(CURRENT_VERSION_ANDROID).StringValue;
                Data.updateDescription = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                    .GetValue(ANDROID_UPDATE_DESCRIPTION).StringValue;

                string percent = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(PERCENT_ADS)
                    .StringValue;
                try
                {
                    var result = percent.Split('_');
                    Data.percentAdmob1 = int.Parse(result[0]);
                    Data.percentAdmob2 = int.Parse(result[1]);

                    int value = UnityEngine.Random.Range(0, 100 + 1);
                    bool flag = value <= Data.percentAdmob1;

                    // var appId = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(ANDROID_APP_ID).StringValue.Split('_');
                    // if (appId.Length > 1)
                    // {
                    //     Data.appId = flag ? appId[0] : appId[1];
                    // }
                    // else if (appId.Length > 0)
                    // {
                    //     Data.appId = appId[0];
                    // }

                    var bannerId = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                        .GetValue(ANDROID_BANNER_ID).StringValue.Split('_');
                    if (bannerId.Length > 1)
                    {
                        Data.bannerId = flag ? bannerId[0] : bannerId[1];
                    }
                    else if (bannerId.Length > 0)
                    {
                        Data.bannerId = bannerId[0];
                    }

                    var intertitialId = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                        .GetValue(ANDROID_INTERTITIAL_ID).StringValue.Split('_');
                    if (intertitialId.Length > 1)
                    {
                        Data.intertitialId = flag ? intertitialId[0] : intertitialId[1];
                    }
                    else if (intertitialId.Length > 0)
                    {
                        Data.intertitialId = intertitialId[0];
                    }

                    var rewardId = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                        .GetValue(ANDROID_REWARDED_ID).StringValue.Split('_');
                    if (rewardId.Length > 1)
                    {
                        Data.rewardedId = flag ? rewardId[0] : rewardId[1];
                    }
                    else if (rewardId.Length > 0)
                    {
                        Data.rewardedId = rewardId[0];
                    }
                }
                catch (Exception e)
                {
                    Data.percentAdmob1 = 100;
                    Data.percentAdmob2 = 0;
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Data.currentVersion = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                    .GetValue(CURRENT_VERSION_IOS).StringValue;
                Data.updateDescription = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                    .GetValue(IOS_UPDATE_DESCRIPTION).StringValue;

                string percent = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(PERCENT_ADS_IOS)
                    .StringValue;

                try
                {
                    var result = percent.Split('_');
                    Data.percentAdmob1 = int.Parse(result[0]);
                    Data.percentAdmob2 = int.Parse(result[1]);
                    int value = UnityEngine.Random.Range(0, 100 + 1);
                    bool flag = value <= Data.percentAdmob1;

                    // var appId = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(IOS_APP_ID).StringValue.Split('_');
                    // if (appId.Length > 1)
                    // {
                    //     Data.appId = flag ? appId[0] : appId[1];
                    // }
                    // else if (appId.Length > 0)
                    // {
                    //     Data.appId = appId[0];
                    // }

                    var bannerId = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(IOS_BANNER_ID)
                        .StringValue.Split('_');
                    if (bannerId.Length > 1)
                    {
                        Data.bannerId = flag ? bannerId[0] : bannerId[1];
                    }
                    else if (bannerId.Length > 0)
                    {
                        Data.bannerId = bannerId[0];
                    }

                    var intertitialId = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                        .GetValue(IOS_INTERTITIAL_ID).StringValue.Split('_');
                    if (intertitialId.Length > 1)
                    {
                        Data.intertitialId = flag ? intertitialId[0] : intertitialId[1];
                    }
                    else if (intertitialId.Length > 0)
                    {
                        Data.intertitialId = intertitialId[0];
                    }

                    var rewardId = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(IOS_REWARDED_ID)
                        .StringValue.Split('_');
                    if (rewardId.Length > 1)
                    {
                        Data.rewardedId = flag ? rewardId[0] : rewardId[1];
                    }
                    else if (rewardId.Length > 0)
                    {
                        Data.rewardedId = rewardId[0];
                    }
                }
                catch (Exception e)
                {
                    Data.percentAdmob1 = 100;
                    Data.percentAdmob2 = 0;
                }
            }
        }
        else
        {
            Data.currentVersion = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue(CURRENT_VERSION_ANDROID).StringValue;
            Data.updateDescription = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                .GetValue(ANDROID_UPDATE_DESCRIPTION).StringValue;
            Debug.Log("ANDROID_UPDATE_DESCRIPTION: " + Data.updateDescription);
        }

        IsFetchComplete = true;
    }
}