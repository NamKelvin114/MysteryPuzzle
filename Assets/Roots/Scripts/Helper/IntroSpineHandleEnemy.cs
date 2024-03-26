using Spine.Unity;
using UnityEngine;

public class IntroSpineHandleEnemy : SpineHandle
{
    [SerializeField, SpineAnimation] private string runAnimation;
    [SerializeField, SpineAnimation] private string idleAnimation;
    [SerializeField, SpineAnimation] private string arrestAnimation;
    [SerializeField, SpineAnimation] private string jumpAnimation;


    public void PlayIdle() { PlayAnimationWithName(idleAnimation, true); }

    public void PlayMove() { PlayAnimationWithName(runAnimation, true); }

    public void PlayArrest() { PlayAnimationWithName(arrestAnimation, true); }

    public void PlayJump() { PlayAnimationWithName(jumpAnimation); }
}