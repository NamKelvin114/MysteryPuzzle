using System;
using System.Runtime.CompilerServices;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupDebug : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private TMP_InputField levelInput;
    [SerializeField] private TMP_InputField coinInput;
    [SerializeField] private TMP_InputField inpIdEgg;
    [SerializeField] private TMP_InputField inpNumberEgg;
    [SerializeField] private TMP_InputField inpEventCoin;
    [SerializeField] private Toggle turnOnAdsToggle;
    [SerializeField] private Toggle doAllTask;

    private Action _actionBack;
    private Action _actionOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void Initialized(Action actionBack, Action actionOk)
    {
        turnOnAdsToggle.isOn = Utils.IsTurnOnAds;
        doAllTask.isOn = Utils.DoAllTask;
        _actionBack = actionBack;
        _actionOk = actionOk;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed()
    {
        _actionBack?.Invoke();
    }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed()
    {
        _actionOk?.Invoke();
        int.TryParse(coinInput.text, out var coin);
        int.TryParse(levelInput.text, out var level);
        int.TryParse(inpIdEgg.text, out var idEgg);
        int.TryParse(inpNumberEgg.text, out var numberEgg);
        int.TryParse(inpEventCoin.text, out var TotalMedalEventCoin);

        if (level != 0)
        {
            Utils.CurrentLevel = level - 1;
            Utils.MaxLevel = level;
        }

        //DataController.instance.petDataController.AddEggShard(idEgg, numberEgg);

        if (Utils.CurrentLevel >= 40)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 0 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 0;
        }

        if (Utils.CurrentLevel >= 80)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 1 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 1;
        }

        if (Utils.CurrentLevel >= 120)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 2 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 2;
        }

        if (Utils.CurrentLevel >= 160)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 3 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 3;
        }

        if (Utils.CurrentLevel >= 200)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 4 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 4;
        }

        if (Utils.CurrentLevel >= 240)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 5 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 5;
        }

        if (Utils.CurrentLevel >= 280)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 6 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 6;
        }

        if (Utils.CurrentLevel >= 320)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 7 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 7;
        }

        if (Utils.CurrentLevel >= 360)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 8 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 8;
        }

        if (Utils.CurrentLevel >= 400)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 9 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 9;
        }

        if (Utils.CurrentLevel >= 440)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 10 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 10;
        }

        if (Utils.CurrentLevel >= 480)
        {
            for (int i = 0; i < 8; i++)
            {
                DataController.instance.SaveItems[i + 11 * 8].unlock = true;
            }

            Data.CurrentMenuWorld = 11;
        }

        Utils.currentCoin = coin;
        Data.TotalGoldMedal = TotalMedalEventCoin;
        if (MenuController.instance != null) MenuController.instance.CheckDisplayWarningDailyGiftEvent();
        if (GameManager.instance != null) GameManager.instance.CheckDisplayWarningDailyGiftEvent();
        DataController.instance.SaveItem();

        // if (unlockAllSkin.isOn)
        // {
        //     DataController.instance.SaveHero.Clear();
        //     var heroLength = HeroData.Length;
        //     for (int i = 0; i < heroLength; i++)
        //     {
        //         if (i == 22 || i == 28 || i == 35 || i == 31 || i == 32 || i == 33 || i == 37 || i == 38 || i == 39 || i == 40)
        //         {
        //             DataController.instance.SaveHero.Add(new SaveHero {unlock = false});
        //             continue;
        //         }
        //
        //         DataController.instance.SaveHero.Add(new SaveHero {unlock = true});
        //     }
        //
        //     PlayerPrefs.SetString(Data.SAVEHERO, JsonMapper.ToJson(DataController.instance.SaveHero));
        // }

        DataController.instance.SaveData();
    }

    public void SetAds(bool isDoneAll)
    {
        Utils.IsTurnOnAds = isDoneAll;
    }

    public void DebugDaily(bool isDebugDaily)
    {
        Utils.IsDebugDaily = isDebugDaily;
    }

    public void DoAllTask(bool doAllTask)
    {
        Utils.DoAllTask = doAllTask;
    }
}