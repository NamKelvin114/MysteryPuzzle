using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Pancake;
using Pancake.Monetization;
using Pancake.UI;
using Spine.Unity;

[RequireComponent(typeof(LevelMap))]
public class MapLevelManager : MonoBehaviour
{
    public SpawnObject lavaObj, waterObj;
    public bool loadListStick;
    public bool isShowTask;
    public bool isEndChapter;
    public bool isTaskTutorial;
    public bool isCollectionTutoria;
    public bool isJigsawLevel;
    public bool isCollectionLevel;
    public bool isNotPullAllPins;
    public bool isFirstJigSaw;
    public bool isGameplay1 = true;
    public bool isShowDercibeItem = false;
    public bool isShowTutorial = false;
    private List<Target> checkTargetList = new List<Target>();

    [ShowIf(nameof(isShowDercibeItem))] public GameObject prefabItem;


    public enum SPAWNTYPE
    {
        WATER,
        LAVA,
        STONE,
        GEMS,
        GAS,
        IceWater
    }


    public static MapLevelManager Instance;

    [HideInInspector] public bool isReadOnly;

    [DrawIf("isReadOnly", true, ComparisonType.Equals, DisablingType.ReadOnly)]
    public Transform trHostage, trGems;

    [ShowIf(nameof(isGameplay1))] [FormerlySerializedAs("questType")]
    public EQuestType eQuestType;

    [HideInInspector] public Transform trTarget;
    [ShowIf(nameof(isGameplay1))] public List<StickBarrier> lstAllStick = new List<StickBarrier>();

    [ShowIf(nameof(isGameplay1), false)] public List<StickBarrier> lstStick = new List<StickBarrier>();

    //[ShowIf(nameof(isGameplay1), false)] public List<BombItem> LstBombItems = new List<BombItem>();
    [ShowIf(nameof(isGameplay1))] public List<EnemyBase> lstAllEnemies = new List<EnemyBase>();
    [ShowIf(nameof(isGameplay1))] public List<BaseCannon> allCannonEnemies = new List<BaseCannon>();
    [ShowIf(nameof(isGameplay1), false)] public List<EnityTarget> lstETargets = new List<EnityTarget>();
    public Action<TargetType, ExpectedType> endGameplay2;
    public GemHandles GemSpawnObject { get; private set; }
    public ESoundStartLevel ESoundStartLevel;
    public bool gameStateWin = false;

    [Button("Update List Target")]
    private void DrawButton()
    {
        UpdateListETarget();
    }

    [Button("Update List Pin")]
    private void UpdateLstPin()
    {
        UpdateListPin();
    }

