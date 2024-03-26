using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupHardModeNoti : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;

    private Action _actionBack;
    private Action _actionOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void Initialized(Action actionBack, Action actionOk)
    {
        _actionBack = actionBack;
        _actionOk = actionOk;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed() { _actionBack?.Invoke(); }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed() { _actionOk?.Invoke(); }
}