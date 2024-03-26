#pragma warning disable 649
using System;
using System.Collections;
using System.Linq;
using Pancake.Monetization;
using Spine;
using Spine.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using Worldreaver.Utility;

public class PopupCastle : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnCloseBoard;
    [SerializeField] private UniButton btnBuild;
    [SerializeField] private UniButton btnAds;
    [SerializeField] private UniButton btnMap;
    [SerializeField] private UniButton btnToShop;
    [SerializeField] private TextMeshProUGUI txtCurrency;
    [SerializeField] private SkeletonGraphic hammerSkeleton;
    [SerializeField] private GameObject effectBuild;
    [SerializeField] private GameObject effectsmokehouse;
    [SerializeField] private GameObject buildBoard;
    [SerializeField] private GameObject groupButton;
    [SerializeField] private CastleBuildElement[] buildElements;
    [SerializeField] private Image[] elements;
    [SerializeField] private GameObject[] maps;
    [SerializeField] private GameObject[] groupBuildCastle;
    [SerializeField] private GameObject[] btnVisits;
    [SerializeField] private GameObject[] locks;

    [SerializeField] private GameObject notiCastleBuild;
    [SerializeField] private GameObject notiCastleMap;
    [SerializeField] private GameObject[] notiCastleMaps;
    [SerializeField] private RectTransform mapContent;

    [SerializeField] private Sprite slotStarSprite;
    [SerializeField] private Sprite lightStarSprite;
    [SerializeField] private Sprite avaiableButtonBuildSprite;
    [SerializeField] private Sprite unAvaiableButtonBuildSprite;
    [SerializeField] private Sprite upgradeButtonSprite;

    [SerializeField] private Color enoughCostColor;
    [SerializeField] private Color notEnoughtCostColor;

    [SerializeField] private int valueWatchVideo = 500;
    [SerializeField] private GameObject fetch;
    [SerializeField] private GameObject selectMap;
    private Action _actionBack;
    private Action _actionCloseBoard;
    private Action _actionBuild;
    private int _currentMap;
    private Action<Action, Action, Action> _actionToShop;

    // private void Update()
    // {
    //     if (Utils.pauseUpdateFetchIcon) return;
    //
    //     if (!Application.isEditor)
    //     {
    //         if (AdsManager.Instance.IsRewardLoaded())
    //         {
    //             if (fetch.activeSelf) fetch.SetActive(false);
    //         }
    //         else
    //         {
    //             if (!fetch.activeSelf) fetch.SetActive(true);
    //         }
    //     }
    //     else
    //     {
    //         if (fetch.activeSelf) fetch.SetActive(false);
    //     }
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionCloseBoard"></param>
    /// <param name="actionBuild"></param>
    public void Initialized(Action actionBack, Action actionCloseBoard, Action actionBuild,
        Action<Action, Action, Action> actionToShop)
    {
        _actionBack = actionBack;
        _actionBuild = actionBuild;
        _actionCloseBoard = actionCloseBoard;
        _actionToShop = actionToShop;

        UpdateCurrencyDisplay();

        for (int i = 0; i < buildElements.Length; i++)
        {
            buildElements[i]
                .Initialized(BuildCastle,
                    GetCostByStar,
                    GetStarById,
                    CastBuild,
                    GetStarSprite,
                    GetBuildButtonSprite,
                    GetCostColor);

            var result = buildElements[i].GetData();
            if (result.Item2)
            {
                elements[result.Item3].gameObject.SetActive(true);
                elements[result.Item3].sprite = result.Item1;
            }
        }


        hammerSkeleton.Initialize(true);
        hammerSkeleton.AnimationState.Event += TrackingEvent;

        btnToShop.onClick.RemoveAllListeners();
        btnToShop.onClick.AddListener(OnToShopButtonPressed);

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnBuild.onClick.RemoveAllListeners();
        btnBuild.onClick.AddListener(OnBuildButtonPressed);

        btnCloseBoard.onClick.RemoveAllListeners();
        btnCloseBoard.onClick.AddListener(OnCloseBoardButtonPressed);

        btnAds.onClick.RemoveAllListeners();
        btnAds.onClick.AddListener(OnWatchAdsButtonPresed);

        btnMap.onClick.RemoveAllListeners();
        btnMap.onClick.AddListener(OnMapButtonPressed);

        SoundManager.Instance.PlayCastlebackgroundMusic();
        _currentMap = Data.CastleCurrentMap;

        foreach (var map in maps)
        {
            map.SetActive(false);
        }

        if (_currentMap >= 0 && _currentMap < maps.Length)
        {
            maps[_currentMap].SetActive(true);
        }

        RefreshNotiCastle();

        float aspect = (float)Screen.height / (float)Screen.width; // Portrait
        //aspect = (float)Screen.width / (float)Screen.height; // Landscape
        if (aspect >= 1.87)
        {
            //Debug.Log("19.5:9"); // iPhone X                  
        }
        else if (aspect >= 1.74) // 16:9
        {
            //Debug.Log("16:9");
        }
        else if (aspect > 1.6) // 5:3
        {
            //Debug.Log("5:3");
        }
        else if (Math.Abs(aspect - 1.6) < Mathf.Epsilon) // 16:10
        {
            //Debug.Log("16:10");
        }
        else if (aspect >= 1.5) // 3:2
        {
            //Debug.Log("3:2");
        }
        else
        {
            // 4:3
            //Debug.Log("4:3 or other");

            mapContent.anchorMin = new Vector2(0.5f, 0);
            mapContent.anchorMax = new Vector2(0.5f, 1f);
            mapContent.pivot = new Vector2(0.5f, 0.5f);
            mapContent.anchoredPosition3D = new Vector3(mapContent.anchoredPosition3D.x, 0, 0);
            mapContent.sizeDelta = new Vector2(mapContent.sizeDelta.x, 0);
        }
    }

    private void OnDisable()
    {
        hammerSkeleton.AnimationState.Event -= TrackingEvent;
    }

    /// <summary>
    /// to shop button pressed
    /// </summary>
    private void OnToShopButtonPressed()
    {
        _actionToShop?.Invoke(UpdateCurrencyDisplay, RefreshAllItem, null);
        ((PopupShop)GamePopup.Instance.popupShopHandler).OnClose = RefreshAllItem;
    }

    public void RefreshAllItem()
    {
        try
        {
            RefreshNotiCastle();
            UpdateCurrencyDisplay();
            RefreshNotiCastleSelectMap();
            for (int i = 0 + _currentMap * 7; i < (_currentMap + 1) * 7; i++)
            {
                buildElements[i].Refresh();
            }
        }
        catch (Exception)
        {
            //
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBackButtonPressed()
    {
        _actionBack?.Invoke();

        SoundManager.Instance.PlayBackgroundMusic();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnCloseBoardButtonPressed()
    {
        _actionCloseBoard?.Invoke();
        buildBoard.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBuildButtonPressed()
    {
        _actionBuild?.Invoke();
        buildBoard.gameObject.SetActive(true);

        foreach (var o in groupBuildCastle)
        {
            o.SetActive(false);
        }

        try
        {
            groupBuildCastle[_currentMap].SetActive(true);

            for (int i = 0 + _currentMap * 7; i < (_currentMap + 1) * 7; i++)
            {
                buildElements[i].Refresh();
            }
        }
        catch (Exception)
        {
            //
        }
    }

    /// <summary>
    /// hardcode to bad :(((
    /// </summary>
    private void OnMapButtonPressed()
    {
        btnVisits[0].SetActive(true);
        btnVisits[1].SetActive(Utils.CurrentLevel >= 200);
        locks[1].SetActive(!(Utils.CurrentLevel >= 200));

        btnVisits[2].SetActive(Utils.CurrentLevel >= 500);
        locks[2].SetActive(!(Utils.CurrentLevel >= 500));

        btnVisits[3].SetActive(Utils.CurrentLevel >= 1000);
        locks[3].SetActive(!(Utils.CurrentLevel >= 1000));

        btnVisits[4].SetActive(Utils.CurrentLevel >= 1500);
        locks[4].SetActive(!(Utils.CurrentLevel >= 1500));

        btnVisits[5].SetActive(Utils.CurrentLevel >= 2000);
        locks[5].SetActive(!(Utils.CurrentLevel >= 2000));

        RefreshNotiCastleSelectMap();
        selectMap.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnWatchAdsButtonPresed()
    {
        if (!Utils.IsMobile || !Utils.IsTurnOnAds)
        {
            Utils.currentCoin += valueWatchVideo;
            UpdateCurrencyDisplay();
            return;
        }

        RescueAnalytic.LogEventAdRewardRequest();

        Advertising.ShowRewardedAd().OnCompleted(() =>
        {
            Utils.currentCoin += valueWatchVideo;
            UpdateCurrencyDisplay();
            Utils.pauseUpdateFetchIcon = false;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    private int BuildCastle(int id)
    {
        var castle = DataController.instance.SaveCastle[id];
        if (castle.star >= 5)
        {
            castle.star = 5;
            return castle.star;
        }

        castle.unlock = true;
        castle.star++;
        UpdateCurrencyDisplay();
        RescueAnalytic.LogEventBuildCastle(Utils.currentCoin, Utils.CurrentLevel, id, castle.star);

        if (castle.star >= 5)
        {
            var result = DataController.instance.SaveCastle.Where(_ => _.star < 5).ToList();
            if (result.IsNullOrEmpty())
            {
                RescueAnalytic.LogEventAllCastleBuilded(Utils.currentCoin, Utils.CurrentLevel);
            }

            castle.star = 5;
        }

        RefreshNotiCastle();
        return castle.star;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="star"></param>
    /// <returns></returns>
    private int GetCostByStar(int star)
    {
        if (star >= 5)
        {
            return 0;
        }

        star += _currentMap * 5;

        return Config.CostPurchaseByStars[star];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private int GetStarById(int id)
    {
        return DataController.instance.SaveCastle[id].star;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator IeDelayActive(int id, Sprite sprite, Vector3 pos)
    {
        hammerSkeleton.AnimationState.SetAnimation(0, "Bua dap", false);
        hammerSkeleton.gameObject.SetActive(true);
        hammerSkeleton.timeScale = 3;
        yield return new WaitForSeconds(0.1f);
        hammerSkeleton.timeScale = 2;
        hammerSkeleton.AnimationState.SetAnimation(0, "Bua dap", false);
        yield return new WaitForSeconds(0.5f);
        hammerSkeleton.AnimationState.SetAnimation(0, "Bua dap", false);
        yield return new WaitForSeconds(0.5f);
        hammerSkeleton.AnimationState.SetAnimation(0, "Bua dap", false);
        yield return new WaitForSeconds(0.5f);
        hammerSkeleton.AnimationState.SetAnimation(0, "Bua dap", false);
        yield return new WaitForSeconds(0.5f);

        hammerSkeleton.gameObject.SetActive(false);
        effectsmokehouse.transform.position = pos;
        effectsmokehouse.SetActive(true);
        effectBuild.SetActive(false);
        elements[id].gameObject.SetActive(true);
        elements[id].sprite = sprite;

        groupButton.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sprite"></param>
    private void CastBuild(int id, Sprite sprite)
    {
        var pos = elements[id].transform.position;
        groupButton.SetActive(false);
        buildBoard.SetActive(false);
        effectsmokehouse.SetActive(false);
        hammerSkeleton.gameObject.SetActive(true);
        hammerSkeleton.transform.position = pos;
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.hammerBuildClip);
        Observable.FromCoroutine(() => IeDelayActive(id, sprite, pos)).Subscribe();
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateCurrencyDisplay()
    {
        txtCurrency.text = $"{Utils.currentCoin}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="trackEntry"></param>
    /// <param name="e"></param>
    private void TrackingEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name.Equals("Bua dap"))
        {
            effectBuild.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private (Sprite, Sprite) GetStarSprite()
    {
        return (lightStarSprite, slotStarSprite);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private (Sprite, Sprite, Sprite) GetBuildButtonSprite()
    {
        return (avaiableButtonBuildSprite, unAvaiableButtonBuildSprite, upgradeButtonSprite);
    }

    private (Color, Color) GetCostColor() => (enoughCostColor, notEnoughtCostColor);

    public void SetActiveMap(int index)
    {
        selectMap.SetActive(false);
        foreach (var map in maps)
        {
            map.SetActive(false);
        }

        if (index >= 0 && index < maps.Length)
        {
            maps[index].SetActive(true);
            _currentMap = index;
            Data.CastleCurrentMap = _currentMap;
            RefreshNotiCastle();
        }
    }

    public void RefreshNotiCastle()
    {
        var regions = DataController.instance.CheckCastleNotification();

        var check = false;
        for (var i = 0; i < regions.Length; i++)
        {
            if (regions[i])
            {
                check = true;
                break;
            }
        }

        notiCastleBuild.SetActive(regions[_currentMap]);
        notiCastleMap.SetActive(check);
    }

    public void RefreshNotiCastleSelectMap()
    {
        var regions = DataController.instance.CheckCastleNotification();

        for (int i = 0; i < regions.Length; i++)
        {
            notiCastleMaps[i].SetActive(regions[i]);
        }
    }
}