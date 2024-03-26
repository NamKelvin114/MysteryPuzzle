using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;

public class BeeHive : LauncherEnemy
{
    [SpineAnimation, SerializeField] protected string ideAnimation;
    [SpineAnimation, SerializeField] protected string dieAnimation;
    [SerializeField] private Animator beeAnimation;
    [SerializeField] private AudioClip idleAudio;
    [SerializeField] private List<BeeEnemy> beeEnemies;
    [SerializeField] private BoxCollider2D boxCollider2D;
    private int temp = 0;
    private int setMiddle;

    private void Start()
    {
        if (idleAudio!=null)
        {
            SoundManager.Instance.PlaySoundContinously(idleAudio);
        }
        DoAnim(ideAnimation, true);
        setMiddle = beeEnemies.Count / 2;
    }

    public override void DoTarget(GameObject target)
    {
        beeAnimation.enabled = false;
        SoundManager.Instance.StopSound();
        InsObj();
    }

    void InsObj()
    {
        if (beeEnemies.Count != temp && state != EnemyBase.CHAR_STATE.DIE)
        {
            var getDestination = (temp - setMiddle) * 0.3f;
            beeEnemies[temp].Move(getDestination, target);
            temp++;
            DOTween.Sequence().AppendInterval(0.2f).OnComplete(() => { InsObj(); });
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var soundManager = SoundManager.Instance;
        if (col.gameObject.CompareTag("Trap_Lava"))
        {
            if (soundManager != null)
            {
                soundManager.PlaySound(soundManager.beeDieByLava);
            }
            beeAnimation.enabled = false;

            ChangeState();
        }
        else if (col.gameObject.CompareTag("Tag_Stone"))
        {
            if (soundManager != null)
            {
                soundManager.PlaySound(soundManager.beeDieByRock);
            }
            beeAnimation.enabled = false;

            ChangeState();
        }
    }

    private void ChangeState()
    {
        state = EnemyBase.CHAR_STATE.DIE;
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false;
        }
        SoundManager.Instance.StopSound();
        OnDie();
        DoAnim(dieAnimation, false);
    }
    private void OnDisable()
    {
        SoundManager.Instance.StopSound();
    }
}
