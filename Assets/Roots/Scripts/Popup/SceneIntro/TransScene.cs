using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class TransScene : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    private const int maxA = 0;
    private float valueChange = maxA;
    private Color _setColor;
    [SerializeField, Range(0, 2)] private float durations;
    private void OnEnable()
    {
        _setColor = new Color();
        _setColor = bgImage.color;
    }
    // Start is called before the first frame update
    public void DoTransScene(Action doneAction)
    {
        SoundManager.Instance.audioSource.Stop();
        DOTween.To(() => valueChange, x => valueChange = x, 1f, durations).OnUpdate((() =>
        {
            _setColor.a = valueChange;
            bgImage.color = _setColor;
        })).OnComplete((() =>
        {
            doneAction?.Invoke();
        }));
    }

}
