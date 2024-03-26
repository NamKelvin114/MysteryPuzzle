using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Spine.Unity;
using Spine;
using UniRx;
using UnityTimer;
using Worldreaver.Utility;
using Random = UnityEngine.Random;

public class PlayerManagerGameplay2 : MonoBehaviour, IFallByStone
{
    [SerializeField] private CharacterAnim PlayerAnim;
    [SerializeField] private SkeletonAnimation pet;
    public SkeletonAnimation realPet;
    [SerializeField] private GameObject dieEffect;
    [SerializeField] private GameObject pot;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject fxMatch;
    public Vector3 positionLeft;
    public Vector3 positionRight;
    [SerializeField] private List<SpineAnim> animsPotActiveList;
    [SerializeField] private List<ExtraAnim> animDieFirExtraAnims;
    [SerializeField] private List<TargetTypeSkin> targetTypeSkinList;
    [SpineEvent, SerializeField] private string eventAttack;
    [SpineAnimation] public string idleAnimationName;
    [SpineAnimation] public string winAnimationName;
    [SpineAnimation] public string win2AnimationName;
    [SpineAnimation] public string win3AnimationName;
    [SpineAnimation] public string dieAnimationName;
    [SpineAnimation] public string dieFireAnimationName;
    [SpineAnimation] public string freezeAnimationName;
    [SpineAnimation] public string failAnimationName;
    [SpineAnimation] public string fail1AnimationName;
    [SpineAnimation] public string moveAnimationName;
    [SpineAnimation] public string openAnimationName;
    [SpineAnimation] public string lootAnimationName;
    [SpineAnimation] public string winEventAnimationName;

    [Header("stick")] [SpineAnimation] public string idleWithStickAnimationName;
    [SpineAnimation] public string moveWithStickAnimationName;
    [SpineAnimation] public string attackAnimationName;

    [Header("super-anim")] [SpineAnimation]
    public string idleSuperMan;

    [SpineAnimation] public string winSuperManAnimationName;
    [SpineAnimation] public string win2SuperManAnimationName;
    [SpineAnimation] public string win3SuperManAnimationName;
    [SpineAnimation] public string winEventSuperManAnimationName;
    [SpineAnimation] public string moveSuperManAnimationName;
    [SpineAnimation] public string attackSuperManAnimationName;
    [SpineAnimation] public string openSuperManAnimationName;
    [SpineAnimation] public string superManApearAnimationName;
    [SpineAnimation] public string defaultIdle;
    public bool IsTakeSword { get; private set; }
    public bool IsTakeHolyWater { get; private set; }

    public SkeletonAnimation skeleton;
    public EUnitState state;
    private RaycastHit2D _hitStick;
    private RaycastHit2D _hitDown;
    private RaycastHit2D _hitObstacle;
    private RaycastHit2D _hitLeft;
    private RaycastHit2D _hitRight;
    private RaycastHit2D _hitEnemy;

    private Vector3 _endPosition, _startPosition;
    private EnemyBase _enemyBase;
    private BaseCannon _baseCannon;
    private bool _isCanMoveToTarget;
    private bool _isMoveLeft;
    private bool _isMoveRight;
    private IDisposable _disposableCollectGem;
    private IDisposable _disposableIdle;
    private bool _flagCollectGem;
    private Vector2 _movement;
    private Timer time;
    private bool is_pre_level_lose;
    private bool endGame = false;
    private TargetType _targetType;

    private void Awake()
    {
        //ChangeSkin();
    }

