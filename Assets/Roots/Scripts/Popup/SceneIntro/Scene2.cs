using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Scene2 : MonoBehaviour
{
    [SerializeField] private RectTransform startPosition;
    [SerializeField] private RectTransform endPosition;
    [SerializeField] private Image handIcon;
    [SerializeField] private TransScene transScene1;
    [SerializeField] private int durationHand;
    [SerializeField, Range(0, 2)] private float timePlaySound;

    private void OnEnable()
    {
        handIcon.rectTransform.localPosition = startPosition.localPosition;
        DoMoveHand();
    }
    void DoMoveHand()
    {
        handIcon.rectTransform.DOLocalMove(endPosition.localPosition, durationHand).OnComplete((() =>
        {
            transScene1.DoTransScene(Done);
        }));
    }
    void Done()
    {
        Observer.nextIntroScene?.Invoke(this.rectTransform());
    }
}
