#pragma warning disable 649
using System;
using System.Collections;
using System.Collections.Generic;
using Lance.Common;
using TMPro;
using UnityEngine;
using Worldreaver.UniUI;
using Pancake.Linq;
using Pancake.Monetization;
using UnityEditor;
using Random = UnityEngine.Random;


public class PopupDailyReward : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnClaimByVideo;
    // [SerializeField] private UniButton btnClaim;
    [SerializeField] private TextMeshProUGUI txtCoinClaimByVideo;
    [SerializeField] private GameObject fetchVideoReward;
    [SerializeField] private RewardItem[] rewardItems;
    [SerializeField] private int[] valueRewards;
    [SerializeField] private int videoValueCoefficient = 2;
    [SerializeField] private RewardItemData _rewardItemData;
    [SerializeField] private GameObject _rewardBoard;
    [SerializeField] private DailyRewardSkinPopup _dailyRewardSkinPopup;
    [SerializeField] private UniButton btnNextDay;
    [SerializeField] private GameObject _giftBox;

    [Header("Day Button")] 
    [SerializeField] private UniButton btnDay1;
    [SerializeField] private UniButton btnDay2;
    [SerializeField] private UniButton btnDay3;
    [SerializeField] private UniButton btnDay4;
    [SerializeField] private UniButton btnDay5;
    [SerializeField] private UniButton btnDay6;
    [SerializeField] private UniButton btnDay7;
    private RewardItem rewarDay1 => btnDay1.GetComponent<RewardItem>();
    private RewardItem rewarDay2 => btnDay2.GetComponent<RewardItem>();
    private RewardItem rewarDay3 => btnDay3.GetComponent<RewardItem>();
    private RewardItem rewarDay4 => btnDay4.GetComponent<RewardItem>();
    private RewardItem rewarDay5 => btnDay5.GetComponent<RewardItem>();
    private RewardItem rewarDay6 => btnDay6.GetComponent<RewardItem>();
    private RewardItem rewarDay7 => btnDay7.GetComponent<RewardItem>();

    
    
    public SkinResources SkinResources;

    private List<SkinData> _allSkinData = new List<SkinData>();
    private Action<Action, Action, Action> _actionToShop;
    private Action _actionBack;
    private Action _actionClaimByVideo;
    private Action _actionClaim;
    private Action<int> _actionSpecial;
    private Action _actionUpdateCurrency;
    private SkinData _day7GiftData;

    public UniButton BtnClaimByVideo => btnClaimByVideo;
    // public UniButton BtnClaim => btnClaim;

    public int Day { get; set; } = 1;

    private void SetupSkinData()
    {
        _allSkinData = new List<SkinData>();
        foreach (var skinResource in SkinResources.skinDataResourcesList)
        {
            //do not use pin for gift daily
            if (skinResource.skinItemType != SkinItemType.Pin)
            {
                foreach (SkinData skinDataList in skinResource.skinDataList)
                {
                    _allSkinData.Add(skinDataList);
                }
            }
        }
    }

    private void Start()
    {
        SetupSkinData();
        RefreshAllItem();
    }

    private void SetDataForValueReward()
    {
        for (int i = 0; i < valueRewards.Length; i++)
        {
            var data = _rewardItemData.listDailyItemData.FirstOrDefault(item => (int)item.day == i);
            if (data != null)
            {
                valueRewards[i] = data.coin;
            }
        }
    }

    private SkinData GetRandomShirt()
    {
        List<SkinData> unUsedSkin = new List<SkinData>();

        foreach (var shirt in _allSkinData)
        {
            if (!shirt.IsUnlocked)
            {
                unUsedSkin.Add(shirt);
            }
        }

        if (unUsedSkin.Count == 0)
        {
            return null;
        }

        int ramIndex = (int)Random.Range(0, unUsedSkin.Count);

        return unUsedSkin[ramIndex];
    }

    private SkinData GetCurrentSkinDataOfDay7(string name)
    {
        return _allSkinData.FirstOrDefault(shirt => shirt.skinName == name);
    }

    private void Update()
    {
        if (Utils.pauseUpdateFetchIcon) return;

        if (!Application.isEditor)
        {
            fetchVideoReward.SetActive(false);
            var status = Advertising.IsRewardedAdReady();
            fetchVideoReward.SetActive(!status);
        }
        else
        {
            if (fetchVideoReward.activeSelf) fetchVideoReward.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionClaimByVideo"></param>
    /// <param name="actionClaim"></param>
    /// <param name="actionSpecial"></param>
    /// <param name="actionUpdateCurrency"></param>
    /// <param name="actionToShop"></param>
    public void Initialized(
        Action actionBack,
        Action actionClaimByVideo,
        Action actionClaim,
        Action<int> actionSpecial,
        Action actionUpdateCurrency,
        Action<Action, Action, Action> actionToShop
    )
    {
        _actionBack = actionBack;
        _actionClaimByVideo = actionClaimByVideo;
        _actionClaim = actionClaim;
        _actionSpecial = actionSpecial;
        _actionUpdateCurrency = actionUpdateCurrency;
        _actionToShop = actionToShop;

        UpdateTextCoinClaimByVideo();

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        // btnClaim.onClick.RemoveAllListeners();
        // btnClaim.onClick.AddListener(OnClaimButtonPressed);

        btnClaimByVideo.onClick.RemoveAllListeners();
        btnClaimByVideo.onClick.AddListener(OnClaimByVideoButtonPressed);
        
        // btnDay1.onClick.RemoveAllListeners();
        // btnDay1.onClick.AddListener(OnClaimButtonPressed);
        // btnDay2.onClick.RemoveAllListeners();
        // btnDay2.onClick.AddListener(OnClaimButtonPressed);
        // btnDay3.onClick.RemoveAllListeners();
        // btnDay3.onClick.AddListener(OnClaimButtonPressed);
        // btnDay4.onClick.RemoveAllListeners();
        // btnDay4.onClick.AddListener(OnClaimButtonPressed);
        // btnDay5.onClick.RemoveAllListeners();
        // btnDay5.onClick.AddListener(OnClaimButtonPressed);
        // btnDay6.onClick.RemoveAllListeners();
        // btnDay6.onClick.AddListener(OnClaimButtonPressed);
        // btnDay7.onClick.RemoveAllListeners();
        // btnDay7.onClick.AddListener(OnClaimButtonPressed);

        RefreshAllItem();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateCurrencyDisplay()
    {
    } //txtCurrency.text = $"{Utils.currentCoin}"; }

    private void SetDailyRewardItemData()
    {
        foreach (var item in rewardItems)
        {
            item.DailyItemData = _rewardItemData.listDailyItemData.FirstOrDefault(itemData => itemData.day == item.day);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void RefreshAllItem()
    {
        btnNextDay.gameObject.SetActive(Utils.IsDebugDaily);

        for (int i = 0; i < rewardItems.Length; i++)
        {
            if (rewardItems[i].txtDay != null)
            {
                if (i == 9)
                {
                    rewardItems[i].txtDay.text = $"{rewardItems[i].dayIndex + Data.WeekNumber * 0}";
                }
                else
                {
                    rewardItems[i].txtDay.text = $"Day {rewardItems[i].dayIndex + Data.WeekNumber * 0}";
                }
            }
        }

        //set daily reward data for this day
        SetDailyRewardItemData();
        //set value for value reward
        SetDataForValueReward();


        foreach (var item in rewardItems)
        {
            item.DisplayAgain();
        }

        if (Utils.curDailyGift > 7)
        {
            rewardItems[6].tick.SetActive(true);
            _giftBox.SetActive(false);
        }
        else
        {
            rewardItems[6].tick.SetActive(false);
            _giftBox.SetActive(true);
        }

        UpdateTextCoinClaimByVideo();
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed()
    {
        Utils.SaveDailyGift();
        GamePopup.Instance.HidePopupMoney();
        _actionBack?.Invoke();
    }

    void UpdateTextCoinClaimByVideo()
    {
        txtCoinClaimByVideo.text = "+" + valueRewards[Math.Min(Utils.curDailyGift - 1, 6)] * videoValueCoefficient;
    }

    /// <summary>
    /// claim by video button pressed
    /// </summary>
    private void OnClaimByVideoButtonPressed()
    {
#if UNITY_EDITOR


        switch (Day)
        {
            case 1:
                AddCoin(valueRewards[0] * videoValueCoefficient);
                _actionUpdateCurrency?.Invoke();
                break;
            case 2:
                AddCoin(valueRewards[1] * videoValueCoefficient);
                _actionUpdateCurrency?.Invoke();
                break;
            case 3:
                AddCoin(valueRewards[2] * videoValueCoefficient);
                _actionUpdateCurrency?.Invoke();
                break;
            case 4:
                AddCoin(valueRewards[3] * videoValueCoefficient);
                _actionUpdateCurrency?.Invoke();
                break;
            case 5:
                AddCoin(valueRewards[4] * videoValueCoefficient);
                _actionUpdateCurrency?.Invoke();
                break;
            case 6:
                AddCoin(valueRewards[5] * videoValueCoefficient);
                _actionUpdateCurrency?.Invoke();
                break;
        }

        Utils.SaveDailyGift();
        RefreshAllItem();
        Utils.HasClaimReward();
        _actionClaimByVideo?.Invoke();
        return;
#endif
        if (Utils.IsTurnOnAds)
        {
            Utils.pauseUpdateFetchIcon = true;

            RescueAnalytic.LogEventAdRewardRequest();

            Advertising.ShowRewardedAd().OnCompleted(Callback);

            void Callback()
            {
                Utils.HasClaimReward();

                switch (Day)
                {
                    case 1:
                        AddCoin(valueRewards[0] * videoValueCoefficient);
                        _actionUpdateCurrency?.Invoke();
                        break;
                    case 2:
                        AddCoin(valueRewards[1] * videoValueCoefficient);
                        _actionUpdateCurrency?.Invoke();
                        break;
                    case 3:
                        AddCoin(valueRewards[2] * videoValueCoefficient);
                        _actionUpdateCurrency?.Invoke();
                        break;
                    case 4:
                        AddCoin(valueRewards[3] * videoValueCoefficient);
                        _actionUpdateCurrency?.Invoke();
                        break;
                    case 5:
                        AddCoin(valueRewards[4] * videoValueCoefficient);
                        _actionUpdateCurrency?.Invoke();
                        break;
                    case 6:
                        AddCoin(valueRewards[5] * videoValueCoefficient);
                        _actionUpdateCurrency?.Invoke();
                        break;
                }

                Utils.SaveDailyGift();
                RefreshAllItem();
                _actionClaimByVideo?.Invoke();
                RescueAnalytic.LogEventClaimDailyRewardByAdReward(Day, Utils.currentCoin, Utils.CurrentLevel);
            }

            Utils.pauseUpdateFetchIcon = false;
        }
        else
        {
            switch (Day)
            {
                case 1:
                    AddCoin(valueRewards[0] * videoValueCoefficient);
                    _actionUpdateCurrency?.Invoke();
                    break;
                case 2:
                    AddCoin(valueRewards[1] * videoValueCoefficient);
                    _actionUpdateCurrency?.Invoke();
                    break;
                case 3:
                    AddCoin(valueRewards[2] * videoValueCoefficient);
                    _actionUpdateCurrency?.Invoke();
                    break;
                case 4:
                    AddCoin(valueRewards[3] * videoValueCoefficient);
                    _actionUpdateCurrency?.Invoke();
                    break;
                case 5:
                    AddCoin(valueRewards[4] * videoValueCoefficient);
                    _actionUpdateCurrency?.Invoke();
                    break;
                case 6:
                    AddCoin(valueRewards[5] * videoValueCoefficient);
                    _actionUpdateCurrency?.Invoke();
                    break;
            }

            Utils.SaveDailyGift();
            RefreshAllItem();
            Utils.HasClaimReward();
            _actionClaimByVideo?.Invoke();
        }
    }

    /// <summary>
    /// claim button pressed
    /// </summary>
    ///
    public void OnClaim(int day)
    {
        if(day != Day) return;
        switch (day)
        {
            case 1:
                if(!rewarDay1.CanClaim) return;
                AddCoin(valueRewards[0]);
                rewarDay1.CanClaim = false;
                _actionUpdateCurrency?.Invoke();
                break;
            case 2:
                if(!rewarDay2.CanClaim) return;
                AddCoin(valueRewards[1]);
                rewarDay2.CanClaim = false;
                _actionUpdateCurrency?.Invoke();
                break;
            case 3:
                if(!rewarDay3.CanClaim) return;
                AddCoin(valueRewards[2]);
                rewarDay3.CanClaim = false;
                _actionUpdateCurrency?.Invoke();
                break;
            case 4:
                if(!rewarDay4.CanClaim) return;
                AddCoin(valueRewards[3]);
                rewarDay4.CanClaim = false;
                _actionUpdateCurrency?.Invoke();
                break;
            case 5:
                if(!rewarDay5.CanClaim) return;
                AddCoin(valueRewards[4]);
                rewarDay5.CanClaim = false;
                _actionUpdateCurrency?.Invoke();
                break;
            case 6:
                if(!rewarDay6.CanClaim) return;
                AddCoin(valueRewards[5]);
                rewarDay6.CanClaim = false;
                _actionUpdateCurrency?.Invoke();
                break;
            case 7:
                if(!rewarDay7.CanClaim) return;
                rewarDay7.CanClaim = false;
                OnSpecialButtonPressed4();
                break;
        }
        DataController.instance.CheckWarningForTask();
        Utils.HasClaimReward();
        _actionClaim?.Invoke();
        Utils.SaveDailyGift();
        RefreshAllItem();
        RescueAnalytic.LogEventClaimDailyReward(Day, Utils.currentCoin, Utils.CurrentLevel);
    }
    private void OnClaimButtonPressed()
    {
        DataController.instance.CheckWarningForTask();
        Utils.HasClaimReward();
        switch (Day)
        {
            case 1:
                AddCoin(valueRewards[0]);
                _actionUpdateCurrency?.Invoke();
                break;
            case 2:
                AddCoin(valueRewards[1]);
                _actionUpdateCurrency?.Invoke();
                break;
            case 3:
                AddCoin(valueRewards[2]);
                _actionUpdateCurrency?.Invoke();
                break;
            case 4:
                AddCoin(valueRewards[3]);
                _actionUpdateCurrency?.Invoke();
                break;
            case 5:
                AddCoin(valueRewards[4]);
                _actionUpdateCurrency?.Invoke();
                break;
            case 6:
                AddCoin(valueRewards[5]);
                _actionUpdateCurrency?.Invoke();
                break;
            case 7:
                OnSpecialButtonPressed4();
                break;
        }

        _actionClaim?.Invoke();
        Utils.SaveDailyGift();
        RefreshAllItem();
        RescueAnalytic.LogEventClaimDailyReward(Day, Utils.currentCoin, Utils.CurrentLevel);
    }

    /// <summary>
    /// special button pressed
    /// </summary>
    private void OnSpecialButtonPressed(int i)
    {
        if (!Utils.cantakegiftdaily && Utils.curDailyGift == 7)
        {
            OnSpecialButtonPressed4();
        }
    }

    /// <summary>
    /// special button pressed
    /// </summary>
    private void OnSpecialButtonPressed4()
    {
        ClaimDay7Gift();
        Utils.HasClaimReward();
        Utils.curDailyGift++;
        Data.WeekNumber++;
        _actionUpdateCurrency?.Invoke();
        Utils.SaveDailyGift();
        Utils.cantakegiftdaily = true;
        // btnClaim.gameObject.SetActive(false);
        RefreshAllItem();
    }

    private void ClaimDay7Gift()
    {
        DataController.instance.CheckWarningForTask();
        var rewardScript = _rewardBoard.GetComponent<RewardCollection>();
        _day7GiftData = GetRandomShirt();

        if (_day7GiftData != null)
        {
            _dailyRewardSkinPopup.SkinData = _day7GiftData;
            _dailyRewardSkinPopup.SetupSkin();
        }
        else
        {
            _dailyRewardSkinPopup.SetupCoin(valueRewards[6], true);
        }

        _dailyRewardSkinPopup.gameObject.SetActive(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.giftAperrence);
    }

    /// <summary>
    /// add coin
    /// </summary>
    /// <param name="coin"></param>
    private void AddCoin(int coin)
    {
        Utils.UpdateCoin(coin);
        if (!Utils.cantakegiftdaily)
        {
            Utils.curDailyGift++;
            Utils.cantakegiftdaily = true;
        }

        UpdateCurrencyDisplay();
    }

    /// <summary>
    /// to shop button pressed
    /// </summary>
    private void OnToShopButtonPressed()
    {
        _actionToShop?.Invoke(UpdateCurrencyDisplay, null, null);
    }

    public void NextDay()
    {
        Utils.cantakegiftdaily = false;
        //Utils.curDailyGift++;
        if (Utils.curDailyGift > 7)
        {
            Utils.curDailyGift = 1;
        }

        RefreshAllItem();
    }
}