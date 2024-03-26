using System;
using System.Collections;
using System.Collections.Generic;
using Pancake.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupClaimCollection : MonoBehaviour
{
    [SerializeField] private CollectionBook collectionBook;
    [SerializeField] private GameObject rotateFx;
    [SerializeField] private Image itemImage;
    [SerializeField] private UIButton claimButton;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private int coinReward;
    [SerializeField] private TextMeshProUGUI textReward;
    [SerializeField] private RewardCollection rewardCollection;
    private Action onEnd;

    public void Initialized(Action onEnd)
    {
        this.onEnd = onEnd;
        if (collectionBook.GetLastestPage().GetLastestItem() == null)
        {
            textReward.gameObject.SetActive(false);
            itemImage.gameObject.SetActive(false);
            claimButton.gameObject.SetActive(false);
            rotateFx.gameObject.SetActive(false);
            rewardCollection.SetupCoinReward(OnClickClaim, coinReward);
            rewardCollection.gameObject.SetActive(true);
            // itemImage.sprite = coinSprite;
            // itemImage.SetNativeSize();
            // itemImage.transform.localScale = Vector3.one * 3;
        }
        else
        {
            rewardCollection.gameObject.SetActive(false);
            textReward.gameObject.SetActive(true);
            textReward.text = "YOU FOUND A " + collectionBook.GetLastestPage().GetLastestItem().ItemName.ToUpper();
            itemImage.sprite = collectionBook.GetLastestPage().GetLastestItem().ItemIcon;
            itemImage.SetNativeSize();
            itemImage.transform.localScale = Vector3.one;
            claimButton.gameObject.SetActive(true);
        }
    }

    public void OnClickClaim()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        if (collectionBook.GetLastestPage().GetLastestItem() != null)
        {
            collectionBook.GetLastestPage().GetLastestItem().IsUnlocked = true;
        }
        else
        {
            Observer.AddFromPosiGenerationCoin(itemImage.gameObject);
            DataController.instance.CheckWarningForTask();
            //Utils.UpdateCoin(coinReward);
        }

        claimButton.gameObject.SetActive(false);
        onEnd?.Invoke();
        gameObject.SetActive(false);
    }
}