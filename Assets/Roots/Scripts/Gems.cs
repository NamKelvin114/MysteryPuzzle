using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Worldreaver.UniTween;
using Random = UnityEngine.Random;

public class Gems :Target
{
    private IDisposable _disposable;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="duration"></param>
    public void CollectByPlayer(Transform parent, float duration = 0.5f)
    {
        transform.SetParent(parent, true);
        _disposable = transform.localPosition.Play(Vector3.zero, Easing.Interpolate(Easing.Type.Linear, duration))
            .DoOnCompleted(() =>
            {
                try
                {
                    gameObject.SetActive(false);
                }
                catch (Exception)
                {
                    //
                }
            })
            .SubscribeToLocalPosition(transform)
            .AddTo(this);
    }

    public void Dispose() { _disposable?.Dispose(); }
}