    // public void ChangeSkin()
    // {
    //     try
    //     {
    //         skeleton.ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item1);
    //         var (hasPet, nameSkin) = HeroData.HeroHasPetByIndex(Data.currentHero);
    //         if (hasPet)
    //         {
    //             if (pet != null)
    //             {
    //                 pet.gameObject.SetActive(true);
    //                 pet.ChangeSkin(nameSkin);
    //
    //                 if (skeleton.skeleton.ScaleX > 0)
    //                 {
    //                     pet.skeleton.ScaleX = -1;
    //                     pet.transform.localPosition = positionLeft;
    //                 }
    //                 else
    //                 {
    //                     pet.skeleton.ScaleX = 1;
    //                     pet.transform.localPosition = positionRight;
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             pet.gameObject.SetActive(false);
    //         }
    //
    //         if (realPet != null)
    //         {
    //             if (Data.currentPet == -1)
    //             {
    //                 SetStateRealPet(false);
    //             }
    //             else
    //             {
    //                 realPet.skeletonDataAsset = PetCollection.GetSkeletonAsset(Data.currentPet);
    //                 SetStateRealPet(true);
    //                 realPet.Initialize(true);
    //                 realPet.ChangeSkin($"Level{Data.petLevel}");
    //                 realPet.skeleton.ScaleX = skeleton.skeleton.ScaleX;
    //             }
    //         }
    //     }
    //     catch (Exception)
    //     {
    //         Timer.Register(0.1f, ChangeSkin);
    //     }
    // }

    private void SetStateRealPet(bool state)
    {
        realPet.transform.parent.gameObject.SetActive(state);
    }

    public bool IsRealPetActive() => realPet.transform.parent.gameObject.activeSelf;


    private void Start()
    {
        GameManager.instance.gTargetFollow = gameObject;
        MapLevelManager.Instance.endGameplay2 += EndGameplay2;
        endGame = false;
        SetupPotActive();
        OnIdleState();
        skeleton.AnimationState.Complete += delegate
        {
            if (skeleton.AnimationName.Equals(lootAnimationName)
                || skeleton.AnimationName.Equals(openAnimationName)
                || skeleton.AnimationName.Equals(openSuperManAnimationName))
            {
                StartCoroutine(IsShowWin());
            }
        };
        Observer.playerEndGameplay2 += EndGame;
        Observer.ActiveFxMatch += ActiveFxMatch;
        Observer.setPostionOfClothe?.Invoke(body.transform);
        Observer.CurrentSkinChanged += MixTargetTypeWhenCurrentSkinChange;
    }

    void ActiveFxMatch()
    {
        fxMatch.gameObject.SetActive(true);
    }

    string AnimDieFire(string initAnimPlayer)
    {
        var animDieFire = animDieFirExtraAnims.FirstOrDefault(t => t.initAnim == initAnimPlayer);
        if (animDieFire != null) return animDieFire.anim;
        return "";
    }

    private IEnumerator IsShowWin()
    {
        MapLevelManager.Instance.is_Plus_countPlayLevel = true;
        is_pre_level_lose = false;
        // Debug.Log("win-1");
        // yield return new WaitForSeconds(0.5f);
        //
        // _disposableIdle?.Dispose();
        // if (!skeleton.AnimationName.Equals(dieAnimationName) || state != EUnitState.Die)
        // {
        //     if (GameManager.instance.mapLevel.eQuestType == EQuestType.SaveHostage)
        //     {
        //         if (Data.idEasterEgg == -1 && Utils.isHardMode != true)
        //         {
        //             PlayWin2Animation(false);
        //             Timer.Register(0.66f, () => PlayWin3Animation());
        //         }
        //         else
        //         {
        //             PlayWinEventAnimation();
        //         }
        //     }
        //     else
        //     {
        //         // Debug.Log("win0");
        //         PlayWinAnimation();
        //         if (!(SoundManager.Instance.heroJumpWin is null))
        //             SoundManager.Instance.PlaySound(SoundManager.Instance.heroJumpWin);
        //         //skeleton.Initialize(true);
        //     }
        // }

        shadow.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        if (GameManager.instance.isShowLosing == false)
        {
            MapLevelManager.Instance.OnWin();
        }
    }
    public void GetStoneHit()
    {
        OnPlayerDie(EDieReason.Normal);
    }

    public void OnIdleState()
    {
        state = EUnitState.Playing;
        PlayAnim(PlayerAnim.initialIdle, true);
    }

