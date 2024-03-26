using System;
using System.Collections.Generic;
using DG.Tweening;
using Pancake.UI;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupTravelStory : UniPopupBase
{
    [SerializeField] private TravelStoryData travelStoryData;
    [SerializeField] private List<TravelStoryItem> listTravelStoryItems;
    [SerializeField] private TextMeshProUGUI pageCount;

    [SerializeField] private UIButton btnNextPage;

    //[SerializeField] private GameObject bgHightLight;
    [SerializeField] private UIButton btnBackPage;

    [SerializeField] private UIButton btnBack;

    //[SerializeField] private TextMeshProUGUI textItemCount;
    [SerializeField] private Sprite lockedSprite;

    [SerializeField] private Sprite unlockSprite;

    //[Header("Debug")] [SerializeField] private UIButton BtnUnlockItem;
    private Action _actionBack;

    private int currentPageID;

    // Start is called before the first frame update
    public void Initialized(Action actionBack)
    {
        _actionBack = actionBack;
        currentPageID = 0;
        Refresh();
        //BtnUnlockItem.gameObject.SetActive(Config.IsDebug);
    }

    void HightLightGift()
    {
        SetUpFirstCollect(true);
    }

    void SetUpFirstCollect(bool isfirst)
    {
        btnBack.gameObject.SetActive(!isfirst);
        //bgHightLight.gameObject.SetActive(isfirst);
        btnBackPage.gameObject.SetActive(!isfirst);
        btnNextPage.gameObject.SetActive(!isfirst);
    }

    private void Refresh()
    {
        pageCount.text = (currentPageID + 1) + "/" +
                         (travelStoryData.TravelStoryDataItemCount / listTravelStoryItems.Count + 1);
        for (int i = 0; i < listTravelStoryItems.Count; i++)
        {
            var item = listTravelStoryItems[i];
            var itemData = travelStoryData.GetTravelStoryById(i + listTravelStoryItems.Count * currentPageID);
            if (itemData == null)
            {
                item.SetupDefaultState(lockedSprite);
                continue;
            }

            if (itemData.Id <= Data.CurrentWorld)
            {
                item.SetupUnlockState(unlockSprite, itemData.Icon, itemData.Id + 1, itemData.CountryName,
                    () => UseRoom(itemData.Id));
            }
            else
            {
                item.SetupDefaultState(lockedSprite);
            }
        }

        //textItemCount.text = count + "/" + listTravelStoryItems.Count;
        btnNextPage.gameObject.SetActive((currentPageID + 1) * listTravelStoryItems.Count <
                                         travelStoryData.TravelStoryDataItemCount);
        btnBackPage.gameObject.SetActive((currentPageID != 0));
    }


    public void OnClickBtnNextBack(bool isNextBtn)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        currentPageID += 1 * (isNextBtn ? 1 : -1);
        Refresh();
    }

    public void OnClickBtnClose()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        _actionBack?.Invoke();
        gameObject.SetActive(false);
        GamePopup.Instance.HidePopupMoney();
    }

    public async void UseRoom(int index)
    {
        // todo
        Data.CurrentMenuWorld = index;
        if (GamePopup.Instance.menuRoom != null) Destroy(GamePopup.Instance.menuRoom.gameObject);
        //MenuController.instance.Block.SetActive(true);
        GamePopup.Instance.menuRoom = null;
        var room = await BridgeData.Instance.GetRoom(index);
        if (room != null) BridgeData.Instance.menuRoomPrefab = room.GetComponent<BaseRoom>();

        GamePopup.Instance.ShowRoom(isDance: true);
        gameObject.SetActive(false);
        _actionBack?.Invoke();
    }
}