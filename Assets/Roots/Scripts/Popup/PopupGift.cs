using System;
using Pancake.Monetization;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PopupGift : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnNoThanks;
    [SerializeField] private Button btnContinue;
    [SerializeField] private GameObject effectClaim;
    [SerializeField, SpineAnimation] private string continueAnimation;
    [SerializeField] private GameObject fetch;
    private Action<Action> _actionClaim;
    private Action _actionNoThank;

    private void Update()
    {
        if (Utils.pauseUpdateFetchIcon) return;

        if (!Application.isEditor)
        {
            fetch.SetActive(!Advertising.IsRewardedAdReady());
        }
        else
        {
            if (fetch.activeSelf) fetch.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionClaim"></param>
    /// <param name="actionNoThank"></param>
    public void Initialized(Action<Action> actionClaim, Action actionNoThank)
    {
        effectClaim.SetActive(false);

        _actionClaim = actionClaim;
        _actionNoThank = actionNoThank;

        btnClaim.onClick.RemoveAllListeners();
        btnClaim.onClick.AddListener(OnClaimButtonPressed);

        btnNoThanks.onClick.RemoveAllListeners();
        btnNoThanks.onClick.AddListener(OnNoThankButtonPressed);

        btnContinue.onClick.RemoveAllListeners();
        btnContinue.onClick.AddListener(OnContinueButtonPressed);

        Utils.pauseUpdateFetchIcon = false;
    }

    private void OnClaimButtonPressed()
    {
        _actionClaim?.Invoke(OnClaimSuccess);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnClaimSuccess()
    {
        btnClaim.gameObject.SetActive(false);
        btnNoThanks.gameObject.SetActive(false);
        btnContinue.gameObject.SetActive(true);
        if (effectClaim != null) effectClaim.SetActive(true);
    }

    private void OnNoThankButtonPressed()
    {
        _actionNoThank?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnContinueButtonPressed()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ChangeAnimationOnClaim()
    {
        skeleton.AnimationState.SetAnimation(0, continueAnimation, false);
    }
}