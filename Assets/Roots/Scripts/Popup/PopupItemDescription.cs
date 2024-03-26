using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupItemDescription : UniPopupBase
{
    // Start is called before the first frame update
    [SerializeField] private Image menuFood;
    [SerializeField] private Image rubberStamp;
    [SerializeField] private Image stampItem;
    [SerializeField] private Image stampFood;
    [SerializeField] private float timeRubberStampMove = .5f;
    [SerializeField] private float timeRubberStampStay = .5f;
    [SerializeField] private RectTransform startPosRubberTime;
    [SerializeField] private GameObject buttonContinue;
    public override void Show()
    {
        base.Show();
        var menu = menuFood.GetComponent<MenuFood>();
        menuFood.gameObject.SetActive(true);
        SoundManager.Instance.PlaySound(SoundManager.Instance.popupItemAppear);
        float _percentValue = 0.0f;
        DOTween.To(x => _percentValue = (int)x,
            0, 360 * menu.numberRotation, menu.menuFoodTimeScale).SetEase(Ease.Linear).OnUpdate(() =>
        {
            menuFood.rectTransform.eulerAngles = new Vector3(0, 0, _percentValue);
        });
        menuFood.rectTransform.DOScale(new Vector3(menu.sizeMax, menu.sizeMax, menu.sizeMax), menu.menuFoodTimeScale)
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                rubberStamp.gameObject.SetActive(true);
                buttonContinue.SetActive(true);
                rubberStamp.rectTransform.DOAnchorPos(stampFood.rectTransform.anchoredPosition, timeRubberStampMove).OnComplete(
                    () =>
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.rubberStamp);
                        // if (MapLevelManager.Instance.isItenFood) stampFood.gameObject.SetActive(true);
                        // else stampItem.gameObject.SetActive(true);
                        DOTween.Sequence().AppendInterval(timeRubberStampStay).OnComplete(() =>
                      {
                          rubberStamp.rectTransform.DOAnchorPos(startPosRubberTime.anchoredPosition,
                              timeRubberStampMove);
                      });
                    });
            });
    }
    public void Setup(Sprite item, string decription, string title)
    {
        var menu = menuFood.GetComponent<MenuFood>();
        menu.Setup(item,decription,title);
        rubberStamp.gameObject.SetActive(false);
    }
}
