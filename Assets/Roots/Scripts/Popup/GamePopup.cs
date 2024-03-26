#pragma warning disable 649
using System;
using Cysharp.Threading.Tasks;
using Pancake.Monetization;
using Pancake.Threading.Tasks;
using UnityEngine;
using Worldreaver.Root.Attribute;
using Worldreaver.UniUI;

public class GamePopup : Singleton<GamePopup>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Canvas canvasRoom;
    [SerializeField] private GameObject popupRoom;
    private UniPopup _uniPopup;
    [SerializeField] private PopupSetting popupSettingPrefab;
    [SerializeField] private PopupDailyReward popupDailyrewardPrefb;
    [SerializeField] private PopupTask popupTaskPrefab;
    [SerializeField] private PopupSkin popupSkinPrefab;
    [SerializeField] private PopupRate popupRatePrefab;
    [SerializeField] private PopupShop popupShopPrefab;
    [SerializeField] private PopupTutorialCollection popupTutorialCollection;
    [SerializeField] private PopupCollectionTutorial popupCollectionTutorial;
    [SerializeField] private PopupCollection popupCollectionPrefab;
    [SerializeField] private PopupChooseLevel popupChooseLevelPrefab;
    [SerializeField] private PopupTutorialJigSaw popupTutorialJigSaw;
    [SerializeField] private PopupUpdate popupUpdatePrefab;
    [SerializeField] private PopupDebug popupDebugPrefab;
    [SerializeField] private PopupChestTask popupChestTask;
    [ReadOnly] public BaseRoom menuRoom;
    [ReadOnly] public BaseRoom currentRoom;
    [ReadOnly] public BaseRoom newRoom;
    [SerializeField] private PopupNotification popupNotificationPrefab;
    [SerializeField] private PopupOkCloseNotification popupOkCloseNotificationPrefab;
    [SerializeField] private PopupFlag popupFlagPrefab;
    [SerializeField] private PopupLeaderboard popupLeaderboardPrefab;
    [SerializeField] private PopupLeaderboardEvent popupLeaderboardEventPrefab;
    [SerializeField] private ChooseNamePopup popupLoginPrefab;
    [SerializeField] private PopupEaster popupEasterPrefab;
    [SerializeField] public PopupEvent popupEventPrefab;
    [SerializeField] private PopupMoney popupMoney;
    [SerializeField] private PopupTutotial popupTutotial;
    [SerializeField] private PopupTransition popupTransition;
    [SerializeField] private PopupTravelStory popupStoryTravelPrefab;

    public IUniPopupHandler popupCollectionTutorialHandler;
    public IUniPopupHandler popupNotificationHandler;
    public IUniPopupHandler popupTutotialHandler;
    public IUniPopupHandler popupTransitionHandler;
    public IUniPopupHandler popupTutotialJigSawHandler;
    public IUniPopupHandler popupTutotialCollectionHandler;
    public IUniPopupHandler popupSettingHandler;
    public IUniPopupHandler popupChestTaskHandler;
    public IUniPopupHandler popupDailyRewardHandler;
    public IUniPopupHandler popupSkinHandler;
    public IUniPopupHandler popupRateHandler;
    public IUniPopupHandler popupLinkFacebookHandler;
    public IUniPopupHandler popupMoneyHandler;
    public IUniPopupHandler popupShopHandler;
    public IUniPopupHandler popupCollectionHandler;
    public IUniPopupHandler popupTravelStoryHandler;
    public IUniPopupHandler popupUpdateHandler;
    public IUniPopupHandler popupDebugHandler;
    public IUniPopupHandler popupChooseLevelHandler;
    public IUniPopupHandler popupFlagHander;
    public IUniPopupHandler popupLeaderboardHandler;
    public IUniPopupHandler popupLeaderboardEventHandler;
    public IUniPopupHandler popupHardModeNotiHandler;
    public IUniPopupHandler popupOkCloseHandler;
    public IUniPopupHandler popupLoginHandler;
    public IUniPopupHandler popupHardModeHandler;
    public IUniPopupHandler popupGifCodeHandler;
    public IUniPopupHandler popupTask;
    public IUniPopupHandler popupEasterHandler;
    public IUniPopupHandler popupEventPrefabHandler;

    public Canvas Canvas => canvas;
    public Canvas CanvasRoom => canvasRoom;

    public UniPopup Popup => _uniPopup ?? (_uniPopup = new UniPopup());
    public Action endLevelJigsaw;

    /// <summary>
    /// 
    /// </summary>
    public void Hide()
    {
        {
            _uniPopup?.Hide();
        }
    }

    public void HideAll()
    {
        {
            _uniPopup?.HideAll();
        }
    }

    #region open api

    #region debug

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void ShowDebugPopup(Action actionBack, Action actionOk)
    {
        if (popupDebugHandler != null)
        {
            if (popupDebugHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupDebugHandler = Instantiate(popupDebugPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var update = (PopupDebug)popupDebugHandler;
            update.Initialized(() =>
                {
                    Hide();
                    actionBack?.Invoke();
                },
                () =>
                {
                    Hide();
                    actionOk?.Invoke();
                });
            Popup.Show(popupDebugHandler);
        }
    }

    #endregion

    #region halloween end

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionOk"></param>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="nameBtnOk"></param>
    public void ShowNotificationPopup(Action actionOk, string message, string title = "", string nameBtnOk = "OK")
    {
        if (popupNotificationHandler != null)
        {
            if (popupNotificationHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupNotificationHandler = Instantiate(popupNotificationPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var noti = (PopupNotification)popupNotificationHandler;
            noti.Initialized(() =>
                {
                    Hide();
                    actionOk?.Invoke();
                },
                null,
                message,
                title,
                nameBtnOk);
            Popup.Show(popupNotificationHandler);
        }
    }

    #endregion

    #region shop

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionUpdateCurrentcy"></param>
    /// <param name="actionRefreshUnlockHero"></param>
    public void ShowShopPopup(Action actionBack, Action actionUpdateCurrentcy, Action actionRefreshUnlockHero)
    {
        if (popupShopHandler != null)
        {
            if (popupShopHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupShopHandler = Instantiate(popupShopPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var shopPopup = (PopupShop)popupShopHandler;
            shopPopup.Initialized(() =>
                {
                    HideAll();
                    actionBack?.Invoke();
                },
                actionUpdateCurrentcy,
                actionRefreshUnlockHero);
            Popup.Show(popupShopHandler);
            ShowPopupMoney();
        }
    }

    public void ShowCollectionPopup(Action actionBack)
    {
        if (popupCollectionHandler != null)
        {
            if (popupCollectionHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupCollectionHandler = Instantiate(popupCollectionPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var collectionPopup = (PopupCollection)popupCollectionHandler;
            collectionPopup.Initialized(() =>
            {
                Hide();
                actionBack?.Invoke();
            }, actionBack);
            Popup.Show(popupCollectionHandler);
            ShowPopupMoney();
        }
    }

    public void ShowTravelStoryPopup(Action actionBack)
    {
        if (popupTravelStoryHandler != null)
        {
            if (popupTravelStoryHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupTravelStoryHandler = Instantiate(popupStoryTravelPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popupStoryTravel = (PopupTravelStory)popupTravelStoryHandler;
            popupStoryTravel.Initialized(() => { actionBack?.Invoke(); });
            Popup.Show(popupTravelStoryHandler);
        }
    }

    #endregion

    #region rate

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void ShowRatePopup(Action actionBack, Action actionOk)
    {
        if (popupRateHandler != null)
        {
            if (popupRateHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupRateHandler = Instantiate(popupRatePrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var ratePoup = (PopupRate)popupRateHandler;
            ratePoup.Initialized(() =>
                {
                    Hide();
                    actionBack?.Invoke();
                },
                () =>
                {
                    Hide();
                    actionOk?.Invoke();
                });
            Popup.Show(popupRateHandler);
        }
    }

    public void ShowPopupMoney()
    {
        if (popupMoneyHandler != null)
        {
            if (popupMoneyHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupMoneyHandler = Instantiate(popupMoney, canvas.transform, false);
        Display();

        void Display()
        {
            Popup.Show(popupMoneyHandler);
        }
    }

    public void HidePopupMoney()
    {
        if (popupMoneyHandler == null) return;
        popupMoneyHandler.Close();
    }

    public void HidePopupTask()
    {
        if (popupTask == null) return;
        popupTask.Close();
        GameManager.instance.isShowEvent = false;
    }

    public void HidePopupTutorial()
    {
        if (popupTutotialHandler == null) return;
        popupTutotialHandler.Close();
    }

    public void HidePopupCollectionTutorial()
    {
        if (popupTutotialCollectionHandler == null) return;
        popupTutotialCollectionHandler.Close();
    }

    public void HidePopupTutorialJigSaw()
    {
        if (popupTutotialJigSawHandler == null) return;
        popupTutotialJigSawHandler.Close();
    }

    #endregion

    #region update

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updateDescription"></param>
    /// <param name="newVersion"></param>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void ShowUpdatePopup(string updateDescription, string newVersion, Action actionBack, Action actionOk)
    {
        if (popupUpdateHandler != null)
        {
            if (popupUpdateHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupUpdateHandler = Instantiate(popupUpdatePrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var update = (PopupUpdate)popupUpdateHandler;
            update.Initialized(updateDescription,
                newVersion,
                () =>
                {
                    //show daily popup if is first open in day
                    Hide();
                    actionBack?.Invoke();
                },
                () =>
                {
                    Hide();
                    actionOk?.Invoke();
                });
            Popup.Show(popupUpdateHandler);
        }
    }

    #endregion

    #region on notification

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void ShowPopupGotoFacebook(Action actionBack = null, Action actionOk = null)
    {
        if (popupLinkFacebookHandler != null)
        {
            if (popupLinkFacebookHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupLinkFacebookHandler = Instantiate(popupNotificationPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var notification = (PopupNotification)popupLinkFacebookHandler;
            notification.Initialized(() =>
                {
                    Hide();
                    actionOk?.Invoke();
                },
                () =>
                {
                    Hide();
                    actionBack?.Invoke();
                },
                "Join <color=#ffc000>Prison Pin: Pull The Pin</color>\ncommunity on Facebook to\nshare your ideas and\nplay another game like this.",
                "JOIN US!",
                "JOIN",
                true);
            Popup.Show(popupLinkFacebookHandler);
        }
    }

    public void ShowPopupChestTask(int getCoint, Sprite getSprite, EChestType getChestType)
    {
        if (popupChestTaskHandler != null)
        {
            if (popupChestTaskHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupChestTaskHandler = Instantiate(popupChestTask, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popupChestTask = (PopupChestTask)popupChestTaskHandler;
            popupChestTask.Init((() => { Hide(); }), getCoint, getSprite, getChestType);
            Popup.Show(popupChestTaskHandler);
            ShowPopupMoney();
        }
    }

    #endregion

    #region castle

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionCloseBoard"></param>
    /// <param name="actionBuild"></param>
    public void ShowCastlePopup(Action actionBack, Action actionCloseBoard, Action actionBuild,
        Action<Action, Action, Action> actionToShop)
    {
        return;
        // if (popupCastleHandler != null)
        // {
        //     if (popupCastleHandler.ThisGameObject.activeSelf) return;
        //
        //     Display();
        //     return;
        // }
        //
        // popupCastleHandler = Instantiate(popupCastlePrefab, canvas.transform, false);
        // Display();

        //         void Display()
        //         {
        //             if (AdsManager.Instance != null) AdsManager.Instance.HideBanner();
        // #if IRONSOURCE
        //             if (AdsIronSourceManager.Instance != null) AdsIronSourceManager.Instance.HideBanner();
        // #endif
        //
        //
        //             // initialize
        //             var castlePoup = (PopupCastle) popupCastleHandler;
        //             castlePoup.Initialized(() =>
        //                 {
        //                     Hide();
        //                     actionBack?.Invoke();
        //                     // if (AdsManager.Instance != null)
        //                     // {
        //                     //     RescueAnalytic.LogEventAdBannerRequest();
        //                     //     AdsManager.Instance.ShowBanner();
        //                     // }
        //                 },
        //                 actionCloseBoard,
        //                 actionBuild,
        //                 actionToShop);
        //             Popup.Show(popupCastleHandler);
        //         }
    }

    #endregion

    #region reward

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionClaimByVideo"></param>
    /// <param name="actionClaim"></param>
    /// <param name="actionSpecial"></param>
    /// <param name="actionUpdateCurrency"></param>
    /// <param name="actionToShop"></param>
    public void ShowDailyRewardPopup(
        Action actionBack,
        Action actionClaimByVideo,
        Action actionClaim,
        Action<int> actionSpecial,
        Action actionUpdateCurrency,
        Action<Action, Action, Action> actionToShop)
    {
        if (popupDailyRewardHandler != null)
        {
            if (popupDailyRewardHandler.ThisGameObject.activeSelf) return;
            else popupDailyRewardHandler.ThisGameObject.SetActive(true);
            Display();
            return;
        }

        popupDailyRewardHandler = Instantiate(popupDailyrewardPrefb, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var rewardPopup = (PopupDailyReward)popupDailyRewardHandler;
            rewardPopup.Initialized(() =>
                {
                    rewardPopup.Close();
                    actionBack?.Invoke();
                },
                actionClaimByVideo,
                actionClaim,
                actionSpecial,
                actionUpdateCurrency,
                actionToShop);
            Popup.Show(popupDailyRewardHandler);
            ShowPopupMoney();
        }
    }

    public void ShowTaskdPopup(
        Action actionBack, bool isCallFromTut)
    {
        if (popupTask != null)
        {
            if (popupTask.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupTask = Instantiate(popupTaskPrefab, canvas.transform, false);
        var taskPopup = (PopupTask)popupTask;
        taskPopup.Initialized(actionBack, isCallFromTut);
        Display();

        void Display()
        {
            // initialize
            var taskPopup = (PopupTask)popupTask;
            taskPopup.Initialized(actionBack, isCallFromTut);
            Popup.Show(popupTask);
            if (!isCallFromTut) ShowPopupMoney();
            // popupMoney.Canvas.sortingOrder = taskPopup.Canvas.sortingOrder + 1;
        }
    }

    #endregion


    #region skin

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionBuyByCoin"></param>
    /// <param name="actionDailyReward"></param>
    /// <param name="actionToShop"></param>
    /// <param name="actionBuyByVideo"></param>
    public void ShowSkinPopup(
        Action actionBack,
        Action actionBuyByCoin,
        Action<Action, Action> actionDailyReward,
        Action<Action, Action, Action> actionToShop,
        Action actionBuyByVideo)
    {
        if (popupSkinHandler != null)
        {
            if (popupSkinHandler.ThisGameObject.activeSelf) return;
            Display();
            return;
        }

        popupSkinHandler = Instantiate(popupSkinPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            Advertising.DestroyBannerAd();


            var skinPopup = (PopupSkin)popupSkinHandler;
            skinPopup.Initialize(actionBack, actionDailyReward);
            Popup.Show(popupSkinHandler);
            ShowPopupMoney();
        }
    }

    #endregion

    #region setting

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionSound"></param>
    /// <param name="actionMusic"></param>
    /// <param name="actionVibrate"></param>
    public void ShowSettingPopup(Action actionBack, Action actionSound, Action actionMusic, Action actionVibrate)
    {
        if (popupSettingHandler != null)
        {
            if (popupSettingHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupSettingHandler = Instantiate(popupSettingPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            var settingPopup = (PopupSetting)popupSettingHandler;
            settingPopup.Initialized(() =>
                {
                    Hide();
                    actionBack?.Invoke();
                },
                actionSound,
                actionMusic,
                actionVibrate);
            Popup.Show(popupSettingHandler);
        }
    }

    #endregion

    #region room

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hide"></param>
    /// <param name="isDance"></param>
    /// <param name="action"></param>
    public async void ShowRoom(bool hide = false, bool isDance = false, Action action = null)
    {
        if (menuRoom != null)
        {
            if (menuRoom.gameObject.activeSelf) return;

            menuRoom.gameObject.SetActive(!hide);
            menuRoom.Initialized(isDance);
            return;
        }

        try
        {
            if (BridgeData.Instance.menuRoomPrefab == null)
            {
                await UniTask.WaitUntil(() => BridgeData.Instance.menuRoomPrefab != null);
            }
        }
        catch (Exception)
        {
            //
        }

        menuRoom = Instantiate(BridgeData.Instance.menuRoomPrefab, CanvasRoom.transform);


        menuRoom.gameObject.SetActive(!hide);
        if (!hide)
        {
            menuRoom.Initialized(isDance);
        }

        menuRoom.ActionActiveCameraEffect = action;
    }

    public async void ShowRoomGameplay(bool hide = false, bool force = false, bool isDance = false,
        Action action = null)
    {
        if (currentRoom != null)
        {
            if (force)
            {
                Destroy(currentRoom.gameObject);
            }
            else
            {
                if (currentRoom.gameObject.activeSelf) return;

                CanvasRoom.worldCamera.depth = 3;
                currentRoom.gameObject.SetActive(true);
                currentRoom.Initialized(isDance);
                return;
            }
        }

        try
        {
            if (BridgeData.Instance.currentRoomPrefab == null)
            {
                await UniTask.WaitUntil(() => BridgeData.Instance.currentRoomPrefab != null);
            }
        }
        catch (Exception)
        {
            //
        }

        CanvasRoom.worldCamera.depth = 3;
        currentRoom = Instantiate(BridgeData.Instance.currentRoomPrefab, CanvasRoom.transform);
        currentRoom.gameObject.SetActive(!hide);
        currentRoom.Initialized(isDance);
        currentRoom.ActionActiveCameraEffect = action;
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideRoom()
    {
        currentRoom.gameObject.SetActive(false);
    }

    #endregion

    #region chooseLevel

    public void ShowChooseLevelPopup(Action actionBack)
    {
        if (popupChooseLevelHandler != null)
        {
            if (popupChooseLevelHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupChooseLevelHandler = Instantiate(popupChooseLevelPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            var choose = (PopupChooseLevel)popupChooseLevelHandler;
            choose.Initialized(() =>
            {
                Hide();
                actionBack?.Invoke();
            });
            Popup.Show(popupChooseLevelHandler);
        }
    }

    #endregion

    #region flag

    /// <summary>
    /// 
    /// </summary>
    public void ShowFlagPopup()
    {
        if (popupFlagHander != null)
        {
            if (popupFlagHander.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupFlagHander = Instantiate(popupFlagPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupFlag)popupFlagHander;
            popup.Initialize(Hide);
            Popup.Show(popupFlagHander);
        }
    }

    #endregion

    #region leaderboard

    /// <summary>
    /// 
    /// </summary>
    public void ShowLeaderboard()
    {
        if (popupLeaderboardHandler != null)
        {
            if (popupLeaderboardHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupLeaderboardHandler = Instantiate(popupLeaderboardPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var shopPopup = (PopupLeaderboard)popupLeaderboardHandler;
            shopPopup.Initialized(Hide);
            Popup.Show(popupLeaderboardHandler);
        }
    }

    public void ShowLeaderboardEvent()
    {
        if (popupLeaderboardEventHandler != null)
        {
            if (popupLeaderboardEventHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupLeaderboardEventHandler = Instantiate(popupLeaderboardEventPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var shopPopup = (PopupLeaderboardEvent)popupLeaderboardEventHandler;
            shopPopup.Initialized(Hide);
            Popup.Show(popupLeaderboardEventHandler);
        }
    }

    #endregion

    #region ok close notification

    /// <summary>
    /// 
    /// </summary>
    public void ShowOkCloseNotification(
        Action actionRestore,
        Action actionBackup,
        string message,
        string title,
        int serverLevel,
        int serverCoin,
        int serverTotalSkin,
        string titleRight,
        string titleLeft,
        bool isBackup)
    {
        if (popupOkCloseHandler != null)
        {
            if (popupOkCloseHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupOkCloseHandler = Instantiate(popupOkCloseNotificationPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popupOkClose = (PopupOkCloseNotification)popupOkCloseHandler;
            popupOkClose.Initialized(() =>
                {
                    Hide();
                    if (!isBackup)
                    {
                        actionRestore?.Invoke();
                    }
                    else
                    {
                        actionBackup?.Invoke();
                    }
                },
                Hide,
                message,
                title,
                serverLevel,
                serverCoin,
                serverTotalSkin,
                titleRight,
                titleLeft,
                isBackup);
            Popup.Show(popupOkCloseHandler);
        }
    }

    #endregion

    public void ShowPopupLogin(Action actionOpenPopup)
    {
        if (popupLoginHandler != null)
        {
            if (popupLoginHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupLoginHandler = Instantiate(popupLoginPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (ChooseNamePopup)popupLoginHandler;
            popup.Initialized(Hide, actionOpenPopup);
            Popup.Show(popupLoginHandler);
        }
    }

    public void ShowPopupTransition()
    {
        if (popupTransitionHandler != null)
        {
            if (popupTransitionHandler.ThisGameObject.activeSelf) return;
            Display();
            return;
        }

        popupTransitionHandler = Instantiate(popupTransition, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupTransition)popupTransitionHandler;
            popup.Initialized();
            popup.Show();
            //popupMoney.Canvas.sortingOrder = popup.Canvas.sortingOrder + 1;
        }
    }

    public void ShowPopupTutorial()
    {
        if (popupTutotialHandler != null)
        {
            if (popupTutotialHandler.ThisGameObject.activeSelf) return;
            Display();
            return;
        }

        popupTutotialHandler = Instantiate(popupTutotial, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupTutotial)popupTutotialHandler;
            popup.Initialized(() =>
            {
                SetActiveMaskTutorialInPopupTask(true);
                Hide();
            });
            Popup.Show(popupTutotialHandler);
        }
    }

    public void ShowPopupTutorialJigSaw()
    {
        if (popupTutotialJigSawHandler != null)
        {
            if (popupTutotialJigSawHandler.ThisGameObject.activeSelf) return;
            Display();
            return;
        }

        popupTutotialJigSawHandler = Instantiate(popupTutorialJigSaw, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupTutorialJigSaw)popupTutotialJigSawHandler;
            popup.Initialized(Hide);
            Popup.Show(popupTutotialJigSawHandler);
            //popupMoney.Canvas.sortingOrder = popup.Canvas.sortingOrder + 1;
        }
    }

    public void ShowPopupTutorialCollection()
    {
        if (popupTutotialCollectionHandler != null)
        {
            if (popupTutotialCollectionHandler.ThisGameObject.activeSelf) return;
            Display();
            return;
        }

        popupTutotialCollectionHandler = Instantiate(popupTutorialCollection, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupTutorialCollection)popupTutotialCollectionHandler;
            popup.Initialized(Hide);
            Popup.Show(popupTutotialCollectionHandler);
            //popupMoney.Canvas.sortingOrder = popup.Canvas.sortingOrder + 1;
        }
    }

    public void ShowPopupCollectionTutorial()
    {
        if (popupCollectionTutorialHandler != null)
        {
            if (popupCollectionTutorialHandler.ThisGameObject.activeSelf) return;
            Display();
            return;
        }

        popupCollectionTutorialHandler = Instantiate(popupCollectionTutorial, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupCollectionTutorial)popupCollectionTutorialHandler;
            popup.Initialized(() => { HideAll(); });
            Popup.Show(popupCollectionTutorialHandler);
        }
    }


    public void ShowPopupEvent(Action actionOpenPopup, TabEvent tab = TabEvent.Event)
    {
        if (popupEventPrefabHandler != null)
        {
            if (popupEventPrefabHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupEventPrefabHandler = Instantiate(popupEventPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            // initialize
            var popup = (PopupEvent)popupEventPrefabHandler;
            popup.Initialized(Hide, actionOpenPopup, tab);
            Popup.Show(popupEventPrefabHandler);
            // GamePopup.Instance.popupEventPrefab.BeforeShow();
        }
    }

    #region gift code

    public void ShowGiftCodePopup(Action actionOk = null, Action actionBack = null)
    {
        // if (popupGifCodeHandler != null)
        // {
        //     if (popupGifCodeHandler.ThisGameObject.activeSelf) return;
        //
        //     Display();
        //     return;
        // }
        //
        // popupGifCodeHandler = Instantiate(popupGiftCodePrefab, canvas.transform, false);
        // Display();
        //
        // void Display()
        // {
        //     var popup = (PopupGiftCode) popupGifCodeHandler;
        //     popup.Initialized(() =>
        //         {
        //             Hide();
        //             actionBack?.Invoke();
        //         },
        //         () =>
        //         {
        //             Hide();
        //             actionOk?.Invoke();
        //         });
        //     Popup.Show(popupGifCodeHandler);
        // }
    }

    public void ShowGiftCodeCompletesPopup(Action actionOk, Action actionBack, int idBoss, int idPrincess, string title,
        string message)
    {
        // if (popupGifCodeCompleteHandler != null)
        // {
        //     if (popupGifCodeCompleteHandler.ThisGameObject.activeSelf) return;
        //
        //     Display();
        //     return;
        // }
        //
        // popupGifCodeCompleteHandler = Instantiate(popupGiftCodeCompletePrefab, canvas.transform, false);
        // Display();
        //
        // void Display()
        // {
        //     var popup = (PopupGiftCodeComplete) popupGifCodeCompleteHandler;
        //     popup.Initialized(() =>
        //         {
        //             Hide();
        //             actionBack?.Invoke();
        //         },
        //         () =>
        //         {
        //             Hide();
        //             actionOk?.Invoke();
        //         },
        //         idBoss,
        //         idPrincess,
        //         title,
        //         message);
        //     Popup.Show(popupGifCodeCompleteHandler);
        // }
    }

    #endregion

    #region easter

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    public void ShowPopupEaster(Action actionBack)
    {
        if (popupEasterHandler != null)
        {
            if (popupEasterHandler.ThisGameObject.activeSelf) return;

            Display();
            return;
        }

        popupEasterHandler = Instantiate(popupEasterPrefab, canvas.transform, false);
        Display();

        void Display()
        {
            var popup = (PopupEaster)popupEasterHandler;
            popup.Initialized(() =>
            {
                Hide();
                actionBack?.Invoke();
            }, MenuController.instance.ShowShop);
            Popup.Show(popupEasterHandler);
        }
    }

    #endregion

    #endregion

    public void ActivePopupRoom()
    {
        popupRoom.SetActive(true);
    }

    public void UnactivePopupRoom()
    {
        popupRoom.SetActive(false);
    }

    public void SetActiveMaskTutorialInPopupTask(bool active)
    {
        return;
    }
}