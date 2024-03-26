using Pancake.Monetization;
using Pancake.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour
{
    [SerializeField] private Image skinActive;
    [SerializeField] private UIButton btnCanBuy;
    [SerializeField] private UIButton btnCanNotBuy;
    [SerializeField] private UIButton btnDailyReward;
    [SerializeField] private UIButton btnTask;
    [SerializeField] private UIButton btnLevel;
    [SerializeField] private UIButton btnWatchAds;
    [SerializeField] private GameObject bgActive;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI levelText;
    public SkinItemState skinItemState;
    private PopupSkin _popupSkin;
    private SkinData _itemData;
    private SkinItemType _skinItemType;

    // Start is called before the first frame update
    public void Init(SkinData item, SkinItemType skinItemType, PopupSkin popupSkin)
    {
        _itemData = item;
        _popupSkin = popupSkin;
        _skinItemType = skinItemType;

        GetDataFromRewardItemData();
    }

    public void GetDataFromRewardItemData()
    {
    }

    public void ReFresh(bool isCurrentlyChoose)
    {
        if (isCurrentlyChoose)
        {
            skinItemState = SkinItemState.Selected;
        }
        else
        {
            skinItemState = SkinItemState.UnSelected;
        }

        SetupUI();
    }

    private void SetupDefaultUI()
    {
        btnCanBuy.gameObject.SetActive(false);
        btnDailyReward.gameObject.SetActive(false);
        btnTask.gameObject.SetActive(false);
        btnCanNotBuy.gameObject.SetActive(false);
        btnLevel.gameObject.SetActive(false);
        btnWatchAds.gameObject.SetActive(false);
        bgActive.SetActive(false);
        skinActive.gameObject.SetActive(true);
        coinText.text = levelText.text = "";
    }

    private void SetupUI()
    {
        SetupDefaultUI();
        skinActive.sprite = _itemData.shopIcon;
        skinActive.SetNativeSize();
        float offSet = 0, scale = 1;
        switch (_skinItemType)
        {
            case SkinItemType.Hat:
                scale = 1.2f;
                break;
            case SkinItemType.Shirt:
                scale = 1.2f;
                break;
            case SkinItemType.Shoe:
                scale = 1f;
                break;
            default:
                scale = 1f;
                break;
        }

        skinActive.transform.localScale = new Vector3(scale, scale, scale);
        float height = skinActive.GetComponent<RectTransform>().rect.height * scale;
        if (skinItemState == SkinItemState.Selected)
            bgActive.SetActive(true);
        if (!_itemData.IsUnlocked)
        {
            switch (_itemData.skinBuyType)
            {
                case SkinBuyType.BuyCoin:
                    coinText.text = _itemData.coinValue.ToString();
                    if (_itemData.coinValue <= Utils.currentCoin)
                    {
                        btnCanBuy.gameObject.SetActive(true);
                    }
                    else
                    {
                        btnCanNotBuy.gameObject.SetActive(true);
                    }

                    break;
                case SkinBuyType.DailyReward:
                    btnDailyReward.gameObject.SetActive(true);
                    break;
                case SkinBuyType.WatchAds:
                    btnWatchAds.gameObject.SetActive(true);
                    break;
                case SkinBuyType.Level:
                    levelText.text = "Level " + _itemData.levelUnlock;
                    btnLevel.gameObject.SetActive(true);
                    break;
                case SkinBuyType.Task:
                    btnTask.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void OnClickShowSkin()
    {
        //Observer.ClickButton?.Invoke(); 
        if (_itemData.IsUnlocked == false)
        {
            if (_itemData.skeletonDataAsset != null)
                Observer.ShowSkin(_itemData.skinName);
            else Observer.ShowSkinPin(_itemData.skinNamePin);
        }
        else
            _popupSkin.OnClickUseItem(_itemData);
        //_popupShop.OnClickShowSkin(_itemData);
    }

    public void OnClickBuyItem()
    {
        //Observer.ClickButton?.Invoke(); 
        Utils.currentCoin -= _itemData.coinValue;
        _itemData.IsUnlocked = true;
        _popupSkin.OnClickUseItem(_itemData);

        Observer.CurrencyTotalChanged?.Invoke(false);
    }

    public void OnClickOpenDaily()
    {
        _popupSkin.OnClickDaily();
    }

    public void OnClickOpenAds()
    {
        if (Utils.IsTurnOnAds && Utils.IsMobile)
        {
            Utils.pauseUpdateFetchIcon = true;
            RescueAnalytic.LogEventAdRewardRequest();
            Advertising.ShowRewardedAd().OnCompleted(() =>
            {
                _itemData.IsUnlocked = true;
                _popupSkin.OnClickUseItem(_itemData);
                Utils.pauseUpdateFetchIcon = false;
            });
        }
        else
        {
            _itemData.IsUnlocked = true;
            _popupSkin.OnClickUseItem(_itemData);
        }
    }

    public void OnClickOpenTask()
    {
    }

    public void ClickButtonSound()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
    }
}

public enum SkinItemState
{
    Selected,
    UnSelected,
}