using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;

public class MoveInHome : MonoBehaviour
{
    [SerializeField] private float moveDistance = 250f;
    [SerializeField] private float timeMove = .5f;
    [SerializeField] private SkeletonGraphic mainGirl;

    public void DoMove(bool isMoveLeft = true)
    {
        this.rectTransform().DOAnchorPosX(moveDistance * (isMoveLeft ? 1f : -1f), timeMove).OnComplete(() =>
        {
            Vector3 scale = mainGirl.gameObject.transform.localScale;
            mainGirl.Skeleton.ScaleX = (isMoveLeft ? -1f : 1f);
            DoMove(!isMoveLeft);
        });
    }

    public void Stop()
    {
        float yPos = this.rectTransform().anchoredPosition.y;
        this.rectTransform().anchoredPosition = new Vector2(0, yPos);
        DOTween.Kill(transform);
    }
        

}
