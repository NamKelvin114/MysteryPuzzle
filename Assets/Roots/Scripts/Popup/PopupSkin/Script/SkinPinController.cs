using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SkinPinController : MonoBehaviour
{
    [SerializeField] private Image imagePin;
    [SerializeField] private SkinDataResources skinResources;
    [SerializeField] private SpriteRenderer spritePin;
    [SerializeField] private ETypePinModel eTypePinModel;

    void OnEnable()
    {
        SetupDefaultSkin();
        Observer.CurrentSkinPinChanged += SetupSkin;
        //Observer.UnlockSkin += UnlockSkin;
        Observer.ShowSkinPin += ShowSkin;
        Observer.UseSkinPin += UseSkin;
    }

    private void OnDisable()
    {
        Observer.CurrentSkinPinChanged -= SetupSkin;
        //Observer.UnlockSkin -= UnlockSkin;
        Observer.ShowSkinPin -= ShowSkin;
        Observer.UseSkinPin -= UseSkin;
    }


    public void ShowSkin(string skinName)
    {
        var pinSkin = skinResources.skinDataList;
        if (skinName != "")
        {
            foreach (var skin in pinSkin)
            {
                if (skinName == skin.skinNamePin)
                {
                    if (spritePin != null) spritePin.sprite = skin.spritePin;
                    if (imagePin != null) imagePin.sprite = skin.spritePin;
                    if (imagePin != null && eTypePinModel == ETypePinModel.Shop) imagePin.sprite = skin.shopIcon;
                }
            }
        }
    }

    public void UseSkin(string skinName)
    {
        var pinSkin = skinResources.skinDataList;
        if (skinName != "")
        {
            skinResources.CurrentSkin = skinName;
            foreach (var skin in pinSkin)
            {
                if (skinName == skin.skinNamePin)
                {
                    if (spritePin != null) spritePin.sprite = skin.spritePin;
                    if (imagePin != null) imagePin.sprite = skin.spritePin;
                    if (imagePin != null && eTypePinModel == ETypePinModel.Shop) imagePin.sprite = skin.shopIcon;
                }
            }
        }
    }

    private void SetupSkin()
    {
        var pinSkin = skinResources.skinDataList;

        var currentSkinName = skinResources.CurrentSkin;
        if (currentSkinName != "")
        {
            foreach (var skin in pinSkin)
            {
                if (currentSkinName == skin.skinNamePin)
                {
                    if (spritePin != null) spritePin.sprite = skin.spritePin;
                    if (imagePin != null) imagePin.sprite = skin.spritePin;
                    if (imagePin != null && eTypePinModel == ETypePinModel.Shop) imagePin.sprite = skin.shopIcon;
                }
            }
        }
    }

    private void SetupDefaultSkin()
    {
        var pinSkin = skinResources.skinDataList;
        var currentSkinName = skinResources.CurrentSkin;
        if (currentSkinName == "")
        {
            foreach (var skin in pinSkin)
            {
                if (skin.skinBuyType == SkinBuyType.Default)
                {
                    skinResources.CurrentSkin = skin.skinNamePin;
                    break;
                }
            }
        }

        SetupSkin();
    }
}

public enum ETypePinModel
{
    None,
    Shop
}