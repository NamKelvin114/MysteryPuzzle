using System;
using System.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTimer;
using Worldreaver.UniUI;

public class SkinShopItem : MonoBehaviour
{
    [SerializeField] private PopupSkin popupSkin;
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private UniButton btnPurchase;
    [SerializeField] private UniButton btnPurchase1;
    [SerializeField] private TextMeshProUGUI txtCoinPurchase;
    [SerializeField] private TextMeshProUGUI txtLabel;
    [SerializeField] private GameObject objectVideo;
    [SerializeField] private GameObject effectSelect;
    [SerializeField] private TabEvent tab = TabEvent.Event;
    public int index;
    public int lastIndexGroup; // change this if it in any group

    //public Image Stand => stand;
    public GameObject EffectSelect => effectSelect;

    private Info _cacheDataInfo;
    private Func<(Sprite, Sprite)> _getPurchaseButtonSprite;
    private Func<(Color, Color)> _getPurhcaseTextColor;

    public UniButton BtnPurchase => btnPurchase;

    public SkeletonGraphic Skeleton => skeleton;

    /// <summary>
    /// 
    /// </summary>
    public void Initialized()
    {
        btnPurchase.gameObject.SetActive(!DataController.instance.SaveHero[index].unlock);
        // btnPurchase1.gameObject.SetActive(!DataController.instance.SaveHero[index].unlock);
        // btnPurchase1.gameObject.SetActive(!DataController.instance.SaveHero[index].unlock);
        if (skeleton.IsValid)
        {
            skeleton.timeScale = index == Data.currentHero ? 1 : 0;
        }
    }

