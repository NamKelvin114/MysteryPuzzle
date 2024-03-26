using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene6 : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;
    public void DoTrans()
    {
        GamePopup.Instance.ShowPopupTransition();
    }
    public void PlaySound()
    {
        SoundManager.Instance.StopSound();
        SoundManager.Instance.PlaySound(startSound);
    }
    public void EndScene()
    {
        GameManager.instance.CutsceneController.CompletedIntro();
    }
}
