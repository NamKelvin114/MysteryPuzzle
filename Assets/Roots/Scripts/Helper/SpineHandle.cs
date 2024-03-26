using Spine.Unity;
using UnityEngine;

/// <summary>
/// control spine
/// </summary>
public class SpineHandle : MonoBehaviour
{
    [SerializeField] protected SkeletonAnimation skeleton;

    public SkeletonAnimation Skeleton => skeleton;

    public void Initialized(SkeletonDataAsset data)
    {
        skeleton.skeletonDataAsset = data;
        skeleton.Initialize(true);
    }

    private float _defaultTimeScale = 1;

    protected void SetLoop(bool isLoop = false) { skeleton.loop = isLoop; }

    protected void SetTimeScale(float timeScale = 1) { skeleton.timeScale = timeScale; }

    public void PlayAnimationWithName(string name, bool isLoop = false, float timeScale = 1, bool forceInit = false)
    {
        SetLoop(isLoop);
        SetTimeScale(timeScale);
        skeleton.AnimationName = name;
        if (forceInit) skeleton.Initialize(true);
    }

    public bool IsPlaying(string name) => skeleton.AnimationName.Equals(name);

    public void Pause()
    {
        _defaultTimeScale = skeleton.timeScale;
        SetTimeScale(0);
    }

    public void Resume() { SetTimeScale(_defaultTimeScale); }

    protected void RegisterEvent() { skeleton.AnimationState.Event += HandleEvent; }

    protected void UnRegisterEvent() { skeleton.AnimationState.Event -= HandleEvent; }

    protected virtual void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e) { }

    /// <summary>
    /// need cache
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected float DurationAnimation(string name)
    {
        var anim = skeleton.Skeleton.Data.FindAnimation(name);
        if (anim != null) return anim.Duration;

        return 0;
    }
}