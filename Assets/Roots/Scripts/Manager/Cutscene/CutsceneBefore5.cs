using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneBefore5 : MonoBehaviour
{
    [SerializeField] private AudioClip planeFly;
    public void PlaySoundFirstScene1()
    {
        SoundManager.Instance.StopSound();
        SoundManager.Instance.StopBGSound();
        SoundManager.Instance.PlaySound(planeFly);
    }
    public void PlaySoundSecondScene2()
    {
        SoundManager.Instance.StopBGSound();
        SoundManager.Instance.PLayBackGroundMusicSceneBefore5();
    }
    public void DoTrans()
    {
        GamePopup.Instance.ShowPopupTransition();
    }
    public void EndCutSceneBefore5()
    {
        GameManager.instance.CutsceneController.CompletedIntro();
    }
    public void StopBGSound()
    {
        SoundManager.Instance.StopBGSound();
    }
    public void EnableSkip()
    {
        GameManager.instance.CutsceneController.skipBtn.gameObject.SetActive(true);
    }
    public void StopSound()
    {
        SoundManager.Instance.StopSound();
    }
}
