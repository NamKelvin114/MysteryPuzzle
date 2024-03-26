using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake.Monetization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject cutscene2;
    [SerializeField] private GameObject cutscene1;
    [SerializeField] private GameObject cutscene3;
    [SerializeField] private GameObject cutscene4;
    [SerializeField] private GameObject cutscene5Before;
    [SerializeField] private GameObject cutscene5;
    [SerializeField] private GameObject cutscene6;
    [SerializeField] private GameObject cutscene7;
    [SerializeField] private GameObject cutscene8;
    [SerializeField] private GameObject cutscene9;
    [SerializeField] public GameObject skipBtn;
    [Header("sound")] public AudioClip startByPlane;
    [SerializeField] private AudioClip accidentPlane;
    [SerializeField] private AudioClip runFromNative;

    public void EndCutscene2()
    {
        GameManager.instance.isShowEvent = true;
        cutscene1.gameObject.SetActive(true);
        cutscene2.gameObject.SetActive(false);
        SoundManager.Instance.StopSound();
        SoundManager.Instance.PlaySound(accidentPlane);
    }

    public void PlayCutscene2()
    {
        GameManager.instance.isShowEvent = true;
        cutscene2.gameObject.SetActive(true);
        SoundManager.Instance.StopSound();
        SoundManager.Instance.StopBGSound();
        DOTween.Sequence().AppendInterval(1f).OnComplete(() => { skipBtn.gameObject.SetActive(true); });
    }

    public void PlayCutscene3()
    {
        GameManager.instance.isShowEvent = true;
        cutscene3.gameObject.SetActive(true);
    }

    public void CompletedIntro()
    {
        //GamePopup.Instance.ShowPopupTransition();
        GameManager.instance.isShowEvent = false;
        SoundManager.Instance.StopSound();
        SoundManager.Instance.PlayBackgroundMusic();
        SoundManager.Instance.PlayStartLevelSound(MapLevelManager.Instance.ESoundStartLevel);
        cutscene1.gameObject.SetActive(false);
        cutscene2.gameObject.SetActive(false);
        cutscene3.gameObject.SetActive(false);
        cutscene4.gameObject.SetActive(false);
        cutscene5.gameObject.SetActive(false);
        cutscene6.gameObject.SetActive(false);
        cutscene7.gameObject.SetActive(false);
        cutscene8.gameObject.SetActive(false);
        cutscene9.gameObject.SetActive(false);
        cutscene5Before.SetActive(false);
        skipBtn.gameObject.SetActive(false);
        ShowBannerAdsWhenEndCutScene();
    }

    public void PlayCutscene4()
    {
        GameManager.instance.isShowEvent = true;
        cutscene4.gameObject.SetActive(true);
        SoundManager.Instance.StopBGSound();
        SoundManager.Instance.PlaySound(runFromNative);
    }

    public void PlayCutsceneBefore5()
    {
        GameManager.instance.isShowEvent = true;
        cutscene5Before.gameObject.SetActive(true);
    }

    public void PlayCutscene7()
    {
        GameManager.instance.isShowEvent = true;
        SoundManager.Instance.StopSound();
        SoundManager.Instance.StopBGSound();
        SoundManager.Instance.PlaySound(runFromNative);
        cutscene7.gameObject.SetActive(true);
    }
    public void PlayCutscene8()
    {
        GameManager.instance.isShowEvent = true;

        SoundManager.Instance.StopSound();
        SoundManager.Instance.StopBGSound();
        cutscene8.gameObject.SetActive(true);
    }
    public void PlayCutscene9()
    {
        GameManager.instance.isShowEvent = true;

        SoundManager.Instance.StopSound();
        SoundManager.Instance.StopBGSound();
        cutscene9.gameObject.SetActive(true);
    }



    public void PlayCutscene5()
    {
        GameManager.instance.isShowEvent = true;
        cutscene5.gameObject.SetActive(true);
    }

    public void EndCutsceneBefore5()
    {
        // skipBtn.gameObject.SetActive(true);
        GameManager.instance.isShowEvent = true;
        cutscene5Before.gameObject.SetActive(false);
        cutscene5.gameObject.SetActive(true);
    }

    public void PlayCutScene6()
    {
        GameManager.instance.isShowEvent = true;
        cutscene6.gameObject.SetActive(true);
    }

    public void EndCutScene5()
    {
        GameManager.instance.isShowEvent = false;
        cutscene5.gameObject.SetActive(false);
        ShowBannerAdsWhenEndCutScene();
    }

    public void EndCutscene1()
    {
        GameManager.instance.isShowEvent = true;
        cutscene1.gameObject.SetActive(false);
        cutscene3.SetActive(true);
    }

    public void EndCutscene3()
    {
        //GamePopup.Instance.ShowPopupTransition();
        GameManager.instance.isShowEvent = false;
        skipBtn.gameObject.SetActive(false);
        cutscene3.gameObject.SetActive(false);
        skipBtn.SetActive(false);
        SoundManager.Instance.PlayBackgroundMusic();
        SoundManager.Instance.PlayStartLevelSound(MapLevelManager.Instance.ESoundStartLevel);
        ShowBannerAdsWhenEndCutScene();
    }

    public void EndCutscene4()
    {
        GameManager.instance.isShowEvent = false;
        CompletedIntro();
        SoundManager.Instance.StopSound();
        SoundManager.Instance.PlayBackgroundMusic();
        SoundManager.Instance.PlayStartLevelSound(MapLevelManager.Instance.ESoundStartLevel);
        ShowBannerAdsWhenEndCutScene();
    }

    private void Start()
    {
        Observer.endCutscene1 += EndCutscene1;
        Observer.endCutscene2 += EndCutscene2;
        Observer.endCutscene3 += EndCutscene3;
        Observer.endCutscene4 += EndCutscene4;
    }

    private void OnDestroy()
    {
        Observer.endCutscene1 -= EndCutscene1;
        Observer.endCutscene2 -= EndCutscene2;
        Observer.endCutscene3 -= EndCutscene3;
        Observer.endCutscene4 -= EndCutscene4;
    }

    private void ShowBannerAdsWhenEndCutScene()
    {
        if (Utils.IsTurnOnAds && Utils.IsMobile)
        {
            Advertising.ShowBannerAd();
        }
    }
}