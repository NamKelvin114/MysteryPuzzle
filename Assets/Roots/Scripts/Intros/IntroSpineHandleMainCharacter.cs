using Spine.Unity;
using UnityEngine;

public class IntroSpineHandleMainCharacter : SpineHandle
{
    [SerializeField, SpineAnimation] private string runAnimation;
    [SerializeField, SpineAnimation] private string idleAnimation;
    [SerializeField, SpineAnimation] private string thiefAnimation;
    [SerializeField, SpineAnimation] private string jumpAnimation;
    [SerializeField, SpineAnimation] private string frightenAnimation;
    [Space] [SerializeField, SpineAnimation] private string cryAnimation;
    [SerializeField, SpineAnimation] private string thinkAnimation;


    public void PlayIdle() { PlayAnimationWithName(idleAnimation, true); }
    public void PlayMove() { PlayAnimationWithName(runAnimation, true); }
    public void PlayThief() { PlayAnimationWithName(thiefAnimation); }
    public void PlayJump() { PlayAnimationWithName(jumpAnimation); }
    public void PlayFrighten() { PlayAnimationWithName(frightenAnimation, true); }

    public void PlayCry() { PlayAnimationWithName(cryAnimation); }
    public void PlayGotIdeal() { PlayAnimationWithName(thinkAnimation, true); }

    public void ChangeSkinIntro() { skeleton.ChangeSkin(Constants.MAIN_INTRO_SKIN); }

    public void ChangeSkinUseForPrison() { skeleton.ChangeSkin(Constants.MAIN_DEFAULT_SKIN); }

    #region sound

    public AudioSource source;
    public AudioClip introManCry;
    public AudioClip introManLamp;
    public AudioClip introPoliceTalk;
    
    public void PlayManCry() { source.PlayOneShot(introManCry); }
    public void PlayManLamp() { source.PlayOneShot(introManLamp); }
    public void PlayPoliceTalk() { source.PlayOneShot(introPoliceTalk); }

    #endregion
}