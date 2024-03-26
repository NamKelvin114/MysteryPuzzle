using System;
using Pancake.Monetization;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupNewSkin : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeleton;

    [SerializeField, SpineSkin(dataField = "skeleton")]
    private string defaultSkin;

    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnNoThanks;
    [SerializeField] private Button btnContinue;
    [SerializeField] private GameObject effectClaim;
    [SerializeField, SpineAnimation] private string continueAnimation;
    [SerializeField] private GameObject fetch;
    [SerializeField] private TextMeshProUGUI txtCost;
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
    /// <param name="id"></param>
    /// <param name="skinName"></param>
    /// <param name="cost"></param>
    /// <param name="actionClaim"></param>
    /// <param name="actionNoThank"></param>
    public void Initialized(SkinData skinData, bool isShirt, Action<Action> actionClaim, Action actionNoThank)
    {
        string skinName = skinData.skinName;
        if (effectClaim != null) effectClaim.SetActive(false);
        var skeletonData = skeleton.Skeleton.Data;
        var mixAndMatchSkin = new Skin("new-skin");
        if (!isShirt)
            mixAndMatchSkin.AddSkin(skeletonData.FindSkin(defaultSkin));
        mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinData.skinName));
        skeleton.Skeleton.SetSkin(mixAndMatchSkin);
        skeleton.Skeleton.SetSlotsToSetupPose();
        // skeleton.LateUpdate();

        _actionClaim = actionClaim;
        _actionNoThank = actionNoThank;

        // if (id == 1 || id == 2)
        // {
        //     txtCost.transform.parent.gameObject.SetActive(false);
        // }
        // else
        // {
        //     txtCost.transform.parent.gameObject.SetActive(true);
        // }
        txtCost.transform.parent.gameObject.SetActive(true);
        txtCost.text = skinData.coinValue.ToString();

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
        txtCost.transform.parent.gameObject.SetActive(false);
    }

    private void OnNoThankButtonPressed()
    {
        _actionNoThank?.Invoke();
        gameObject.SetActive(false);
        if (MapLevelManager.Instance.isShowDercibeItem) GameManager.instance.ShowPopupItemDecription();
        else GameManager.instance.btnTabNext.gameObject.SetActive(true);
    }

    private void OnContinueButtonPressed()
    {
        gameObject.SetActive(false);
        if (MapLevelManager.Instance.isShowDercibeItem) GameManager.instance.ShowPopupItemDecription();
        else GameManager.instance.btnTabNext.gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ChangeAnimationOnClaim()
    {
        skeleton.AnimationState.SetAnimation(0, continueAnimation, true);
    }

    private void OnEnable()
    {
        GamePopup.Instance.HidePopupMoney();
    }

    private void OnDisable()
    {
        GamePopup.Instance.ShowPopupMoney();
    }
}