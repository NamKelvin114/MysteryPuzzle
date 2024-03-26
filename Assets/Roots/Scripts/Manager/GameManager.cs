using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lean.Touch;
using MoreMountains.NiceVibrations;
using Pancake.Monetization;
using Pancake.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEngine.Serialization;
using Worldreaver.UniUI;
using Worldreaver.Utility;
using Timer = Worldreaver.Timer.Timer;

public class GameManager : MonoBehaviour
{
    public CutsceneController CutsceneController;
    [SerializeField] private UniButton BtnEvent;
    private bool isPlus = false;
    [SerializeField] private CoinGeneration coinGeneration;
    [SerializeField] private UniButton btnCastle;
    [SerializeField] private UniButton btnToShop;
    [SerializeField] private UniButton btnToShop2;
    [SerializeField] private UniButton btnTask;
    [SerializeField] private GameObject notiCastle;
    [SerializeField] private GameObject warningTask;
    public GameObject warningEventValentine;
    public GameObject BtnReplay;
    public GameObject groupDebug;
    public GameObject bannerSimulator;
    public GameObject bouderCoinFly;
    public GameObject btnx3Coin;
    public GameObject btnHome;
    public GameObject btnTabNext;
    public GameObject phaohoa;
    public GameObject gacchaWin;

    public GameObject titleWin, titleLose;
    public PhysicsMaterial2D matStone;
    public bool playerMove;
    public List<Transform> targetCollects = new List<Transform>();
    public CancellationTokenSource Source { get; set; } = new CancellationTokenSource();
    public int CurrentLevel { get; set; }
    public int CurrentHardLevel { get; set; }
    public bool isTutorial { get; set; }
    public LevelRoot Root { get; set; } = null;

    public static GameManager instance;

    private bool isCallWinLose;

    // public MissionType mSavePrincess;
    // public MissionType mCollect;
    // public MissionType mOpenChest;
    // public MissionType mKill;
    // public MissionType mCollectRoomItem;
    public Image imgQuestImage;
    public TextMeshProUGUI txtQuestText;

    //public Text levelTextGameOver;
    public TextMeshProUGUI txtCoin;
    public TextMeshProUGUI txtCoinWin;
    public Text txtCoinReward;
    public Text txtX5CoinReward;
    public bool isShowLosing = false;
    public bool isShowEvent = false;
    public bool isTest;
    public bool canUseTrail;
    public EGameState gameState;
    public MapLevelManager mapLevel;
    [SerializeField] private List<Sprite> backgroudSprites;
    public int TotalGems { get; set; }
    public CamFollow _camFollow;
    public GameObject loseObj;
    public GameObject loseObjNormal;
    public GameObject gPanelWin;
    [HideInInspector] public GameObject gTargetFollow;
    public int CoinTemp { get; set; }
    public int EnemyKill { get; set; }
    private bool _playingSoundLava = false;
    private static int countPlayLevel;
    [SerializeField] private GameObject fetchSkipLevelInGame;
    [SerializeField] private GameObject fetchIncreaseRewardWin;
    [SerializeField] private GameObject fetchSkipLevelLose;
    [SerializeField] private PopupDoneJigsaw popupDoneJigsaw;
    [SerializeField] private PopupItemDescription popupItemDescription;
    [SerializeField] private PopupClaimCollection popupClaimCollection;

    [Space] [SerializeField] private TextAsset winAhapFile;
    [SerializeField] private MMNVAndroidWaveFormAsset winWaveFormAsset;
    [SerializeField] private MMNVRumbleWaveFormAsset rumbleWaveFormAsset;
    [SerializeField] private PopupNewSkin popupNewSkin;
    [SerializeField] private PopupGift popupGift;
    [SerializeField] private WinProgressHandle progressHandle;

    [SerializeField] private CanvasGroup fade;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer fadeBackground;
    [SerializeField] private GameObject canvasUI;
    [SerializeField] private Transform JigsawLevelSpawner;
    private bool _flagCheckFetchRewardIcon;
    private bool _canClickBtn = true;
    public Transform CanvasUI => canvasUI.transform;

    #region flag gameplay reset when next level

    public bool FlagCharacterCollectGem { get; set; }
    public bool FlagGemMeetLava { get; set; }
    public bool FlagCharacterDie { get; set; }
    public bool FlagHostageMeetCharacter { get; set; }
    public bool FlagHostageDie { get; set; }
    public bool FlagSoundCoinStickbarrie { get; set; }
    public bool FlagSoundWaterStickbarrie { get; set; }
    public bool FlagSoundLavaStickbarrie { get; set; }
    public IDisposable DisposableChangeStone { get; set; }
    public Timer TimerChangeStone { get; set; } = null;
    public bool FlagShowRewardWinLevel { get; set; } = false;
    public bool FlagGetRoomItem { get; set; }
    public bool LevelContainRoomItem { get; set; }
    private int bonusAdsButtonReward;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public void ResetFlagNextLevel()
    {
        FlagCharacterCollectGem = false;
        FlagGemMeetLava = false;
        FlagCharacterDie = false;
        FlagHostageMeetCharacter = false;
        FlagHostageDie = false;
        FlagSoundCoinStickbarrie = false;
        FlagSoundWaterStickbarrie = false;
        FlagSoundLavaStickbarrie = false;
        FlagShowRewardWinLevel = false;
        FlagGetRoomItem = false;
        Utils.pauseUpdateFetchIcon = false;
        targetCollects.Clear();
        LevelContainRoomItem = false;
    }

    private void PlayWinAhap()
    {
        MMVibrationManager.AdvancedHapticPattern(winAhapFile.text,
            winWaveFormAsset.WaveForm.Pattern,
            winWaveFormAsset.WaveForm.Amplitudes,
            -1,
            rumbleWaveFormAsset.WaveForm.Pattern,
            rumbleWaveFormAsset.WaveForm.LowFrequencyAmplitudes,
            rumbleWaveFormAsset.WaveForm.HighFrequencyAmplitudes,
            -1,
            HapticTypes.LightImpact,
            this);
    }

    public void LoseDisplay()
    {
        btnTabNext.SetActive(false);
        btnx3Coin.SetActive(false);
        phaohoa.SetActive(false);
        titleWin.SetActive(false);
        titleLose.SetActive(true);

        if (Utils.isHardMode)
        {
            loseObj.SetActive(true);
            loseObjNormal.SetActive(false);
        }
        else
        {
            loseObjNormal.SetActive(true);
            loseObj.SetActive(false);
        }
    }

    private void Awake()
    {
        instance = this;
        bool isFirtGame = Utils.IsFirstTimePLay;

        if (isFirtGame) SoundManager.Instance.StopBGSound();
        // intro.gameObject.SetActive(isFirtGame);
        if (Data.DateTimeStartRescueParty == "")
        {
            Data.DateTimeStartRescueParty = DateTime.Now.ToString();
        }

        CheckDisplayWarningDailyGiftEvent();
        BtnEvent.gameObject.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0 && Data.isTimeValentine);
        Observer.ShowHideTaskWarning += ShowHideTaskWarning;

