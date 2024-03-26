using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CamFollowUI : MonoBehaviour
{
    public CharacterGamePlayBlock character;
    [SerializeField] private float posRectY;
    [SerializeField] private float sizeToScale;
    public GameObject fade;

    private void OnEnable()
    {
        Observer.StartWinFollow += FollowPlayer;
    }

    private void OnDisable()
    {
        Observer.StartWinFollow -= FollowPlayer;
    }

    public void FollowPlayer()
    {
        var rectransform = this.gameObject.GetComponent<RectTransform>();
        fade.SetActive(true);
        float zoomDuration = 1.0f;
        var newPos = (rectransform.localPosition - character.GetComponent<RectTransform>().localPosition) * sizeToScale;
        Sequence sequenceMove = DOTween.Sequence();
        sequenceMove.Append(transform.DOScale(new Vector3(sizeToScale, sizeToScale, sizeToScale), zoomDuration)).
            Join(rectransform.DOLocalMove(new Vector3(newPos.x +(28 * sizeToScale), newPos.y + (350 * sizeToScale), newPos.z), zoomDuration));
        // float limitSize = Mathf.Min(Screen.height - 500 - 600, 0.3f * Screen.height);
        // var zoomObj = character.zoomPos;
        // float height = zoomObj.GetComponent<RectTransform>().rect.height;
        // float scale = height / limitSize;
        // transform.DOScale(new Vector3(scale, scale, scale), zoomDuration).SetEase(Ease.Linear);
        // Vector3 newPos = new Vector3(
        //     transform.position.x + (transform.position.x - zoomObj.transform.position.x) * scale,
        //     transform.position.y + (transform.position.y - zoomObj.transform.position.y) * scale, transform.position.z);
        // transform.DOMove(newPos, zoomDuration).SetEase(Ease.Linear);
    }
}
