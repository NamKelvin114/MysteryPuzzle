//#if LANCE_SPINE_SUPPORT

using Spine.Unity;
using UnityEngine;

public static class SkeletonExtension
{
    /// <summary>
    ///  change skin spine
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="nameSkin"></param>
    public static void ChangeSkin(this SkeletonAnimation skeleton, string nameSkin)
    {
        var ske = skeleton.Skeleton;
        var newSkin = new Spine.Skin("new_skin");
        newSkin.AddSkin(ske.Data.FindSkin(nameSkin));
        ske.SetSkin(newSkin);
        ske.SetSlotsToSetupPose();
        skeleton.AnimationState.Apply(ske);
        skeleton.LateUpdate();
    }

    /// <summary>
    ///  change skin spineu
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="nameSkin"></param>
    public static void ChangeSkin(this SkeletonGraphic skeleton, string nameSkin)
    {
        var ske = skeleton.Skeleton;
        var newSkin = new Spine.Skin("new_skin");
        newSkin.AddSkin(ske.Data.FindSkin(nameSkin));
        ske.SetSkin(newSkin);
        ske.SetSlotsToSetupPose();
        skeleton.AnimationState.Apply(ske);
        skeleton.LateUpdate();

        // skeleton.Skeleton.SetSkin(newSkin);
        // skeleton.Skeleton.SetSlotsToSetupPose();
        // ske.Update(0);
        // skeleton.LateUpdate();
    }

    /// <summary>
    /// need cache
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static float DurationAnimation(this SkeletonAnimation skeleton, string name)
    {
        var anim = skeleton.Skeleton.Data.FindAnimation(name);
        if (anim != null) return anim.Duration;

        return 0;
    }
}
//#endif