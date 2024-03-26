using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GoogleMobileAds.Api;
using Pancake.Monetization;
using Pancake.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worldreaver.UniUI;
using Image = UnityEngine.UI.Image;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

    [Header("Button")] [SerializeField] private UniButton btnDebug;
    [SerializeField] private UniButton btnSetting;
    [SerializeField] private UniButton btnDailyReward;
    [SerializeField] private UniButton btnLevel;
    [SerializeField] private UniButton btnSkinShop;
    [SerializeField] private UniButton btnStartLevel;

    [Header("Warning")] public GameObject warningTask;
    public GameObject warningShop;
    public GameObject warningCollection;
    public GameObject warningDailyReward;
    public GameObject warningEvent;
    public GameObject warningEventValentine;

    [SerializeField] private GameObject lockTaskBtn;
    [SerializeField] private GameObject lockCollectionBtn;
    [SerializeField] private TextMeshProUGUI textLockTaskBtn;
    [SerializeField] private TextMeshProUGUI textUnLockTaskBtn;
    [SerializeField] private TextMeshProUGUI textUnLockCollectBtn;
    [SerializeField] private TextMeshProUGUI textLockCollectBtn;

    [SerializeField] private UniButton btnRemoveAds;
    [SerializeField] private UniButton btnToShop;
    [SerializeField] private UniButton btnCollection;
    [SerializeField] private UniButton btnEvent;
    [SerializeField] private UniButton btnTask;
    [SerializeField] private GameObject block;
    [SerializeField] private TextMeshProUGUI txtCoin;
    [SerializeField] private UniButton btnLeaderboard;
    [SerializeField] private CollectionBook _collectionBook;


    //public TextMeshProUGUI txtCurrentLevel;
    public GameObject homeGameObject;
    public GameObject intro;
    [SerializeField] private CanvasGroup fade;
    [SerializeField] private Sprite hardModeSprite;
    [SerializeField] private Sprite hardModeSpriteLock;
    [SerializeField] private SkinResources _skinResources;

    private List<SkinData> _allSkinDataLocked = new List<SkinData>();
    public GameObject Block => block;
    public UniButton BtnRemoveAds => btnRemoveAds;
    public TextMeshProUGUI TxtCoin => txtCoin;
    public static List<Info> SkinValentine = new List<Info>();

    private void Awake()
    {
        if (instance == null) instance = this;
        for (int i = 0; i < HeroData.Instance.infos.Length; i++)
        {
            if (HeroData.Instance.infos[i].typeUnlock == EHeroTypeUnlock.Valentine)
                SkinValentine.Add(HeroData.Instance.infos[i]);
        }

        btnEvent.gameObject.SetActive(Data.isTimeValentine && Data.TimeToRescueParty.TotalMilliseconds > 0);
        intro.gameObject.SetActive(Utils.IsFirstTimePLay);
        Observer.ShowHideTaskWarning += ShowHideTaskWarning;
        Observer.ShowHideShopWarning += CheckShopWarning;
    }

    private async void Start()
    {
        Utils.LoadGameData();
        GamePopup.Instance.HidePopupMoney();
        //txtCurrentLevel.text = "LEVEL " + (Utils.CurrentLevel + 1);
        txtCoin.text = $"{Utils.currentCoin}";
        SoundManager.Instance.PlayBackgroundMusic();
        CheckDisplayWarningDailyGift();
        CheckDisplayWarningDailyGiftEvent();
        CheckWaringForCollection();
        Observer.ShowTaskToturial?.Invoke();
        Observer.ShowCollectionTutorial?.Invoke();

        btnSetting.onClick.RemoveAllListeners();
        btnSetting.onClick.AddListener(OnSettingButtonPressed);
        CheckTask();

        btnDailyReward.onClick.RemoveAllListeners();
        btnDailyReward.onClick.AddListener(OnDailyRewaredButtonPressed);

        btnSkinShop.onClick.RemoveAllListeners();
        btnSkinShop.onClick.AddListener(OnSkinButtonPressed);


        btnStartLevel.onClick.RemoveAllListeners();
        btnStartLevel.onClick.AddListener(OnStartLevelButtonPressed);

        btnLevel.onClick.RemoveAllListeners();
        btnLevel.onClick.AddListener(OnLevelButtonPressed);

        btnToShop.onClick.RemoveAllListeners();
        btnToShop.onClick.AddListener(OnShopButtonPressed);

        btnEvent.onClick.RemoveAllListeners();
        btnEvent.onClick.AddListener(OnEventButtonPressed);


        btnLeaderboard.onClick.RemoveAllListeners();
        btnLeaderboard.onClick.AddListener(OnLeaderboardButtonPressed);
        CheckCollection();


        btnDebug.gameObject.SetActive(Config.IsDebug);


        SetupSkinData();

        #region btn remove ads

        if (Data.RemoveAds || Data.UserVip)
        {
            btnRemoveAds.gameObject.SetActive(false);
        }
        else
        {
            //disable button remove ads
            btnRemoveAds.gameObject.SetActive(false);
        }

        btnRemoveAds.onClick.RemoveAllListeners();
        btnRemoveAds.onClick.AddListener(OnRemoveAdsButtonPressed);

        Observer.ShowHideShopWarning?.Invoke();

        #endregion

        GamePopup.Instance.ShowRoom(isDance: true);
        await UniTask.WaitUntil(() => GamePopup.Instance.menuRoom != null);

        homeGameObject.SetActive(true); // TODO
        RefreshNotiCastle();

        if (Data.WeekNumber >= 1)
        {
            DataController.instance.SaveHero[3].unlock = true;
        }

        if (Data.WeekNumber >= 2)
        {
            DataController.instance.SaveHero[3].unlock = true;
            DataController.instance.SaveHero[4].unlock = true;
        }

        if (Data.WeekNumber >= 3)
        {
            DataController.instance.SaveHero[3].unlock = true;
            DataController.instance.SaveHero[4].unlock = true;
            DataController.instance.SaveHero[5].unlock = true;
        }

        Advertising.DestroyBannerAd();

        GamePopup.Instance.CanvasRoom.worldCamera.depth = 0;
        if (Utils.CurrentLevel >= 2 && Utils.CurrentLevel != 10)
        {
            if (Data.turnOnUpdate && Data.currentVersion != Application.version && !Data.DontShowUpdate)
            {
                GamePopup.Instance.ShowUpdatePopup(Data.updateDescription, Data.currentVersion, () =>
                    {
                        CheckIsFirstOpen();
                        SoundClickButton();
                    },
                    SoundClickButton);
            }
            else
            {
                CheckIsFirstOpen();
            }
        }

        DataController.instance.CheckWarningForTask();
    }

    public void CheckIsFirstOpen()
    {
        var currentLv = BridgeData.Instance.previousLevelLoaded.GetComponent<MapLevelManager>();
        if (!currentLv.isTaskTutorial)
        {
            if (Utils.IsFirstOpenInDay())
            {
                ShowDailyReward(null);
                Utils.SetFirstOpenKey();
            }
        }
    }

    private void SetupSkinData()
    {
        _allSkinDataLocked = new List<SkinData>();
        foreach (var skinResource in _skinResources.skinDataResourcesList)
        {
            foreach (SkinData skinDataList in skinResource.skinDataList)
            {
                if (!skinDataList.IsUnlocked && skinDataList.skinBuyType != SkinBuyType.Level)
                {
                    _allSkinDataLocked.Add(skinDataList);
                }
            }
        }
    }

    private void ShowHideTaskWarning(bool isShow)
    {
        warningTask.SetActive(isShow);
    }

    private void CheckShopWarning()
    {
        foreach (var skin in _allSkinDataLocked)
        {
            if (Utils.currentCoin >= skin.coinValue && skin.skinBuyType == SkinBuyType.BuyCoin && !skin.IsUnlocked)
            {
                warningShop.SetActive(true);
                return;
            }
        }

        warningShop.SetActive(false);
    }

    void CheckTask()
    {
        if (Utils.CurrentLevel < 10)
        {
            lockTaskBtn.gameObject.SetActive(true);
            textUnLockTaskBtn.gameObject.SetActive(false);
            textLockTaskBtn.gameObject.SetActive(true);
        }
        else
        {
            lockTaskBtn.gameObject.SetActive(false);
            textUnLockTaskBtn.gameObject.SetActive(true);
            textLockTaskBtn.gameObject.SetActive(false);
            btnTask.onClick.RemoveAllListeners();
            btnTask.onClick.AddListener(OnTaskButtonPressed);
        }
    }

    void CheckCollection()
    {
        if (Utils.CurrentLevel < 10)
        {
            lockCollectionBtn.gameObject.SetActive(true);
            textUnLockCollectBtn.gameObject.SetActive(false);
            textLockCollectBtn.gameObject.SetActive(true);
        }
        else
        {
            lockCollectionBtn.gameObject.SetActive(false);
            textUnLockCollectBtn.gameObject.SetActive(true);
            textLockCollectBtn.gameObject.SetActive(false);
            btnCollection.onClick.RemoveAllListeners();
            btnCollection.onClick.AddListener(OnCollectionButtonPressed);
        }
    }

    private void OnPetButtonPressed()
    {
        GamePopup.Instance.HideAll();
        GamePopup.Instance.ShowPopupEaster(RefreshNotiPet);
    }

    public void RefreshNotiPet()
    {
        if (warningEvent != null)
        {
            warningEvent.SetActive(Data.CheckConditionRewardEventEaster());
        }

        DataController.instance.SaveData();
        txtCoin.text = $"{Utils.currentCoin}";
    }

    private void OnShopButtonPressed()
    {
        ShowShop(null, () =>
        {
            foreach (var skin in _allSkinDataLocked)
            {
                skin.IsUnlocked = true;
            }

            CheckShopWarning();
        }, null);
        SoundClickButton();
    }

    private void CheckWaringForCollection()
    {
        List<CollectionPage> listPage = _collectionBook.ListPage;
        foreach (CollectionPage page in listPage)
        {
            if (!page.IsCollected && page.GetLastestItem() == null)
            {
                warningCollection.SetActive(true);
                return;
            }
        }

        warningCollection.SetActive(false);
    }

    private void OnCollectionButtonPressed()
    {
        GamePopup.Instance.HideAll();
        SoundClickButton();
        ShowCollection();
    }

    /// <summary>
    /// button setting pressed
    /// </summary>
    private void OnSettingButtonPressed()
    {
        ShowSetting();
        SoundClickButton();
    }

    private void OnHomePressed()
    {
        GamePopup.Instance.HideAll();
        SoundClickButton();
    }

    /// <summary>
    /// daily reward button pressed
    /// </summary>
    private void OnDailyRewaredButtonPressed()
    {
        ShowDailyReward(null);
        SoundClickButton();
    }

    private void OnTaskButtonPressed()
    {
        ShowTask(false);
        SoundClickButton();
    }

    /// <summary>
    /// daily reward button pressed
    /// </summary>
    private void OnEventButtonPressed()
    {
        SoundClickButton();
        ShowEventReward(null);
    }


    /// <summary>
    /// skin button pressed
    /// </summary>
    private void OnSkinButtonPressed()
    {
        GamePopup.Instance.HideAll();
        ShowSkinShop();
        SoundClickButton();
    }

    /// <summary>
    /// castle button pressed
    /// </summary>
    private void OnCastleButtonPressed()
    {
        ShowCastle();
        SoundClickButton();
    }

    private void OnLevelButtonPressed()
    {
        GamePopup.Instance.HideAll();
        ShowTravelStory();
        SoundClickButton();
    }

    /// <summary>
    /// start button pressed
    /// </summary>
    public void OnStartLevelButtonPressed()
    {
        InternalNextLevel();
        SoundManager.Instance.PlaySound(SoundManager.Instance.btnStart);
    }

    public void FadeIn2(float duration, Action action)
    {
        fade.gameObject.SetActive(true);
        fade.alpha = 0;
        fade.DOFade(1, duration).OnComplete(() => { action?.Invoke(); });
    }

    private void OnLeaderboardButtonPressed()
    {
        if (!string.IsNullOrEmpty(Playfab.displayName))
        {
            GamePopup.Instance.ShowLeaderboard();
        }
        else
        {
            GamePopup.Instance.ShowPopupLogin(GamePopup.Instance.ShowLeaderboard);
        }
    }

    public void InternalNextLevel()
    {
        txtCoin.transform.parent.gameObject.SetActive(false);
        GamePopup.Instance.ShowPopupTransition();
        FadeIn2(0.25f,
            () =>
            {
                float time = Utils.IsJigsawMode == true ? 0f : 0.3f;
                DOTween.Sequence().AppendInterval(time).OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
                    GamePopup.Instance.HidePopupTutorial();
                });
                if (GamePopup.Instance.menuRoom != null) GamePopup.Instance.menuRoom.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// facebook button pressed
    /// </summary>
    private void OnFacebookButtonPressed()
    {
        GamePopup.Instance.ShowPopupGotoFacebook(SoundClickButton, SoundClickButton);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnRemoveAdsButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Data.RemoveAds = true;
            btnRemoveAds.gameObject.SetActive(false);
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_REMOVE_ADS);
    }

    public void UpdateTextCoin()
    {
        txtCoin.text = $"{Utils.currentCoin}";
    }

    private void ShowChoseLevel()
    {
        GamePopup.Instance.ShowChooseLevelPopup(SoundClickButton);
    }

    private void ShowTravelStory()
    {
        GamePopup.Instance.ShowTravelStoryPopup(SoundClickButton);
    }

    /// <summary>
    /// 
    /// </summary>
    public void CheckDisplayWarningDailyGift()
    {
        if (!Utils.IsClaimReward()) warningDailyReward.SetActive(true);
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
                warningEventValentine.SetActive(true);
            }
        }
    }


    /// <summary>
    /// play sound click button
    /// </summary>
    public void SoundClickButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
    }

    /// <summary>
    /// show setting popup
    /// </summary>
    private void ShowSetting()
    {
        GamePopup.Instance.ShowSettingPopup(SoundClickButton, SoundClickButton, SoundClickButton, SoundClickButton);
    }

    /// <summary>
    /// display daily reward popup
    /// </summary>
    private void ShowDailyReward(Action actionUpdateCurrency, Action actionBack = null)
    {
        warningDailyReward.SetActive(false);
        GamePopup.Instance.ShowDailyRewardPopup(() =>
            {
                SoundClickButton();
                CheckDisplayWarningDailyGift();
                RefreshNotiCastle();
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
                if (actionBack != null)
                    actionBack?.Invoke();
            },
            () => txtCoin.text = $"{Utils.currentCoin}",
            () =>
            {
                txtCoin.text = $"{Utils.currentCoin}";
                SoundClickButton();
            },
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

    /// <summary>
    /// display event reward popup
    /// </summary>
    public void ShowEventReward(Action actionUpdateCurrency, TabEvent tab = TabEvent.Event)
    {
        GamePopup.Instance.ShowPopupEvent(() =>
            {
                SoundClickButton();
                CheckDisplayWarningDailyGiftEvent();
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
                // if (MenuController.instance!=null)
                // {
                warningEventValentine.SetActive(false);
                // }
            }
            , tab);
    }

    public void ShowTask(bool isCallFromTut)
    {
        GamePopup.Instance.ShowTaskdPopup(() =>
            {
                SoundClickButton();
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
                // }
            }, isCallFromTut
        );
    }

    /// <summary>
    /// display shop popup
    /// </summary>
    public void ShowShop(Action updateCurrency, Action actionRefreshUnlockAllHero, Action actionRefreshAllItem)
    {
        GamePopup.Instance.ShowShopPopup(() =>
            {
                SoundClickButton();
                actionRefreshAllItem?.Invoke();
                RefreshNotiCastle();
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
            },
            updateCurrency,
            actionRefreshUnlockAllHero);
    }

    public void ShowCollection()
    {
        GamePopup.Instance.ShowCollectionPopup(() =>
        {
            SoundClickButton();
            DataController.instance.SaveData();
            txtCoin.text = $"{Utils.currentCoin}";
            CheckWaringForCollection();

            DataController.instance.CheckWarningForTask();
            //GamePopup.Instance.HidePopupMoney();
        });
    }

    private void RefreshNotiCastle()
    {
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

        //warningCastle.SetActive(check);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowSkinShop()
    {
        GamePopup.Instance.ShowSkinPopup(() =>
            {
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
            },
            SoundClickButton,
            ShowDailyReward,
            ShowShop,
            null);
    }

    /// <summary>
    /// show garden
    /// </summary>
    private void ShowCastle()
    {
        GamePopup.Instance.ShowCastlePopup(() =>
            {
                SoundClickButton();
                RefreshNotiCastle();
                DataController.instance.SaveData();
                txtCoin.text = $"{Utils.currentCoin}";
            },
            SoundClickButton,
            SoundClickButton,
            ShowShop);
    }

    public void OnDebugButtonPressed()
    {
        SoundClickButton();
        GamePopup.Instance.ShowDebugPopup(SoundClickButton, SoundClickButton);
    }

    private void OnDestroy()
    {
        instance = null;
        Observer.ShowHideTaskWarning -= ShowHideTaskWarning;
        Observer.ShowHideShopWarning -= CheckShopWarning;
    }
}