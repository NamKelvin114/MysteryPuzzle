using System;
using System.Collections.Generic;
using System.Linq;
using Pancake.Monetization;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupSkin : UniPopupBase
{
    [Header("UI")] [SerializeField] private GameObject groupSkinItemPrefab;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private Image shopBGTop;
    [SerializeField] private List<ButtonChangeSkin> btnChangeSkinList;
    [Header("Data")] [SerializeField] private CollectionBook _collectionBook;
    [SerializeField] private List<SkinResources> skinResourcesList;
    private bool isFirstOpen = true;
    private List<SkinItem> _listSkinItem = new List<SkinItem>();
    [SerializeField] private List<SkinDataShopList> _listSkinDataShop = new List<SkinDataShopList>();
    private List<GameObject> _listBar = new List<GameObject>();
    private int _totalSkinItem = 0;
    private SkinType _skinType;
    private SkinItemType _skinItemType;
    private Action _actionBack;
    private Action<Action, Action> _openDaily;
    [SerializeField] private GameObject mainGirl;
    [SerializeField] private GameObject pinModel;
    [SerializeField] private GameObject btnMain;
    [SerializeField] private GameObject btnPin;
    [SerializeField] private GameObject groupbtnSkinMain;

    public void Initialize(Action actionBack, Action<Action, Action> openDaily)
    {
        _actionBack = actionBack;
        _openDaily = openDaily;
        if (isFirstOpen)
        {
            InitData();
            isFirstOpen = false;
        }

        SetupDefaultUI();
        Refresh();
    }

    private void SetupDefaultUI()
    {
        shopBGTop.sprite = _collectionBook.GetLastestPage().pageSprite;
        shopBGTop.SetNativeSize();
        _skinType = SkinType.GirlSkin;
        _skinItemType = SkinItemType.Shirt;
        int bar = _totalSkinItem / 3 + ((_totalSkinItem % 3 != 0) ? 1 : 0);
        for (int i = 0; i < bar; i++)
        {
            var item = Instantiate(groupSkinItemPrefab, contentTransform);
            _listBar.Add(item);
            var listItem = item.GetComponentsInChildren<SkinItem>().ToList();
            foreach (var skinItem in listItem)
            {
                _listSkinItem.Add(skinItem);
            }
        }

        mainGirl.SetActive(true);
        pinModel.SetActive(false);
        groupbtnSkinMain.SetActive(true);
        btnMain.SetActive(false);
        btnPin.SetActive(true);
    }

    private void InitData()
    {
        foreach (var skinResources in skinResourcesList)
        {
            foreach (var skinDataResource in skinResources.skinDataResourcesList)
            {
                var newListDataShop = new SkinDataShopList
                {
                    skinType = skinResources.skinType,
                    skinDataResources = skinDataResource
                };
                _listSkinDataShop.Add(newListDataShop);
            }
        }

        foreach (var item in _listSkinDataShop)
        {
            _totalSkinItem = Math.Max(_totalSkinItem, item.skinDataResources.skinDataList.Count);
        }

        foreach (var button in btnChangeSkinList)
        {
            button.UIButton.onClick.AddListener(() => OnClickBtnChangeSkin(button.SkinItemType));
        }
    }

    private void OnClickBtnChangeSkin(SkinItemType newSkinItemType)
    {
        _skinItemType = newSkinItemType;
        Refresh();
    }

    public void OnClickDaily()
    {
        _openDaily?.Invoke(null, GamePopup.Instance.ShowPopupMoney);
    }

    private void Refresh()
    {
        foreach (var item in _listSkinItem)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in _listBar)
        {
            item.SetActive(false);
        }

        //Observer.CurrentSkinChanged?.Invoke();

        foreach (var button in btnChangeSkinList)
        {
            button.Refresh(_skinItemType);
        }

        var currentList = GetCurrentListSkinData();
        int cnt = 0, cnt1 = 0;
        foreach (var itemData in currentList.skinDataResources.skinDataList)
        {
            if (itemData.skinBuyType == SkinBuyType.Default || itemData.skinBuyType == SkinBuyType.Level)
                continue;
            if (cnt % 3 == 0)
            {
                _listBar[cnt1].SetActive(true);
                cnt1++;
            }

            _listSkinItem[cnt].gameObject.SetActive(true);
            _listSkinItem[cnt].Init(itemData, _skinItemType, this);
            _listSkinItem[cnt].ReFresh(itemData.skinName == currentList.skinDataResources.CurrentSkin ||
                                       itemData.skinNamePin == currentList.skinDataResources.CurrentSkin);
            cnt++;
        }
    }

    private SkinDataShopList GetCurrentListSkinData()
    {
        var currentList = new SkinDataShopList();
        foreach (var skinDataShopList in _listSkinDataShop)
        {
            if (skinDataShopList.skinDataResources.skinItemType == _skinItemType)
                //&& skinDataShopList.skinType == _skinType)
            {
                currentList = skinDataShopList;
                break;
            }
        }

        return currentList;
    }

    public void OnClickUseItem(SkinData currentSkinData)
    {
        Observer.ShowHideShopWarning?.Invoke();
        var currentListSkinData = GetCurrentListSkinData();
        if (currentListSkinData.skinDataResources.skinItemType != SkinItemType.Pin)
            currentListSkinData.skinDataResources.CurrentSkin = currentSkinData.skinName;
        else
            currentListSkinData.skinDataResources.CurrentSkin = currentSkinData.skinNamePin;
        Refresh();
    }

    public void OnClickBtnBack()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        Refresh();
        GamePopup.Instance.HidePopupMoney();
        gameObject.SetActive(false);
        GamePopup.Instance.HidePopupMoney();
        _actionBack?.Invoke();
        Observer.CurrentSkinChanged?.Invoke();
    }
}

[Serializable]
public class SkinDataShopList
{
    public SkinDataResources skinDataResources;
    public SkinType skinType;
}