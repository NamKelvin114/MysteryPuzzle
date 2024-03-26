using System;
using TMPro;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupNotification : UniPopupBase
{
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private TextMeshProUGUI txtBtnOk;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private UniButton btnBack;

    private Action _actionOk;
    private Action _actionBack;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionOk"></param>
    /// <param name="actionBack"></param>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="nameBtnOk"></param>
    /// <param name="hasBackButton"></param>
    public void Initialized(Action actionOk, Action actionBack, string message, string title = "", string nameBtnOk = "OK", bool hasBackButton = false)
    {
        _actionOk = actionOk;
        _actionBack = actionBack;
        txtMessage.text = message;
        if (!string.IsNullOrEmpty(title)) txtTitle.text = title;
        if (string.IsNullOrEmpty(nameBtnOk)) txtBtnOk.text = nameBtnOk;
        btnOk.onClick.RemoveListener(OnOkButtonPressed);
        btnOk.onClick.AddListener(OnOkButtonPressed);

        btnBack.gameObject.SetActive(hasBackButton);
        if (hasBackButton)
        {
            btnBack.onClick.RemoveListener(OnBackButtonPressed);
            btnBack.onClick.AddListener(OnBackButtonPressed);
        }
    }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed() { _actionOk?.Invoke(); }

    /// <summary>
    /// 
    /// </summary>
    private void OnBackButtonPressed() { _actionBack?.Invoke(); }
}