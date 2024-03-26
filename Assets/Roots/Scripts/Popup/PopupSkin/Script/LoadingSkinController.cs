using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class LoadingSkinController : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private SkinData shirt;
    [SerializeField] private SkinData shoe;
    [SerializeField] private SkinData hat;
    void Start()  
    {
        SetupSkin();
    }

    private void SetupSkin()
    {
        var skeleton = skeletonGraphic.Skeleton;

        var skeletonData = skeleton.Data;
        var mixAndMatchSkin = new Skin("new-skin");
        
        mixAndMatchSkin.AddSkin(skeletonData.FindSkin(shirt.skinName));
        mixAndMatchSkin.AddSkin(skeletonData.FindSkin(hat.skinName));
        
        skeleton.SetSkin(mixAndMatchSkin);
        skeleton.SetSlotsToSetupPose();
    }
}