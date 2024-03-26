
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
public class ChooseNamePopup : UniPopupBase
{
    [SerializeField] private InputField inputName;
    [SerializeField] private Transform content;
    [SerializeField] Transform itemChooseCountry;
    [SerializeField] ItemChooseCountry itemHeadDisplay;
    [SerializeField] GameObject tapChooseContry = null;
    [SerializeField] private GameObject connecting;
    [SerializeField] private Text textWarning;
    string _countryCode = "US";
    ItemChooseCountry _itemPrevious = null;
    ItemChooseCountry _itemCurrent = null;

    private Action _actionBack;
    private Action _openPopupAction;

    // [SerializeField] private LeaderboardCountryItem countryItemCurrent;
    public void Initialized(Action actionBack, Action actionOpenPopup)
    {
        _actionBack = actionBack;
        _openPopupAction = actionOpenPopup;
        HideWarning();
        InitData();
        if (!Playfab.isLogin)
        {
            Playfab.Login();
        }
    }
    void InitData()
    {
        var listCodeCoutry = BridgeData.Instance.countryCodes;
        for (int index = 0; index < listCodeCoutry.Length; index++)
        {
            var element = listCodeCoutry[index];
            var nameContry = BridgeData.Instance.GetCountryName(element);
            Transform item;
            if (index < this.content.childCount)
                item = this.content.GetChild(index);
            else
            {
                item = Instantiate(this.itemChooseCountry, this.content);
            }
            var comp = item.GetComponent<ItemChooseCountry>();
            comp.InitInfor(nameContry, element, (nameC, iconC, Item) =>
            {
                this._itemPrevious = this._itemCurrent;
                // if (this._itemPrevious)
                //     this._itemPrevious.deActiveIconCheck();
                this._itemCurrent = Item;

                this.UpdateInfo(nameContry, element);
            });

        }

    }
    public void OnClickOKButton()
    {
        SetConnecting(true);
        if (!Playfab.isLogin)
            Playfab.Login((e) =>
            {
                SetUpDisPlayname();
            }, (e) =>
            {
                ShowWarning("Check your network connection again!");

            });
        else
        {
            SetUpDisPlayname();
        }
    }
    void SetUpDisPlayname()
    {

        if (inputName.text == "")
        {
            ShowWarning("Name can't be empty!");
        }

        if (this.inputName.text == "") return;
        var nameUserCb = this.inputName.text + "_" + this._countryCode;
        Playfab.UpdateDisPlayName(nameUserCb, (ec) =>
        {
            Close();
            var a = ec.DisplayName;
            Playfab.UpdateScoreLevelLB((e) =>
            {
                GamePopup.Instance.ShowLeaderboard();
                // _openPopupAction?.Invoke();
            }, (err) =>
            {
                Debug.Log(err);
            });
            Playfab.UpdateScoreLevelCountryLB();
        }, (err) =>
        {
            ShowWarning("The name you choose already exists!");
        });
    }
    void SetConnecting(bool state)
    {
        connecting.SetActive(state);
    }
    private void ShowWarning(string a)
    {
        SetConnecting(false);
        textWarning.gameObject.SetActive(true);
        textWarning.text = a;
    }
    private void HideWarning()
    {
        SetConnecting(false);
        textWarning.gameObject.SetActive(false);
    }

    public void UpdateInfo(string nameContry, string element)
    {
        this._countryCode = element;
        // this.lbNameCountry.string = nameContry;
        // var snowExpore = I2.Loc.ResourceManager.pInstance.LoadFromResources<UnityEngine.Sprite>("Prefabs/Effect/Snow_Explosion");
        itemHeadDisplay.InitInfor(nameContry, element);
        // ResUtil.getIconCountry(element, (err, img) =>
        // {
        //     if (err) return;
        //     this.iconCountry.spriteFrame = img;
        // });

    }
    public void OnClickOpenTap()
    {
        tapChooseContry.SetActive(!tapChooseContry.active);
    }
    public void OnBackButtonPressed() { _actionBack?.Invoke(); }

}