    /// <summary>
    /// try init skin
    /// </summary>
    public void TryInitChangeSkin()
    {
        try
        {
            // Debug.Log(HeroData.SkinHeroByIndex(index).Item1);
            skeleton.ChangeSkin(HeroData.SkinHeroByIndex(index).Item1);
        }
        catch (Exception)
        {
            Timer.Register(0.1f, TryInitChangeSkin);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void FirstInitialized(Func<(Sprite, Sprite)> getPurchaseButtonSprite, Func<(Color, Color)> getPurhcaseTextColor)
    {
        _getPurchaseButtonSprite = getPurchaseButtonSprite;
        _getPurhcaseTextColor = getPurhcaseTextColor;
        _cacheDataInfo = HeroData.HeroInfoByIndex(index);
        Refresh(_cacheDataInfo);
        TryInitChangeSkin();

        btnPurchase.onClick.RemoveAllListeners();

        switch (_cacheDataInfo.typeUnlock)
        {
            case EHeroTypeUnlock.Easter:
                objectVideo.SetActive(false);
                if (txtLabel != null)
                {
                    txtLabel.gameObject.SetActive(true);
                    txtLabel.text = "EASTER";
                }

                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                //btnPurchase.onClick.AddListener(popupSkin.OnEventEasterButtonPressed);
                break;
            case EHeroTypeUnlock.GiftCode:
                if (txtLabel != null)
                {
                    txtLabel.gameObject.SetActive(true);
                    txtLabel.text = "GIFT CODE";
                }

                objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                //btnPurchase.onClick.AddListener(popupSkin.OnGiftCodeButtonPressed);
                break;
            case EHeroTypeUnlock.ShareFacebook:
                if (txtLabel != null)
                {
                    txtLabel.gameObject.SetActive(true);
                    txtLabel.text = "SHARE FB";
                }

                objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                //btnPurchase.onClick.AddListener(() => popupSkin.OnShareFacebook(index));
                break;
            case EHeroTypeUnlock.DailyReward:
                if (txtLabel != null)
                {
                    txtLabel.gameObject.SetActive(true);
                    txtLabel.text = "DAILY REWARD";
                }

                objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                //btnPurchase.onClick.AddListener(popupSkin.OnDailyRewardButtonPressed);
                break;
            case EHeroTypeUnlock.Coin:
                txtLabel.gameObject.SetActive(false);
                objectVideo.SetActive(false);
                txtCoinPurchase.transform.parent.gameObject.SetActive(true);
                txtCoinPurchase.text = $"{_cacheDataInfo.price}";
                btnPurchase.onClick.AddListener(OnBuyByCoinPressed);
                break;
            case EHeroTypeUnlock.VideoReward:
                txtLabel.gameObject.SetActive(false);
                objectVideo.SetActive(true);
                txtCoinPurchase.transform.parent.gameObject.SetActive(false);
                btnPurchase.onClick.AddListener(OnBuyByVideoAdsPressed);
                break;
            case EHeroTypeUnlock.Valentine:
                if (Data.TimeToRescueParty.Milliseconds > 0 && Data.isTimeValentine)
                {
                    btnPurchase.gameObject.SetActive(!DataController.instance.SaveHero[index].unlock);
                }
                else
                {
                    btnPurchase1.gameObject.SetActive(!DataController.instance.SaveHero[index].unlock);
                    btnPurchase.gameObject.SetActive(false);
                    txtLabel.gameObject.SetActive(false);
                    txtCoinPurchase.transform.parent.gameObject.SetActive(true);
                    txtCoinPurchase.text = $"{_cacheDataInfo.price}";
                    btnPurchase1.onClick.AddListener(OnBuyByCoinPressed);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBuyByCoinPressed()
    {
        //popupSkin.OnBuyByCoinPressed(_cacheDataInfo.price, Unlock);
        TurnOnEffectWhenBuy();
        Refresh(_cacheDataInfo);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBuyByVideoAdsPressed()
    {
        //popupSkin.OnBuyByVideoAdsPressed(Unlock);
    }


    public void Unlock()
    {
        if (!DataController.instance.SaveHero[index].unlock)
        {
            btnPurchase.gameObject.SetActive(false);
            btnPurchase1.gameObject.SetActive(false);
            DataController.instance.SaveHero[index].unlock = true;
            RescueAnalytic.LogEventUnlockSkinHero(HeroData.NameHeroWithIndex(index), Utils.currentCoin, Utils.CurrentLevel + 1);
        }
    }
    public void OnClickValentineEvent()
    {
        MenuController.instance.ShowEventReward(null, tab);
    }
    void SetStateGroupCharacter(bool state)
    {
        // popupSkin.btnSelectGroup1.gameObject.SetActive(state);
        // popupSkin.btnSelectGroup2.gameObject.SetActive(state);
        // popupSkin.btnSelectGroup3.gameObject.SetActive(state);
        //
        // popupSkin.effectSelectGroup1.gameObject.SetActive(false);
        // popupSkin.effectSelectGroup2.gameObject.SetActive(false);
        // popupSkin.effectSelectGroup3.gameObject.SetActive(false);
    }

    public void TurnOnEffectWhenBuy()
    {
        bool isGroup = false;
        bool isUnlocked = true;
        Data.currentHero = index;

        // if (popupSkin.IsInGroup(index))
        // {
        //     isGroup = true;
        //     popupSkin.characterGroup.alpha = 1;
        //     SetStateGroupCharacter(true);
        //     popupSkin.skeletonCanvasGroup.alpha = 0;
        // }
        // else
        // {
        //     popupSkin.characterGroup.alpha = 0;
        //     popupSkin.skeletonCanvasGroup.alpha = 1;
        // }
        // popupSkin.CurrentClickHero = index;
        // popupSkin.DisplaySelect();

        var (hasPet, nameSkinPet) = HeroData.HeroHasPetByIndex(index);
        if (!isGroup)
        {
            //popupSkin.ChangeSkin(HeroData.SkinHeroByIndex(index).Item1, nameSkinPet);
        }
        else
        {
            string skin1 = HeroData.SkinHeroByIndex(lastIndexGroup - 2).Item1;
            string skin2 = HeroData.SkinHeroByIndex(lastIndexGroup - 1).Item1;
            string skin3 = HeroData.SkinHeroByIndex(lastIndexGroup).Item1;

            if (isUnlocked)
            {
                //popupSkin.UpdateSkinSelectGroup(lastIndexGroup - 2, lastIndexGroup - 1, lastIndexGroup, Data.currentHero);
            }

            //popupSkin.ChangeSkinGroup(skin1, skin2, skin3, nameSkinPet);
        }

        if (GamePopup.Instance.menuRoom != null && isUnlocked) GamePopup.Instance.menuRoom.ChangeSkin(HeroData.SkinHeroByIndex(index).Item1);

        UpdateAnimationItem();

        if (SceneManager.GetActiveScene().name.Equals("MainGame") && isUnlocked) PlayerManager.instance.ChangeSkin();
    }
    public void Click()
    {
        
        bool isGroup = false;
        bool isUnlocked = false;
        if (DataController.instance.SaveHero[index].unlock)
        {
            isUnlocked = true;
            Data.currentHero = index;

            // if (popupSkin.IsInGroup(index))
            // {
            //     isGroup = true;
            //     popupSkin.characterGroup.alpha = 1;
            //     SetStateGroupCharacter(true);
            //     popupSkin.skeletonCanvasGroup.alpha = 0;
            // }
            // else
            // {
            //     popupSkin.characterGroup.alpha = 0;
            //     popupSkin.skeletonCanvasGroup.alpha = 1;
            // }
        }
        else
        {
            isUnlocked = false;
            // if (popupSkin.IsInGroupLast(index))
            // {
            //     isGroup = true;
            //     popupSkin.characterGroup.alpha = 1;
            //     SetStateGroupCharacter(false);
            //     popupSkin.skeletonCanvasGroup.alpha = 0;
            // }
            // else
            // {
            //     popupSkin.characterGroup.alpha = 0;
            //     popupSkin.skeletonCanvasGroup.alpha = 1;
            // }
        }

        //popupSkin.CurrentClickHero = index;
        //popupSkin.DisplaySelect();

        var (hasPet, nameSkinPet) = HeroData.HeroHasPetByIndex(index);
        if (!isGroup)
        {
            //popupSkin.ChangeSkin(HeroData.SkinHeroByIndex(index).Item1, nameSkinPet);
        }
        else
        {
            string skin1 = HeroData.SkinHeroByIndex(lastIndexGroup - 2).Item1;
            string skin2 = HeroData.SkinHeroByIndex(lastIndexGroup - 1).Item1;
            string skin3 = HeroData.SkinHeroByIndex(lastIndexGroup).Item1;

            if (isUnlocked)
            {
                //popupSkin.UpdateSkinSelectGroup(lastIndexGroup - 2, lastIndexGroup - 1, lastIndexGroup, Data.currentHero);
            }

            //popupSkin.ChangeSkinGroup(skin1, skin2, skin3, nameSkinPet);
        }

        if (GamePopup.Instance.menuRoom != null && isUnlocked) GamePopup.Instance.menuRoom.ChangeSkin(HeroData.SkinHeroByIndex(index).Item1);

        UpdateAnimationItem();

        if (SceneManager.GetActiveScene().name.Equals("MainGame") && isUnlocked) PlayerManager.instance.ChangeSkin();

        MenuController.instance.SoundClickButton();
    }

    public void UpdateAnimationItem()
    {
        //popupSkin.StopTimeScaleAllItem();

        if (DataController.instance.SaveHero[index].unlock)
        {
            if (skeleton.IsValid)
            {
                if (HeroData.HeroHasPetByIndex(index).Item1)
                {
                    var result = skeleton.GetComponentsInChildren<SkeletonGraphic>().ToList();
                    result.Remove(skeleton);
                    result.First()?.AnimationState.SetAnimation(0, "Fly", true);
                }
                skeleton.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Refresh(Info heroInfo)
    {
        if (heroInfo.typeUnlock != EHeroTypeUnlock.Coin && heroInfo.typeUnlock != EHeroTypeUnlock.Valentine) return;

        var colors = _getPurhcaseTextColor();
        var sprites = _getPurchaseButtonSprite();
        if (Utils.currentCoin >= heroInfo.price)
        {
            if (Data.TimeToRescueParty.Milliseconds <= 0 && !Data.isTimeValentine)
            {
                btnPurchase.image.sprite = sprites.Item1;
                btnPurchase.interactable = true;

            }
            btnPurchase1.interactable = true;
            btnPurchase1.image.sprite = sprites.Item1;
            txtCoinPurchase.color = colors.Item1;
        }
        else
        {


            if (Data.TimeToRescueParty.Milliseconds <= 0 && !Data.isTimeValentine)
            {
                btnPurchase.image.sprite = sprites.Item2;
                btnPurchase.interactable = false;

            }
            btnPurchase1.image.sprite = sprites.Item2;
            btnPurchase1.interactable = false;
            txtCoinPurchase.color = colors.Item2;
        }
    }
}