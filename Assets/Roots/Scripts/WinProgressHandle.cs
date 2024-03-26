using System;
using System.Collections.Generic;
using System.Linq;
using Pancake.OdinSerializer;
using Spine.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using Worldreaver.Utility;
using Random = UnityEngine.Random;

public class WinProgressHandle : MonoBehaviour
{
    [SerializeField] private UniProgressBarTMP progressBar;
    [SerializeField] private Image giftEmpty;
    [SerializeField] private Sprite giftEmptySprite;
    [SerializeField] private Sprite giftFullSprite;

    [SerializeField] private float valueIncrease;

    [SerializeField] private SkeletonGraphic gift;

    [SpineAnimation] public string open;
    [SpineAnimation] public string idleOpen;
    [SerializeField] public SkinResources skinResources;

    public void UpdateProcess()
    {
        var value = valueIncrease;

        progressBar.Initialized(0f, 1f);
        progressBar.Current = Data.WinProgress;
        progressBar.ForegroundBar.GetComponent<Image>().fillAmount = Data.WinProgress;

        Data.WinProgress += value;
        if (Data.WinProgress > 1)
        {
            Data.WinProgress = 1;
        }

        bool isShirt = Random.Range(0, 3) == 0 ? true : false;
        List<SkinData> temp = new List<SkinData>();
        List<SkinData> temp1 = skinResources.GetLockedSkin(true);
        List<SkinData> temp2 = skinResources.GetLockedSkin(false);
        temp = isShirt ? temp1 : temp2;
        if (temp1.Count == 0)
        {
            isShirt = false;
            temp = temp2;
        }

        if (temp2.Count == 0)
        {
            isShirt = true;
            temp = temp1;
        }

        bool flag = (temp.Count == 0);

        //giftEmpty.gameObject.SetActive(true);

        if (!flag)
        {
            if (Data.WinProgress >= 1f)
            {
                // var id = temp.Keys.ToList().PickRandom();
                // var skin = temp[id];
                SkinData newSkin = temp[Random.Range(0, temp.Count)];
                //giftEmpty.sprite = giftFullSprite;
                gift.AnimationState.SetAnimation(0, open, false);
                gift.AnimationState.AddAnimation(0, idleOpen, true, 0);
                Observable.Timer(TimeSpan.FromSeconds(1.2f))
                    .Subscribe(_ =>
                    {
                        GameManager.instance.DisplayPopupNewSkin(newSkin, isShirt);
                        Data.WinProgress = 0;
                    })
                    .AddTo(this);
            }
            else
            {
                giftEmpty.sprite = giftEmptySprite;
                if (!MapLevelManager.Instance.isShowDercibeItem)
                    GameManager.instance.btnTabNext.gameObject.SetActive(true);
                else GameManager.instance.ShowPopupItemDecription();
            }
        }
        else
        {
            if (Data.WinProgress >= 1f)
            {
                giftEmpty.sprite = giftFullSprite;
                Observable.Timer(TimeSpan.FromSeconds(1.2f))
                    .Subscribe(_ =>
                    {
                        GameManager.instance.DisplayPopupGift();
                        Data.WinProgress = 0;
                    })
                    .AddTo(this);
            }
            else
            {
                giftEmpty.sprite = giftEmptySprite;
                if (!MapLevelManager.Instance.isShowDercibeItem)
                    GameManager.instance.btnTabNext.gameObject.SetActive(true);
                else GameManager.instance.ShowPopupItemDecription();
            }
        }

        progressBar.IncreaseGuard(value);
        progressBar.Text.text = $"{(int)(Data.WinProgress * 100)}%";
    }
}