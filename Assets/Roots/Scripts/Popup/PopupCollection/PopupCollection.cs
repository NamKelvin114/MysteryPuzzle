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

public class PopupCollection : UniPopupBase
{
    [SerializeField] private CollectionBook collectionBook;
    [SerializeField] private List<CollectionItem> listCollectionItems;
    [SerializeField] private TextMeshProUGUI pageTitle;
    [SerializeField] private TextMeshProUGUI pageCount;
    [SerializeField] private Image pageContent;
    [SerializeField] private UIButton btnNextPage;
    [SerializeField] private GameObject bgHightLight;
    [SerializeField] private UIButton btnBackPage;
    [SerializeField] private UIButton btnBack;
    [SerializeField] private SkeletonGraphic chest;

    [SerializeField, SpineAnimation(dataField = "chest")]
    private string cantClaim;

    [SerializeField, SpineAnimation(dataField = "chest")]
    private string readyToClaim;

    [SerializeField, SpineAnimation(dataField = "chest")]
    private string open;

    [SerializeField, SpineAnimation(dataField = "chest")]
    private string opened;

    [SerializeField] private UIButton btnChest;
    [SerializeField] private TextMeshProUGUI textItemCount;
    [SerializeField] private RewardCollection popupClaim;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockSprite;
    [SerializeField] private GameObject vfxGiftClaim;
    [Header("Debug")] [SerializeField] private UIButton BtnUnlockItem;
    private Action _actionBack;
    private Action _actionBackWithoutHide;

    private int currentPageID;

    // Start is called before the first frame update
    public void Initialized(Action actionBack, Action actionBackWithoutHide = null)
    {
        chest.AnimationState.SetAnimation(0, cantClaim, false);
        _actionBack = actionBack;
        _actionBackWithoutHide = actionBackWithoutHide;
        popupClaim.gameObject.SetActive(false);
        currentPageID = collectionBook.GetPageReadyToClaimReward();
        if (currentPageID == -1)
        {
            currentPageID = collectionBook.GetLastestPageID();
        }

        Observer.HightLightGiftBoxCollection += HightLightGift;
        Refresh();
        BtnUnlockItem.gameObject.SetActive(Config.IsDebug);
    }

    void HightLightGift()
    {
        SetUpFirstCollect(true);
    }

    void SetUpFirstCollect(bool isfirst)
    {
        btnBack.gameObject.SetActive(!isfirst);
        bgHightLight.gameObject.SetActive(isfirst);
        btnBackPage.gameObject.SetActive(!isfirst);
        btnNextPage.gameObject.SetActive(!isfirst);
    }

    private void Refresh()
    {
        CollectionPage currentPage = null;
        currentPage = collectionBook.GetPageByID(currentPageID);
        if (currentPage == null) return;
        pageContent.sprite = currentPage.Content;
        pageContent.SetNativeSize();
        pageCount.text = (currentPageID + 1) + "/" + collectionBook.GetBookSize();
        pageTitle.text = "Chapter " + (currentPageID + 1);
        int count = 0;
        for (int i = 0; i < listCollectionItems.Count; i++)
        {
            var item = listCollectionItems[i];
            var itemData = currentPage.GetItemById(i);
            if (itemData.IsUnlocked)
            {
                item.SetupUnlockState(unlockSprite, currentPage.GetItemById(i).ItemIcon);
                count++;
            }
            else
            {
                item.SetupDefaultState(lockedSprite);
            }
        }

        textItemCount.text = count + "/" + listCollectionItems.Count;
        btnChest.interactable = false;
        if (currentPage.GetLastestItem() == null) // is ready to claim/ claimed
        {
            if (currentPage.IsCollected) // claimed
            {
                //chest.AnimationState.SetAnimation(0, opened, false);
                btnChest.gameObject.SetActive(false);
                vfxGiftClaim.SetActive(false);
            }
            else // ready to claim
            {
                vfxGiftClaim.SetActive(true);
                btnChest.gameObject.SetActive(true);
                btnChest.interactable = true;
                chest.AnimationState.SetAnimation(0, readyToClaim, true);
            }
        }
        else // can't claim
        {
            btnChest.gameObject.SetActive(true);
            vfxGiftClaim.SetActive(false);
            chest.AnimationState.SetAnimation(0, cantClaim, false);
        }

        popupClaim.SetupCoinReward(OnClaimReward, collectionBook.GetLastestPage().RewardMoney, _actionBack);
        btnNextPage.gameObject.SetActive((collectionBook.GetPageByID(currentPageID + 1) != null));
        btnBackPage.gameObject.SetActive((currentPageID != 0));
    }

    private void OnClaimReward()
    {
        CollectionPage currentPage = collectionBook.GetPageByID(currentPageID);
        currentPage.IsCollected = true;
        _actionBackWithoutHide.Invoke();
        GamePopup.Instance.ShowPopupMoney();
        OnClickBtnNextBack(true);
        Refresh();
    }

    public void OnClickOpenReward()
    {
        SetUpFirstCollect(false);
        GamePopup.Instance.HidePopupCollectionTutorial();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        btnChest.interactable = false;
        btnChest.gameObject.SetActive(true);
        OpenReward(null);
        GamePopup.Instance.HidePopupMoney();
        //get next page when close
        //OnClickBtnNextBack(true);
    }

    private void OpenReward(TrackEntry animation)
    {
        chest.AnimationState.SetAnimation(0, cantClaim, false);
        btnChest.gameObject.SetActive(false);
        // This method will be called when the animation ends
        if (SoundManager.Instance != null)
        {
            // SoundManager.Instance.PlaySound(SoundManager.Instance.acClick); can be sound Open Reward
        }

        if (popupClaim != null)
        {
            popupClaim.gameObject.SetActive(true);
            // popupClaim.Play();
        }
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
        _actionBackWithoutHide?.Invoke();
        gameObject.SetActive(false);
        GamePopup.Instance.HidePopupMoney();
    }

    public void UnlockItem()
    {
        var item = collectionBook.GetLastestPage().GetLastestItem();
        if (item != null)
        {
            item.IsUnlocked = true;
            Refresh();
        }

        _actionBackWithoutHide?.Invoke();
        //GamePopup.Instance.ShowPopupMoney();
    }

    public void ShowTutForCollection()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        GamePopup.Instance.ShowPopupCollectionTutorial();
    }
}