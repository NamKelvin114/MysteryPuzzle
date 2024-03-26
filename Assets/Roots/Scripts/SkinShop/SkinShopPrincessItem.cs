using System;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTimer;
using Worldreaver.UniUI;

/// <summary>
///  OMG so bad code , when refactor ?
/// </summary>
public class SkinShopPrincessItem : MonoBehaviour
{
    [SerializeField] private PopupSkin popupSkin;
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private UniButton btnPurchase;
    [SerializeField] private UniButton btnPurchase1;
    [SerializeField] private GameObject objectVideo;
    [SerializeField] private GameObject effectSelect;
    [SerializeField] private Sprite lockSprite;
    public int index;
    public GameObject EffectSelect => effectSelect;
    private Info _cacheDataInfo;

    [SerializeField] private TextMeshProUGUI txtCoinPurchase;
    [SerializeField] private TextMeshProUGUI txtEvent;

    public SkeletonGraphic Skeleton => skeleton;

    public void Initialized()
    {
        Refresh();

        if (skeleton.IsValid)
        {
            skeleton.timeScale = index == Data.currentPrincess ? 1 : 0;
        }

        TryInitChangeSkin();
    }

    /// <summary>
    /// try init skin
    /// </summary>
    public void TryInitChangeSkin()
    {
        try
        {
            // Debug.Log(HeroData.SkinPrincessByIndex(index));
            skeleton.ChangeSkin(HeroData.SkinPrincessByIndex(index));
        }
        catch (Exception)
        {
            Timer.Register(0.1f, TryInitChangeSkin);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBuyByCoinPressed()
    {
        //popupSkin.OnBuyByCoinPressed(_cacheDataInfo.price, Unlock);
        RefreshPrincess(_cacheDataInfo, index);
    }

    /// <summary>
    /// refresh
    /// </summary>
    public void Refresh()
    {
        if (DataController.instance.SavePrincess[index].unlock)
        {
            if (btnPurchase != null) btnPurchase.gameObject.SetActive(false);
            return;
        }

        _cacheDataInfo = HeroData.PrincessInfoByIndex(index);
        RefreshPrincess(_cacheDataInfo, index);
        btnPurchase.onClick.RemoveAllListeners();

        switch (_cacheDataInfo.typeUnlock)
        {
            case EHeroTypeUnlock.Coin:
                if (objectVideo != null) objectVideo.SetActive(false);
                if (txtEvent != null) txtEvent.gameObject.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(true);
                txtCoinPurchase.text = $"{_cacheDataInfo.price}";
                btnPurchase.onClick.AddListener(OnBuyByCoinPressed);
                break;
            case EHeroTypeUnlock.ShareFacebook:
                if (objectVideo != null) objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                if (txtEvent != null)
                {
                    txtEvent.gameObject.SetActive(true);
                    txtEvent.text = "SHARE FB";
                }

                //btnPurchase.onClick.AddListener(() => popupSkin.OnShareFacebook(index, false));
                break;
            case EHeroTypeUnlock.GiftCode:
                if (objectVideo != null) objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                if (txtEvent != null)
                {
                    txtEvent.gameObject.SetActive(true);
                    txtEvent.text = "GIFT CODE";
                }

                btnPurchase.gameObject.SetActive(!DataController.instance.SavePrincess[index].unlock);
                //btnPurchase.onClick.AddListener(popupSkin.OnGiftCodeButtonPressed);
                break;
            case EHeroTypeUnlock.Easter:
                if (objectVideo != null) objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                if (txtEvent != null)
                {
                    txtEvent.gameObject.SetActive(true);
                    txtEvent.text = "EASTER";
                }

                var flag = Data.IsEventEasterStarted();
                if (index == 4)
                {
                    if (!Data.GetStatusRewardEaster(3))
                    {
                        btnPurchase.gameObject.SetActive(true);
                        btnPurchase.interactable = flag;
                        if (!flag) btnPurchase.image.sprite = lockSprite;
                    }
                    else
                    {
                        btnPurchase.gameObject.SetActive(false);
                    }
                }
                else if (index == 5)
                {
                    if (!Data.GetStatusRewardEaster(4))
                    {
                        btnPurchase.gameObject.SetActive(true);
                        btnPurchase.interactable = flag;
                        if (!flag) btnPurchase.image.sprite = lockSprite;
                    }
                    else
                    {
                        btnPurchase.gameObject.SetActive(false);
                    }
                }
                else if (index == 6)
                {
                    if (!Data.GetStatusRewardEaster(5))
                    {
                        btnPurchase.gameObject.SetActive(true);
                        btnPurchase.interactable = flag;
                        if (!flag) btnPurchase.image.sprite = lockSprite;
                    }
                    else
                    {
                        btnPurchase.gameObject.SetActive(false);
                    }
                }

                //btnPurchase.onClick.AddListener(popupSkin.OnEventEasterButtonPressed);
                break;

            case EHeroTypeUnlock.VideoReward:
                if (objectVideo != null) objectVideo.SetActive(true);
                if (txtEvent != null) txtEvent.gameObject.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                btnPurchase.onClick.AddListener(OnBuyByVideoAdsPressed);
                break;
            case EHeroTypeUnlock.Valentine:
                // txtLabel.gameObject.SetActive(false);
                // objectVideo.SetActive(true);
                // txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                // btnPurchase.onClick.AddListener(OnBuyByVideoAdsPressed);
                if(Data.TimeToRescueParty.Milliseconds>0 && Data.isTimeValentine)
                {
                    btnPurchase.gameObject.SetActive(!DataController.instance.SavePrincess[index].unlock);
                }
                else
                {
                    btnPurchase.gameObject.SetActive(false);
                    btnPurchase1.gameObject.SetActive(!DataController.instance.SavePrincess[index].unlock);
                    if (txtEvent != null) txtEvent.gameObject.SetActive(false);
                    txtCoinPurchase.transform.parent.gameObject.SetActive(true);
                    txtCoinPurchase.text = $"{_cacheDataInfo.price}";
                    btnPurchase1.onClick.AddListener(OnBuyByCoinPressed);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBuyByVideoAdsPressed()
    {
        //popupSkin.OnBuyByVideoAdsPressed(Unlock);
    }

    /// <summary>
    /// unlock skin
    /// </summary>
    public void Unlock()
    {
        if (!DataController.instance.SavePrincess[index].unlock)
        {
            btnPurchase.gameObject.SetActive(false);
            btnPurchase1.gameObject.SetActive(false);
            DataController.instance.SavePrincess[index].unlock = true;
            // RescueAnalytic.LogEventUnlockSkinHero(DataController.instance.heroData.infos[index].itemName, Utils.currentCoin, Utils.CurrentLevel + 1);
        }
    }

    /// <summary>
    /// select skin
    /// </summary>
    public void Click()
    {
        if (DataController.instance.SavePrincess[index].unlock)
        {
            Data.currentPrincess = index;
        }

        // popupSkin.CurrentClickPrincess = index;
        // popupSkin.DisplaySelectPrincess();
        // popupSkin.StopTimeScaleAllPrincessItem();
        if (DataController.instance.SavePrincess[index].unlock)
        {
            if (skeleton.IsValid)
            {
                skeleton.timeScale = 1;
            }
        }

        // popupSkin.ChangeWifeSkin(HeroData.SkinPrincessByIndex(index));

        if (SceneManager.GetActiveScene().name.Equals("MainGame"))
        {
            if (HostageManager.instance != null) HostageManager.instance.ChangeSkin();
        }

        if (MenuController.instance != null) MenuController.instance.SoundClickButton();
    }

    /// <summary>
    /// 
    /// </summary>
    public void RefreshPrincess(Info heroInfo, int id)
    {
        btnPurchase.gameObject.SetActive(!DataController.instance.SavePrincess[id].unlock);

        if (heroInfo.typeUnlock != EHeroTypeUnlock.Coin && heroInfo.typeUnlock != EHeroTypeUnlock.Valentine) return;

        var popup = (PopupSkin) GamePopup.Instance.popupSkinHandler;
        if (popup != null)
        {
            
            // var colors = popup.GetColorTextPurchaseSprite();
            // var sprites = popup.GetButtonPurchaseSprite();
            // if (Utils.currentCoin >= heroInfo.price)
            // {
            //     
            //     if(Data.TimeToRescueParty.Milliseconds<=0 && !Data.isTimeValentine)
            //     {
            //         
            //         btnPurchase.image.sprite = sprites.Item1;
            //         btnPurchase.interactable = true;
            //     }
            //     txtCoinPurchase.color = colors.Item1;
            //     btnPurchase1.image.sprite = sprites.Item1;
            //     btnPurchase1.interactable = true;
            //     
            //     
            // }
            // else
            // {
            //     
            //     if(Data.TimeToRescueParty.Milliseconds<=0 && !Data.isTimeValentine)
            //     {
            //         
            //         btnPurchase.image.sprite = sprites.Item2;
            //         btnPurchase.interactable = false;
            //     }
            //     txtCoinPurchase.color = colors.Item2;
            //     btnPurchase1.image.sprite = sprites.Item2;
            //     btnPurchase1.interactable = false;
            //     
            // }
        }
    }
}