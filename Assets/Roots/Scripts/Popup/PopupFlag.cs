using System;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupFlag : UniPopupBase
{
    [SerializeField] private FlagScrollController scroller;
    [SerializeField] private Transform container;

    private Action _action;

    public void Initialize(Action action)
    {
        Utils.tempCountryCode = Data.UserCountryCode;
        _action = action;
    }

    private void OnEnable()
    {
        container.Clear();
        scroller.ForeInitialized();
    }

    public void OnOkButtonPresed()
    {
        Data.UserCountryCode = Utils.tempCountryCode;
        var popup = (PopupLogin) GamePopup.Instance.popupLoginHandler;

        var icon = scroller.CountryCode.GetIcon(BridgeData.Instance.GetCountryCode());
        var nameCountry = BridgeData.Instance.GetCountryName(BridgeData.Instance.GetCountryCode());
        if (popup != null)
        {
            popup.IconCountry.sprite = icon;
            popup.CountryName.text = nameCountry;
        }

        _action?.Invoke();
    }
}