    public void UpdateListPin()
    {
        lstAllStick = new List<StickBarrier>();
        StickBarrier[] stick = GetComponentsInChildren<StickBarrier>();
        for (int i = 0; i < stick.Length; i++)
        {
            if (stick[i].key && !lstAllStick.Contains(stick[i]))
            {
                lstAllStick.Add(stick[i]);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        FetchGemObject();
    }

    public void FetchGemObject(GemHandles spawn = null)
    {
        if (spawn != null)
        {
            GemSpawnObject = spawn;
            return;
        }

        if (GemSpawnObject == null) GemSpawnObject = GetComponentsInChildren<GemHandles>().FirstOrDefault();
    }

    private void OnValidate()
    {
        if (!loadListStick)
        {
            StickBarrier[] stick = GetComponentsInChildren<StickBarrier>();
            for (int i = 0; i < stick.Length; i++)
            {
                if (stick[i].key && !lstAllStick.Contains(stick[i]))
                {
                    lstAllStick.Add(stick[i]);
                }
            }

            loadListStick = true;
        }
    }

    private void Start()
    {
        if (Utils.CurrentLevel > Config.MaxLevelCanReach)
        {
            isShowTutorial = false;
            isFirstJigSaw = false;
            isCollectionTutoria = false;
            isTaskTutorial = false;
        }

        gameStateWin = false;
        SoundManager.Instance.audioSouceBG.volume = .8f;
        Observer.ResetTaskTempValue?.Invoke();
        GamePopup.Instance.HidePopupMoney();
        if (Utils.CurrentLevel == Config.LevelShowCutscene1 && !Utils.IsShowCutscene1)
        {
            Utils.IsShowCutscene1 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutscene2();
        }
        else
            // if (Utils.CurrentLevel == Config.LevelShowCutscene3 && !Utils.IsShowCutscene3)
            // {
            //     Utils.IsShowCutscene3 = true;
            //     GameManager.instance.CutsceneController.PlayCutscene3();
            // }
            // else
        if (Utils.CurrentLevel == Config.LevelShowCutscene4 && !Utils.IsShowCutscene4)
        {
            Utils.IsShowCutscene4 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutscene4();
        }
        else if (Utils.CurrentLevel == Config.LevelShowCutsceneBefore5 && !Utils.IsShowCutsceneBefore5)
        {
            Utils.IsShowCutsceneBefore5 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutsceneBefore5();
        }
        else if (Utils.CurrentLevel == Config.LevelShowCutscene6 && !Utils.IsShowCutscene6)
        {
            Utils.IsShowCutscene6 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutScene6();
        }
        else if (Utils.CurrentLevel == Config.LevelShowCutscene7 && !Utils.IsShowCutscene7)
        {
            Utils.IsShowCutscene7 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutscene7();
        }
        else if (Utils.CurrentLevel == Config.LevelShowCutscene8 && !Utils.IsShowCutscene8)
        {
            Utils.IsShowCutscene8 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutscene8();
        }
        else if (Utils.CurrentLevel == Config.LevelShowCutscene9 && !Utils.IsShowCutscene9)
        {
            Utils.IsShowCutscene9 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutscene9();
        }
        else if (Utils.CurrentLevel == Config.LevelShowCutscene5 && !Utils.IsShowCutscene5)
        {
            Utils.IsShowCutscene5 = true;
            Advertising.HideBannerAd();
            GameManager.instance.CutsceneController.PlayCutscene5();
        }
        else
            SoundManager.Instance.PlayStartLevelSound(ESoundStartLevel);

        //if (isShowTutorial) tutorialHand.
        switch (eQuestType)
        {
            case EQuestType.Collect:
                trTarget = trGems;
                break;
            case EQuestType.SaveHostage:
                trTarget = trHostage;
                break;
        }

        if (GameManager.instance != null)
        {
            if (GameManager.instance.isTest)
            {
                GameManager.instance.gameState = EGameState.Playing;
                GameManager.instance.mapLevel = this;
                if (lstAllStick.Count > 0)
                    GameManager.instance.playerMove = true;
            }

            GameManager.instance.OnInitQuestText();
        }

        if (isFirstJigSaw)
        {
            GamePopup.Instance.ShowPopupTutorialJigSaw();
        }
    }

    public bool is_Plus_countPlayLevel = false;

    public void OnWin()
    {
        // if (GameManager.instance.gameState == EGameState.Playing)
        //     return;
        //GameManager.instance.gameState = EGameState.Win;
        Observer.UIMove?.Invoke(null);
        CheckShowCollectionLevel(() =>
        {
            GameManager.instance.FadeBackgroundEndGame();
            if (GameManager.instance.isShowLosing == false)
            {
                // startWinFollow only effect jigsaw gameplay 
                Observer.StartWinFollow?.Invoke();

                GameManager.instance.ShowWinPanel();
            }

            if (is_Plus_countPlayLevel == true)
            {
                GameManager.instance.Plus_countPlayLevel();
                is_Plus_countPlayLevel = false;
            }
        });
    }

    private void CheckShowCollectionLevel(Action onEnd)
    {
        if (!isCollectionLevel)
        {
            onEnd?.Invoke();
            return;
        }

        GameManager.instance.DisplayPopupClaimCollection(onEnd);
    }

    public void OnLose()
    {
        if (GameManager.instance.gameState != EGameState.Playing)
            return;
        Observer.UIMove?.Invoke(null);
        GameManager.instance.gameState = EGameState.Lose;
        GameManager.instance.FadeBackgroundEndGame();
        if (Data.isinterads)
        {
            if (is_Plus_countPlayLevel == true)
            {
                GameManager.instance.Plus_countPlayLevel();
                is_Plus_countPlayLevel = false;
            }
        }

        if (isGameplay1) PlayerManager.instance.is_pre_level_lose = true;
        // Debug.Log("22");
        GameManager.instance.ShowLosePanel();
    }

    private float _currentMaxActivePercent;

    public void OnTargetOut(Target target)
    {
        if (target == null) return;
        foreach (var enityTarget in lstETargets)
        {
            if (enityTarget.TargetType == target.TargetType)
            {
                if (!checkTargetList.Contains(target))
                {
                    checkTargetList.Add(target);
                    enityTarget.countMaxActive--;
                }

                if (enityTarget.isTargetToWin)
                {
                    if (enityTarget.isUsePercent)
                    {
                        _currentMaxActivePercent =
                            (float)enityTarget.countMaxActive / (float)enityTarget.countMax * 100f;
                        enityTarget.countAlive--;
                        CheckRunOutOfTarget(target);
                    }
                    else
                    {
                        if (enityTarget.countMaxActive < enityTarget.numberToWin &&
                            enityTarget.countMaxActive <= enityTarget.countMax)
                            WaitToEndGame();
                    }
                }
            }
        }
    }

    public void CheckRunOutOfTarget(Target target)
    {
        foreach (var enityTarget in lstETargets)
        {
            if (enityTarget.TargetType == target.TargetType)
            {
                if (enityTarget.countAlive <= 0)
                {
                    Observer.CheckWinTargetPercent?.Invoke();
                }
            }
        }
    }

    void WaitToEndGame()
    {
        DOTween.KillAll();
        Observer.playerEndGameplay2?.Invoke(false);
    }

    public void UpdateListETarget()
    {
        lstETargets = new List<EnityTarget>();
        Target[] foundTargets = GetComponentsInChildren<Target>();
        foreach (Target target in foundTargets)
        {
            var etarget =
                from c in lstETargets
                where c.TargetType == target.TargetType
                select c;
            var arrayetarget = etarget.ToArray();
            if (arrayetarget.Count() == 0)
            {
                EnityTarget newEtarget = new EnityTarget();
                newEtarget.TargetType = target.TargetType;
                newEtarget.ExpectedType = target.ExpectedType;
                lstETargets.Add(newEtarget);
            }
            else
            {
                arrayetarget[0].countMax++;
                arrayetarget[0].countMaxActive = arrayetarget[0].countMax;
                arrayetarget[0].countAlive = arrayetarget[0].countMax;
            }
        }
    }

    public EnityTarget GetETarget(TargetType _targetType)
    {
        var etarget =
            from c in lstETargets
            where c.TargetType == _targetType
            select c;
        var letarget = etarget.ToArray();
        if (letarget.Count() == 0) return null;
        else
            return letarget[0];
    }

    public EnityTarget GetETargetToWin()
    {
        var etarget =
            from c in lstETargets
            where c.isTargetToWin == true
            select c;
        var letarget = etarget.ToArray();
        if (letarget.Count() == 0) return null;
        else
            return letarget[0];
    }
}

[Serializable]
public class EnityTarget
{
    public TargetType TargetType;
    public ExpectedType ExpectedType;
    public int countMax = 1;
    public int countAlive;
    public int countMaxActive = 1;
    public int countCurent = 0;
    public bool isUsePercent;
    public bool isTargetToWin = false;
    public bool disappearWhenCollider = false;
    [ShowIf(nameof(isUsePercent))] public float percentToWin = 100f;
    [ShowIf(nameof(isUsePercent), false)] public int numberToWin = 1;
}
