using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class EnemyGamePlay2 : MonoBehaviour, IExplodeReceiver
{
    [SerializeField, SpineAnimation] private string idle;
    [SerializeField, SpineAnimation] private string win;
    [SerializeField, SpineAnimation] private string lose;
    [SerializeField, SpineAnimation] private string dieNormal;
    [SerializeField, SpineAnimation] private string dieFire;
    private bool _isDie;
    [SerializeField] private SkeletonAnimation skeleton;
    [SerializeField] private AudioClip idleAudio;
    [SerializeField] AudioClip dieAudio;

    private void Start()
    {
        PlayIdleAudio();
        _isDie = false;
        Observer.playerEndGameplay2 += EndGame;
        PlayAnim(idle, true);
    }
    void PlayIdleAudio()
    {
        if (SoundManager.Instance != null && idleAudio != null)
        {
            SoundManager.Instance.PlaySound(idleAudio);
        }
    }
    void PLayDieAudio()
    {
        if (SoundManager.Instance != null && idleAudio != null)
        {
            SoundManager.Instance.PlaySound(dieAudio);
        }
    }

    void PlayAnim(string anim, bool isLoop)
    {
        skeleton.AnimationState.SetAnimation(0, anim, isLoop);
        // if (!skeleton.AnimationName.Equals(anim))
        // {
        //     Debug.Log("dsđáoađóaods");
        //     Debug.Log($"Condition: +{isLoop}");
        //     skeleton.AnimationState.SetAnimation(0, anim, isLoop);
        // }
    }

    public void OnDie(EDieReason dieReason)
    {
        _isDie = true;
        PLayDieAudio();
        switch (dieReason)
        {
            case EDieReason.Normal:
                PlayAnim(dieNormal, false);
                break;
            case EDieReason.Fire:
                PlayAnim(dieFire, false);
                break;
            // case EDieReason.Ice:
            //     PlayAnim(dieIceAnimation, false);
            //     break;
        }
        Observer.UpdateTempValue?.Invoke(ETaskType.DestroyEnemy);
        Observer.playerEndGameplay2?.Invoke(true);
    }

    public void OnExplodedAt(BombItem bomb)
    {
        if (_isDie == false)
        {
            OnDie(EDieReason.Fire);
        }
    }

    void EndGame(bool playerWin)
    {
        if (_isDie == false)
        {
            if (playerWin) PlayAnim(lose, false);
            else PlayAnim(win, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Utils.TAG_STONE))
        {
            OnDie(EDieReason.Normal);
        }
        if (col.gameObject.CompareTag(Utils.TAG_LAVA))
        {
            OnDie(EDieReason.Fire);
        }
    }

    private void OnDestroy()
    {
        Observer.playerEndGameplay2 -= EndGame;
    }
}
