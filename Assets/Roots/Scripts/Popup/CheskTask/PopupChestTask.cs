using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupChestTask : UniPopupBase
{
    [SerializeField, SpineAnimation] private string openAnim;
    [SerializeField] private UniButton btnBack;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private GameObject fromGnerateCoin;
    [SerializeField] float duration;
    [SerializeField] private ItemReward itemReward;
    [SerializeField] private HorizontalLayoutGroup horizontal;
    [SerializeField] private TextMeshProUGUI cointText;
    [SerializeField] private GameObject coin;
    [SerializeField] private float scaleItem;
    [SerializeField] private GameObject fxLightChest;
    private Action _actionBack;
    private int _cointCanReward;
    private Sprite _icon;

    public void Init(Action actionBack, int getCoint, Sprite getSprite, EChestType getEChestType)
    {
        switch (getEChestType)
        {
            case EChestType.Bronze:
                skeletonGraphic.Skeleton.SetSkin("Chest1");
                break;
            case EChestType.Silver:
                skeletonGraphic.Skeleton.SetSkin("Chest2");
                break;
            case EChestType.Gold:
                skeletonGraphic.Skeleton.SetSkin("Chest3");
                break;
        }
        skeletonGraphic.Skeleton.SetToSetupPose();
        _actionBack = actionBack;
        _icon = getSprite;
        _cointCanReward = getCoint;
        cointText.text = $"+{_cointCanReward}";
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);
        btnBack.gameObject.SetActive(false);
        fxLightChest.SetActive(false);
        Observer.AddFromPosiGenerationCoin?.Invoke(fromGnerateCoin);
    }

    public override void Show()
    {
        base.Show();
        this.StartCoroutine(WaitForChestAnim());
    }

    IEnumerator WaitForChestAnim()
    {
        if (skeletonGraphic && skeletonGraphic.SkeletonData.FindAnimation(openAnim) != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.openChest);
            var doAnim = skeletonGraphic.AnimationState.SetAnimation(0, openAnim, false);
            yield return new WaitForSpineAnimationComplete(doAnim);
            SetClaim();
            SetUpItemReward();
        }
    }
    void SetUpItemReward()
    {
        fxLightChest.SetActive(true);
        coin.SetActive(true);
        Utils.UpdateCoin(_cointCanReward);
        if (_icon != null)
        {
            var getItem = SpawnItemReward();
            getItem.SetIcon(_icon);

            getItem.transform.DOMove(horizontal.transform.position, duration).OnStart((() =>
            {
                getItem.transform.DOScale(transform.localScale * scaleItem, duration);
            })).OnComplete((() =>
            {
                btnBack.gameObject.SetActive(true);
            }));
        }
        else
        {
            btnBack.gameObject.SetActive(true);
        }
    }
    ItemReward SpawnItemReward()
    {
        var itemReward = Instantiate(this.itemReward, skeletonGraphic.transform);
        return itemReward;
    }
    void OnBackButtonPressed()
    {
        coin.SetActive(false);
        Clear(horizontal.transform);
        Clear(skeletonGraphic.transform);
        _actionBack?.Invoke();
        gameObject.SetActive(false);
        GamePopup.Instance.HidePopupMoney();
    }
    void SetClaim()
    {
        DataController.instance.SaveData();
        if (_icon != null)
        {
            //Set new value for Utils.SetMapTaskProcess
            Observer.NextMapTask?.Invoke();
            Utils.TaskToLoad = "";
            Utils.ProcessTask = 0;
            Utils.GiftTaskRewardAmount = 0;
        }
    }
    void Clear(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
public enum EChestType
{
    Bronze,
    Silver,
    Gold,
}
