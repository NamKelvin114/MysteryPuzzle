using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene5 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioClip cutSceneSound;

    public void EndCutscene5()
    {
        GameManager.instance.CutsceneController.EndCutScene5();
        GameManager.instance.CutsceneController.CompletedIntro();
    }

    public void PlaySound()
    {
        if (SoundManager.Instance != null && cutSceneSound != null)
        {
            SoundManager.Instance.StopSound();
            SoundManager.Instance.StopBGSound();
            SoundManager.Instance.PlayBGCustom(cutSceneSound);
        }
    }

    public void StopSound()
    {
        SoundManager.Instance.StopBGSound();
    }

    public void EnableSkip()
    {
        GameManager.instance.CutsceneController.skipBtn.gameObject.SetActive(true);
    }

    public void DoTrans()
    {
        GamePopup.Instance.ShowPopupTransition();
    }
}