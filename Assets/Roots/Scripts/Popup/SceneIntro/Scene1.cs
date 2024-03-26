using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;

public class Scene1 : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic mainGirl;
    [SerializeField, SpineAnimation] private string sweepAnim;
    [SerializeField, SpineAnimation] private string openAnim;
    [SerializeField] private TransScene transScene1;
    [SerializeField, Range(0, 2)] private float durationsTransMainGirl;
    [SerializeField, Range(0, 1)] float animTimeScale;
    [SerializeField, Range(0, 1)] private float holdingTime;
    [SerializeField, Range(0, 2)] private float timeToPlaySound;
    private float value = 1f;
    private Color _color;
    private void OnEnable()
    {
        _color = new Color();
        _color = mainGirl.color;
        
        this.StopAllCoroutines();
        this.StartCoroutine(PlayAnim((() =>
        {
            SoundManager.Instance.audioSource.Stop();
            this.StartCoroutine(PlaySound());
            this.StartCoroutine(PlayAnim((() =>
            {
                this.StartCoroutine(WaitDoneAnimation());
            }), openAnim));
        }), sweepAnim));
        SoundManager.Instance.PlaySound(SoundManager.Instance.chillSweep);
    }
    IEnumerator WaitDoneAnimation()
    {
        yield return new WaitForSeconds(holdingTime);
        // DOTween.To(() => value, x => value = x, .1f, durationsTransMainGirl).OnUpdate((() =>
        // {
        //     _color.a = value;
        //     mainGirl.color = _color;
        // }));
        transScene1.DoTransScene(Done);
    }
    void Done()
    {
        Observer.nextIntroScene?.Invoke(this.rectTransform());
    }
    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(timeToPlaySound);
        SoundManager.Instance.PlaySound(SoundManager.Instance.confused);
    }
    IEnumerator PlayAnim(Action doneAction, string animation)
    {
        while (!mainGirl.IsValid)
        {
            yield return null;
        }
        var doAnim = mainGirl.AnimationState.SetAnimation(0, animation, false);
        doAnim.TimeScale = animTimeScale;
        yield return new WaitForSpineAnimationComplete(doAnim);
        doneAction?.Invoke();
    }
}
