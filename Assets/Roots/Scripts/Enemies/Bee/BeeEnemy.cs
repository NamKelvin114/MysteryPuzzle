using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Spine;
using Spine.Unity;
using UnityEngine;

public class BeeEnemy : MonoBehaviour
{
    [SerializeField] private EnemyBase.CHAR_STATE _state;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SpineAnimation, SerializeField] protected string moveAnimation;
    private GameObject _target;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SpineAnimation, SerializeField] protected string ideAnimation;
    [SpineAnimation, SerializeField] protected string attackAnimation;
    [SpineAnimation, SerializeField] protected string dieAnimation;
    private List<string> animContinue = new List<string>();
    private float _distanceEnd;
    private bool _isMove;
    private int _isRight;
    private void Start()
    {
        _state = EnemyBase.CHAR_STATE.PLAYING;
        DoSingleAnim(ideAnimation, true);
    }
    public void Move(float distance, GameObject target)
    {
        if (_state != EnemyBase.CHAR_STATE.DIE)
        {
            _target = target;
            _isMove = true;
            _distanceEnd = distance;
            var checkDir = transform.position.x - target.transform.position.x;
            _isRight = checkDir > 0 ? -1 : 1;
            //transform.localScale = new Vector3(transform.localScale.x * _isRight, transform.localScale.y, transform.localScale.z);
            DoSingleAnim(moveAnimation, true);
        }

    }
    private void Update()
    {
        if (_target != null && _isMove && _state != EnemyBase.CHAR_STATE.DIE)
        {
            // var dirLook = transform.position - _target.transform.position;
            // transform.right = dirLook;
            var endPos = _target.transform.position + new Vector3(-0.3f * _isRight, 0.8f + _distanceEnd, 0);
            transform.position = Vector3.MoveTowards(transform.position, endPos, 3 * Time.deltaTime);
            if (Vector3.Distance(transform.position, endPos) <= 0.02f)
            {
                _isMove = false;
                DoAttack(_target);
            }
            else
            {
                _isMove = true;
            }
        }
    }
    public void DoAttack(GameObject target)
    {
        if (_state == EnemyBase.CHAR_STATE.DIE)
        {
            return;
        }
        animContinue.Add(attackAnimation);
        DoContinueAnim(animContinue);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Trap_Lava") || col.gameObject.CompareTag("Tag_Stone"))
        {
            _state = EnemyBase.CHAR_STATE.DIE;
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            skeletonAnimation.AnimationState.ClearTracks();
            skeletonAnimation.AnimationState.Complete -= DoneAttack;
            DoSingleAnim(dieAnimation, false);
        }
    }
    void DoSingleAnim(string animName, bool isLoop)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animName, isLoop);
    }
    void DoContinueAnim(List<string> getContinueAnim)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, getContinueAnim[0], false);
        for (int i = 0; i < getContinueAnim.Count; i++)
        {
            bool isLast = false;
            if (i == getContinueAnim.Count - 1)
            {
                isLast = true;
            }
            if (isLast)
            {
                skeletonAnimation.AnimationState.Complete += DoneAttack;
            }
            skeletonAnimation.AnimationState.AddAnimation(0, getContinueAnim[i], isLast, 0);
        }
    }
    void DoneAttack(TrackEntry trackEntry)
    {
        if (GameManager.instance.gameState == EGameState.Win || GameManager.instance.gameState == EGameState.Lose)
        {
            return;
        }
        var setTarget = _target.gameObject.GetComponentInParent<IBeeTringger>();
        setTarget.OnGetBeeAttack();
    }
    private void OnDestroy()
    {
        animContinue.Clear();
        skeletonAnimation.AnimationState.Complete -= DoneAttack;
    }
}
