using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabHelper : Singleton<PlayfabHelper>
{
    [SerializeField] private string nameTableLeaderboard = "PRISON_LB";
    [SerializeField] private string nameTableEvent1 = "PRISON_LB_EVENT1";
    [SerializeField] private string nameTableEvent2 = "PRISON_LB_EVENT2";

    public string NameTableLeaderboard => nameTableLeaderboard;
    public string NameTableEvent1 => nameTableEvent1;
    public string NameTableEvent2 => nameTableEvent2;

    #region login

    [SerializeField] private string titleId = "9B398";

    public bool StatusLogin { get; private set; }
    public bool CompletedRun { get; private set; }

    public void ResetRun()
    {
        CompletedRun = false;
        StatusLogin = false;
    }

    public void RequestLogin()
    {
        ResetRun();
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = titleId;

        var request = new LoginWithCustomIDRequest {CustomId = Data.CustomId, CreateAccount = true,};
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private Action _actionCompleted;
    private Action _funcLogin;
    private bool _isBackup;

    public void FacebookLogin(string tokenId, Action actionCompleted, bool isBackup, Action funcLogin)
    {
        ResetRun();
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) PlayFabSettings.staticSettings.TitleId = titleId;

        var request = new LoginWithFacebookRequest() {AccessToken = tokenId, CreateAccount = true,};
        _actionCompleted = actionCompleted;
        _funcLogin = funcLogin;
        _isBackup = isBackup;
        PlayFabClientAPI.LoginWithFacebook(request, OnLoginSuccessFacebook, error => { });
    }

    private void OnLoginSuccessFacebook(LoginResult result)
    {
        _actionCompleted?.Invoke();
        LoadUserData(result.PlayFabId);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        // todo
        CompletedRun = true;
        StatusLogin = true;
        if (!string.IsNullOrEmpty(Data.UserName) || string.IsNullOrEmpty(Data.PlayerId))
        {
            GetPlayerProfile();
        }

        void GetPlayerProfile() { PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), Successs, _ => { }); }

        void Successs(GetPlayerProfileResult profileResult)
        {
            Data.PlayerId = profileResult.PlayerProfile.PlayerId;
            var displayName = profileResult.PlayerProfile.DisplayName;
            if (string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(Data.UserName))
            {
                UpdateUserDisplayName(Data.UserName, BridgeData.Instance.GetCountryCode(), _ => { }, error => { }); // update user display name
            }
        }

        // RequestLeaderboard(leaderboardResult =>
        // {
        //     foreach (var entry in leaderboardResult.Leaderboard)
        //     {
        //         if (entry.DisplayName != null)
        //         {
        //             Debug.Log(entry.Profile.Locations.Count);
        //             Debug.Log(entry.Profile.Locations[0].CountryCode.ToString());
        //             Debug.Log(entry.DisplayName);
        //             Debug.Log(entry.StatValue);
        //             Debug.Log(entry.Position);
        //         }
        //     }
        //
        // }, error => { }, 0 , 100);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        // todo
        CompletedRun = true;
        StatusLogin = false;
    }

    public void UpdateUserDisplayName(string newName, string countryCode, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {DisplayName = $"{newName}_{countryCode}"}, resultCallback, errorCallback);
    }

    #endregion

    public void SubmitScore(int score, string nameTable, Action<UpdatePlayerStatisticsResult> callbackResult, Action<PlayFabError> callbackError)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest {Statistics = new List<StatisticUpdate> {new StatisticUpdate {StatisticName = nameTable, Value = score}}},
            callbackResult,
            callbackError);
    }

    //Get the players with the top 10 high scores in the game
    public void RequestLeaderboard(
        string nameTable,
        Action<GetLeaderboardResult> callbackResult,
        Action<PlayFabError> callbackError,
        int startPosition = 0,
        int maxResultQuery = 100)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
            {
                StatisticName = nameTable,
                StartPosition = startPosition,
                MaxResultsCount = maxResultQuery,
                ProfileConstraints = new PlayerProfileViewConstraints() {ShowDisplayName = true, ShowLocations = true,}
            },
            callbackResult,
            callbackError);
    }

    public void SaveUserData()
    {
        DataController.instance.SaveData();

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {
                        "data_part1",
                        $"{Data.DataVersion}_{Utils.CurrentLevel}_{Utils.currentCoin}_{Utils.CurrentHardLevel}_{Utils.MaxLevel}_{Data.CurrentWorld}_{Data.CurrentMenuWorld}_{Utils.MaxHardLevel}_{Utils.curDailyGift}_{Data.UserSound}_{Data.UserMusic}_{Data.UserVibrate}_{Data.UserRated}_{Data.CountShowRate}_{Data.LastLevelShowRate}_{Data.RemoveAds}_{Data.UserVip}"
                    },
                    // {"DataVersion", $""},
                    // {"CurrentLevel", $""},
                    // {"currentCoin", $""},
                    // {"CurrentHardLevel", $""},
                    // {"MaxLevel", $""},
                    // {"CurrentRoom", $""},
                    // {"CurrentMenuRoom", $""},
                    // {"MaxHardLevel", $""},
                    // {"curDailyGift", $""},
                    // {"UserSound", $""},
                    // {"UserMusic", $""},
                    // {"UserVibrate", $""},
                    // {"UserRated", $""},
                    // {"CountShowRate", $""},
                    // {"LastLevelShowRate", $""},
                    // {"RemoveAds", $""},
                    // {"UserVip", $""},

                    {
                        "data_part2",
                        $"{Data.DontShowUpdateAgain}_{Data.CastleCurrentMap}_{Data.WeekNumber}_{Data.UserName}_{Data.UserCountryCode}_{Data.NotiHardMode}_{Data.DoubleGold}_{Data.CustomId}_{Data.PlayerId}_{Data.AllHeroUnlocked}_{Data.AllCastleBuilded}_{Data.WinProgress}"
                    },

                    // {"DontShowUpdateAgain", $""},
                    // {"CastleCurrentMap", $""},
                    // {"WeekNumber", $""},
                    // {"UserName", $""},
                    // {"UserCountryCode", $""},
                    // {"NotiHardMode", $""},
                    // {"DoubleGold", $""},
                    // {"CustomId", $""},
                    // {"PlayerId", $""},
                    // {"AllHeroUnlocked", $""},
                    // {"AllCastleBuilded", $""},
                    // {"WinProgress", $""},

                    {
                        "data_part3",
                        $"{PlayerPrefs.GetString(Data.SAVEHERO)}_{PlayerPrefs.GetString(Data.SAVEITEM)}_{PlayerPrefs.GetString(Data.SAVE_PRINCESS)}_{PlayerPrefs.GetString(Data.SAVECASTLE)}"
                    },

                    // {"savehero", $""},
                    // {"saveItems", $""},
                    // {"savePrincess", $""},
                    // {"saveCastle", $""},

                    {"data_part4", $"{Data.currentHero}_{Data.currentPrincess}_0_0_0_0_0_0_0_0_{Data.CountPlayLevel}"}
                },
            },
            result => { Data.DataVersion++; },
            error => { Debug.Log("ERROR:" + error.ErrorMessage); });
    }

    public void LoadUserData(string id)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest {PlayFabId = id, Keys = null},
            result =>
            {
                if (result.Data == null)
                {
                }
                else
                {
                    string title;
                    string titleLeft;
                    string titleRight;
                    string message;
                    if (_isBackup)
                    {
                        message = "Do you want to backup data\nfrom your device to server?\n\nData in server will be overrode";
                        title = "BACKUP DATA";
                        titleLeft = "Device Data";
                        titleRight = "Server Data";
                    }
                    else
                    {
                        message = "Do you want to restore data\nfrom your server to device?\n\nData in device will be overrode";
                        title = "RESTORE DATA";
                        titleLeft = "Server Data";
                        titleRight = "Device Data";
                    }

                    var dataVersion = -1;
                    string[] dataPart1 = new string[20];
                    if (result.Data.ContainsKey("data_part1"))
                    {
                        dataPart1 = result.Data["data_part1"].Value.Split('_');
                        dataVersion = int.Parse(dataPart1[0]);
                    }

                    if (dataVersion != -1)
                    {
                        int serverTotalSkin = 0;
                        int serverCurrentLevel = 0;
                        int serverCoin = 0;

                        if (result.Data.ContainsKey("data_part3"))
                        {
                            string[] part3 = result.Data["data_part3"].Value.Split('_');

                            string hero = part3[0];
                            string princess = part3[2];

                            if (!string.IsNullOrEmpty(hero))
                            {
                                var jsonData = LitJson.JsonMapper.ToObject(hero);
                                var count = jsonData.Count;
                                if (count > HeroData.Length) count = HeroData.Length;
                                for (int i = 0; i < count; i++)
                                {
                                    if (jsonData[i] != null)
                                    {
                                        var h = LitJson.JsonMapper.ToObject<SaveHero>(jsonData[i].ToJson());
                                        if (h.unlock)
                                        {
                                            serverTotalSkin++;
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(princess))
                            {
                                var jsonData = LitJson.JsonMapper.ToObject(princess);
                                var count = jsonData.Count;
                                if (count > HeroData.LengthPrincess) count = HeroData.LengthPrincess;
                                for (int i = 0; i < count; i++)
                                {
                                    if (jsonData[i] != null)
                                    {
                                        var p = LitJson.JsonMapper.ToObject<SaveHero>(jsonData[i].ToJson());
                                        if (p.unlock)
                                        {
                                            serverTotalSkin++;
                                        }
                                    }
                                }
                            }
                        }

                        if (result.Data.ContainsKey("data_part1"))
                        {
                            serverCurrentLevel = int.Parse(dataPart1[1]);
                            serverCoin = int.Parse(dataPart1[2]);
                        }

                        GamePopup.Instance.ShowOkCloseNotification(async () =>
                            {
                                Data.DataVersion = dataVersion + 2;

                                // {"DataVersion", $""}, // 0
                                // {"CurrentLevel", $""}, // 1
                                // {"currentCoin", $""}, // 2
                                // {"CurrentHardLevel", $""}, // 3
                                // {"MaxLevel", $""}, // 4
                                // {"CurrentRoom", $""}, // 5
                                // {"CurrentMenuRoom", $""}, // 6
                                // {"MaxHardLevel", $""}, // 7
                                // {"curDailyGift", $""}, // 8
                                // {"UserSound", $""}, // 9
                                // {"UserMusic", $""}, // 10
                                // {"UserVibrate", $""}, // 11
                                // {"UserRated", $""}, //12
                                // {"CountShowRate", $""}, // 13
                                // {"LastLevelShowRate", $""}, // 14
                                // {"RemoveAds", $""}, // 15
                                // {"UserVip", $""}, // 16

                                if (result.Data.ContainsKey("data_part1"))
                                {
                                    Utils.CurrentLevel = int.Parse(dataPart1[1]);
                                    Utils.currentCoin = int.Parse(dataPart1[2]);
                                    PlayerPrefs.SetInt(Data.TOTALCOIN, Utils.currentCoin);
                                    Data.firsttime = PlayerPrefs.GetInt(Data.FIRSTTIME);
                                    Utils.CurrentHardLevel = int.Parse(dataPart1[3]);
                                    Utils.MaxLevel = int.Parse(dataPart1[4]);
                                    Data.CurrentWorld = int.Parse(dataPart1[5]);
                                    Data.CurrentMenuWorld = int.Parse(dataPart1[6]);
                                    Utils.MaxHardLevel = int.Parse(dataPart1[7]);
                                    Utils.curDailyGift = int.Parse(dataPart1[8]);
                                    Utils.curEventDailyGift = int.Parse(dataPart1[17]);
                                    Utils.SaveDailyGift();
                                    Utils.SaveEventDailyGift();
                                    Data.UserSound = bool.Parse(dataPart1[9].ToLower());
                                    Data.UserMusic = bool.Parse(dataPart1[10].ToLower());
                                    Data.UserVibrate = bool.Parse(dataPart1[11].ToLower());
                                    Data.UserRated = bool.Parse(dataPart1[12].ToLower());
                                    Data.CountShowRate = int.Parse(dataPart1[13]);
                                    Data.LastLevelShowRate = int.Parse(dataPart1[14]);
                                    Data.RemoveAds = bool.Parse(dataPart1[15].ToLower());
                                    Data.UserVip = bool.Parse(dataPart1[16].ToLower());
                                }

                                if (result.Data.ContainsKey("data_part2"))
                                {
                                    string[] dataPart2 = result.Data["data_part2"].Value.Split('_');

                                    // {"DontShowUpdateAgain", $""}, // 0
                                    // {"CastleCurrentMap", $""}, // 1
                                    // {"WeekNumber", $""}, // 2
                                    // {"UserName", $""}, // 3
                                    // {"UserCountryCode", $""}, // 4
                                    // {"NotiHardMode", $""}, // 5
                                    // {"DoubleGold", $""}, // 6
                                    // {"CustomId", $""}, // 7
                                    // {"PlayerId", $""}, // 8
                                    // {"AllHeroUnlocked", $""}, // 9
                                    // {"AllCastleBuilded", $""}, // 10
                                    // {"WinProgress", $""}, // 11

                                    Data.DontShowUpdateAgain = bool.Parse(dataPart2[0]);
                                    Data.CastleCurrentMap = int.Parse(dataPart2[1]);
                                    Data.WeekNumber = int.Parse(dataPart2[2]);
                                    Data.UserName = string.IsNullOrEmpty(dataPart2[3]) ? "" : dataPart2[3];
                                    Data.UserCountryCode = string.IsNullOrEmpty(dataPart2[4]) ? "" : dataPart2[4];
                                    Data.NotiHardMode = bool.Parse(dataPart2[5]);
                                    Data.DoubleGold = bool.Parse(dataPart2[6]);
                                    Data.CustomId = dataPart2[7];
                                    Data.PlayerId = dataPart2[8];
                                    Data.AllHeroUnlocked = bool.Parse(dataPart2[9]);
                                    Data.AllCastleBuilded = bool.Parse(dataPart2[10]);
                                    Data.WinProgress = float.Parse(dataPart2[11]);
                                }

                                if (result.Data.ContainsKey("data_part3"))
                                {
                                    string[] dataPart3 = result.Data["data_part3"].Value.Split('_');

                                    PlayerPrefs.SetString(Data.SAVEHERO, dataPart3[0]);
                                    PlayerPrefs.SetString(Data.SAVEITEM, dataPart3[1]);
                                    PlayerPrefs.SetString(Data.SAVE_PRINCESS, dataPart3[2]);
                                    PlayerPrefs.SetString(Data.SAVECASTLE, dataPart3[3]);
                                }

                                if (result.Data.ContainsKey("data_part4"))
                                {
                                    string[] dataPart4 = result.Data["data_part4"].Value.Split('_');
                                    Data.currentHero = int.Parse(dataPart4[0]);
                                    Data.currentPrincess = int.Parse(dataPart4[1]);
                                    PlayerPrefs.SetInt(Data.CURRENTHERO, Data.currentHero);
                                    PlayerPrefs.SetInt(Data.CURRENTPRINCESS, Data.currentPrincess);

                                    try
                                    {
                                        if (dataPart4.Length > 10)
                                        {
                                            Data.CountPlayLevel = int.Parse(dataPart4[10]);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //
                                    }
                                }

                                DataController.instance.LoadByServer();

                                // update text display level
                                if (MenuController.instance != null)
                                {
                                    //MenuController.instance.txtCurrentLevel.text = $"LEVEL {Utils.CurrentLevel + 1}";
                                    MenuController.instance.TxtCoin.text = $"{Utils.currentCoin}";
                                }

                                var setting = (PopupSetting) GamePopup.Instance.popupSettingHandler;
                                setting.SetActiveBlock(true);

                                // re calculate list level

                                // re calculate list level
                                var go = await BridgeData.Instance.GetLevel(Utils.CurrentLevel);
                                if (go.Item1 != null)
                                {
                                    LevelMap levelMap = go.Item1.GetComponent<LevelMap>();

                                    BridgeData.Instance.nextLevelLoaded = levelMap;
                                    BridgeData.Instance.nextLevelLoaded.SetLevelLoaded(go.Item2);

                                    BridgeData.Instance.previousLevelLoaded = levelMap;
                                    BridgeData.Instance.previousLevelLoaded.SetLevelLoaded(go.Item2);
                                }

                                var room = await BridgeData.Instance.GetRoom(Data.CurrentMenuWorld);
                                Debug.Log(room == null);
                                if (room != null) BridgeData.Instance.menuRoomPrefab = room.GetComponent<BaseRoom>();
                                if (room != null) BridgeData.Instance.currentRoomPrefab = room.GetComponent<BaseRoom>();

                                var cacheRoom = GamePopup.Instance.menuRoom;
                                GamePopup.Instance.menuRoom = null;
                                GamePopup.Instance.currentRoom = null;
                                // update current menu room
                                GamePopup.Instance.ShowRoom(isDance: true);
                                GamePopup.Instance.ShowRoomGameplay();
                                if (cacheRoom != null)
                                {
                                    Destroy(cacheRoom.gameObject);
                                }

                                setting.SetActiveBlock(false);
                            },
                            SaveUserData,
                            message,
                            title,
                            serverCurrentLevel,
                            serverCoin,
                            serverTotalSkin,
                            titleRight,
                            titleLeft,
                            _isBackup);
                    }
                    else
                    {
                        SaveUserData();
                    }
                }
            },
            (error) => { Debug.Log(error.GenerateErrorReport()); });
    }
}