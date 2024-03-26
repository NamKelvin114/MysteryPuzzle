using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene8 : MonoBehaviour
{
    [SerializeField] private AudioClip bgAudio;
    [SerializeField] private AudioClip screamAudio;
    public void PLaySoundBG()
    {
        if (SoundManager.Instance != null && bgAudio != null)
        {
            SoundManager.Instance.StopBGSound();
            SoundManager.Instance.PlayBGCustom(bgAudio);
        }
    }
    public void PLaySoundScream()
    {
        if (SoundManager.Instance != null && screamAudio != null)
        {
            SoundManager.Instance.StopSound();
            SoundManager.Instance.PlaySound(screamAudio);
        }
    }
    // Start is called before the first frame update
    public void EndCutscene8()
    {
        GameManager.instance.CutsceneController.CompletedIntro();
    }
    public void DoTrans()
    {
        GamePopup.Instance.ShowPopupTransition();
    }
    public void EnableSkip()
    {
        GameManager.instance.CutsceneController.skipBtn.gameObject.SetActive(true);
    }
    public void PlaySound()
    {
        StopSound();
        SoundManager.Instance.PlaySound(GameManager.instance.CutsceneController.startByPlane);
    }
    public void StopSound()
    {
        SoundManager.Instance.StopSound();
    }
}
