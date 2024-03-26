#pragma warning disable 649
using System;
using MoreMountains.NiceVibrations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniTween;
using Worldreaver.UniUI;
using Worldreaver.Utility;

public class PopupSetting : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnSound;
    [SerializeField] private UniButton btnMusic;

    [SerializeField] private UniButton btnVibration;

    //[SerializeField] private UniButton btnUpdate;
    [SerializeField] private GameObject btnRestore;
    [SerializeField] private GameObject block;
    [SerializeField] private UniButton btnBackupData;
    [SerializeField] private UniButton btnRestoreData;
    [SerializeField] private TextMeshProUGUI txtVersion;
    [SerializeField] private Image soundHandle;
    [SerializeField] private Image musicHandle;
    [SerializeField] private Image vibrateHandle;

    private Action _actionBack;
    private Action _actionSound;
    private Action _actionMusic;
    private Action _actionVibrate;
    private IDisposable _moveHandleDisposable;

    /// <summary>
    /// initialize
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionSound"></param>
    /// <param name="actionMusic"></param>
    /// <param name="actionVibrate"></param>
    public void Initialized(Action actionBack, Action actionSound, Action actionMusic, Action actionVibrate)
    {
        _actionBack = actionBack;
        _actionSound = actionSound;
        _actionMusic = actionMusic;
        _actionVibrate = actionVibrate;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnSound.onClick.RemoveAllListeners();
        btnSound.onClick.AddListener(OnSoundButtonPressed);

        btnMusic.onClick.RemoveAllListeners();
        btnMusic.onClick.AddListener(OnMusicButtonPressed);

        btnVibration.onClick.RemoveAllListeners();
        btnVibration.onClick.AddListener(OnVibrationButtonPressed);

        // btnBackupData.onClick.RemoveAllListeners();
        // btnBackupData.onClick.AddListener(OnButtonBackupPressed);

        btnRestoreData.onClick.RemoveAllListeners();
        btnRestoreData.onClick.AddListener(OnButtonRestoreDataPressed);

        // if (!Data.DontShowUpdateAgain && Data.currentVersion != $"v{Application.version.Replace('.', '_')}" && !string.IsNullOrEmpty(Data.currentVersion) &&
        //     Utils.CheckInternetConnection())
        // {
        //     btnUpdate.gameObject.SetActive(true);
        //     btnUpdate.onClick.RemoveAllListeners();
        //     btnUpdate.onClick.AddListener(OnUpdateButtonPressed);
        // }
        // else
        // {
        //     btnUpdate.gameObject.SetActive(false);
        // }

        var soundState = Data.UserSound;
        soundHandle.gameObject.SetActive(!soundState);

        var musicState = Data.UserMusic;
        musicHandle.gameObject.SetActive(!musicState);

        var vibrateState = Data.UserVibrate;
        vibrateHandle.gameObject.SetActive(!vibrateState);


        txtVersion.text = $"VERSION {Application.version}";

#if UNITY_IOS
        btnRestore.gameObject.SetActive(true);
#else
        btnRestore.gameObject.SetActive(false);
#endif
    }


    private void OnUpdateButtonPressed()
    {
        Utils.GotoStore();
        MenuController.instance.SoundClickButton();
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed()
    {
        _actionBack?.Invoke();
        MenuController.instance.SoundClickButton();
    }

    /// <summary>
    /// sound button pressed
    /// </summary>
    private void OnSoundButtonPressed()
    {
        var currentState = Data.UserSound;
        Data.UserSound = !currentState;
        soundHandle.gameObject.SetActive(!Data.UserSound);
        _actionSound?.Invoke();
    }

    /// <summary>
    /// music button pressed
    /// </summary>
    private void OnMusicButtonPressed()
    {
        var currentState = Data.UserMusic;
        Data.UserMusic = !currentState;
        musicHandle.gameObject.SetActive(!Data.UserMusic);
        SoundManager.Instance.PlayBackgroundMusic();
        _actionMusic?.Invoke();
    }

    /// <summary>
    /// vibration button pressed
    /// </summary>
    private void OnVibrationButtonPressed()
    {
        var currentState = Data.UserVibrate;
        Data.UserVibrate = !currentState;

        MMVibrationManager.SetHapticsActive(Data.UserVibrate);

        vibrateHandle.gameObject.SetActive(!Data.UserVibrate);
        _actionVibrate?.Invoke();
    }

    private void OnButtonRestoreDataPressed()
    {
        if (!Utils.CheckInternetConnection())
        {
            GamePopup.Instance.ShowNotificationPopup(null, "Please check your connection\nand try again late!");
            return;
        }

        //     if (FacebookManager.Instance.IsLoggedIn)
        //     {
        //         SetActiveBlock(true);
        //         PlayfabHelper.Instance.FacebookLogin(FacebookManager.Instance.Token,
        //             ChangeLogin,
        //             false,
        //             () => { GamePopup.Instance.ShowNotificationPopup(null, "Restore data success!"); });
        //     }
        //     else
        //     {
        //         SetActiveBlock(true);
        //
        //         FacebookManager.Instance.Login(() =>
        //             {
        //                 PlayfabHelper.Instance.FacebookLogin(FacebookManager.Instance.Token,
        //                     ChangeLogin,
        //                     false,
        //                     () => { GamePopup.Instance.ShowNotificationPopup(null, "Restore data success!"); });
        //             },
        //             () =>
        //             {
        //                 SetActiveBlock(false);
        //                 GamePopup.Instance.ShowNotificationPopup(null, "Faild to login Facebook!\nPlease try again!");
        //             },
        //             () =>
        //             {
        //                 SetActiveBlock(false);
        //                 GamePopup.Instance.ShowNotificationPopup(null, "Error when login Facebook!\nPlease try again!");
        //             });
        //     }
        // }
        //
        // private void OnButtonBackupPressed()
        // {
        //     if (!Utils.CheckInternetConnection())
        //     {
        //         GamePopup.Instance.ShowNotificationPopup(null, "Please check your connection\nand try again late!");
        //         return;
        //     }
        //
        //     if (FacebookManager.Instance.IsLoggedIn)
        //     {
        //         SetActiveBlock(true);
        //         PlayfabHelper.Instance.FacebookLogin(FacebookManager.Instance.Token,
        //             ChangeLogin,
        //             true,
        //             () => { GamePopup.Instance.ShowNotificationPopup(null, "Backup data success!"); });
        //     }
        //     else
        //     {
        //         SetActiveBlock(true);
        //
        //         FacebookManager.Instance.Login(() =>
        //             {
        //                 PlayfabHelper.Instance.FacebookLogin(FacebookManager.Instance.Token,
        //                     ChangeLogin,
        //                     true,
        //                     () => { GamePopup.Instance.ShowNotificationPopup(null, "Backup data success!"); });
        //             },
        //             () =>
        //             {
        //                 SetActiveBlock(false);
        //                 GamePopup.Instance.ShowNotificationPopup(null, "Faild to login Facebook!\nPlease try again!");
        //             },
        //             () =>
        //             {
        //                 SetActiveBlock(false);
        //                 GamePopup.Instance.ShowNotificationPopup(null, "Error when login Facebook!\nPlease try again!");
        //             });
        //     }
    }

    private void ChangeLogin()
    {
        SetActiveBlock(false);
    }

    public void SetActiveBlock(bool state)
    {
        block.SetActive(state);
    }
}