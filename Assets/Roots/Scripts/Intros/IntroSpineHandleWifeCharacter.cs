using Spine.Unity;
using UnityEngine;

public class IntroSpineHandleWifeCharacter : SpineHandle
{
    [SerializeField, SpineAnimation] private string runAnimation;
    [SerializeField, SpineAnimation] private string idleAnimation;
    [SerializeField, SpineAnimation] private string happyAnimation;
    [SerializeField, SpineAnimation] private string scareRunAnimation;
    [SerializeField, SpineAnimation] private string scareAnimation;

    public void PlayIdle() { PlayAnimationWithName(idleAnimation, true); }
    public void PlayMove() { PlayAnimationWithName(runAnimation, true); }
    public void PlayHappy() { PlayAnimationWithName(happyAnimation, true); }
    public void PlayScareRun() { PlayAnimationWithName(scareRunAnimation, true); }
    public void PlayScare() { PlayAnimationWithName(scareAnimation, true); }
    public void ChangeSkinIntro() { skeleton.ChangeSkin(Constants.MAIN_INTRO_SKIN); }
    public void ChangeSkinUseForPrison() { skeleton.ChangeSkin(Constants.MAIN_DEFAULT_SKIN); }

    public AudioSource source;
    public AudioClip introWarning;
    public AudioClip introGirlScream;

    public void PlayWarning() { source.PlayOneShot(introWarning); }

    public void PlayGirlScream() { source.PlayOneShot(introGirlScream); }
}