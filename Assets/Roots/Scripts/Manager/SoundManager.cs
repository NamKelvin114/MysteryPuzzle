using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using DG.Tweening;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip acMainMenuMusic, acIntro;

    public AudioClip acClick,
        acWin,
        acLose,
        acTakeSword,
        acOpenChest,
        acEnemyDie,
        acMoveStick,
        acMeleeAttack,
        acPrincessApear,
        acLavaOnWater,
        acLavaApear,
        acStoneApear,
        btnStart,
        acMoveStickClick,
        acFoundOtherStick,
        acCutRope,
        acPrincessSave,
        acPrincessDie,
        acPlaceBlock,
        acPlaceBlockFalse,
        acTakeBlock,
        acShiny,
        acFxMatch,
        giftCollectionPopup;

    public AudioClip coinGain;
    public AudioClip audioEnemyShoot;

    public AudioClip fireBallExplode;
    public AudioClip dragonDeath;
    public AudioClip dragonAttack;
    public AudioClip wolfAttack;
    public AudioClip wolfDie;
    public AudioClip teleportSound;
    public AudioClip hammerBuildClip;
    public AudioClip punch;
    public AudioClip castleBackgroundClip;
    public AudioClip mineExplodeClip;
    public AudioClip mineActiveClip;
    public AudioClip useHolyWater;
    public AudioClip planeSound;
    public AudioClip intro4;
    public AudioClip intro5;
    public AudioClip girlIntro5;

    public AudioClip introWow;
    public AudioClip confused;
    public AudioClip chillSweep;
    public AudioClip openBoxFX;

    [Header("player")] public AudioClip heroJumpWin;
    public AudioClip heroCry;
    public AudioClip heroDieFire;
    public AudioClip heoroScream;
    public AudioClip heroDie;
    [SerializeField] private List<AudioClip> endGameWin;
    [SerializeField] private List<AudioClip> endGameLose;

    public AudioClip[] acCoinApear;
    public AudioSource audioSource;
    public AudioSource audioSouceBG;

    [Header("Enemy")] public AudioClip beeDieByRock;

    public AudioClip beeDieByLava;


    [Header("Start Level")] public List<EnityStartLevelSoundList> StartLevelSoundLists;
    [Header("PopupTask")] public AudioClip notDoneTask;
    public AudioClip claimTask;
    public AudioClip claimGift;
    public AudioClip giftAperrence;
    public AudioClip openChest;
    public AudioClip mainTaskClaim;
    public AudioClip giftOpen;
    [Header("stamp")] public AudioClip popupItemAppear;
    public AudioClip rubberStamp;
    Sequence mySequence = DOTween.Sequence();


    public void PlaySound(AudioClip audio)
    {
        if (Data.UserSound)
        {
            audioSource.mute = false;
            if (audio == acWin || audio == acLose || audio == giftAperrence || audio == giftOpen
                || audio == openBoxFX)
                audioSouceBG.volume = 0.1f;
            if (audio == heroJumpWin)
            {
                DoPlaySoundEndGame(true);
            }
            else if (audio == heroCry)
            {
                DoPlaySoundEndGame(false);
            }
            else
            {
                audioSource.PlayOneShot(audio);
            }
        }
        else audioSource.mute = true;
    }

    void DoPlaySoundEndGame(bool isWin)
    {
        var getaudio = SoundPlayerEndGame(isWin);
        DOTween.Kill(this);
        DOTween.Sequence().AppendInterval(MapLevelManager.Instance.isGameplay1 ? 0f : 1f)
            .AppendCallback(() => { audioSource.PlayOneShot(getaudio); });
    }

    public void PlaySoundContinously(AudioClip audio)
    {
        if (Data.UserSound)
        {
            audioSource.mute = false;
            // if (audio == acWin || audio == acLose || audio == giftAperrence || audio == giftOpen)
            //     audioSouceBG.volume = 0.1f;
            // if (audio == heroJumpWin) audio = SoundPlayerEndGame(true);
            // if (audio == heroCry) audio = SoundPlayerEndGame(false);
            audioSource.clip = audio;
            audioSource.loop = true;
            audioSource.Play();
        }
        else audioSource.mute = true;
    }

    public void StopSoundContinously()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void StopBGSound()
    {
        audioSouceBG.Stop();
    }

    public void PlayBackgroundMusic()
    {
        //var isMainMenu = SceneManager.GetActiveScene().name.Equals(Constants.MENU_SCENE_NAME);
        PlayBackGroundMusic(acMainMenuMusic);
    }

    public void PlayBackgroundIntroMusic()
    {
        //var isMainMenu = SceneManager.GetActiveScene().name.Equals(Constants.MENU_SCENE_NAME);
        PlayBackGroundMusic(acIntro);
    }

    public void PLayBackGroundMusicSceneBefore5()
    {
        PlayBackGroundMusic(girlIntro5);
    }

    public void PlayBGCustom(AudioClip audioClip)
    {
        PlayBackGroundMusic(audioClip);
    }

    private AudioClip SoundPlayerEndGame(bool isWin)
    {
        if (isWin)
        {
            int ranPos = Random.Range(0, endGameWin.Count);
            return endGameWin[ranPos];
        }
        else
        {
            int ranPos = Random.Range(0, endGameLose.Count);
            return endGameLose[ranPos];
        }
    }

    void PlayBackGroundMusic(AudioClip clip)
    {
        if (Data.UserMusic)
        {
            audioSouceBG.mute = false;
            audioSouceBG.clip = clip;
            if (!audioSouceBG.isPlaying)
                audioSouceBG.Play();
        }
        else
        {
            audioSouceBG.mute = true;
        }
    }

    public void PlayStartLevelSound(ESoundStartLevel eSoundStartLevel)
    {
        var sound = StartLevelSoundLists.FirstOrDefault(t => t.ESoundStartLevel == eSoundStartLevel);
        if (sound != null)
            PlaySound(sound.startLevelSound);
    }

    public void PlayCastlebackgroundMusic()
    {
        if (Data.UserMusic)
        {
            audioSouceBG.mute = false;
            audioSouceBG.clip = castleBackgroundClip;
            audioSouceBG.Play();
        }
        else
        {
            audioSouceBG.mute = true;
        }
    }

    public bool IsBgPlaying()
    {
        return audioSouceBG.isPlaying;
    }

    public void CheckBgMusic()
    {
        audioSouceBG.mute = !Data.UserMusic;
    }

    public void CheckSound()
    {
        audioSource.mute = !Data.UserSound;
    }
}


[Serializable]
public class EnityStartLevelSoundList
{
    public ESoundStartLevel ESoundStartLevel;
    public AudioClip startLevelSound;
}

[Serializable]
public enum ESoundStartLevel
{
    HowItsExciting,
    LookOverThere,
    MainHelp,
    FoundIt,
    MainIdle,
}