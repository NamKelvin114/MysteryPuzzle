#pragma warning disable 0649
using System;
using UnityEngine;


// ReSharper disable once InconsistentNaming
public class IAPListener : MonoBehaviour
{
    private IAPManager _iapManager;
    public Action actionSuccess;
    public Action actionFail;

    public void Initialized(IAPManager iapManager)
    {
        _iapManager = iapManager;
        iapManager.PurchaseSucceededEvent += HandlePurchaseSuccess;
        iapManager.PurchaseFailedEvent += HandlePurchaseFaild;
    }

    private void HandlePurchaseFaild(string obj)
    {
        //SoundController.Current.PLayPurchaseFaild();
        // var popupController = ServiceLocator.Instance.Resolve<PopupController>();
        // popupController.ShowNotification(Constant.TEXT_NOTIFICATION_TITLE, Constant.TEXT_PURCHASE_FAILD, () =>
        // {
        //     popupController.ReleaseStack();
        //     actionFail?.Invoke();
        //     actionFail = null;
        // });
    }

    private void HandlePurchaseSuccess(string obj)
    {
        //SoundController.Current.PLayPurchaseSuccess();
#if !UNITY_EDITOR
		    //if (_iapManager.receiptInfo != null)
			//ClientRequest.UpdateIAP (JsonUtility.ToJson (_iapManager), obj);
#endif

        var shop = GamePopup.Instance.popupShopHandler as PopupShop;

        //TODO show purchase success.
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (obj)
        {
            case "com.saveherpin.5kgold":
                Utils.currentCoin += 50000;
                if (shop != null)
                {
                    shop.UpdateCurrencyDisplay();
                }

                break;
            case "com.saveherpin.120kgold":
                Utils.currentCoin += 120000;
                if (shop != null)
                {
                    shop.UpdateCurrencyDisplay();
                }

                break;
            case "com.saveherpin.500kgold":
                Utils.currentCoin += 500000;

                if (shop != null)
                {
                    shop.UpdateCurrencyDisplay();
                }

                break;
            case "com.saveherpin.removeads":

                Data.RemoveAds = true;
                try
                {
                    if (MenuController.instance != null)
                    {
                        MenuController.instance.BtnRemoveAds.gameObject.SetActive(false);
                    }
                }
                catch (Exception)
                {
                    //
                }

                if (shop != null)
                {
                    shop.PurchasedRemoveAds.SetActive(true);
                }

                break;
            case "com.saveherpin.x2gold":
                Data.DoubleGold = true;
                if (shop != null) shop.PurchasedDoubleGold.SetActive(true);
                RescueAnalytic.LogEventDoubleGoldPurchase(Utils.currentCoin, Utils.CurrentLevel);
                break;
            case "com.saveherpin.unlockskin":
            case "com.saveherpin.unlockoutfit":
                Data.UnlockedHero = true;
                if (shop != null)
                {
                    DataController.instance.UnlockAllHero();
                    shop.PurchasedAllHero.SetActive(true);
                    shop.ActionRefreshUnlockAllHero?.Invoke();
                }
                RescueAnalytic.LogEventAllHeroUnlocked(Utils.currentCoin, Utils.CurrentLevel);

                break;
            case "com.saveherpin.vip":
                Data.UserVip = true;
                Data.DoubleGold = true;
                Data.UnlockedHero = true;
                Data.RemoveAds = true;
                Utils.currentCoin += 500000;

                if (shop != null)
                {
                    DataController.instance.UnlockAllHero();
                    shop.PurchasedRemoveAds.SetActive(true);
                    shop.PurchasedAllHero.SetActive(true);
                    shop.PurchasedDoubleGold.SetActive(true);
                    shop.PurchasedVip.SetActive(true);
                    shop.UpdateCurrencyDisplay();
                    shop.ActionRefreshUnlockAllHero?.Invoke();
                }
                
                RescueAnalytic.LogEventAllHeroUnlocked(Utils.currentCoin, Utils.CurrentLevel);
                break;
        }
    }
}