    public void SetupPotActive()
    {
        var animIdle = animsPotActiveList.FirstOrDefault(t => t.anim == PlayerAnim.initialIdle);
        pot.SetActive(animIdle != null);
    }

    public void OnPlayerDie(EDieReason reason)
    {
        if (GameManager.instance.gameState != EGameState.Win)
        {
            
            state = EUnitState.Die;
            PlayAnimLosePet();
            transform.rotation = Quaternion.identity;
            pot.SetActive(false);
            Observer.playerDieGameplay2?.Invoke(false);
            skeleton.AnimationState.SetEmptyAnimation(1, 0.2f);
            switch (reason)
            {
                case EDieReason.Normal:
                    if (dieEffect != null)
                    {
                        dieEffect.SetActive(true);
                    }
                    PlayAnim(dieAnimationName, false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDie != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroDie);
                    break;
                case EDieReason.Fire:
                    string animDieFire = AnimDieFire(PlayerAnim.initialIdle);
                    if (animDieFire != "") PlayAnim(animDieFire, false);
                    else
                        PlayAnim(dieFireAnimationName, false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDieFire != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroDieFire);
                    break;
                case EDieReason.Despair:
                    if (dieEffect != null)
                    {
                        dieEffect.SetActive(false);
                    }
                    PlayAnim(failAnimationName, false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroCry != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroCry);
                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => { PlayAnim(fail1AnimationName, true); })
                        .AddTo(this);
                    break;
                case EDieReason.Ice:
                    PlayAnim(freezeAnimationName, false);
                    if (dieEffect != null)
                    {
                        dieEffect.SetActive(false);
                    }
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDieFire != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroDieFire);
                    break;
            }

