using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemChooseCountry : MonoBehaviour
{
    [SerializeField] Image iconCountry;
    [SerializeField] Text textNameCountry;
    string nameCountry = "";
    string iconC = "";
    Action<string, string, ItemChooseCountry> chooseCountryClickCallback;

    public void InitInfor(string _name, string _iconCode, Action<string, string, ItemChooseCountry> action = null)
    {
        chooseCountryClickCallback = action;

        iconC = _iconCode;
        var sprite = I2.Loc.ResourceManager.pInstance.LoadFromResources<UnityEngine.Sprite>("Img/CountryIcon/" + iconC);
        iconCountry.sprite = sprite;

        nameCountry = _name;
        textNameCountry.text = nameCountry;
    }
    public void onClickChooseCountry()
    {
        chooseCountryClickCallback?.Invoke(nameCountry, iconC, this);
    }
}

