using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupEvent : UniPopupBase
{
    //////////////event///////////
    [SerializeField] private UniButton btnBack;
    // [SerializeField] private UniButton btnOk;
    [SerializeField] private List<EventItem> EventItems;
    [SerializeField] private MedalTotal medalTotal;
    [SerializeField] private TimeCountEvent timeCountEvent;
    // [SerializeField] private Image progress;
    // [SerializeField] private List<GameObject> lineClaimActive;
    // [SerializeField] private List<GameObject> lineClaimDisable;
    [SerializeField] private GameObject Event;
    [SerializeField] private GameObject GiftEvent;
    [SerializeField] private GameObject ButtonEventEnable;
    [SerializeField] private GameObject ButtonGiftEnable;

    //////////////gift///////////

    [SerializeField] private List<UniButton> btnClaim;
    [SerializeField] private TextMeshProUGUI txtCurrency;
    private Action<int> _actionSpecial;
    [SerializeField] public EventRewardItem[] eventRewardItem;

    [SerializeField] private int[] valueRewards;
    private Action _actionClaim;
    private Action _actionUpdateCurrency;
    public List<UniButton> BtnClaim => btnClaim;
    public int Day { get; set; }
    [SerializeField] public bool hasItemCanClaim = false;
    // private Action<bool> _actionCheckNoti;
    private Action _actionBack;

    ////////////////gift/////////////////
    public void UpdateCurrencyDisplay() { txtCurrency.text = $"{Utils.currentCoin}"; }

    /// <summary>
    /// 
    /// </summary>

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed()
    {
        // Utils.SaveEventDailyGift();
        _actionBack?.Invoke();
        if (MenuController.instance != null) MenuController.instance.CheckDisplayWarningDailyGiftEvent();
        if (GameManager.instance != null) GameManager.instance.CheckDisplayWarningDailyGiftEvent();
        if (GameManager.instance != null) GameManager.instance.isShowEvent = false;
    }


    /// <summary>
    /// claim button pressed
    /// </summary>
    private void OnClaimButtonPressed()
    {
        // Debug.Log(1);
        Utils.HasClaimEventReward();
        switch (Day)
        {
            case 1:
                AddCoin(valueRewards[0]);
                Data.TotalGoldMedal += 5;
                _actionUpdateCurrency?.Invoke();
                break;
            case 2:
                AddCoin(valueRewards[1]);
                _actionUpdateCurrency?.Invoke();
                Data.TotalGoldMedal += 10;
                break;
            case 3:
                AddCoin(valueRewards[2]);
                _actionUpdateCurrency?.Invoke();
                Data.TotalGoldMedal += 15;
                break;
            case 4:
                AddCoin(valueRewards[3]);
                _actionUpdateCurrency?.Invoke();
                Data.TotalGoldMedal += 20;
                break;
            case 5:
                AddCoin(valueRewards[4]);
                _actionUpdateCurrency?.Invoke();
                Data.TotalGoldMedal += 25;
                break;
            case 6:
                AddCoin(valueRewards[5]);
                _actionUpdateCurrency?.Invoke();
                Data.TotalGoldMedal += 30;
                break;
            case 7:
                Utils.canTakeEventGiftDaily = false;
                DataController.instance.SaveHero[16].unlock = true;
                Data.currentHero = 16;
                GamePopup.Instance.menuRoom.ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item1);
                eventRewardItem[6].imageDay7.SetActive(false);
                Utils.curEventDailyGift++;
                // BtnClaim[6].gameObject.SetActive(false);
                // BtnClaim[6].gameObject.SetActive(false);
                // eventRewardItem[6].tick.SetActive(true);
                // Data.TotalGoldMedal+=2;
                break;
        }
        if (MenuController.instance != null) MenuController.instance.CheckDisplayWarningDailyGiftEvent();
        if (GameManager.instance != null) GameManager.instance.CheckDisplayWarningDailyGiftEvent();
        ResetEventItem();
        _actionClaim?.Invoke();
        Utils.SaveEventDailyGift();
        RescueAnalytic.LogEventClaimEventDailyReward(Day, Utils.currentCoin, Utils.CurrentLevel);
    }

    /// <summary>
    /// add coin
    /// </summary>
    /// <param name="coin"></param>
    private void AddCoin(int coin)
    {
        Utils.currentCoin += coin;
        if (!Utils.canTakeEventGiftDaily)
        {
            Utils.curEventDailyGift++;
            Utils.canTakeEventGiftDaily = true;
        }

        UpdateCurrencyDisplay();
    }

    /////////////////event///////////////
    private void Awake()
    {
        ResetEvent();
        // Event.SetActive(true);
        // GiftEvent.SetActive(false);
        // ButtonEventEnable.SetActive(true);
        // ButtonGiftEnable.SetActive(false);

    }
    public void SetStateTab(TabEvent tab)
    {
        if (tab == TabEvent.Event)
        {
            Event.SetActive(true);
            GiftEvent.SetActive(false);
            ButtonEventEnable.SetActive(true);
            ButtonGiftEnable.SetActive(false);
        }
        else
        {
            Event.SetActive(false);
            GiftEvent.SetActive(true);
            ButtonEventEnable.SetActive(false);
            ButtonGiftEnable.SetActive(true);
        }
    }
    private void loadInfoHero()
    {
        int index = 0;
        EventItems.ForEach(item =>
        {
            Info Data = MenuController.SkinValentine.Find(data => data.ValentineType == item.Type);
            item.Init(Data, this);
            index++;
        });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void Initialized(Action actionBack, Action actionClaim, TabEvent tab = TabEvent.Event)
    {
        ////////////////event///////////
        _actionBack = actionBack;
        _actionClaim = actionClaim;


        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        ///////////gift/////////
        btnClaim[0].onClick.RemoveAllListeners();
        btnClaim[0].onClick.AddListener(OnClaimButtonPressed);

        btnClaim[1].onClick.RemoveAllListeners();
        btnClaim[1].onClick.AddListener(OnClaimButtonPressed);

        btnClaim[2].onClick.RemoveAllListeners();
        btnClaim[2].onClick.AddListener(OnClaimButtonPressed);

        btnClaim[3].onClick.RemoveAllListeners();
        btnClaim[3].onClick.AddListener(OnClaimButtonPressed);

        btnClaim[4].onClick.RemoveAllListeners();
        btnClaim[4].onClick.AddListener(OnClaimButtonPressed);

        btnClaim[5].onClick.RemoveAllListeners();
        btnClaim[5].onClick.AddListener(OnClaimButtonPressed);

        btnClaim[6].onClick.RemoveAllListeners();
        btnClaim[6].onClick.AddListener(OnClaimButtonPressed);

        Utils.SaveEventDailyGift();
        ResetEvent();
        UpdateCurrencyDisplay();
        ResetEventItem();
        medalTotal.Reset();

        SetStateTab(tab);
    }

    public void ResetEventItem()
    {
        foreach (var item in eventRewardItem)
        {
            item.DisplayAgain();
        }
        medalTotal.Reset();
        if (MenuController.instance != null) { MenuController.instance.CheckDisplayWarningDailyGiftEvent(); }
        if (GameManager.instance != null) { GameManager.instance.CheckDisplayWarningDailyGiftEvent(); }
    }


    /// <summary>
    /// back button pressed
    /// </summary>

    /// <summary>
    /// ok button pressed
    /// </summary>
    // private void OnOkButtonPressed() { _actionOk?.Invoke(); }
    public void OnClickEvent()
    {
        Event.SetActive(true);
        GiftEvent.SetActive(false);
        ButtonEventEnable.SetActive(true);
        ButtonGiftEnable.SetActive(false);
        ResetEvent();
    }
    public void OnClickGift()
    {
        GiftEvent.SetActive(true);
        Event.SetActive(false);
        ButtonGiftEnable.SetActive(true);
        ButtonEventEnable.SetActive(false);

    }

    public void ResetEvent()
    {
        // _actionCheckNoti?.Invoke(!hasItemCanClaim);
        EventItems.ForEach(item => item.ResetEvent());
        loadInfoHero();



        // if (Data.TotalGoldMedal < 300)
        // {
        //     if (50 > Data.TotalGoldMedal && Data.TotalGoldMedal >= 0)
        //     {
        //         lineClaimActive[0].SetActive(true);
        //         lineClaimDisable[0].SetActive(false);
        //     }
        //     else if (100 > Data.TotalGoldMedal && Data.TotalGoldMedal >= 50)
        //     {
        //         lineClaimActive[0].SetActive(true);
        //         lineClaimDisable[0].SetActive(false);
        //         lineClaimActive[1].SetActive(true);
        //         lineClaimDisable[1].SetActive(false);
        //     }
        //     else if (200 > Data.TotalGoldMedal && Data.TotalGoldMedal >= 100)
        //     {
        //         lineClaimActive[0].SetActive(true);
        //         lineClaimActive[1].SetActive(true);
        //         lineClaimActive[2].SetActive(true);
        //         lineClaimDisable[0].SetActive(false);
        //         lineClaimDisable[1].SetActive(false);
        //         lineClaimDisable[2].SetActive(false);
        //     }
        //     else if (300 > Data.TotalGoldMedal && Data.TotalGoldMedal >= 200)
        //     {
        //         lineClaimActive[0].SetActive(true);
        //         lineClaimActive[1].SetActive(true);
        //         lineClaimActive[2].SetActive(true);
        //         lineClaimActive[3].SetActive(true);
        //         lineClaimDisable[0].SetActive(false);
        //         lineClaimDisable[1].SetActive(false);
        //         lineClaimDisable[2].SetActive(false);
        //         lineClaimDisable[3].SetActive(false);
        //     }

        // }
        // else
        // {
        //     lineClaimActive[0].SetActive(true);
        //     lineClaimActive[1].SetActive(true);
        //     lineClaimActive[2].SetActive(true);
        //     lineClaimActive[3].SetActive(true);
        //     lineClaimActive[4].SetActive(true);
        //     lineClaimDisable[0].SetActive(false);
        //     lineClaimDisable[1].SetActive(false);
        //     lineClaimDisable[2].SetActive(false);
        //     lineClaimDisable[3].SetActive(false);
        //     lineClaimDisable[4].SetActive(false);
        // }  
        // if (Data.TotalGoldMedal < 300)
        //     {
        //         if (50 > Data.TotalGoldMedal && Data.TotalGoldMedal >= 0)
        //         {
        //             progress.fillAmount = 0;
        //         }
        //         else if (100 > Data.TotalGoldMedal)
        //         {
        //             progress.fillAmount = 0.25f;
        //         }
        //         else if (200 > Data.TotalGoldMedal)
        //         {
        //             progress.fillAmount = 0.5f;
        //         }
        //         else if (300 > Data.TotalGoldMedal)
        //         {
        //             Debug.Log("200");
        //             progress.fillAmount = 0.75f;
        //         }
        //     }
        //     else
        //     {
        //         progress.fillAmount = 1;
        //     }

    }
}
public enum TabEvent
{
    Gift,
    Event
}