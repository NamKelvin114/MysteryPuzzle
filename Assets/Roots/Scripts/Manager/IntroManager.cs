using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroManager : MonoBehaviour
{
    [SerializeField] private AudioClip mainIdle;
    [SerializeField] private AudioClip mainWhat;
    [SerializeField] private AudioClip mainWow;
    [SerializeField] private GameObject btnSkip;

    private void Start()
    {
        Invoke("ActiveSkip", 1f);
    }

    void ActiveSkip()
    {
        btnSkip.SetActive(true);
    }
    public void CompletedIntro()
    {
        GamePopup.Instance.ShowPopupTransition();
        SoundManager.Instance.PlayBackgroundMusic();
        Utils.IsFirstTimePLay = false;
        //SoundManager.Instance.PlayStartLevelSound(MapLevelManager.Instance.ESoundStartLevel);
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
    }

    public void PlayBackgroundIntro()
    {
        SoundManager.Instance.PlayBackgroundIntroMusic();
    }

    public void PlaySoundMainIdle()
    {
        SoundManager.Instance.PlaySound(mainIdle);
    }
    public void PlaySoundMainWhat()
    {
        SoundManager.Instance.StopSound();
        SoundManager.Instance.PlaySound(mainWhat);
    }
    public void PlaySoundMainWow()
    {
        SoundManager.Instance.PlaySound(mainWow);
    }
}