            StartCoroutine(IEWait());
        }
    }

    private IEnumerator IEWait()
    {
        GameManager.instance.isShowLosing = true;
        yield return new WaitForSeconds(0.1f);
        MapLevelManager.Instance.OnLose();
    }

    private void EndGame(bool playerWin)
    {
        if (state == EUnitState.Die) return;
        if (endGame) return;
        endGame = true;
        DefaultAnims defaultAnims = PlayerAnim.defaultAnims();
        var animIdle = animsPotActiveList.FirstOrDefault(t => t.anim == PlayerAnim.initialIdle);
        //if (animIdle != null) pot.SetActive(false);
        if (playerWin)
        {
            if (defaultAnims.TypeRunListAnimWin == TypeRunListAnim.Random)
            {
                var animsWin = defaultAnims.winAnims;
                int randomPos = Random.Range(0, animsWin.Count() - 1);
                var anim = animsWin[randomPos];
                PlayAnim(anim, true);
            }
            else
            {
                PlayListAnim(defaultAnims.winAnims);
            }

            OnWin();
            if (!(SoundManager.Instance.heroJumpWin is null))
                SoundManager.Instance.PlaySound(SoundManager.Instance.heroJumpWin);
        }
        else
        {
            pot.gameObject.SetActive(false);
            if (SoundManager.Instance != null && SoundManager.Instance.heroCry != null)
                SoundManager.Instance.PlaySound(SoundManager.Instance.heroCry);
            PlayAnim(defaultAnims.loseAnims, true);
            MapLevelManager.Instance.OnLose();
        }
    }

    public bool PotActive()
    {
        var animIdle = animsPotActiveList.FirstOrDefault(t => t.anim == PlayerAnim.initialIdle);
        return animIdle != null;
    }

    public void MixItemSkin(TargetType targetType)
    {
        var targetTypeSkin = targetTypeSkinList.FirstOrDefault(t => t.targetType == targetType);
        if (targetTypeSkin != null)
        {
            var skeleton = this.skeleton.skeleton;
            var skeletonData = skeleton.Data;
            Observer.UpdateTempValue.Invoke(ETaskType.EatFoods);
            skeleton.Skin.AddSkin((skeletonData.FindSkin(targetTypeSkin.skinName)));
            this.skeleton.AnimationState.Apply(skeleton);
            skeleton.SetToSetupPose();
        }
    }

    private void PlayAnim(string anim, bool isLoop)
    {
        if (!skeleton.AnimationName.Equals(anim))
        {
            skeleton.AnimationState.SetAnimation(0, anim, isLoop);
        }
    }

    private void PlayListAnim(string[] anim)
    {
        bool firstAnimLoop = true;
        if (anim.Length > 1) firstAnimLoop = false;
        skeleton.skeleton.SetToSetupPose();
        skeleton.AnimationState.SetAnimation(0, anim[0], firstAnimLoop);
        for (int i = 1; i < anim.Length - 1; i++) skeleton.AnimationState.AddAnimation(0, anim[i], false, 0);
        skeleton.AnimationState.AddAnimation(0, anim[anim.Length - 1], true, 0);
    }

    private void PlayIdleAnimation(bool isLoop = true)
    {
        if (IsTakeSword)
        {
            PlayAnim(idleWithStickAnimationName, isLoop);
        }
        else if (IsTakeHolyWater)
        {
            PlayAnim(idleSuperMan, isLoop);
        }
        else
        {
            PlayAnim(idleAnimationName, isLoop);
        }

        PlayIdlePet();
    }

    private void PlayIdlePet()
    {
        if (realPet != null && IsRealPetActive())
        {
            realPet.loop = true;
            realPet.AnimationName = "Idle";
        }
    }

    private void PlayAnimWinPet()
    {
        if (realPet != null && IsRealPetActive())
        {
            realPet.loop = true;
            realPet.AnimationName = "Win";
        }
    }

    private void PlayAnimLosePet()
    {
        if (realPet != null && IsRealPetActive())
        {
            realPet.loop = true;
            realPet.AnimationName = "Lose";
        }
    }

    private void PlayMoveAnimation(bool isLoop = true)
    {
        if (IsTakeSword)
        {
            PlayAnim(moveWithStickAnimationName, isLoop);
        }
        else if (IsTakeHolyWater)
        {
            PlayAnim(moveSuperManAnimationName, isLoop);
        }
        else
        {
            PlayAnim(moveAnimationName, isLoop);
        }

        if (realPet != null && IsRealPetActive())
        {
            realPet.loop = isLoop;
            realPet.AnimationName = "Run";
        }
    }

    private void PlayAttackAnimation()
    {
        if (IsTakeHolyWater)
        {
            PlayAnim(attackSuperManAnimationName, false);
        }
        else
        {
            PlayAnim(attackAnimationName, false);
        }

        PlayIdlePet();
    }

    private void PlayOpenAnimation(bool isSaveWife = false)
    {
        if (IsTakeHolyWater)
        {
            PlayAnim(openSuperManAnimationName, false);
        }
        else
        {
            PlayAnim(isSaveWife ? openAnimationName : lootAnimationName, false);
        }

        PlayIdlePet();
    }

    private void PlayWinAnimation(bool isLoop = true)
    {
        PlayAnim(IsTakeHolyWater ? winSuperManAnimationName : winAnimationName, isLoop);
        PlayIdlePet();
    }

    private void PlayWin2Animation(bool isLoop = true)
    {
        PlayAnim(IsTakeHolyWater ? win2SuperManAnimationName : win2AnimationName, isLoop);
        PlayIdlePet();
    }

    private void PlayWinEventAnimation(bool isLoop = true)
    {
        PlayAnim(IsTakeHolyWater ? winEventSuperManAnimationName : winEventAnimationName, isLoop);
        PlayIdlePet();
    }

    private void PlayWin3Animation(bool isLoop = true)
    {
        PlayAnim(IsTakeHolyWater ? win3SuperManAnimationName : win3AnimationName, isLoop);
        PlayIdlePet();
    }

    private void PlaySuperManApear()
    {
        PlayAnim(superManApearAnimationName, false);
        PlayIdlePet();
    }

    public void PlaySound()
    {
        if (SoundManager.Instance == null) return;
        var index = UnityEngine.Random.Range(0, SoundManager.Instance.acCoinApear.Length);
        StartCoroutine(PlaySoundCoinApear(index));
    }

    private IEnumerator PlaySoundCoinApear(int ranIndex)
    {
        yield return new WaitForSeconds(0.1f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.acCoinApear[ranIndex]);
    }

    private IEnumerator DelayWin()
    {
        // Debug.Log("win3");
        yield return new WaitForSeconds(1f);
        if (state != EUnitState.Die)
        {
            GameManager.instance.gameState = EGameState.Win;
            state = EUnitState.Win;
        }

        // StartCoroutine(IsShowWin());
        if (GameManager.instance.isShowLosing == false)
        {
            MapLevelManager.Instance.OnWin();
        }
    }

    private bool _win;

    public void OnWin()
    {
        // transform.rotation = Quaternion.identity;
        // if (HostageManager.instance != null)
        // {
        //     skeleton.skeleton.ScaleX =
        //         Utils.ChangeScale(transform.position, HostageManager.instance.transform.position);
        //     var result = HeroData.HeroHasPetByIndex(Data.currentHero);
        //     if (result.Item1)
        //     {
        //         if (skeleton.skeleton.ScaleX > 0)
        //         {
        //             pet.skeleton.ScaleX = -1;
        //             pet.transform.localPosition = positionLeft;
        //         }
        //         else
        //         {
        //             pet.skeleton.ScaleX = 1;
        //             pet.transform.localPosition = positionRight;
        //         }
        //     }
        // }
        //
        GameManager.instance.gameState = EGameState.Win;
        StartCoroutine(IsShowWin());
        //PlayAnimWinPet();
    }

    public void EndGameplay2(TargetType targetType, ExpectedType expectedType)
    {
        if (state == EUnitState.Die) return;
        if (endGame) return;
        endGame = true;
        Observer.StopCheckPinLoseCondition?.Invoke();
        ElementAction elementAction = PlayerAnim.listAnims(targetType);
        string[] anims = elementAction.anims;
        MixItemSkin(targetType);
        _targetType = targetType;
        elementAction.callBack?.Invoke(targetType);
        if (expectedType == ExpectedType.Expected)
        {
            if (!(SoundManager.Instance.heroJumpWin is null))
                SoundManager.Instance.PlaySound(SoundManager.Instance.heroJumpWin);
        }
        else
        {
            if (SoundManager.Instance != null && SoundManager.Instance.heroCry != null)
                SoundManager.Instance.PlaySound(SoundManager.Instance.heroCry);
        }

        StartCoroutine(EndGameplay2(anims, expectedType));
    }

    private IEnumerator EndGameplay2(string[] EndLevelAnim, ExpectedType expectedType)
    {
        if (state == EUnitState.Die) yield break;
        DOTween.Sequence().AppendInterval(1f).OnComplete(() => Observer.UnActiveWaterPosion?.Invoke());
        PlayListAnim(EndLevelAnim);
        yield return new WaitForSeconds(1f);
        shadow.gameObject.SetActive(false);
        if (expectedType == ExpectedType.Expected)
        {
            MapLevelManager.Instance.OnWin();
        }
        else
        {
            //pot.gameObject.SetActive(false);
            MapLevelManager.Instance.OnLose();
        }
    }

    private void MixTargetTypeWhenCurrentSkinChange()
    {
        MixItemSkin(_targetType);
    }

    private void OnDestroy()
    {
        MapLevelManager.Instance.endGameplay2 -= EndGameplay2;
        Observer.CurrentSkinChanged -= MixTargetTypeWhenCurrentSkinChange;
        Observer.ActiveFxMatch -= ActiveFxMatch;
        Observer.playerEndGameplay2 -= EndGame;
    }
}

[Serializable]
class TargetTypeSkin
{
    public TargetType targetType;
    [SpineSkin] public string skinName;
}

[Serializable]
public class SpineAnim
{
    [SpineAnimation] public string anim;
}

[Serializable]
public class ExtraAnim
{
    [SpineAnimation] public string initAnim;
    [SpineAnimation] public string anim;
}
