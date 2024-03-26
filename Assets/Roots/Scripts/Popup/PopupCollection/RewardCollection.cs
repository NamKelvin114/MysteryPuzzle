using System;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCollection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private SkeletonGraphic giftBox;
    [SerializeField] private Image gift;
    [SerializeField] private Animation animationController;
    [SerializeField] private GameObject giftCoin;
    [SerializeField] private GameObject giftSkin;

    [SerializeField, SpineAnimation(dataField = "giftBox")]
    private string actionIdle;

    [SerializeField, SpineAnimation(dataField = "giftBox")]
    private string actionOpen;

    private Action _returnAction;
    private int _coinValue;
    private SkinData _skinData;
    private Action _actionBack;

    public void SetupCoinReward(Action returnAction, int coinValue = 0, Action actionBack = null)
    {
        _actionBack = actionBack;
        giftCoin.SetActive(true);
        giftSkin.SetActive(false);
        _returnAction = returnAction;
        _coinValue = coinValue;
        _skinData = null;
        coinText.text = "+" + coinValue;
    }

    public void SetupSkinReward(Action returnAction, SkinData skinData)
    {
        giftCoin.SetActive(false);
        giftSkin.SetActive(true);
        gift.sprite = skinData.shopIcon;
        gift.SetNativeSize();
        _returnAction = returnAction;
        _coinValue = 0;
        _skinData = skinData;
    }

    // public void Play()
    // {
    //     animationController.Play();
    // }

    public void ClaimReward()
    {
        if (_coinValue > 0)
        {
            GamePopup.Instance.ShowPopupMoney();
            Utils.UpdateCoin(_coinValue);
        }
        if (_skinData != null)
        {
            _skinData.IsUnlocked = true;
        }

        _returnAction?.Invoke();
        giftBox.AnimationState.AddAnimation(0, actionIdle, true, 0);
        gameObject.SetActive(false);
        _actionBack?.Invoke();
    }

    public void PlayAnimIdle()
    {
        giftBox.AnimationState.AddAnimation(0, actionIdle, true, 0);
    }

    public void PlayAnimOpen()
    {
        giftBox.AnimationState.AddAnimation(0, actionOpen, false, 0);
        DOTween.Sequence().AppendInterval(1.26f).OnComplete(() =>
        {
            var sound = SoundManager.Instance;
            if(sound != null) 
                sound.PlaySound(sound.giftCollectionPopup);
        });
    }

    public void ClaimRewardInPopupCollection()
    {
        if (_coinValue > 0)
        {
            GamePopup.Instance.ShowPopupMoney();
            Utils.UpdateCoin(_coinValue);
        }
        if (_skinData != null)
        {
            _skinData.IsUnlocked = true;
        }

        giftBox.AnimationState.AddAnimation(0, actionIdle, true, 0);
        gameObject.SetActive(false);
        _returnAction?.Invoke();
    }

    private void OnEnable()
    {
        giftBox.Initialize(true);
    }
}