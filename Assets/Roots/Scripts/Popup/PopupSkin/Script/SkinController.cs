using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SkinController : MonoBehaviour
{
    [SerializeField] private SkinType skinType;
    [SerializeField] private SkinResources skinResources;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private List<SkinData> extraSkinList;

    void OnEnable()
    {
        SetupDefaultSkin();
        Observer.CurrentSkinChanged += SetupSkin;
        //Observer.UnlockSkin += UnlockSkin;
        Observer.ShowSkin += ShowSkin;
        Observer.UseSkin += UseSkin;
    }

    private void OnDisable()
    {
        Observer.CurrentSkinChanged -= SetupSkin;
        //Observer.UnlockSkin -= UnlockSkin;
        Observer.ShowSkin -= ShowSkin;
        Observer.UseSkin -= UseSkin;
    }


    public void ShowSkin(string skinName)
    {
        Skeleton skeleton;
        if (skeletonGraphic != null)
            skeleton = skeletonGraphic.Skeleton;
        else
            skeleton = skeletonAnimation.Skeleton;
        var skeletonData = skeleton.Data;
        var mixAndMatchSkin = new Skin("new-skin");
        var clothesSkin = skinResources.skinDataResourcesList.Find(item => item.skinItemType == SkinItemType.Shirt);
        var otherSkinNotPin =
            skinResources.skinDataResourcesList.FindAll(item => item.skinItemType != SkinItemType.Pin);
        var otherSkin = otherSkinNotPin.FindAll(item => item.skinItemType != SkinItemType.Shirt);
        bool isShowed = false;
        if (clothesSkin.Contains(skinName))
        {
            mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinName));
            isShowed = true;
        }
        else
        {
            var currentSkinName = clothesSkin.CurrentSkin;
            if (currentSkinName != "")
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(currentSkinName));
        }

        foreach (var skinDataResources in otherSkin)
        {
            if (skinDataResources.Contains(skinName))
            {
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinName));
                isShowed = true;
            }
            else
            {
                var currentSkinName = skinDataResources.CurrentSkin;
                if (currentSkinName != "")
                    mixAndMatchSkin.AddSkin(skeletonData.FindSkin(currentSkinName));
            }
        }

        if (isShowed == false)
        {
            mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinName));
        }

        foreach (var skin in extraSkinList)
        {
            if (skin.IsUnlocked)
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skin.skinName));
        }

        skeleton.SetSkin(mixAndMatchSkin);
        skeleton.SetSlotsToSetupPose();
    }

    public void UseSkin(string skinName, SkinType _skinType = SkinType.None)
    {
        //Debug.Log(skinName + _skinType);
        if (_skinType != SkinType.None)
        {
            if (_skinType != skinType)
            {
                return;
            }
        }

        Skeleton skeleton;
        if (skeletonGraphic != null)
            skeleton = skeletonGraphic.Skeleton;
        else
            skeleton = skeletonAnimation.Skeleton;
        var skeletonData = skeleton.Data;
        var mixAndMatchSkin = new Skin("new-skin");
        var clothesSkin = skinResources.skinDataResourcesList.Find(item => item.skinItemType == SkinItemType.Shirt);
        var otherSkinNotPin =
            skinResources.skinDataResourcesList.FindAll(item => item.skinItemType != SkinItemType.Pin);
        var otherSkin = otherSkinNotPin.FindAll(item => item.skinItemType != SkinItemType.Shirt);
        bool isShowed = false;
        if (clothesSkin.Contains(skinName))
        {
            mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinName));
            clothesSkin.CurrentSkin = skinName;
            isShowed = true;
        }
        else
        {
            var currentSkinName = clothesSkin.CurrentSkin;
            if (currentSkinName != "")
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(currentSkinName));
        }

        foreach (var skinDataResources in otherSkin)
        {
            if (skinDataResources.Contains(skinName))
            {
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinName));
                skinDataResources.CurrentSkin = skinName;
                isShowed = true;
            }
            else
            {
                var currentSkinName = skinDataResources.CurrentSkin;
                if (currentSkinName != "")
                    mixAndMatchSkin.AddSkin(skeletonData.FindSkin(currentSkinName));
            }
        }

        if (isShowed == false)
        {
            mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skinName));
        }

        foreach (var skin in extraSkinList)
        {
            if (skin.IsUnlocked)
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skin.skinName));
        }

        skeleton.SetSkin(mixAndMatchSkin);
        skeleton.SetSlotsToSetupPose();
    }

    private void SetupSkin()
    {
        Skeleton skeleton;
        if (skeletonGraphic != null)
            skeleton = skeletonGraphic.Skeleton;
        else
            skeleton = skeletonAnimation.Skeleton;
        var skeletonData = skeleton.Data;
        var mixAndMatchSkin = new Skin("new-skin");
        var clothesSkin = skinResources.skinDataResourcesList.Find(item => item.skinItemType == SkinItemType.Shirt);
        var otherSkinNotPin =
            skinResources.skinDataResourcesList.FindAll(item => item.skinItemType != SkinItemType.Pin);
        var otherSkin = otherSkinNotPin.FindAll(item => item.skinItemType != SkinItemType.Shirt);

        var currentSkinName = clothesSkin.CurrentSkin;
        if (currentSkinName != "")
            mixAndMatchSkin.AddSkin(skeletonData.FindSkin(currentSkinName));

        foreach (var skinDataResources in otherSkin)
        {
            currentSkinName = skinDataResources.CurrentSkin;
            Debug.Log(currentSkinName);
            if (currentSkinName != "")
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(currentSkinName));
        }

        foreach (var skin in extraSkinList)
        {
            if (skin.IsUnlocked)
                mixAndMatchSkin.AddSkin(skeletonData.FindSkin(skin.skinName));
        }

        skeleton.SetSkin(mixAndMatchSkin);
        skeleton.SetSlotsToSetupPose();
    }

    private void SetupDefaultSkin()
    {
        var clothesSkin = skinResources.skinDataResourcesList.Find(item => item.skinItemType == SkinItemType.Shirt);
        var otherSkinNotPin =
            skinResources.skinDataResourcesList.FindAll(item => item.skinItemType != SkinItemType.Pin);
        var otherSkin = otherSkinNotPin.FindAll(item => item.skinItemType != SkinItemType.Shirt);
        var currentSkinName = clothesSkin.CurrentSkin;
        if (currentSkinName == "")
        {
            foreach (var skin in clothesSkin.skinDataList)
            {
                if (skin.skinBuyType == SkinBuyType.Default)
                {
                    clothesSkin.CurrentSkin = skin.skinName;
                    break;
                }
            }
        }

        foreach (var skinDataResources in otherSkin)
        {
            currentSkinName = skinDataResources.CurrentSkin;
            if (currentSkinName == "")
            {
                foreach (var skin in skinDataResources.skinDataList)
                {
                    if (skin.skinBuyType == SkinBuyType.Default)
                    {
                        skinDataResources.CurrentSkin = skin.skinName;
                        break;
                    }
                }
            }
        }

        SetupSkin();
    }
}