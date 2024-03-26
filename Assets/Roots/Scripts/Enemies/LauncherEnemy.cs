using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UniRx;
using UnityEngine;

public abstract class LauncherEnemy : MonoBehaviour
{
    [SerializeField] protected Transform leftRayCast;
    [SerializeField] protected Transform rightRaycast;
    [SerializeField, Range(1, 10)] protected float distance;
    private RaycastHit2D _raycastHitRight, _raycastHitLeft;
    [SerializeField] protected SkeletonAnimation skeletonAnimation;
    [SerializeField] protected AudioClip dieSound;
    [SerializeField] protected LayerMask layerTarget;
    [SerializeField] protected GameObject target;
    [SerializeField] protected EnemyBase.CHAR_STATE state;
    private bool _isMove;
    private void Start()
    {
        state = EnemyBase.CHAR_STATE.PLAYING;
    }
    private void FixedUpdate()
    {
        if (state != EnemyBase.CHAR_STATE.DIE)
        {
            if (target == null)
            {
                #if UNITY_EDITOR
            Debug.DrawRay(leftRayCast.position, -leftRayCast.right * distance, Color.red);
            Debug.DrawRay(rightRaycast.position, rightRaycast.right * distance, Color.green);
#endif
                DoRaycastLeft();
                DoRaycastRight();
            }
            else
            {
                if (!_isMove)
                {
                    _isMove = true;
                    DoTarget(target);
                }
            }
        }
    }
    void DoRaycastRight()
    {
        if (target == null && state != EnemyBase.CHAR_STATE.DIE)
        {
            _raycastHitRight = Physics2D.Raycast(rightRaycast.position, rightRaycast.right, distance, layerTarget);
            var checkRight = _raycastHitRight.collider;
            if (!checkRight.gameObject.CompareTag("StickBarrie") && !checkRight.gameObject.CompareTag("Chan"))
            {
                target = checkRight.transform.parent.gameObject;
            }
        }
    }
    void DoRaycastLeft()
    {
        if (target == null && state != EnemyBase.CHAR_STATE.DIE)
        {
            _raycastHitLeft = Physics2D.Raycast(leftRayCast.position, -leftRayCast.right, distance, layerTarget);
            var checkLeft = _raycastHitLeft.collider;
            if (!checkLeft.gameObject.CompareTag("StickBarrie") && !checkLeft.gameObject.CompareTag("Chan"))
            {
                target = checkLeft.transform.parent.gameObject;
            }
        }
    }
    protected void DoAnim(string animName, bool isLoop)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animName, isLoop);
    }
    public abstract void DoTarget(GameObject target);
    public virtual void OnDie()
    {
        if (dieSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(dieSound);
        }
    }
    private void OnDestroy()
    {
        _isMove = false;
    }
}
