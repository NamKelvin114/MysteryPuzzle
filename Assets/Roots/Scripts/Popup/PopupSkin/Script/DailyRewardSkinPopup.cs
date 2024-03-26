using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Spine.Unity;
using UnityEngine;
using TMPro;

public class DailyRewardSkinPopup : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic _player;
    [SerializeField, SpineAnimation(dataField = "ske")]
    private string _actionIdle;
    [SerializeField] private SkinController _skinController;
    [SerializeField] private GameObject _coin;
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private GameObject _newSkinText;

    private bool _isUseCoin = false;
 
    private SkinData _skinData;
    public SkinData SkinData { get => _skinData; set => _skinData = value; }

    private int _coinValue;

    public void SetupSkin()
    {
        _newSkinText.SetActive(true);
        _coin.SetActive(false);
        _skinController.gameObject.SetActive(true);
        
        //_skinController.ShowSkin(_skinData.skinName);
        _skinController.UseSkin(_skinData.skinName, SkinType.GirlSkin);
        GamePopup.Instance.HidePopupMoney();
    }

    public void SetupCoin(int coin, bool isUseCoin)
    {
        _newSkinText.SetActive(false);
        _coin.SetActive(true);
        _skinController.gameObject.SetActive(false);
        
        _coinText.SetText(coin.ToString());
        _coinValue = coin;

        _isUseCoin = true;
        GamePopup.Instance.HidePopupMoney();
    }

    public void UseSkin()
    {
        //_skinController.UseSkin(_skinData.skinName, SkinType.GirlSkin);
        if (!_isUseCoin)
        {
            _skinData.IsUnlocked = true;
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("test claim coin");
            Utils.UpdateCoin(_coinValue);
            Observer.CurrencyTotalChanged?.Invoke(true);
            this.gameObject.SetActive(false);
        }
        
        GamePopup.Instance.ShowPopupMoney();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.claimGift);
    }
}
