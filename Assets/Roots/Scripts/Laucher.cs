using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Pancake.Linq;
using Pancake.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Worldreaver.Loading;
using Random = UnityEngine.Random;

public class Laucher : MonoBehaviour
{
    [SerializeField] private IAPManager iapManager;
    [SerializeField] private Loader loader;
    [SerializeField] private Image bg;
    [SerializeField] private GameObject fadeImage;

    [SerializeField] private BGData _bgData;
    private static LevelMap _temLevelMap;

    private void Start()
    {
        fadeImage.SetActive(true);
        bg.gameObject.SetActive(true);
        Utils.showNewWorld = false;
        Utils.isHardMode = false;
        Data.idEasterEgg = -1;
        MMVibrationManager.SetHapticsActive(Data.UserVibrate);
        DOTween.Init();
        Addressables.InitializeAsync();
        Playfab.Login();
        //Data.appId = Config.AdmobAppId;
        Data.bannerId = Config.AdmobBannerId;
        Data.intertitialId = Config.AdmobInterstitialId;
        Data.rewardedId = Config.AdmobRewardedId;

        if (!iapManager.IsInitialize)
        {
            iapManager.Initialized(new[]
            {
                new IAPData(Constants.IAP_PACK1, ProductType.Consumable.ToString()),
                new IAPData(Constants.IAP_PACK2, ProductType.Consumable.ToString()),
                new IAPData(Constants.IAP_PACK3, ProductType.Consumable.ToString()),
                new IAPData(Constants.IAP_REMOVE_ADS, ProductType.NonConsumable.ToString()),
                new IAPData(Constants.IAP_UNLOCK_OUTFIT, ProductType.NonConsumable.ToString()),
                new IAPData(Constants.IAP_VIP, ProductType.NonConsumable.ToString()),
                new IAPData(Constants.IAP_DOUBLE_GOLD, ProductType.NonConsumable.ToString()),
            });
        }

        BridgeData.Instance.showUpdatePopupAction = async () =>
        {
            await UniTask.WaitUntil(() => MyAnalytic.Instance.IsFetchComplete && GamePopup.Instance != null);
            
            if (!Data.DontShowUpdateAgain && Data.currentVersion != $"v{Application.version.Replace('.', '_')}" &&
                !string.IsNullOrEmpty(Data.currentVersion) &&
                Utils.CheckInternetConnection())
            {
                void PlaySoundClick()
                {
                    if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
                }


                BridgeData.Instance.showUpdatePopupAction = null;
            }
        };

        BridgeData.Instance.StartDetectCountry();

        // if (Data.CurrentWorld == 0 && !Data.GetStateCutScene(0))
        // {
        //     loader.Load("CutScene", LoadSceneMode.Single, null);
        // }
        // else
        // {
        //}

        LoadLevel();
        LoadRoom();
        ChangeBG();
    }

    private async void ChangeBG()
    {
        string currentBgName = Utils.CurrentBackGround;
        var currentBgData = _bgData.listBGData.FirstOrDefault(bg => bg.MapType.ToString().Equals(currentBgName)) ?? _bgData.listBGData[0];
        
        int ranIndex = (int)Random.Range(0, currentBgData.listSpriteBg.Count);

        bg.sprite = currentBgData.listSpriteBg[ranIndex];
        fadeImage.SetActive(false);

        // var go = await BridgeData.Instance.GetLevel(Utils.CurrentLevel);
        // if (go.Item1 != null)
        // {
        //     LevelMap levelMap = go.Item1.GetComponent<LevelMap>();
        //     BgData curBgData = _bgData.listBGData.FirstOrDefault(bg => bg.MapType == levelMap.mapType);
        //     if (curBgData != null)
        //     {
        //         int ranIndex = (int)Random.Range(0, curBgData.listSpriteBg.Count);
        //         try
        //         {
        //             bg.sprite = curBgData.listSpriteBg[ranIndex];
        //             fadeImage.SetActive(false);
        //         }
        //         catch (Exception e)
        //         {
        //             Debug.Log("DEBUG_LOADING " + e.Message);
        //         }
        //     }
        // }

        var go = await BridgeData.Instance.GetLevel(Utils.CurrentLevel);
        
        LoadMenu();
    }

    private static async void LoadLevel()
    {
        var go = await BridgeData.Instance.GetLevel(Utils.CurrentLevel);
        if (go.Item1 != null)
        {
            LevelMap levelMap = go.Item1.GetComponent<LevelMap>();

            BridgeData.Instance.nextLevelLoaded = levelMap;
            BridgeData.Instance.nextLevelLoaded.SetLevelLoaded(go.Item2);

            BridgeData.Instance.previousLevelLoaded = levelMap;
            BridgeData.Instance.previousLevelLoaded.SetLevelLoaded(go.Item2);
            
        }
    }

    private static async void LoadRoom()
    {
        var room = await BridgeData.Instance.GetRoom(Data.CurrentMenuWorld);
        if (room != null) BridgeData.Instance.menuRoomPrefab = room.GetComponent<BaseRoom>();
        if (room != null) BridgeData.Instance.currentRoomPrefab = room.GetComponent<BaseRoom>();
    }

    private async void WaitingDone()
    {
        await UniTask.WaitUntil(() =>
            BridgeData.Instance.nextLevelLoaded != null && BridgeData.Instance.menuRoomPrefab != null);
    }

    private async void WaitInitializeIap()
    {
        await UniTask.WaitUntil(() => iapManager.IsInitialize);
    }

    private void LoadMenu()
    {
        loader.Load("MainMenu", OnSceneLoaded, WaitingDone, WaitInitializeIap);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        BridgeData.Instance.showUpdatePopupAction?.Invoke();
    }
}