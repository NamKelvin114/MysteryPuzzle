using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class CannonAttackHandler : MonoBehaviour
{
    [SpineEvent] public string onBullet = "OnBullet";
    [SpineEvent] public string endAttack = "EndAttack";
    public SkeletonAnimation SkeletonAnimation { get; private set; }

    public Action onBulletAction;
    public Action endAttackAction;

    public void Start()
    {
        SkeletonAnimation = GetComponent<SkeletonAnimation>();
        if (SkeletonAnimation == null) return;

        SkeletonAnimation.AnimationState.Event -= HandleEvent;
        SkeletonAnimation.AnimationState.Event += HandleEvent;
    }

    private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == onBullet)
        {
            onBulletAction?.Invoke();
        }

        if (e.Data.Name == endAttack)
        {
            endAttackAction?.Invoke();
        }
    }
}