using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Scene4 : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic mainGirl;
    [SerializeField] private float durationToMove;
    [SerializeField, Range(0, 2)] private float durationsTransMainGirl;
    [SerializeField] private float delayTimeFly;
    [SerializeField] private float timeFly;
    [SerializeField] private GameObject airPlane;
    [SerializeField, SpineAnimation] private string walkAnimVali;
    [SerializeField] private TransScene transScene1;
    private float value = 1f;
    private Color _color;
    private void OnEnable()
    {
        _color = new Color();
        _color = mainGirl.color;
        DoGirlWalk();
    }
    void DoGirlWalk()
    {
        mainGirl.rectTransform().DOLocalMoveX(-mainGirl.rectTransform().localPosition.x, durationToMove).SetEase(Ease.Linear).OnStart((() =>
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.intro4);
            StartCoroutine(WaitToPlane());
            mainGirl.AnimationState.SetAnimation(0, walkAnimVali, true);
        })).OnComplete((() =>
        {
            mainGirl.rectTransform().DOLocalMoveX(mainGirl.rectTransform().localPosition.x + 1000, durationToMove).SetEase(Ease.Linear);
            transScene1.DoTransScene(Done);
        }));
    }
    void Done()
    {
        Observer.nextIntroScene?.Invoke(this.rectTransform());
    }
    IEnumerator WaitToPlane()
    {
        yield return new WaitForSeconds(delayTimeFly);
        airPlane.rectTransform().DOLocalMoveX(airPlane.rectTransform().localPosition.x + 3000, timeFly);
    }
}
