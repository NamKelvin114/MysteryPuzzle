using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.UI;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class GiftTask : MonoBehaviour
{
    [SerializeField] public SetUpGift setUpGift;
    private Vector3 _scaleUp;
    public bool isCalmed { set; get; }
    public bool isLastGift;
    [SerializeField] private Sprite open;
    [SerializeField] private EChestType chestType;
    [SerializeField] private Sprite close;
    [ShowIf("isLastGift")] public Sprite ticketIcon;
    [ShowIf("isLastGift", false)] public Image line;
    private Vector3 _scaleDown;
    private UIButton _uiButton;
    private bool _isRewarded;
    [SerializeField] Image setIcon;
    float _offset = 1.2f;
    public void Init()
    {
        transform.localScale = new Vector3(1, 1, 1);
        Observer.UpdateProcess += UpdateGift;
        _uiButton = gameObject.GetComponent<UIButton>();
        UpdateGift();
    }
    private void OnDisable()
    {
        transform.DOKill();
    }
    void UpdateGift()
    {
        if (Utils.ProcessTask >= setUpGift.starAmount)
        {
            if (Utils.GiftTaskRewardAmount < setUpGift.starAmount)
            {
                Observer.UpdateGiftReward?.Invoke(this);
            }
            else
            {
                Rewarded();
            }
        }
        else
        {
            setIcon.sprite = close;
            _uiButton.enabled = false;
        }
    }
    public void CanReWard()
    {
        setIcon.sprite = close;
        Observer.UpdateProcess -= UpdateGift;
        _uiButton.enabled = true;
        Scale();
    }
    public void CannotRewarded()
    {
        _uiButton.enabled = false;
    }
    void Rewarded()
    {
        setIcon.sprite = open;
        if (isLastGift == false)
        {
            line.gameObject.SetActive(false);
        }
        isCalmed = true;
        _uiButton.enabled = false;
    }
    void Scale()
    {
        transform.DOScale(transform.localScale * _offset, 1f)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
    public void OnClaim()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.claimGift);
        setIcon.sprite = open;
        transform.DOKill();
        if (isLastGift == false)
        {
            line.gameObject.SetActive(false);
        }
        isCalmed = true;
        Observer.UpdateProcess -= UpdateGift;
        GamePopup.Instance.ShowPopupChestTask(setUpGift.coinCanReward, ticketIcon, chestType);
        Utils.GiftTaskRewardAmount += setUpGift.starAmount;
        UpdateGift();
        Observer.CloseTask?.Invoke(true);
        Observer.UpdateProcess?.Invoke();

    }
}
[Serializable]
public class SetUpGift
{
    public int starAmount;
    public int coinCanReward;
}
