using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using PlayFab.ClientModels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using DG.Tweening;

public class PopupCollectionTutorial : UniPopupBase
{
    [SerializeField] private GameObject txtContinue;
    [SerializeField] private Image bg;
    private Action _actionBack;

    public void Initialized(Action actionBack)
    {
        _actionBack = actionBack;
        bg.DOFade(1, 0.5f);
        txtContinue.SetActive(true);
    }

    public void OnCLickTapToContinue()
    {
        _actionBack?.Invoke();
        MenuController.instance.OnStartLevelButtonPressed();
    }

    private void PlaySoundButtonClicked()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
    }

    private void OnDisable()
    {
        bg.DOFade(100/255, 0f);
        txtContinue.SetActive(false);
    }

}
