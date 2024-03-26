using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class EnemyHandleEvent : MonoBehaviour
{
    [SpineEvent] [SerializeField] private string eventAttack;
    public Action actionAttack;

    private SkeletonAnimation _skeletonAnimation;

    void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (_skeletonAnimation == null) return;

        _skeletonAnimation.AnimationState.Event += HandleEvent;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="trackEntry"></param>
    /// <param name="e"></param>
    private void HandleEvent(
        TrackEntry trackEntry,
        Spine.Event e)
    {
        if (e.Data.Name == eventAttack)
        {
            actionAttack?.Invoke();
        }
    }

    private void OnDisable()
    {
        if (!(_skeletonAnimation is null)) _skeletonAnimation.AnimationState.Event -= HandleEvent;
    }
}