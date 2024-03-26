using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupUpdate : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private TextMeshProUGUI txtUpdateDescription;
    [SerializeField] private TextMeshProUGUI txtNewVersion;
    [SerializeField] private Toggle toggleDontShow;

    private Action _actionBack;
    private Action _actionOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updateDescription"></param>
    /// <param name="newVersion"></param>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void Initialized(string updateDescription, string newVersion, Action actionBack, Action actionOk)
    {
        _actionBack = actionBack;
        _actionOk = actionOk;

        updateDescription = updateDescription.Replace("\\n", "\n");
        txtNewVersion.text = $"New Version {newVersion.Replace('_', '.')}";
        txtUpdateDescription.SetText(updateDescription);
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed()
    {
        _actionBack?.Invoke();
        if (toggleDontShow.isOn)
        {
            Data.DontShowUpdateAgain = true;
            Data.DontShowUpdate = true;
        }
    }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed()
    {
        _actionOk?.Invoke();
        Utils.GotoStore();
        if (toggleDontShow.isOn)
        {
            Data.DontShowUpdateAgain = true;
            Data.DontShowUpdate = true;
        }
    }
}