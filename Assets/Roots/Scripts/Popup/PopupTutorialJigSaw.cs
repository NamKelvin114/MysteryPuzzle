using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake.UI;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupTutorialJigSaw : UniPopupBase
{
    private Action _actionBack;
    [SerializeField] private GameObject HandTur;
    [SerializeField, Range(0, 5)] private float timeToHold;
    private float _timeToHold;
    private bool _isAction;
    [SerializeField] private Transform pos1;
    [SerializeField] GraphicRaycaster raycaster;
    [SerializeField] private Transform pos2;
    [SerializeField] private float moveDuration = 1;
    public void Initialized(Action getActionBack)
    {
        Observer.HidePopupTutorialJigSaw += Hide;
        Observer.correctPeacePosi += SetPos2;
        raycaster.enabled = true;
        HandTur.gameObject.SetActive(false);
        _isAction = true;
        _timeToHold = timeToHold;
        _actionBack = getActionBack;
    }
    void SetPos2(Vector3 pos)
    {
        pos2.position = pos;
    }
    private void OnDisable()
    {
        Observer.HidePopupTutorialJigSaw -= Hide;
        Observer.correctPeacePosi -= SetPos2;
    }
    private void Update()
    {
        if (_timeToHold > 0)
        {
            _timeToHold -= Time.deltaTime;
        }
        else
        {
            if (_isAction)
            {
                _isAction = false;
                DoTutorialJigSaw();
            }
        }
    }
    void DoTutorialJigSaw()
    {
        raycaster.enabled = false;
        HandTur.gameObject.SetActive(true);
        HandTur.transform.position = pos1.position;
        HandTur.transform.DOMove(pos2.position, moveDuration).SetEase(Ease.Linear).OnComplete((() =>
        {
            DoTutorialJigSaw();
        }));
    }

    public void Hide()
    {
        HandTur.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