        // if (Data.IsHalloweenStarted() || Data.IsXmasStarted())
        // {
        //     backgroudSprites[0] = backgroudSprites[5];
        //     backgroudSprites[1] = backgroudSprites[6];
        // }
        //
        // backgroudSprites.RemoveAt(backgroudSprites.Count - 1);
        // backgroudSprites.RemoveAt(backgroudSprites.Count - 1);
    }

    private void OnUpdateCoin()
    {
        var srt = Utils.currentCoin.ToString();
        txtCoin.text = srt;
        txtCoinWin.text = srt;
    }

    private void Start()
    {
        DataController.instance.CheckWarningForTask();
        GamePopup.Instance.HidePopupMoney();
        DOTween.KillAll();
        isCallWinLose = false;
        var index = 0;
        if (Utils.CurrentLevel > 1)
        {
            background.sprite = backgroudSprites[index];
            fadeBackground.sprite = backgroudSprites[index];
        }
        else
        {
            background.sprite = backgroudSprites[backgroudSprites.Count - 1];
            fadeBackground.sprite = backgroudSprites[backgroudSprites.Count - 1];
        }

        //levelTextGameOver.text = Utils.isHardMode ? $"HARD LEVEL {Utils.CurrentHardLevel + 1}" : $"LEVEL {Utils.CurrentLevel + 1}";
        fade.gameObject.SetActive(false);
        background.color = new Color(1f, 1f, 1f);
        txtCoinWin.text = $"{Utils.currentCoin}";
        CoinTemp = Utils.currentCoin;
        OnUpdateCoin();
        Observer.UpdateBonusAdsButton += UpdateTextBonusAdsButon;
        if (!isTest)
        {
            if (Utils.IsJigsawMode)
            {
                LoadLevelJigsawToPlay(Utils.CurrentJigsawLevel);
            }
            else
            {
                if (Utils.isHardMode)
                {
                    LoadHardLevelToPlay(Utils.CurrentHardLevel);
                }
                else
                {
                    LoadLevelToPlay(Utils.CurrentLevel);
                }
            }
        }

        if (Utils.IsTurnOnAds && Utils.IsMobile)
        {
            RescueAnalytic.LogEventAdBannerRequest();
            Advertising.ShowBannerAd();
            bannerSimulator.SetActive(false);
        }

        BtnEvent.onClick.RemoveAllListeners();
        BtnEvent.onClick.AddListener(OnEventButtonPressed);

        btnCastle.onClick.RemoveAllListeners();
        btnCastle.onClick.AddListener(OnCastleButtonPressed);

        btnToShop.onClick.RemoveAllListeners();
        btnToShop.onClick.AddListener(OnToShopButtonPressed);

        btnToShop2.onClick.RemoveAllListeners();
        btnToShop2.onClick.AddListener(OnToShopButtonPressed);

        if (!Config.IsDebug)
        {
            groupDebug.SetActive(false);
        }

        if (btnCastle.gameObject.activeSelf)
        {
            RefreshNotiCastle();
        }

        if (Utils.CurrentLevel >= 440)
        {
            Data.CurrentWorld = 11;
        }
        else if (Utils.CurrentLevel >= 400)
        {
            Data.CurrentWorld = 10;
        }
        else if (Utils.CurrentLevel >= 360)
        {
            Data.CurrentWorld = 9;
        }
        else if (Utils.CurrentLevel >= 320)
        {
            Data.CurrentWorld = 8;
        }
        else if (Utils.CurrentLevel >= 280)
        {
            Data.CurrentWorld = 7;
        }
        else if (Utils.CurrentLevel >= 240)
        {
            Data.CurrentWorld = 6;
        }
        else if (Utils.CurrentLevel >= 200)
        {
            Data.CurrentWorld = 5;
        }
        else if (Utils.CurrentLevel >= 160)
        {
            Data.CurrentWorld = 4;
        }
        else if (Utils.CurrentLevel >= 120)
        {
            Data.CurrentWorld = 3;
        }
        else if (Utils.CurrentLevel >= 80)
        {
            Data.CurrentWorld = 2;
        }
        else if (Utils.CurrentLevel >= 40)
        {
            Data.CurrentWorld = 1;
        }
        else
        {
            Data.CurrentWorld = 0;
        }

        if (GamePopup.Instance.menuRoom != null) GamePopup.Instance.menuRoom.gameObject.SetActive(false);
    }

    private void ShowHideTaskWarning(bool isShow)
    {
        warningTask.SetActive(isShow);
    }

    bool IsFirstLevels()
    {
        if (Utils.CurrentLevel < 10)
        {
            return true;
        }

        return false;
    }

    void SetUpFirstLevels()
    {
        if (IsFirstLevels())
        {
            btnTask.gameObject.SetActive(false);
        }
        else
        {
            btnTask.gameObject.SetActive(true);
        }
    }

    void SetUpFirst3()
    {
        bool isFrst3 = Utils.CurrentLevel < 3 ? true : false;
        BtnReplay.gameObject.SetActive(!isFrst3);
        btnHome.gameObject.SetActive(!isFrst3);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnCastleButtonPressed()
    {
        ShowCastle();
        SoundClickButton();
    }

    private void OnEventButtonPressed()
    {
        ShowEvent();
        SoundClickButton();
        isShowEvent = true;
    }

    public void ShowPopupWinJigsawMode(ETpyeContent eTpyeContent)
    {
        var popupShow = popupDoneJigsaw;
        popupShow.SetUp(eTpyeContent);
        popupShow.Show();
    }

    public void ShowPopupItemDecription()
    {
        popupItemDescription.Show();
    }

    public void ShowPopupTask()
    {
        SoundClickButton();
        isShowEvent = true;
        GamePopup.Instance.ShowTaskdPopup(() =>
            {
                SoundClickButton();
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
                // }
            }, false
        );
    }

    public void SetUpPopupItemDescription(Sprite item, string decription, string title)
    {
        popupItemDescription.Setup(item, decription, title);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnToShopButtonPressed()
    {
        GamePopup.Instance.ShowShopPopup(() =>
            {
                SoundClickButton();
                RefreshNotiCastle();
                DataController.instance.SaveData();
            },
            OnUpdateCoin,
            DataController.instance.UnlockAllHero);
    }

    private void ShowCastle()
    {
        FadeIn(0.5f, null);
        GamePopup.Instance.ShowCastlePopup(() =>
            {
                SoundClickButton();
                RefreshNotiCastle();
                txtCoin.text = Utils.currentCoin.ToString();
                DataController.instance.SaveData();
            },
            SoundClickButton,
            SoundClickButton,
            ShowShop);
    }

    private void ShowEvent()
    {
        FadeIn(0.5f, null);
        GamePopup.Instance.ShowPopupEvent(() =>
            {
                SoundClickButton();
                CheckDisplayWarningDailyGiftEvent();
                txtCoin.text = Utils.currentCoin.ToString();
                DataController.instance.SaveData();
                warningEventValentine.SetActive(false);
            }
        );
    }

    public void GenerateCoin(GameObject from, GameObject to, int bonus)
    {
        int coinTotal = Data.TotalGoldMedal + bonus;
        MenuController.instance.CheckDisplayWarningDailyGiftEvent();
        CheckDisplayWarningDailyGiftEvent();

        coinGeneration.GenerateCoin(() => { Data.TotalGoldMedal++; }, () => { Data.TotalGoldMedal = coinTotal; }, from,
            to);
    }

    /// <summary>
    /// display shop popup
    /// </summary>
    private void ShowShop(Action updateCurrency, Action actionRefreshUnlockAllHero, Action actionRefreshAll)
    {
        GamePopup.Instance.ShowShopPopup(() =>
            {
                SoundClickButton();
                RefreshNotiCastle();
                DataController.instance.SaveData();
            },
            updateCurrency,
            actionRefreshUnlockAllHero);
    }

    /// <summary>
    /// display daily reward popup
    /// </summary>
    private void ShowDailyReward(Action actionUpdateCurrency)
    {
        GamePopup.Instance.ShowDailyRewardPopup(() =>
            {
                SoundClickButton();
                RefreshNotiCastle();
            },
            null,
            SoundClickButton,
            (i) =>
            {
                SoundClickButton();
                if (GamePopup.Instance.popupSkinHandler != null)
                {
                    //(GamePopup.Instance.popupSkinHandler as PopupSkin)?.SkinShopItems[i].Unlock();
                }
                else
                {
                    DataController.instance.SaveHero[i].unlock = true;
                    RescueAnalytic.LogEventUnlockSkinHero(HeroData.NameHeroWithIndex(i), Utils.currentCoin,
                        Utils.CurrentLevel + 1);
                }
            },
            actionUpdateCurrency,
            ShowShop);
    }

    private void OnChange()
    {
        //imgQuestImage.sprite = sprite;
        if (Utils.IsJigsawMode)
        {
            txtQuestText.text = "JIGSAW LEVEL " + (Utils.CurrentHardLevel + 1); //+ "</color> " + text.ToUpper();
        }
        else
        {
            if (Utils.isHardMode)
            {
                txtQuestText.text = "HARD LEVEL " + (Utils.CurrentHardLevel + 1); //+ "</color> " + text.ToUpper();
            }
            else
            {
                txtQuestText.text = "LEVEL " + (Utils.CurrentLevel + 1); //+ "</color> " + text.ToUpper();
            }
        }
    }

    public void OnInitQuestText()
    {
        OnChange();
        // switch (eQuestType)
        // {
        //     case EQuestType.Collect:
        //         OnChange(mCollect.spr_, mCollect.strQuest);
        //         break;
        //     case EQuestType.Kill:
        //         OnChange(mKill.spr_, mKill.strQuest);
        //         break;
        //     case EQuestType.OpenChest:
        //         OnChange(mOpenChest.spr_, mOpenChest.strQuest);
        //         break;
        //     case EQuestType.SaveHostage:
        //         Data.idEasterEgg = PetCollection.PickRandomEggShard();
        //         OnChange(mSavePrincess.spr_, mSavePrincess.strQuest);
        //         break;
        //     case EQuestType.CollectRoomItem:
        //         OnChange(mCollectRoomItem.spr_,
        //             string.Format(mCollectRoomItem.strQuest, mCollectRoomItem.strQuest));
        //         break;
        // }
    }

    private void OnDisable()
    {
        Advertising.DestroyBannerAd();

        instance.Source?.Token.ThrowIfCancellationRequested();
    }

    private async void LoadHardLevelToPlay(int hardLevel)
    {
        if (!Utils.isHardMode) return;

        var index = 0;

        SetUpFirstLevels();
        SetUpFirst3();
        if (Utils.CurrentLevel > 1)
        {
            background.sprite = backgroudSprites[index];
            fadeBackground.sprite = backgroudSprites[index];
        }
        else
        {
            background.sprite = backgroudSprites[backgroudSprites.Count - 1];
            fadeBackground.sprite = backgroudSprites[backgroudSprites.Count - 1];
        }

        MapLevelManager mapInstall = null;
        {
            try
            {
                var level = await BridgeData.Instance.GetHardLevel(hardLevel);
                mapInstall = level.GetComponent<MapLevelManager>();
            }
            catch (Exception)
            {
                Debug.Log("real level can not install :" + hardLevel);
            }
        }

        FirstPlayHardLevel(Utils.CurrentHardLevel, mapInstall.GetComponent<LevelMap>());
        mapLevel = Instantiate(mapInstall, Vector3.zero, Quaternion.identity);
        if (mapLevel.lstAllStick.Count > 0) playerMove = true;
    }

    private async void LoadLevelJigsawToPlay(int jigsawLevel)
    {
        if (!Utils.IsJigsawMode) return;

        var index = 0;
        SetUpFirstLevels();
        SetUpFirst3();
        if (Utils.CurrentLevel > 1)
        {
            background.sprite = backgroudSprites[index];
            fadeBackground.sprite = backgroudSprites[index];
        }
        else
        {
            background.sprite = backgroudSprites[backgroudSprites.Count - 1];
            fadeBackground.sprite = backgroudSprites[backgroudSprites.Count - 1];
        }

        MapLevelManager mapInstall = null;
        {
            try
            {
                var level = await BridgeData.Instance.GetJigsawLevel(jigsawLevel);
                mapInstall = level.GetComponent<MapLevelManager>();
            }
            catch (Exception)
            {
                Debug.Log("real level can not install :" + jigsawLevel);
            }
        }

        mapLevel = Instantiate(mapInstall, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hardLevel"></param>
    /// <param name="levelMap"></param>
    public void FirstPlayHardLevel(int hardLevel, LevelMap levelMap)
    {
        ResetFlagNextLevel();
        CurrentHardLevel = hardLevel;

        if (Utils.MaxHardLevel < CurrentHardLevel) Utils.MaxHardLevel = CurrentHardLevel;
        Utils.CurrentHardLevel = CurrentHardLevel;
        if (Root != null) DestroyImmediate(Root.gameObject);

        Root = Instantiate(PrefabResources.Instance.root);
        Root.Setup(CurrentHardLevel);

        Root.SetupDynamics();
        Root.StartPlaying();
        RescueAnalytic.LogEventHardLevelStart(Utils.CurrentHardLevel + 1, Utils.currentCoin);
    }

    private async void LoadLevelToPlay(int levelIndex)
    {
        var index = 0;

        if (Utils.CurrentLevel > 1)
        {
            background.sprite = backgroudSprites[index];
            fadeBackground.sprite = backgroudSprites[index];
        }
        else
        {
            background.sprite = backgroudSprites[backgroudSprites.Count - 1];
            fadeBackground.sprite = backgroudSprites[backgroudSprites.Count - 1];
        }


        async void LoadNextLevel(int index)
        {
            var go = await BridgeData.Instance.GetLevel(index + 1);
            if (go.Item1 != null)
            {
                BridgeData.Instance.nextLevelLoaded = go.Item1.GetComponent<LevelMap>();
                BridgeData.Instance.nextLevelLoaded.SetLevelLoaded(go.Item2);
            }
        }

        void SavePreviousLevel(GameObject go)
        {
            var levelMap = go.GetComponent<LevelMap>();
            
            Utils.CurrentBackGround = levelMap.mapType.ToString();
            
            BridgeData.Instance.previousLevelLoaded = levelMap;
            BridgeData.Instance.previousLevelLoaded.SetLevelLoaded(levelMap.CurrentLevelIndex);
        }

        ResetFlagNextLevel();
        MapLevelManager mapInstall = null;

        if (BridgeData.Instance.isReplay)
        {
            BridgeData.Instance.isReplay = false;

            if (BridgeData.Instance.previousLevelLoaded != null &&
                BridgeData.Instance.previousLevelLoaded.CurrentLevelIndex == levelIndex)
            {
                mapInstall = BridgeData.Instance.previousLevelLoaded.GetComponent<MapLevelManager>();
                LoadNextLevel(levelIndex);
            }
            else
            {
                BridgeData.Instance.nextLevelLoaded = null;
                var level = await BridgeData.Instance.GetLevel(levelIndex);
                if (level.Item1 != null)
                {
                    mapInstall = level.Item1.GetComponent<MapLevelManager>();
                    mapInstall.GetComponent<LevelMap>().SetLevelLoaded(level.Item2);
                }

                LoadNextLevel(levelIndex);
            }
        }
        else
        {
            if (BridgeData.Instance.nextLevelLoaded != null &&
                BridgeData.Instance.nextLevelLoaded.CurrentLevelIndex == levelIndex)
            {
                mapInstall = BridgeData.Instance.nextLevelLoaded.GetComponent<MapLevelManager>();
                LoadNextLevel(levelIndex);
            }
            else
            {
                BridgeData.Instance.nextLevelLoaded = null;
                var level = await BridgeData.Instance.GetLevel(levelIndex);
                if (level.Item1 != null)
                {
                    mapInstall = level.Item1.GetComponent<MapLevelManager>();
                    mapInstall.GetComponent<LevelMap>().SetLevelLoaded(level.Item2);
                }

                LoadNextLevel(levelIndex);
            }
        }

        background.gameObject.SetActive(mapInstall.GetComponent<LevelMap>().isBlockGameplay);
        FirstPlayLevel(Utils.CurrentLevel);
        SavePreviousLevel(mapInstall.gameObject);
        if (mapInstall.isJigsawLevel)
            mapLevel = Instantiate(mapInstall, JigsawLevelSpawner);
        else
            mapLevel = Instantiate(mapInstall, Vector3.zero, Quaternion.identity);

        if (mapLevel.lstAllStick.Count > 0)
            playerMove = true;
        SetUpFirstLevels();
        SetUpFirst3();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public void FirstPlayLevel(int level)
    {
        ResetFlagNextLevel();
        CurrentLevel = level;

        if (Root != null)
        {
            DestroyImmediate(Root.gameObject);
        }

        Root = Instantiate(PrefabResources.Instance.root);
        Root.Setup(CurrentLevel);

        Root.SetupDynamics();
        Root.StartPlaying();
        RescueAnalytic.LogEventLevelStart(Utils.CurrentLevel + 1, Utils.currentCoin);
    }

    private void ActiveCamEff()
    {
        _camFollow.objectToFollow = gTargetFollow;
        _camFollow.beginFollow = true;
    }

    public void ShowWinPanel()
    {
        // Debug.Log(isShowLosing);
        gameState = EGameState.Win;
        _flagCheckFetchRewardIcon = true;
        if (isShowLosing == false)
        {
            if (mapLevel.eQuestType == EQuestType.Collect || mapLevel.eQuestType == EQuestType.OpenChest)
            {
                if (isPlus == false)
                {
                    Data.TotalGoldMedal += 1;
                    isPlus = true;
                    // Debug.Log("athere");
                }
            }
        }

        StartCoroutine(IEWaitToShowWinLose(true));
    }

    private bool _isActiveCameraEffect;

    public void FadeBackgroundEndGame()
    {
        fadeBackground.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
        Color endColor = fadeBackground.color;
        endColor.a = .8f;
        fadeBackground.DOColor(endColor, 1.5f);
    }

    private IEnumerator IEWaitToShowWinLose(bool isWin)
    {
        _isActiveCameraEffect = false;
        yield return new WaitForSeconds(0.8f);
        if (isCallWinLose) yield break;
        isShowLosing = false;
        Advertising.DestroyBannerAd();
        BtnReplay.SetActive(false);
        groupDebug.SetActive(false);
        bannerSimulator.SetActive(false);
        bouderCoinFly.SetActive(false);
        imgQuestImage.transform.parent.gameObject.SetActive(false);
        if (isWin)
        {
            if (mapLevel.eQuestType == EQuestType.CollectRoomItem)
            {
                //GameManager.instance.GenerateCoin(gameObject.GetComponentInChildren<RoomItem>().gameObject,gameObject.GetComponentInChildren<RoomItem>().gameObject, 1);
                var room = MapLevelManager.Instance.trTarget.GetComponent<RoomItem>();
                Data.CurrentItemID = room.ID;
                Data.CurrentRoomID = room.RoomId;
                Data.CurrentLevelMapItem = MapLevelManager.Instance.GetComponent<LevelMap>().CurrentLevelIndex;
                if (instance.FlagGetRoomItem && !DataController.instance.SaveItems[room.ID + room.RoomId * 8].unlock)
                {
                    _isActiveCameraEffect = true;
                    DataController.instance.SaveItems[Data.CurrentItemID + Data.CurrentRoomID * 8].unlock = true;
                    DataController.instance.SaveItem();
                    if (Data.CurrentWorld != room.RoomId)
                    {
                        Data.CurrentWorld = room.RoomId;
                    }

                    var taskYieldInstruction0 = BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();

                    while (!taskYieldInstruction0.IsCompleted)
                    {
                        yield return null;
                    }

                    var baseRoom = taskYieldInstruction0.Result.GetComponent<BaseRoom>();
                    BridgeData.Instance.currentRoomPrefab = baseRoom;

                    FadeIn(1f,
                        () =>
                        {
                            GamePopup.Instance.ShowRoomGameplay();
                            GamePopup.Instance.currentRoom.UnlockItem(room.ID,
                                actionActiveCameraEffect: () =>
                                {
                                    if (SoundManager.Instance != null)
                                        SoundManager.Instance.PlaySound(SoundManager.Instance.acWin);
                                    if (phaohoa != null) phaohoa.SetActive(true);
                                    ActiveCamEff();
                                });
                        });

                    bool check = true;
                    for (int i = 0; i < 8; i++)
                    {
                        if (!DataController.instance.SaveItems[i + room.RoomId * 8].unlock)
                        {
                            check = false;
                        }
                    }

                    if (Data.CurrentWorld < Config.MaxRoomCanReach)
                        if (check)
                        {
                            if (Data.CurrentWorld < 9)
                            {
                                Utils.showNewWorld = false;
                                Data.CurrentWorld++;
                                var taskYieldInstruction = BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();

                                while (!taskYieldInstruction.IsCompleted)
                                {
                                    yield return null;
                                }

                                var baseNewRoom = taskYieldInstruction.Result.GetComponent<BaseRoom>();
                                BridgeData.Instance.currentRoomPrefab = baseNewRoom;

                                BridgeData.Instance.menuRoomPrefab = baseNewRoom;
                                Data.CurrentMenuWorld = Data.CurrentWorld;
                                if (GamePopup.Instance.menuRoom != null)
                                {
                                    Destroy(GamePopup.Instance.menuRoom.gameObject);
                                }

                                GamePopup.Instance.ShowRoom(hide: true);

                                Utils.showNewWorld = true;

                                if (Data.CurrentWorld < (Config.MaxLevelCanReach / 40) - 1)
                                {
                                    var taskYieldInstruction2 =
                                        BridgeData.Instance.GetRoom(Data.CurrentWorld + 1).AsTask();
                                    while (!taskYieldInstruction2.IsCompleted)
                                    {
                                        yield return null;
                                    }

                                    BridgeData.Instance.newRoomPrefab =
                                        taskYieldInstruction2.Result.GetComponent<BaseRoom>();
                                }
                            }
                        }
                        else
                        {
                            // BridgeData.Instance.menuRoomPrefab = baseRoom;
                            // Utils.CurrenMenuRoom = Data.CurrentRoom;
                            // if (GamePopup.Instance.menuRoom != null)
                            // {
                            //     Destroy(GamePopup.Instance.menuRoom.gameObject);
                            // }

                            GamePopup.Instance.ShowRoom(hide: true);
                        }
                }
            }

            if (!gPanelWin.gameObject.activeSelf)
            {
                Utils.CurrentLevel += 1;
                Data.CountPlayLevel += 1;
                if (Utils.MaxLevel < Utils.CurrentLevel)
                {
                    Utils.MaxLevel = Utils.CurrentLevel;
                }

                MapLevelManager.Instance.gameStateWin = true;
                if (Utils.isHardMode)
                {
                    RescueAnalytic.LogEventHardLevelComplete(Utils.CurrentHardLevel + 1, Utils.currentCoin);
                }
                else
                {
                    RescueAnalytic.LogEventLevelComplete(Utils.CurrentLevel + 1, Utils.currentCoin);
                }

                if (!isCallWinLose)
                {
                    Observer.UpdateTempValue?.Invoke(ETaskType.WinLevel);
                    isCallWinLose = true;
                }

                Observer.UpdateTaskValue?.Invoke();

                if (!_isActiveCameraEffect)
                {
                    phaohoa.SetActive(true);
                    ActiveCamEff();
                }

                txtCoinReward.text = $"+{Utils.CoinReward}";
                txtX5CoinReward.text = $"+{Utils.CoinReward * Config.ValueIncreateWatchAdsWinLevel}";

                OnUpdateCoin();
                gPanelWin.gameObject.SetActive(true);
                if (mapLevel.eQuestType != EQuestType.CollectRoomItem)
                {
                    GamePopup.Instance.ShowPopupMoney();
                }

                if (progressHandle != null)
                {
                    if (Utils.CurrentLevel > 1)
                    {
                        progressHandle.UpdateProcess();
                        progressHandle.gameObject.SetActive(true);
                    }
                    else
                    {
                        progressHandle.gameObject.SetActive(false);
                    }
                }


                //PlayWinAhap();
                if (mapLevel.eQuestType != EQuestType.CollectRoomItem)
                {
                    if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acWin);
                }

                if (mapLevel.eQuestType == EQuestType.SaveHostage)
                {
                    if (Data.idEasterEgg != -1 && !Utils.isHardMode)
                    {
                        DataController.instance.petDataController.AddEggShard(Data.idEasterEgg);
                    }
                }

                if (MapLevelManager.Instance.eQuestType != EQuestType.CollectRoomItem)
                {
                    //CheckShowRate();
                }


                if (DataController.instance != null) DataController.instance.SaveData();
            }
        }
        else
        {
            if (!gPanelWin.gameObject.activeSelf)
            {
                Observer.UpdateTaskValue?.Invoke();
                ActiveCamEff();
                gPanelWin.gameObject.SetActive(true);
                if (mapLevel.eQuestType != EQuestType.CollectRoomItem)
                {
                    GamePopup.Instance.ShowPopupMoney();
                }

                if (progressHandle != null) progressHandle.gameObject.SetActive(false);
                gacchaWin.SetActive(false);
                LoseDisplay();
                //   countpasslevel = 0;
                if (SoundManager.Instance != null && SoundManager.Instance.acLose != null)
                    SoundManager.Instance.PlaySound(SoundManager.Instance.acLose);
            }
        }

        if (MapLevelManager.Instance.isTaskTutorial)
        {
            GamePopup.Instance.ShowPopupTutorial();
        }

        if (MapLevelManager.Instance.isCollectionTutoria)
        {
            GamePopup.Instance.ShowPopupTutorialCollection();
        }

        btnTask.gameObject.SetActive(false);
    }

    public void CheckShowAds()
    {
        if (!Utils.IsMobile && !Utils.IsTurnOnAds)
        {
            OnReplay();
            return;
        }
        countPlayLevel++;
        if (Utils.IsTurnOnAds
            && (countPlayLevel > Data.levelshowinterads)
            && ((DateTime.Now - Data.oldTimeShowAdsLose).TotalSeconds >= Data.timeshowinteradslosedelay))
        {
            countPlayLevel = 0;
            RescueAnalytic.LogEventAdInterstitialRequest();
            Advertising.ShowInterstitialAd().OnCompleted(() =>
            {
                Data.oldTimeShowAdsLose = DateTime.Now;
                OnReplay();
            });
        }
        else
        {
            OnReplay();
        }
    }

    public void PlaySoundLavaOnWater()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acStoneApear);
            if (!_playingSoundLava)
            {
                _playingSoundLava = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
            }
        }
    }

    public void ShowLosePanel()
    {
        Utils.pauseUpdateFetchIcon = false;

        if (Utils.isHardMode)
        {
            RescueAnalytic.LogEventHardLevelFaild(Utils.CurrentHardLevel + 1, Utils.currentCoin);
        }
        else
        {
            RescueAnalytic.LogEventLevelFaild(Utils.CurrentLevel + 1, Utils.currentCoin);
        }

        StartCoroutine(IEWaitToShowWinLose(false));
    }

    public void OnNextLevel(Action actionIncreaseLevel)
    {
        isPlus = false;
        if (!instance.FlagShowRewardWinLevel)
        {
            if (Data.firsttime == 0)
            {
                if (Utils.CurrentLevel >= Data.levelpassshowad && Utils.IsTurnOnAds)
                {
                    RescueAnalytic.LogEventAdInterstitialRequest();
                    Advertising.ShowInterstitialAd().OnCompleted(() =>
                    {
                        Data.oldTimeShowAds = DateTime.Now;
                    });
                    Data.firsttime = 1;
                }
            }
            else
            {
                countPlayLevel++;
                if (Utils.IsTurnOnAds
                    && (countPlayLevel > Data.levelshowinterads)
                    && ((DateTime.Now - Data.oldTimeShowAds).TotalSeconds >= Data.timedelayShowAds))
                {
                    countPlayLevel = 0;
                    RescueAnalytic.LogEventAdInterstitialRequest();
                    Advertising.ShowInterstitialAd().OnCompleted(() =>
                    {
                        Data.oldTimeShowAds = DateTime.Now;
                    });
                }
            }
        }

        actionIncreaseLevel?.Invoke();

        ObjectPoolerManager.Instance.ClearAllPool();
        if (background != null) background.gameObject.SetActive(false);
        if (canvasUI != null) canvasUI.SetActive(false);
        if (mapLevel != null) Destroy(mapLevel.gameObject);
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
    }

    public void OnNextLevel(int coinReward = -1)
    {
        if (!_canClickBtn)
        {
            return;
        }

        Debug.Log(countPlayLevel);
        Debug.Log(Data.levelshowinterads);
        _canClickBtn = false;
        if (!instance.FlagShowRewardWinLevel)
        {
            if (Data.firsttime == 0)
            {
                if (Utils.CurrentLevel >= Data.levelpassshowad && Utils.IsTurnOnAds)
                {
                    RescueAnalytic.LogEventAdInterstitialRequest();
                    Advertising.ShowInterstitialAd().OnCompleted(() =>
                    {
                        Data.oldTimeShowAds = DateTime.Now;
                    });
                    Data.firsttime = 1;
                }
            }
            else
            {
                countPlayLevel++;
                if (Utils.IsTurnOnAds
                    && (countPlayLevel > Data.levelshowinterads)
                    && ((DateTime.Now - Data.oldTimeShowAds).TotalSeconds >= Data.timedelayShowAds))
                {
                    countPlayLevel = 0;
                    RescueAnalytic.LogEventAdInterstitialRequest();
                    Advertising.ShowInterstitialAd().OnCompleted(() =>
                    {
                        Data.oldTimeShowAds = DateTime.Now;
                    });
                }
            }
        }

        Observer.AddFromPosiGenerationCoin?.Invoke(coinReward == -1 ? btnTabNext : gacchaWin);
        Utils.UpdateCoin(coinReward == -1 ? Utils.CoinReward : coinReward);
        DOTween.Sequence().AppendInterval(1.5f).OnComplete(() =>
        {
            GamePopup.Instance.HidePopupMoney();
            background.gameObject.SetActive(false);
            canvasUI.SetActive(false);
            Destroy(mapLevel.gameObject);
            ObjectPoolerManager.Instance.ClearAllPool();
            _canClickBtn = true;
            SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
        });
    }

    public void Plus_countPlayLevel()
    {
        if (Data.firsttime == 1)
        {
            //countPlayLevel++;
        }
    }

    private IEnumerator WaitForArrow()
    {
        yield return new WaitForSeconds(1f);

        if (!Utils.IsMobile || !Utils.IsTurnOnAds)
        {
            OnUpdateCoin();
            gacchaWin.SetActive(false);
            btnTabNext.transform.localPosition = Vector3.zero;
            OnNextLevel(bonusAdsButtonReward * Utils.CoinReward);
        }
        else
        {
            if (Utils.IsTurnOnAds)
            {
                bool checkCompleteAds = false;
                Utils.pauseUpdateFetchIcon = true;
                RescueAnalytic.LogEventAdRewardRequest();
                Advertising.ShowRewardedAd().OnCompleted(() =>
                {
                    instance.FlagShowRewardWinLevel = true;
                    OnUpdateCoin();
                    gacchaWin.SetActive(false);
                    btnTabNext.SetActive(false);
                    OnNextLevel(bonusAdsButtonReward * Utils.CoinReward);
                    Utils.pauseUpdateFetchIcon = false;
                    checkCompleteAds = true;
                }).OnClosed(() =>
                {
                    if(!checkCompleteAds)
                        gPanelWin.GetComponent<PanelWinHandle>().OutGameOnClaimVideo();
                });
            }
        }
    }

    public void OnX2Coin()
    {
        StartCoroutine(WaitForArrow());
    }

    public void OnClickEvent()
    {
        // Debug.Log(MenuController.instance)
        MenuController.instance.ShowEventReward(null);
    }

    private void UpdateTextBonusAdsButon(int bonus)
    {
        bonusAdsButtonReward = bonus;
    }

    public void OnBackLevel()
    {
        Utils.CurrentLevel -= 1;
        Data.CountPlayLevel -= 1;
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
    }

    public void OnWinLevel()
    {
        if (Utils.IsJigsawMode)
        {
            Utils.IsJigsawMode = false;
            Utils.CurrentJigsawLevel += 1;
            GamePopup.Instance.endLevelJigsaw?.Invoke();
        }
        else MapLevelManager.Instance.OnWin();
    }

    public void OnSkipByVideo()
    {
        if (Utils.IsTurnOnAds && Utils.IsMobile)
        {
            Advertising.ShowRewardedAd().OnCompleted(OnCompleteSkipAds);
            RescueAnalytic.LogEventAdRewardRequest();
        }
        else
        {
            OnCompleteSkipAds();
        }

        //}
        // else
        // if (Data.onlyUseAdmob)
        // {
        //     if (AdsManager.Instance != null)
        //     {
        //         Utils.pauseUpdateFetchIcon = true;
        //         RescueAnalytic.LogEventAdRewardRequest();
        //         AdsManager.Instance.ShowRewardedVideo(async (b) =>
        //         {
        //             if (b)
        //             {
        //                 if (Utils.isHardMode)
        //                 {
        //                     OnNextLevel(() =>
        //                     {
        //                         Utils.CurrentHardLevel += 1;
        //                         if (Utils.CurrentHardLevel >= 40)
        //                         {
        //                             Utils.CurrentHardLevel = 0;
        //                         }
        //                     });
        //                     RescueAnalytic.LogEventLevelSkipHardMode(Utils.currentCoin, Utils.CurrentHardLevel);
        //                 }
        //                 else
        //                 {
        //                     if (!LevelContainRoomItem)
        //                     {
        //                         OnNextLevel(() =>
        //                         {
        //                             Utils.CurrentLevel += 1;
        //                             Data.CountPlayLevel += 1;
        //                         });
        //                     }
        //                     else
        //                     {
        //                         if (mapLevel.eQuestType == EQuestType.CollectRoomItem)
        //                         {
        //                             var room = MapLevelManager.Instance.trTarget.GetComponent<RoomItem>();
        //                             DataController.instance.SaveItems[room.ID + room.RoomId * 8].unlock = true;
        //                             DataController.instance.SaveItem();
        //
        //                             if (Data.CurrentWorld != room.RoomId)
        //                             {
        //                                 Data.CurrentWorld = room.RoomId;
        //                             }
        //
        //                             var taskYieldInstruction0 = BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();
        //
        //                             while (!taskYieldInstruction0.IsCompleted)
        //                             {
        //                                 await Task.Yield();
        //                             }
        //
        //                             var baseRoom = taskYieldInstruction0.Result.GetComponent<BaseRoom>();
        //                             BridgeData.Instance.currentRoomPrefab = baseRoom;
        //
        //                             FadeIn(1f,
        //                                 () =>
        //                                 {
        //                                     GamePopup.Instance.ShowRoomGameplay();
        //                                     GamePopup.Instance.currentRoom.UnlockItem(room.ID,
        //                                         () => OnNextLevel(() =>
        //                                         {
        //                                             Utils.CurrentLevel += 1;
        //                                             Data.CountPlayLevel += 1;
        //                                         }));
        //                                 });
        //
        //                             bool check = true;
        //                             for (int i = 0; i < 8; i++)
        //                             {
        //                                 if (!DataController.instance.SaveItems[i + room.RoomId * 8].unlock)
        //                                 {
        //                                     check = false;
        //                                 }
        //                             }
        //
        //                             if (check)
        //                             {
        //                                 if (Data.CurrentWorld < 9)
        //                                 {
        //                                     Utils.showNewWorld = false;
        //                                     Data.CurrentWorld++;
        //                                     var r = await BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();
        //
        //                                     var baseNewRoom = r.GetComponent<BaseRoom>();
        //                                     BridgeData.Instance.currentRoomPrefab = baseNewRoom;
        //
        //                                     BridgeData.Instance.menuRoomPrefab = baseNewRoom;
        //                                     Data.CurrentMenuWorld = Data.CurrentWorld;
        //                                     if (GamePopup.Instance.menuRoom != null)
        //                                     {
        //                                         Destroy(GamePopup.Instance.menuRoom.gameObject);
        //                                     }
        //
        //                                     GamePopup.Instance.ShowRoom(hide: true);
        //                                     Utils.showNewWorld = true;
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 // BridgeData.Instance.menuRoomPrefab = baseRoom;
        //                                 // Utils.CurrenMenuRoom = Data.CurrentRoom;
        //                                 // if (GamePopup.Instance.menuRoom != null)
        //                                 // {
        //                                 //     Destroy(GamePopup.Instance.menuRoom.gameObject);
        //                                 // }
        //                                 GamePopup.Instance.ShowRoom(hide: true);
        //                             }
        //                         }
        //                     }
        //
        //                     RescueAnalytic.LogEventLevelSkip(Utils.currentCoin, Utils.CurrentLevel);
        //                 }
        //             }
        //
        //             Utils.pauseUpdateFetchIcon = false;
        //         });
        //     }
        // }
        // else if (Data.onlyUseMax)
        // {
        //     if (AdsApplovinManager.Instance != null)
        //     {
        //         Utils.pauseUpdateFetchIcon = true;
        //         RescueAnalytic.LogEventAdRewardRequest();
        //         AdsApplovinManager.Instance.ShowReward(async (b) =>
        //         {
        //             if (b)
        //             {
        //                 if (Utils.isHardMode)
        //                 {
        //                     OnNextLevel(() =>
        //                     {
        //                         Utils.CurrentHardLevel += 1;
        //                         if (Utils.CurrentHardLevel >= 40)
        //                         {
        //                             Utils.CurrentHardLevel = 0;
        //                         }
        //                     });
        //                     RescueAnalytic.LogEventLevelSkipHardMode(Utils.currentCoin, Utils.CurrentHardLevel);
        //                 }
        //                 else
        //                 {
        //                     if (!LevelContainRoomItem)
        //                     {
        //                         OnNextLevel(() =>
        //                         {
        //                             Utils.CurrentLevel += 1;
        //                             Data.CountPlayLevel += 1;
        //                         });
        //                     }
        //                     else
        //                     {
        //                         if (mapLevel.eQuestType == EQuestType.CollectRoomItem)
        //                         {
        //                             var room = MapLevelManager.Instance.trTarget.GetComponent<RoomItem>();
        //                             DataController.instance.SaveItems[room.ID + room.RoomId * 8].unlock = true;
        //                             DataController.instance.SaveItem();
        //
        //                             if (Data.CurrentWorld != room.RoomId)
        //                             {
        //                                 Data.CurrentWorld = room.RoomId;
        //                             }
        //
        //                             var taskYieldInstruction0 = BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();
        //
        //                             while (!taskYieldInstruction0.IsCompleted)
        //                             {
        //                                 await Task.Yield();
        //                             }
        //
        //                             var baseRoom = taskYieldInstruction0.Result.GetComponent<BaseRoom>();
        //                             BridgeData.Instance.currentRoomPrefab = baseRoom;
        //
        //                             FadeIn(1f,
        //                                 () =>
        //                                 {
        //                                     GamePopup.Instance.ShowRoomGameplay();
        //                                     GamePopup.Instance.currentRoom.UnlockItem(room.ID,
        //                                         () => OnNextLevel(() =>
        //                                         {
        //                                             Utils.CurrentLevel += 1;
        //                                             Data.CountPlayLevel += 1;
        //                                         }));
        //                                 });
        //
        //                             bool check = true;
        //                             for (int i = 0; i < 8; i++)
        //                             {
        //                                 if (!DataController.instance.SaveItems[i + room.RoomId * 8].unlock)
        //                                 {
        //                                     check = false;
        //                                 }
        //                             }
        //
        //                             if (check)
        //                             {
        //                                 if (Data.CurrentWorld < 9)
        //                                 {
        //                                     Utils.showNewWorld = false;
        //                                     Data.CurrentWorld++;
        //                                     var r = await BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();
        //
        //                                     var baseNewRoom = r.GetComponent<BaseRoom>();
        //                                     BridgeData.Instance.currentRoomPrefab = baseNewRoom;
        //
        //                                     BridgeData.Instance.menuRoomPrefab = baseNewRoom;
        //                                     Data.CurrentMenuWorld = Data.CurrentWorld;
        //                                     if (GamePopup.Instance.menuRoom != null)
        //                                     {
        //                                         Destroy(GamePopup.Instance.menuRoom.gameObject);
        //                                     }
        //
        //                                     GamePopup.Instance.ShowRoom(hide: true);
        //                                     Utils.showNewWorld = true;
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 // BridgeData.Instance.menuRoomPrefab = baseRoom;
        //                                 // Utils.CurrenMenuRoom = Data.CurrentRoom;
        //                                 // if (GamePopup.Instance.menuRoom != null)
        //                                 // {
        //                                 //     Destroy(GamePopup.Instance.menuRoom.gameObject);
        //                                 // }
        //                                 GamePopup.Instance.ShowRoom(hide: true);
        //                             }
        //                         }
        //                     }
        //
        //                     RescueAnalytic.LogEventLevelSkip(Utils.currentCoin, Utils.CurrentLevel);
        //                 }
        //             }
        //
        //             Utils.pauseUpdateFetchIcon = false;
        //         });
        //     }
        // }   
    }

    private async void OnCompleteSkipAds()
    {
        Data.oldTimeShowAdsLose = DateTime.Now;
        Data.oldTimeShowAds = DateTime.Now;
        if (Utils.isHardMode)
        {
            OnNextLevel(() =>
            {
                Utils.CurrentHardLevel += 1;
                if (Utils.CurrentHardLevel >= 40)
                {
                    Utils.CurrentHardLevel = 0;
                }
            });
        }
        else if (!Utils.IsJigsawMode)
        {
            if (!LevelContainRoomItem)
            {
                OnNextLevel(() =>
                {
                    Utils.CurrentLevel += 1;
                    Data.CountPlayLevel += 1;
                });
            }
            else
            {
                if (mapLevel.eQuestType == EQuestType.CollectRoomItem)
                {
                    var room = MapLevelManager.Instance.trTarget.GetComponent<RoomItem>();
                    DataController.instance.SaveItems[room.ID + room.RoomId * 8].unlock = true;
                    DataController.instance.SaveItem();
                    Data.CurrentItemID = room.ID;
                    Data.CurrentRoomID = room.RoomId;
                    Data.CurrentLevelMapItem = MapLevelManager.Instance.GetComponent<LevelMap>().CurrentLevelIndex;

                    if (Data.CurrentWorld != room.RoomId)
                    {
                        Data.CurrentWorld = room.RoomId;
                    }

                    var taskYieldInstruction0 = BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();

                    while (!taskYieldInstruction0.IsCompleted)
                    {
                        await Task.Yield();
                    }

                    var baseRoom = taskYieldInstruction0.Result.GetComponent<BaseRoom>();
                    BridgeData.Instance.currentRoomPrefab = baseRoom;

                    FadeIn(1f,
                        () =>
                        {
                            GamePopup.Instance.ShowRoomGameplay();
                            GamePopup.Instance.currentRoom.UnlockItem(Data.CurrentItemID,
                                onCompleted: () => OnNextLevel(() =>
                                {
                                    Utils.CurrentLevel += 1;
                                    Data.CountPlayLevel += 1;
                                    SceneManager.LoadScene("MainGame");
                                }));
                        });

                    bool check = Data.CurrentItemID == 7;
                    // for (int i = 0; i < 8; i++)
                    // {
                    //     if (!DataController.instance.SaveItems[i + room.RoomId * 8].unlock)
                    //     {
                    //         check = false;
                    //     }
                    // }
                    if (Data.CurrentWorld < Config.MaxRoomCanReach)
                        if (check)
                        {
                            if (Data.CurrentWorld < 9)
                            {
                                Utils.showNewWorld = false;
                                Data.CurrentWorld++;
                                var r = await BridgeData.Instance.GetRoom(Data.CurrentWorld).AsTask();

                                var baseNewRoom = r.GetComponent<BaseRoom>();
                                BridgeData.Instance.currentRoomPrefab = baseNewRoom;

                                BridgeData.Instance.menuRoomPrefab = baseNewRoom;
                                Data.CurrentMenuWorld = Data.CurrentWorld;
                                if (GamePopup.Instance.menuRoom != null)
                                {
                                    Destroy(GamePopup.Instance.menuRoom.gameObject);
                                }

                                GamePopup.Instance.ShowRoom(hide: true);
                                Utils.showNewWorld = true;
                            }
                        }
                        else
                        {
                            GamePopup.Instance.ShowRoom(hide: true);
                        }
                }
            }
        }
        else
        {
            Utils.IsJigsawMode = false;
            Utils.CurrentJigsawLevel += 1;
            GamePopup.Instance.endLevelJigsaw?.Invoke();
        }
    }

    private void Update()
    {
        // Debug.Log(countPlayLevel);
        // CheckDisplayWarningDailyGiftEvent();
        if (_flagCheckFetchRewardIcon || Utils.pauseUpdateFetchIcon) return;

        if (!Application.isEditor)
        {
            var status = Advertising.IsRewardedAdReady();
            fetchSkipLevelLose.SetActive(!status);
            fetchIncreaseRewardWin.SetActive(!status);
            fetchSkipLevelInGame.SetActive(!status);
        }
        else
        {
            if (fetchSkipLevelLose.activeSelf) fetchSkipLevelLose.SetActive(false);
            if (fetchIncreaseRewardWin.activeSelf) fetchIncreaseRewardWin.SetActive(false);
        }
    }

    public void OnReplay()
    {
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }

        Observer.HidePopupTutorialJigSaw?.Invoke();

        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
    }

    public void GoToMenu()
    {
        GamePopup.Instance.HidePopupMoney();
        if (!_canClickBtn)
        {
            return;
        }

        _canClickBtn = false;
        if (gameState == EGameState.Win)
        {
            Utils.currentCoin += Utils.CoinReward;
        }

        Observer.HidePopupTutorialJigSaw?.Invoke();

        if (Utils.IsJigsawMode)
        {
            mapLevel.gameObject.SetActive(false);
            GamePopup.Instance.ActivePopupRoom();
            if (GamePopup.Instance.currentRoom == null) GamePopup.Instance.ShowRoomGameplay();
            GamePopup.Instance.currentRoom.gameObject.SetActive(false);
            if (GamePopup.Instance.menuRoom == null) GamePopup.Instance.ShowRoom();
            GamePopup.Instance.menuRoom.gameObject.SetActive(true);
            GamePopup.Instance.menuRoom.ActiveDance();
        }

        FadeIn2(0.25f,
            () =>
            {
                if (ObjectPoolerManager.Instance != null) ObjectPoolerManager.Instance.ClearAllPool();
                _canClickBtn = true;
                SceneManager.LoadSceneAsync(Constants.MENU_SCENE_NAME);
                MenuController.instance.CheckDisplayWarningDailyGiftEvent();
                CheckDisplayWarningDailyGiftEvent();
            });
    }

    public void ShowPopupTransition()
    {
        GamePopup.Instance.ShowPopupTransition();
    }

    public void ToNormal()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.audioSouceBG.volume = 1;
        Utils.isHardMode = false;
        //background.gameObject.SetActive(false);
        canvasUI.SetActive(false);
        Destroy(mapLevel.gameObject);
        ObjectPoolerManager.Instance.ClearAllPool();
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
    }

    /// <summary>
    /// play sound click button
    /// </summary>
    public void SoundClickButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckShowRate()
    {
        var countShowRate = Data.CountShowRate;
        if (countShowRate > Config.RateConfigs.Length - 1)
        {
            countShowRate = Config.RateConfigs.Length - 1;
        }

        if (Utils.CurrentLevel == Data.LastLevelShowRate + Config.RateConfigs[countShowRate])
        {
            if (!Data.UserRated)
            {
                GamePopup.Instance.ShowRatePopup(SoundClickButton, SoundClickButton);
            }
        }
        else if (!Data.UserRated && Utils.CurrentLevel >
                 Data.LastLevelShowRate + Config.RateConfigs[Config.RateConfigs.Length - 1])
        {
            Data.LastLevelShowRate = Utils.CurrentLevel;
        }
    }

    public void SelectionButton()
    {
        MMVibrationManager.Haptic(HapticTypes.Selection, false, true, this);
    }

    public void SuccessButton()
    {
        MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
    }

    public void WarningButton()
    {
        MMVibrationManager.Haptic(HapticTypes.Warning, false, true, this);
    }

    public void FailureButton()
    {
        MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);
    }

    public void RigidButton()
    {
        MMVibrationManager.Haptic(HapticTypes.RigidImpact, false, true, this);
    }

    public void SoftButton()
    {
        MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
    }

    public void LightButton()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);
    }

    public void MediumButton()
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
    }

    public void HeavyButton()
    {
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact, false, true, this);
    }

    public void DisplayPopupNewSkin(SkinData skinData, bool isShirt)
    {
        popupNewSkin.gameObject.SetActive(true);
        popupNewSkin.Initialized(skinData, isShirt,
            ac => OnClaimSkin(skinData, popupNewSkin.ChangeAnimationOnClaim, ac),
            null);
    }

    public void DisplayPopupClaimCollection(Action onEnd)
    {
        popupClaimCollection.Initialized(onEnd);
        popupClaimCollection.gameObject.SetActive(true);
    }

    public void DisplayPopupGift()
    {
        popupGift.Initialized((ac) => OnClaimGift(popupGift.ChangeAnimationOnClaim, ac), null);
        popupGift.gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skinId"></param>
    /// <param name="skinName"></param>
    /// <param name="changeAnimation"></param>
    /// <param name="claimSuccess"></param>
    private void OnClaimSkin(SkinData skinData, Action changeAnimation, Action claimSuccess)
    {
#if UNITY_EDITOR
        // DataController.instance.SaveHero[skinId].unlock = true;
        // Data.currentHero = skinId;
        // if (SceneManager.GetActiveScene().name.Equals("MainGame"))
        // {
        //     PlayerManager.instance.ChangeSkin();
        // }
        skinData.IsUnlocked = true;
        Observer.UseSkin?.Invoke(skinData.skinName, SkinType.None);
        Observer.CurrentSkinChanged?.Invoke();
        changeAnimation?.Invoke();
        claimSuccess?.Invoke();
#else
        if(Utils.IsTurnOnAds)
        {Utils.pauseUpdateFetchIcon = true;
        RescueAnalytic.LogEventAdRewardRequest();
        Advertising.ShowRewardedAd().OnCompleted(() =>
        {
            Observable.Timer(TimeSpan.FromSeconds(0.3f))
                .Subscribe(_ =>
                {
                    skinData.IsUnlocked = true;
                    Observer.UseSkin?.Invoke(skinData.skinName, SkinType.None);
                    Observer.CurrentSkinChanged?.Invoke();
                    changeAnimation?.Invoke();
                    claimSuccess?.Invoke();
                    RescueAnalytic.LogEventProcessClaimSkin(skinData.skinName, Utils.currentCoin,
                        Utils.CurrentLevel);
                });
            Utils.pauseUpdateFetchIcon = false;
        });}
        else
        {
            skinData.IsUnlocked = true;
            Observer.UseSkin?.Invoke(skinData.skinName, SkinType.None);
            Observer.CurrentSkinChanged?.Invoke();
            changeAnimation?.Invoke();
            claimSuccess?.Invoke();
        }
#endif
    }

    private void OnClaimGift(Action actionChangeAnimation, Action action)
    {
#if UNITY_EDITOR
        Utils.currentCoin += 10000;
        OnUpdateCoin();
        actionChangeAnimation?.Invoke();
        action?.Invoke();
#else
if(Utils.IsTurnOnAds){
        Utils.pauseUpdateFetchIcon = true;
        RescueAnalytic.LogEventAdRewardRequest();
        Advertising.ShowRewardedAd().OnCompleted(() =>
        {
            Observable.Timer(TimeSpan.FromSeconds(0.3f))
                .Subscribe(_ =>
                {
                    instance.FlagShowRewardWinLevel = true;
                    Utils.currentCoin += 10000;
                    OnUpdateCoin();
                    actionChangeAnimation?.Invoke();
                    action?.Invoke();
                });

            Utils.pauseUpdateFetchIcon = false;
        });
}
        else
        {
            Utils.currentCoin += 10000;
            OnUpdateCoin();
            actionChangeAnimation?.Invoke();
            action?.Invoke();
        }

#endif
    }

    public void FadeIn(float duration, Action action)
    {
        fade.gameObject.SetActive(true);
        fade.alpha = 0;
        fade.DOFade(1, duration)
            .OnComplete(() =>
            {
                action?.Invoke();
                fade.gameObject.SetActive(false);
            });
    }

    public void FadeIn2(float duration, Action action)
    {
        fade.gameObject.SetActive(true);
        fade.alpha = 0;
        fade.DOFade(1, duration).OnComplete(() => { action?.Invoke(); });
    }

    public void FadeOut(float duration, Action action)
    {
        fade.gameObject.SetActive(true);
        fade.alpha = 1;
        fade.DOFade(0, duration)
            .OnComplete(() =>
            {
                action?.Invoke();
                fade.gameObject.SetActive(fade);
            });
    }

    public void RefreshNotiCastle()
    {
        if (DataController.instance == null) return;
        var regions = DataController.instance.CheckCastleNotification();

        var check = false;
        for (var i = 0; i < regions.Length; i++)
        {
            if (regions[i])
            {
                check = true;
                break;
            }
        }

        notiCastle.SetActive(check);
    }

    public void CheckDisplayWarningDailyGiftEvent()
    {
        if (Data.TotalGoldMedal >= 100 && Data.TotalGoldMedal < 200)
        {
            if (!DataController.instance.SaveHero[13].unlock ||
                !Utils.IsClaimEventReward() && Utils.curEventDailyGift <= 7)
            {
                warningEventValentine.SetActive(true);
            }
            else
                warningEventValentine.SetActive(false);
        }
        else if (Data.TotalGoldMedal >= 200 && Data.TotalGoldMedal < 300)
        {
            if (!DataController.instance.SaveHero[14].unlock ||
                !Utils.IsClaimEventReward() && Utils.curEventDailyGift <= 7)
            {
                warningEventValentine.SetActive(true);
            }
            else
                warningEventValentine.SetActive(false);
        }
        else if (Data.TotalGoldMedal >= 300 && Data.TotalGoldMedal < 400)
        {
            if (!DataController.instance.SaveHero[15].unlock ||
                !Utils.IsClaimEventReward() && Utils.curEventDailyGift <= 7)
            {
                warningEventValentine.SetActive(true);
            }
            else
                warningEventValentine.SetActive(false);
        }
        else if (Data.TotalGoldMedal >= 400 && Data.TotalGoldMedal < 500)
        {
            if (!DataController.instance.SavePrincess[6].unlock ||
                !Utils.IsClaimEventReward() && Utils.curEventDailyGift <= 7)
            {
                warningEventValentine.SetActive(true);
            }
            else
                warningEventValentine.SetActive(false);
        }
        else if (Data.TotalGoldMedal >= 500)
        {
            if (!DataController.instance.SavePrincess[7].unlock ||
                !Utils.IsClaimEventReward() && Utils.curEventDailyGift <= 7)
            {
                warningEventValentine.SetActive(true);
            }
            else
                warningEventValentine.SetActive(false);
        }
        else if (!Utils.IsClaimEventReward())
        {
            if (Utils.curEventDailyGift > 7)
            {
                warningEventValentine.SetActive(false);
            }
            else
            {
                warningEventValentine.SetActive(false);
            }
        }
    }

    public void VibrateStone()
    {
        if (instance.TimerChangeStone == null || !instance.TimerChangeStone.IsPlaying)
        {
            instance.TimerChangeStone = new Timer();
            instance.TimerChangeStone.FinishedAsObservable.Subscribe(_ =>
            {
                instance.TimerChangeStone?.Stop();
                instance.TimerChangeStone = null;
            });
            instance.TimerChangeStone.Start(0.2f);
            instance.RigidButton();
        }
    }

    private void OnDestroy()
    {
        Observer.UpdateBonusAdsButton -= UpdateTextBonusAdsButon;
        Observer.ShowHideTaskWarning -= ShowHideTaskWarning;
    }
}


[Serializable]
public class MissionType
{
    public Sprite spr_;
    public string strQuest;
}