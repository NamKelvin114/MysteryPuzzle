using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjMoveUI : MonoBehaviour
{
    [SerializeField] private EMoveType moveType;
    [SerializeField] private float distane = 500;
    [SerializeField] private float time = .5f;
    [SerializeField] private Ease ease = Ease.InBack;

    private Vector3 positionDefaut = Vector3.zero;
    private RectTransform thisRectTransform;

    private void Awake()
    {
        //positionDefaut = this.transform.localPosition;
        Observer.UIMove += Move;
        thisRectTransform = this.GetComponent<RectTransform>();
        positionDefaut = thisRectTransform.anchoredPosition;
    }

    public void Move(Action actionCompleted = null)
    {
        thisRectTransform.DOKill();
        //Data.IsUIMoving = true;
        switch (moveType)
        {
            case EMoveType.MOVE_UP:
                //this.transform.DOLocalMoveY(positionDefaut.y + distane, time).SetEase(ease);
                thisRectTransform.DOAnchorPosY(positionDefaut.y + distane, time).SetEase(ease).OnComplete(() =>
                {
                    //Data.IsUIMoving = false;
                    actionCompleted?.Invoke();
                });
                break;
            case EMoveType.MOVE_DOWN:
                //this.transform.DOLocalMoveY(positionDefaut.y - distane, time).SetEase(ease);
                thisRectTransform.DOAnchorPosY(positionDefaut.y - distane, time).SetEase(ease).OnComplete(() =>
                {
                    //Data.IsUIMoving = false;
                    actionCompleted?.Invoke();
                });
                break;
            case EMoveType.MOVE_RIGHT:
                //this.transform.DOLocalMoveX(positionDefaut.x + distane, time).SetEase(ease);
                thisRectTransform.DOAnchorPosX(positionDefaut.x + distane, time).SetEase(ease).OnComplete(() =>
                {
                    //Data.IsUIMoving = false;
                    actionCompleted?.Invoke();
                });
                break;
            case EMoveType.MOVE_LEFT:
                //this.transform.DOLocalMoveX(positionDefaut.x - distane, time).SetEase(ease);
                thisRectTransform.DOAnchorPosX(positionDefaut.x - distane, time).SetEase(ease).OnComplete(() =>
                {
                    //Data.IsUIMoving = false;
                    actionCompleted?.Invoke();
                });
                break;
        }
    }

    public void Defaut()
    {
        if (thisRectTransform == null) return;
        //Data.IsUIMoving = false;
        thisRectTransform.anchoredPosition = positionDefaut;
    }

    public void MoveBack()
    {
        thisRectTransform.DOKill();
        thisRectTransform.DOAnchorPos(positionDefaut, time / 2).SetEase(ease);
    }

    private void OnDestroy()
    {
        Observer.UIMove -= Move;
    }
}

public enum EMoveType
{
    MOVE_UP,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT
}