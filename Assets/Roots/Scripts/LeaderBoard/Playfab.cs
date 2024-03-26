using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
// using Pancake.Common;
public static class Playfab
{
    public static string TitleId = "2B9BC";
    public static string displayName = "";
    public static string playfabID = "";
    public static bool isLogin = false;
    public static int MaxResultsCount = 100;
    private const string CUSTOM_ID_STORE_KEY = "PLAYFAB_CUSTOM_ID_AUTH";

    public static void Login(Action<LoginResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = TitleId;

        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = Data.CustomId,
                TitleId = TitleId,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true,
                }
            },
            (result) =>
            {
                playfabID = result.PlayFabId;
                isLogin = true;
                if (result.InfoResultPayload.PlayerProfile.DisplayName != null)
                    displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
                callbackResult?.Invoke(result);
            },
            callbackError
        );
    }

    public static void UpdateDisplayName(string displayName, Action<UpdateUserTitleDisplayNameResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest { DisplayName = displayName },
            (e) =>
            {
                displayName = e.DisplayName;
                callbackResult(e);
            },
            callbackError
        );
    }

    public static void UpdateScore(int score, string nameTable, Action<UpdatePlayerStatisticsResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        if (displayName == null || displayName == "") return;
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = nameTable, Value = score } } },
            callbackResult,
            callbackError
        );
    }

    public static void GetPlayerProfile(Action<GetPlayerProfileResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), callbackResult, callbackError);
    }

    public static void GetLeaderboard(string nameTable, int startPosition, int maxResultsCountCustom = 100, Action<GetLeaderboardResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = nameTable,
                StartPosition = startPosition,
                MaxResultsCount = maxResultsCountCustom,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true, ShowLocations = true }
            },
            callbackResult,
            callbackError
        );
    }

    public static void LoginWithFacebook(string tokenId, Action<LoginResult> onLoginSuccessFacebook)
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = TitleId;

        var request = new LoginWithFacebookRequest() { AccessToken = tokenId, CreateAccount = true, };
        PlayFabClientAPI.LoginWithFacebook(request, onLoginSuccessFacebook, error => { });
    }
    public static void getMyRankLeadBoard(string statisticName, Action<GetLeaderboardAroundPlayerResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = statisticName,
            PlayFabId = playfabID,
            MaxResultsCount = 1,
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, callbackResult, callbackError);
    }
    public static void UpdateDisPlayName(string name, Action<UpdateUserTitleDisplayNameResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, (result) =>
        {
            displayName = result.DisplayName;
            callbackResult(result);

        }, callbackError);
    }
    public static void UpdateScoreLevelLB(Action<UpdatePlayerStatisticsResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        Playfab.UpdateScore(Utils.CurrentLevel + 1, PlayfabConstants.LEVEL_STATISTIC_NAME, callbackResult, callbackError);
    }
    public static void UpdateScoreLevelCountryLB(Action<UpdatePlayerStatisticsResult> callbackResult = null, Action<PlayFabError> callbackError = null)
    {
        var str = Utils.getCodeLeaderBoardCountry(displayName, PlayfabConstants.LEVEL_STATISTIC_NAME);
        Playfab.UpdateScore(Utils.CurrentLevel + 1, str, callbackResult, callbackError);
    }

}
