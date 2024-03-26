using System;
using Pancake.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupShop : UniPopupBase
{
    [Header("BUTTON")]
    [SerializeField] private UniButton btnClose;
    [SerializeField] private UniButton btnAdsFree;
    [SerializeField] private UniButton btnPurchaseItem1;
    [SerializeField] private UniButton btnPurchaseItem2;
    [SerializeField] private UniButton btnPurchaseItem3;
    [SerializeField] private UniButton btnPurchaseAllHero;
    [SerializeField] private UniButton btnPurchaseNoAds;
    [SerializeField] private UniButton btnPurchaseVip;
    
    [Header("ICON")]
    [SerializeField] private Image iconItem1;
    [SerializeField] private Image iconItem2;
    [SerializeField] private Image iconItem3;
    [SerializeField] private Image iconItemAllHero;
    [SerializeField] private Image iconItemNoAds;
    [SerializeField] private Image iconItemAdsFree;

    [Header("TEXT")]
    [SerializeField] private TextMeshProUGUI txtCurrency;
    [SerializeField] private TextMeshProUGUI txtValueFreeAds;
    [SerializeField] private TextMeshProUGUI txtValueItem1;
    [SerializeField] private TextMeshProUGUI txtValueItem2;
    [SerializeField] private TextMeshProUGUI txtValueItem3;
    [SerializeField] private TextMeshProUGUI txtValueVip;
    [SerializeField] private TextMeshProUGUI txtPriceItem1;
    [SerializeField] private TextMeshProUGUI txtPriceItem2;
    [SerializeField] private TextMeshProUGUI txtPriceItem3;
    [SerializeField] private TextMeshProUGUI txtPriceAllHero;
    [SerializeField] private TextMeshProUGUI txtPriceNoAds;
    [SerializeField] private TextMeshProUGUI txtPriceVip;

    [Header("GAME OBJECT")]
    [SerializeField] private GameObject purchasedRemoveAds;
    [SerializeField] private GameObject purchasedDoubleGold;
    [SerializeField] private GameObject purchasedAllHero;
    [SerializeField] private GameObject purchasedVip;
    [SerializeField] private GameObject fetch;

    [Header("[stat]")] [SerializeField] private int valueWatchVideo = 500;
    [SerializeField] private int valuePack1 = 50000;
    [SerializeField] private int valuePack2 = 120000;
    [SerializeField] private int valuePack3 = 500000;
    [SerializeField] private int valuePackVip = 500000;

    [SerializeField] private float[] costs;

    private Action _actionClose;
    private Action _actionUpdateCurreny;
    public Action ActionRefreshUnlockAllHero { get; set; }
    private const string TEMPLATE_VIP = "REMOVE ADS\nALL HERO\n{0}";

    public GameObject PurchasedAllHero => purchasedAllHero;
    public GameObject PurchasedRemoveAds => purchasedRemoveAds;
    public GameObject PurchasedDoubleGold => purchasedDoubleGold;
    public GameObject PurchasedVip => purchasedVip;

    public Action OnClose { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionClose"></param>
    /// <param name="actionUpdateCurrency"></param>
    /// <param name="actionRefreshUnlockAllHero"></param>
    public void Initialized(Action actionClose, Action actionUpdateCurrency, Action actionRefreshUnlockAllHero)
    {
        _actionClose = actionClose;
        _actionUpdateCurreny = actionUpdateCurrency;
        ActionRefreshUnlockAllHero = actionRefreshUnlockAllHero;

        txtValueItem1.text = $"{valuePack1:0,000}";
        // txtValueItem2.text = $"{valuePack2:0,000}";
        txtValueItem3.text = $"{valuePack3:0,000}";
        txtValueFreeAds.text = $"+{valueWatchVideo}";

        // txtPriceItem1.text = $"{costs[0]}$";
        // txtPriceItem2.text = $"{costs[1]}$";
        // txtPriceItem3.text = $"{costs[2]}$";
        // txtPriceAllHero.text = $"{costs[3]}$";
        // txtPriceNoAds.text = $"{costs[4]}$";
        // txtPriceVip.text = $"{costs[5]}$";

        //txtValueVip.text = string.Format(TEMPLATE_VIP, valuePackVip);

        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(OnCloseButtonPressed);

        btnAdsFree.onClick.RemoveAllListeners();
        btnAdsFree.onClick.AddListener(OnAdsFreeButtonPressed);

        btnPurchaseItem1.onClick.RemoveAllListeners();
        btnPurchaseItem1.onClick.AddListener(OnPurchaseItem1ButtonPressed);

        btnPurchaseItem2.onClick.RemoveAllListeners();
        btnPurchaseItem2.onClick.AddListener(OnPurchaseDoubleGoldButtonPressed);

        btnPurchaseItem3.onClick.RemoveAllListeners();
        btnPurchaseItem3.onClick.AddListener(OnPurchaseItem3ButtonPressed);

        btnPurchaseAllHero.onClick.RemoveAllListeners();
        btnPurchaseAllHero.onClick.AddListener(OnPurchaseAllHeroButtonPressed);

        btnPurchaseNoAds.onClick.RemoveAllListeners();
        btnPurchaseNoAds.onClick.AddListener(OnPurchaseNoAdsButtonPressed);

        btnPurchaseVip.onClick.RemoveAllListeners();
        btnPurchaseVip.onClick.AddListener(OnPurchaseVipButtonPressed);

        PurchasedVip.SetActive(Data.UserVip);
        PurchasedRemoveAds.SetActive(Data.UserVip || Data.RemoveAds);
        PurchasedDoubleGold.SetActive(Data.DoubleGold);
        PurchasedAllHero.SetActive(Data.UnlockedHero);

        UpdateCurrencyDisplay();
    }

    private void Update()
    {
        if (Utils.pauseUpdateFetchIcon) return;

        if (!Application.isEditor)
        {
            fetch.SetActive(!Advertising.IsRewardedAdReady());
        }
        else
        {
            if (fetch.activeSelf) fetch.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateCurrencyDisplay()
    {
        txtCurrency.text = $"{Utils.currentCoin}";
        _actionUpdateCurreny?.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnCloseButtonPressed()
    {
        MenuController.instance.SoundClickButton();
        OnClose?.Invoke();
        _actionClose?.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnAdsFreeButtonPressed()
    {
        if (!Utils.IsMobile || !Utils.IsTurnOnAds)
        {
            Observer.AddFromPosiGenerationCoin(iconItemAdsFree.gameObject);
            Utils.UpdateCoin(valueWatchVideo);
            UpdateCurrencyDisplay();
            return;
        }

        Utils.pauseUpdateFetchIcon = true;
        RescueAnalytic.LogEventAdRewardRequest();

        Advertising.ShowRewardedAd().OnCompleted(() =>
        {
            Observer.AddFromPosiGenerationCoin(iconItemAdsFree.gameObject);
            Utils.UpdateCoin(valueWatchVideo);
            UpdateCurrencyDisplay();
            Utils.pauseUpdateFetchIcon = false;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseItem1ButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Observer.AddFromPosiGenerationCoin(iconItem1.gameObject);
            Utils.UpdateCoin(valuePack1);
            UpdateCurrencyDisplay();
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_PACK1);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseItem2ButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Observer.AddFromPosiGenerationCoin(iconItem2.gameObject);
            Utils.UpdateCoin(valuePack1);
            UpdateCurrencyDisplay();
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_PACK2);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseItem3ButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Observer.AddFromPosiGenerationCoin(iconItem3.gameObject);
            Utils.UpdateCoin(valuePack3);
            UpdateCurrencyDisplay();
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_PACK3);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseAllHeroButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Data.UnlockedHero = true;
            PurchasedAllHero.SetActive(true);
            ActionRefreshUnlockAllHero?.Invoke();
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_UNLOCK_OUTFIT);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseNoAdsButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Data.RemoveAds = true;
            PurchasedRemoveAds.SetActive(true);
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_REMOVE_ADS);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseDoubleGoldButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Data.DoubleGold = true;
            PurchasedDoubleGold.SetActive(true);
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_DOUBLE_GOLD);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnPurchaseVipButtonPressed()
    {
        if (!Utils.IsMobile)
        {
            Data.UserVip = true;
            Data.RemoveAds = true;
            Data.DoubleGold = true;
            Data.UnlockedHero = true;
            PurchasedVip.SetActive(true);
            PurchasedRemoveAds.SetActive(true);
            PurchasedAllHero.SetActive(true);
            Observer.AddFromPosiGenerationCoin(txtValueVip.gameObject);
            Utils.UpdateCoin(valuePack3);
            UpdateCurrencyDisplay();
            ActionRefreshUnlockAllHero?.Invoke();
            return;
        }

        IAPManager.Instance.PurchaseProduct(Constants.IAP_VIP);
    }
}