using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Scene7 : MonoBehaviour
{
    [SerializeField] private GameObject book;
    [SerializeField] private TransScene transScene;
    [SerializeField] private float endScaleBook = 1.2f;
    [SerializeField] private Vector2 endPostionScaleBook;
    [SerializeField, Range(1, 10)] private float timeScale;
    [SerializeField, Range(1, 5)] private float timeToRead;
    private Sequence _sequence;
    private void OnEnable()
    {
        StartCoroutine(WaitToRead());
    }
    IEnumerator WaitToRead()
    {
        yield return new WaitForSeconds(timeToRead);
        //SoundManager.Instance.PlaySound(SoundManager.Instance.introWow);
        DoScaleBook();
    }
    void DoScaleBook()
    {
        _sequence.Append(
        book.transform.DOScale(endScaleBook, timeScale).OnComplete((() =>
        {
            transScene.DoTransScene(Done);
            SoundManager.Instance.PlaySound(SoundManager.Instance.introWow);
        }))).Join(book.GetComponent<RectTransform>().DOAnchorPos(endPostionScaleBook,timeScale));
    }
    void Done()
    {
        Observer.nextIntroScene?.Invoke(this.rectTransform());
    }
}
