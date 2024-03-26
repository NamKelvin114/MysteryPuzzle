using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Scene3 : MonoBehaviour
{
    [SerializeField] private GameObject book;
    [SerializeField] private TransScene transScene;
    [SerializeField] private float endScaleBook = 2.5f;
    [SerializeField] private Vector2 endPostionScaleBook;
    [SerializeField] private float distanceToMove = 1800f;
    [SerializeField, Range(1, 10)] private float timeScale;
    [SerializeField, Range(1, 5)] private float timeToRead;
    [SerializeField, Range(1, 5)] private float timeToEnd;
    private Sequence _sequence;
    private void OnEnable()
    {
        StartCoroutine(WaitToRead());
    }
    IEnumerator WaitToRead()
    {
        yield return new WaitForSeconds(timeToRead);
        DoScaleBook();
    }
    void DoScaleBook()
    {
        book.rectTransform().DOLocalMoveX(book.rectTransform().localPosition.x - distanceToMove, 2).SetEase(Ease.Linear)
                    .OnComplete((() =>
                    {
                        StartCoroutine(WaitToEnd());
                    }));
    }
    IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(timeToEnd);
        transScene.DoTransScene(Done);
    }
    void Done()
    {
        Observer.nextIntroScene?.Invoke(this.rectTransform());
    }
}
