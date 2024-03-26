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

public class PlayerManager : MonoBehaviour, IBombTrigger, IExplodeReceiver, IBeeTringger, IPushAway
{
    public bool is_pre_level_lose;
    [SerializeField] private List<SpineAnim> animsWin;
    [SerializeField] private SkeletonAnimation pet;
    public SkeletonAnimation realPet;
    [SerializeField] private GameObject dieEffect;
    public Vector3 positionLeft;
    public Vector3 positionRight;
    public static PlayerManager instance;
    [SpineEvent, SerializeField] private string eventAttack;
    [SpineAnimation] public string idleAnimationName;
    [SpineAnimation] public string winAnimationName;
    [SpineAnimation] public string win2AnimationName;
    [SpineAnimation] public string win3AnimationName;
    [SpineAnimation] public string dieAnimationName;
    [SpineAnimation] public string dieFireAnimationName;
    [SpineAnimation] public string freezeAnimationName;
    [SpineAnimation] public string swoonAnimationName;
    [SpineAnimation] public string beeAnimationName;
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

    //Edit AnimationCurve when Level need to use Spring
    [Header("AnimationCurve")] [SerializeField]
    private AnimationCurve animationCurve;

    private float _startValueCurve = 0;
    [SerializeField] private float durationCurve;
    public Transform center, left, right, leftAttack, rightAttack;
    public bool IsTakeSword { get; private set; }
    public bool IsTakeHolyWater { get; private set; }
    public bool IsJump { get; set; }
    private Guid _guid;
    Sequence _pushSequence;

    public LayerMask lmColl;
    public LayerMask lmMapObject;
    public LayerMask lmWhenSuperMode;
    public SkeletonAnimation skeleton;
    public Rigidbody2D rigid;
    public float moveSpeed;
    public EUnitState state;

    [HideInInspector] public bool beginMove = false;

    [Range(0.01f, 1f)] public float durationMoveCollectGem = 0.5f;
    [Range(0.01f, 1f)] public float durationIncreasePerGem = 0.05f;
    [SerializeField] private Transform gemGroupTransform;
    private RaycastHit2D _hitStick;
    private RaycastHit2D _hitDown;
    private RaycastHit2D _hitObstacle;
    private RaycastHit2D _hitLeft;
    private RaycastHit2D _hitRight;
    private RaycastHit2D _hitEnemy;

    private Vector3 _endPosition, _startPosition;
    private EnemyBase _enemyBase;
    private BaseCannon _baseCannon;
    private bool _isCollect;
    private bool _isCanMoveToTarget;
    private bool _isPushing;
    protected Vector3 vStartHitDown;
    protected Vector3 vEndHitDown;
    protected Vector2 vEnd;
    protected Vector2 vStart;
    protected Vector2 vStartForward;
    protected Vector2 vEndForward;
    protected Vector2 vStartHit;
    protected Vector2 vEndHit;
    private bool _isMoveLeft;
    private bool _isMoveRight;
    private IDisposable _disposableCollectGem;
    private IDisposable _disposableIdle;
    private bool _flagCollectGem;
    private Vector2 _movement;
    public GameObject Target;
    public bool FlagAttackAffterCollect { get; set; }
    public bool CallWin { get; set; }
    private bool CallIsShowWin = false;
    private Timer time;
    private float _durationCurve;

    public Timer Timer
    {
        get { return time; }
        set => time = value;
    }

    [SerializeField] public CHECKATTACK _checkattack;

    public enum CHECKATTACK
    {
        Attack,
        idle
    }

    private void Awake()
    {
        instance = this;
    }

    public void ChangeSkin()
    {
        // try
        // {
        //     skeleton.ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item1);
        //
        //     var (hasPet, nameSkin) = HeroData.HeroHasPetByIndex(Data.currentHero);
        //     if (hasPet)
        //     {
        //         if (pet != null)
        //         {
        //             pet.gameObject.SetActive(true);
        //             pet.ChangeSkin(nameSkin);
        //
        //             if (skeleton.skeleton.ScaleX > 0)
        //             {
        //                 pet.skeleton.ScaleX = -1;
        //                 pet.transform.localPosition = positionLeft;
        //             }
        //             else
        //             {
        //                 pet.skeleton.ScaleX = 1;
        //                 pet.transform.localPosition = positionRight;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         pet.gameObject.SetActive(false);
        //     }
        //
        //     if (realPet != null)
        //     {
        //         if (Data.currentPet == -1)
        //         {
        //             SetStateRealPet(false);
        //         }
        //         else
        //         {
        //             realPet.skeletonDataAsset = PetCollection.GetSkeletonAsset(Data.currentPet);
        //             SetStateRealPet(true);
        //             realPet.Initialize(true);
        //             realPet.ChangeSkin($"Level{Data.petLevel}");
        //             realPet.skeleton.ScaleX = skeleton.skeleton.ScaleX;
        //         }
        //     }
        // }
        // catch (Exception)
        // {
        //     Timer.Register(0.1f, ChangeSkin);
        // }
    }

    private void SetStateRealPet(bool state)
    {
        realPet.transform.parent.gameObject.SetActive(state);
    }

    public void GetPushed(SpringItem spring)
    {
        DOTween.Kill(_guid);
        _pushSequence = null;
        durationCurve = _durationCurve;
        _startValueCurve = 0;
        bool preState = beginMove;
        beginMove = false;
        _isPushing = true;
        // skeleton.skeleton.ScaleX *= -1;
        Vector2 speed = new Vector2();
        if (transform.position.x < spring.transform.position.x)
        {
            speed = Vector2.left;
        }
        else
        {
            speed = Vector2.right;
        }

        if (_pushSequence == null)
        {
            _pushSequence = DOTween.Sequence();
            _pushSequence.Append(DOTween.To(() => _startValueCurve, x => _startValueCurve = x,
                    animationCurve.keys[animationCurve.length - 1].time, durationCurve).OnStart((() =>
                {
                    //target = null;
                }))
                .OnUpdate((() =>
                {
                    if (state != EUnitState.Die && GameManager.instance.gameState != EGameState.Win &&
                        GameManager.instance.gameState != EGameState.Lose)
                    {
                        rigid.velocity = new Vector2(speed.x * animationCurve.Evaluate(_startValueCurve),
                            rigid.velocity.y);
                        PlayIdleAnimation();
                    }
                })).OnComplete((() =>
                {
                    if (preState)
                    {
                        PrepareRotate();
                    }
                    else
                    {
                        beginMove = preState;
                    }

                    _isCanMoveToTarget = false;
                    _isPushing = false;
                    _startValueCurve = 0;
                })));
            _guid = System.Guid.NewGuid();
            _pushSequence.id = _guid;
        }

        _pushSequence.Play();
    }


    public bool IsRealPetActive() => realPet.transform.parent.gameObject.activeSelf;

    private void Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name.Equals(eventAttack))
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acMeleeAttack);

            if (_enemyBase != null && _enemyBase._charStage != EnemyBase.CHAR_STATE.DIE)
                _enemyBase.OnDie(EDieReason.Normal);
            else
            {
                if (Target != null)
                {
                    var cannon = Target.GetComponentInParent<BaseCannon>();
                    if (cannon)
                    {
                        cannon.PlayDeathAnimation();
                        cannon.DoBreak();
                    }
                    else
                    {
                        _enemyBase = Target.GetComponentInParent<EnemyBase>();
                        if (_enemyBase != null) _enemyBase.OnDie(EDieReason.Normal);
                    }
                }
            }
        }
    }


    private void Start()
    {
        _durationCurve = durationCurve;
        _checkattack = CHECKATTACK.idle;
        GameManager.instance.gTargetFollow = gameObject;

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
        skeleton.AnimationState.Event += Event;
        skeleton.AnimationState.Start += HandleEndAnimation;
    }

    public void OnGetBeeAttack()
    {
        OnDeath(EDieReason.Bee);
    }

    protected void HandleEndAnimation(TrackEntry trackentry)
    {
        if (GameManager.instance.gameState == EGameState.Win || GameManager.instance.gameState == EGameState.Lose)
        {
            skeleton.AnimationState.Start -= HandleEndAnimation;
        }

        if (trackentry.Animation.Name.Equals(attackAnimationName))
        {
            time = Timer.Register(skeleton.DurationAnimation(attackAnimationName), () => PlayIdleAnimation());
        }
        else if (trackentry.Animation.Name.Equals(attackSuperManAnimationName))
        {
            Timer = Timer.Register(skeleton.DurationAnimation(attackSuperManAnimationName), () => PlayIdleAnimation());
        }
    }

    private IEnumerator IeWaitToIdleAfterAttack()
    {
        yield return new WaitForSeconds(0.73f);
        OnIdleState();
    }

    private IEnumerator IsShowWin()
    {
        if (state == EUnitState.Die) yield break;
        if (CallIsShowWin) yield break;
        CallIsShowWin = true;
        MapLevelManager.Instance.is_Plus_countPlayLevel = true;
        is_pre_level_lose = false;
        // Debug.Log("win-1");
        yield return new WaitForSeconds(1.5f);
        if (state == EUnitState.Die) yield break;
        _disposableIdle?.Dispose();
        if (!skeleton.AnimationName.Equals(dieAnimationName) || state != EUnitState.Die)
        {
            if (GameManager.instance.mapLevel.eQuestType == EQuestType.SaveHostage)
            {
                if (Data.idEasterEgg == -1 && Utils.isHardMode != true)
                {
                    PlayWin2Animation(false);
                    Timer.Register(0.66f, () => PlayWin3Animation());
                }
                else
                {
                    PlayWinEventAnimation();
                }
            }
            else
            {
                // Debug.Log("win0");
                PlayWinAnimation();
                if (!(SoundManager.Instance.heroJumpWin is null))
                    SoundManager.Instance.PlaySound(SoundManager.Instance.heroJumpWin);
                //skeleton.Initialize(true);
            }
        }

        yield return new WaitForSeconds(0.5f);
        if (state == EUnitState.Die) yield break;
        if (GameManager.instance.isShowLosing == false)
        {
            MapLevelManager.Instance.OnWin();
        }
    }

    private void HeroJump()
    {
        if (state != EUnitState.Die && state != EUnitState.Win)
        {
            if (!IsJump)
            {
                rigid.velocity = new Vector2(skeleton.skeleton.ScaleX, 4);
                IsJump = true;
            }
        }
    }

    private void CheckStickBarrier()
    {
        if (state == EUnitState.Die)
            return;
        _startPosition = body.position;
        _endPosition = skeleton.skeleton.ScaleX > 0
            ? rightCheckStick.transform.position
            : leftCheckStick.transform.position;
        _hitStick = Physics2D.Linecast(_startPosition, _endPosition, lmColl);
#if UNITY_EDITOR
        Debug.DrawLine(_startPosition, _endPosition, Color.yellow);
#endif
    }

    private void OnMoveToTarget()
    {
        if (GameManager.instance.FlagCharacterCollectGem || CallWin)
            return;
        if (GameManager.instance.gameState == EGameState.Playing && state != EUnitState.Die && Target != null &&
            _isPushing == false)
        {
            var scaleDireaction = Utils.ChangeScale(transform.position, Target.transform.position);
            skeleton.skeleton.ScaleX = scaleDireaction;
            if (realPet != null && IsRealPetActive())
            {
                realPet.skeleton.ScaleX = scaleDireaction;
            }

            var direction = transform.position.x < Target.transform.position.x ? Vector2.right : Vector2.left;

            //_rig2D.velocity = moveSpeed * (saPlayer.skeleton.ScaleX > 0 ? Vector2.right : Vector2.left);
            if (skeleton.AnimationName.Equals(lootAnimationName) || skeleton.AnimationName.Equals(openAnimationName) ||
                skeleton.AnimationName.Equals(openSuperManAnimationName)) return;
            var pos = moveSpeed * direction;
            rigid.velocity = new Vector2(pos.x, rigid.velocity.y);
            PlayMoveAnimation();
        }
    }

    private void HitDownMapObject()
    {
        if (state == EUnitState.Die) return;
        vStartHitDown = ground.transform.position;
        vEndHitDown = new Vector2(vStartHitDown.x, vStartHitDown.y - 0.15f);
        _hitDown = Physics2D.Linecast(vStartHitDown, vEndHitDown, lmMapObject);
#if UNITY_EDITOR
        Debug.DrawLine(vStartHitDown, vEndHitDown, Color.red);
#endif
    }

    public Transform leftCheckStick, rightCheckStick;
    public Transform leftJump, rightJump, ground, body;
    private Vector2 _checkJumpStart;
    private Vector2 _checkJumpEnd;

    private void CheckVatCan()
    {
        if (state == EUnitState.Die || !beginMove)
            return;
        _checkJumpStart = ground.transform.position;
        _checkJumpEnd = skeleton.skeleton.ScaleX > 0 ? rightJump.transform.position : leftJump.transform.position;
        _hitObstacle = Physics2D.Linecast(_checkJumpStart, _checkJumpEnd, lmMapObject);
#if UNITY_EDITOR
        Debug.DrawLine(_checkJumpStart, _checkJumpEnd, Color.red);
#endif
    }

    private void FixedUpdate()
    {
        if (state == EUnitState.Die || GameManager.instance.gameState == EGameState.Win ||
            GameManager.instance.gameState == EGameState.Lose || _isPushing)
        {
            return;
        }

        //if (!GameManager.instance.playerMove && !IsTakeHolyWater) return;
        if (IsStop()) return;

        CheckStickBarrier();
        CheckVatCan();
        HitDownMapObject();
        if (_hitDown.collider != null)
        {
            if (IsJump) IsJump = false;

            if (!beginMove)
            {
                if (!IsTakeSword && !IsTakeHolyWater)
                {
                    if (GameManager.instance.mapLevel.lstAllStick.Count <= 0) PrepareRotate();
                }

                if (IsTakeHolyWater)
                {
                    #region Check Hit Ahead

                    vStart = center.position;
                    vEnd = left.position;

                    vStartForward = center.position;
                    vEndForward = right.position;

#if UNITY_EDITOR
                    Debug.DrawLine(vStart, vEnd, Color.red);
                    Debug.DrawLine(vStartForward, vEndForward, Color.green);
#endif

                    _hitRight = Physics2D.Linecast(vStartForward, vEndForward, lmWhenSuperMode);
                    _hitLeft = Physics2D.Linecast(vStart, vEnd, lmWhenSuperMode);

                    if (_hitEnemy.collider == null)
                    {
                        if (_hitRight.collider != null)
                        {
                            if (!_hitRight.collider.CompareTag(Utils.TAG_STICKBARRIE) &&
                                !_hitRight.collider.CompareTag("Chan") &&
                                !_hitRight.collider.CompareTag("StickBarrie"))
                            {
                                _isCanMoveToTarget = true;
                                Target = _hitRight.collider.gameObject;

                                if (_isCanMoveToTarget) OnMoveToTarget();
                            }
                        }

                        if (_hitLeft.collider != null)
                        {
                            if (!_hitLeft.collider.CompareTag(Utils.TAG_STICKBARRIE) &&
                                !_hitLeft.collider.CompareTag("Chan") &&
                                !_hitLeft.collider.CompareTag("StickBarrie"))
                            {
                                _isCanMoveToTarget = true;
                                Target = _hitLeft.collider.gameObject;

                                if (_isCanMoveToTarget) OnMoveToTarget();
                            }
                        }
                    }

                    #endregion

                    #region Check Hit Player

                    vStartHit = center.position;

                    vEndHit = skeleton.skeleton.ScaleX < 0
                        ? new Vector2(rightAttack.transform.position.x, rightAttack.transform.position.y)
                        : new Vector2(leftAttack.transform.position.x, leftAttack.transform.position.y);
                    _hitEnemy = Physics2D.Linecast(vStartHit, vEndHit, lmWhenSuperMode);
#if UNITY_EDITOR
                    Debug.DrawLine(vStartHit, vEndHit, Color.magenta);
#endif
                    if (_hitEnemy.collider != null && !_hitEnemy.collider.CompareTag("StickBarrie"))
                    {
                        if (_hitEnemy.collider.CompareTag("Hostage") || _hitEnemy.collider.CompareTag("Chest") ||
                            _hitEnemy.collider.gameObject.layer == LayerMask.NameToLayer("RoomItem"))
                        {
                            Target = _hitEnemy.collider.gameObject;
                            OnMoveToTarget();
                        }
                        else
                        {
                            if (Target == null) Target = _hitEnemy.collider.gameObject;

                            OnPrepareAttack();
                        }
                    }

                    #endregion
                }
            }
        }

        if (_hitObstacle.collider != null)
        {
            if (!_hitObstacle.collider.gameObject.CompareTag("Wall_Bottom"))
            {
                if (_hitObstacle.collider.gameObject.CompareTag("Tag_Stone") &&
                    _hitObstacle.collider.gameObject.name != "FallingStone" ||
                    _hitObstacle.collider.gameObject.CompareTag("Chan"))
                {
                    HeroJump();
                }
            }
        }

        if (!IsCanMove())
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else
        {
            if (beginMove)
            {
                if (state != EUnitState.Die && state != EUnitState.Win)
                {
                    if (_hitDown.collider != null)
                    {
                        beginMove = true;
                        if (skeleton.skeleton.ScaleX < 0)
                        {
                            _isMoveLeft = true;
                            _isMoveRight = false;
                        }
                        else
                        {
                            _isMoveLeft = false;
                            _isMoveRight = true;
                        }
                    }
                }

                if (GameManager.instance.gameState != EGameState.Win)
                {
                    if (_isMoveLeft) MoveLeft();
                    else if (_isMoveRight) MoveRight();
                }
            }
        }
    }

    public virtual void OnPrepareAttack()
    {
        _checkattack = CHECKATTACK.Attack;
        if (GameManager.instance.gameState == EGameState.Win ||
            GameManager.instance.gameState == EGameState.Lose) return;

        rigid.velocity = new Vector2(0, rigid.velocity.y);

        if (_hitEnemy.collider != null)
        {
            if (state != EUnitState.Die)
            {
                PlayAttackAnimation();
                StartCoroutine(CountToEndAttack(0.833f));
            }
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private bool IsCanMove()
    {
        if (GameManager.instance.FlagCharacterCollectGem || CallWin)
            return false;
        return state != EUnitState.Die || state != EUnitState.Win;
    }

    #region Player movement

    private void MoveLeft()
    {
        if (GameManager.instance.FlagCharacterCollectGem || CallWin)
            return;
        if (_hitStick.collider != null && !IsTakeSword && !IsTakeHolyWater)
        {
            if (state != EUnitState.Die)
            {
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                if (IsStop()) return;
                PlayMoveAnimation();
            }

            return;
        }

        if (state != EUnitState.Die)
        {
            _movement = Vector2.left * moveSpeed;
            rigid.velocity = new Vector2(_movement.x, rigid.velocity.y);
            if (IsStop()) return;
            PlayMoveAnimation();
        }
        else rigid.velocity = new Vector2(0, rigid.velocity.y);
    }

    public void MoveRight()
    {
        if (GameManager.instance.FlagCharacterCollectGem || CallWin)
            return;
        if (_hitStick.collider != null && !IsTakeSword && !IsTakeHolyWater)
        {
            if (state != EUnitState.Die)
            {
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                if (IsStop()) return;
                PlayMoveAnimation();
            }

            return;
        }

        if (state != EUnitState.Die)
        {
            _movement = Vector2.right * moveSpeed;
            rigid.velocity = new Vector2(_movement.x, rigid.velocity.y);
            if (IsStop()) return;
            PlayMoveAnimation();
        }
        else rigid.velocity = new Vector2(0, rigid.velocity.y);
    }

    void CheckChangeDir()
    {
        var _hitChan = Physics2D.Linecast(vStartHit, vEndHit, lmWhenSuperMode);
    }

    private IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(0.25f);

        if (!IsStop() && !IsTakeSword && !IsTakeHolyWater)
        {
            if (state != EUnitState.Die && state != EUnitState.Win)
            {
                if (_hitDown.collider != null)
                {
                    beginMove = true;
                    if (skeleton.skeleton.ScaleX < 0)
                    {
                        _isMoveLeft = true;
                        _isMoveRight = false;
                    }
                    else
                    {
                        _isMoveLeft = false;
                        _isMoveRight = true;
                    }
                }
            }
        }
    }

    public void PrepareRotate()
    {
        if (!GameManager.instance.playerMove) return;

        StartCoroutine(DelayMove());
    }

    #endregion

    #region animation

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
        if (GameManager.instance.FlagCharacterCollectGem || CallWin)
            return;
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

    private void PlayDeathAnimation()
    {
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
        if (state == EUnitState.Die) return;
        int randomPos = Random.Range(0, animsWin.Count);
        var anim = animsWin[randomPos].anim;
        PlayAnim(anim, isLoop);
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

    #endregion

    #region Player action

    public void OnIdleState()
    {
        state = EUnitState.Playing;
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        beginMove = false;
        PlayIdleAnimation();
    }

    public IEnumerator CountToEndAttack(float time)
    {
        yield return new WaitForSeconds(time);
        if (state != EUnitState.Die)
        {
            _checkattack = CHECKATTACK.idle;
        }
        else
        {
            state = EUnitState.Die;
        }
    }

    public void OnAttackEnemy(EnemyBase enemy)
    {
        if (state == EUnitState.Die)
        {
            return;
        }

        _enemyBase = enemy;
        if (_enemyBase != null)
        {
            var scaleDirection = Utils.ChangeScale(transform.position, _enemyBase.transform.position);
            skeleton.skeleton.ScaleX = scaleDirection;
            if (realPet != null && IsRealPetActive())
            {
                realPet.skeleton.ScaleX = scaleDirection;
            }

            var result = HeroData.HeroHasPetByIndex(Data.currentHero);
            if (result.Item1)
            {
                if (skeleton.skeleton.ScaleX > 0)
                {
                    pet.skeleton.ScaleX = -1;
                    pet.transform.localPosition = positionLeft;
                }
                else
                {
                    pet.skeleton.ScaleX = 1;
                    pet.transform.localPosition = positionRight;
                }
            }
        }

        PlayAttackAnimation();
        StartCoroutine(CountToEndAttack(0.833f));

        beginMove = false;
        _isMoveLeft = false;
        _isMoveRight = false;
        rigid.velocity = new Vector2(0, rigid.velocity.y);

        _disposableIdle = Observable.FromCoroutine(IeWaitToIdleAfterAttack).Subscribe().AddTo(this);
    }

    public void OnTakeSword(Transform tr)
    {
        IsTakeSword = true;
        tr.gameObject.SetActive(false);

        if (GameManager.instance.isTest)
        {
            skeleton.ChangeSkin(Constants.MAIN_DEFAULT_SKIN_ATTACK);
        }
        else
        {
            skeleton.ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item2);
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeSword);

        _isMoveRight = false;
        _isMoveLeft = false;
        OnIdleState();
    }

    public void OnTakeHolyWater(Transform holyWater)
    {
        IsTakeHolyWater = true;
        holyWater.gameObject.SetActive(false);

        skeleton.ChangeSkin(HeroData.TransformSuperMan());
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeSword);
        _isMoveRight = false;
        _isMoveLeft = false;
        // play Apear
        state = EUnitState.Playing;
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        rigid.angularVelocity = 0;
        beginMove = false;
        PlaySuperManApear();
        Timer.Register(0.67f, () => PlayIdleAnimation());
    }

    public void OnPlayerDie(EDieReason reason)
    {
        if (state == EUnitState.Die) return;
        if (GameManager.instance.gameState != EGameState.Win)
        {
            DisposeCollectGem();
            state = EUnitState.Die;
            PlayAnimLosePet();
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            transform.rotation = Quaternion.identity;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

            skeleton.AnimationState.SetEmptyAnimation(1, 0.2f);
            switch (reason)
            {
                case EDieReason.Normal:
                    dieEffect.SetActive(true);
                    var coll = ground.GetComponent<BoxCollider2D>();
                    if (coll != null)
                    {
                        coll.size = new Vector2(0.1f, 0.01f);
                        coll.offset = new Vector2(0, -0.1f);
                    }

                    PlayAnim(dieAnimationName, false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDie != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroDie);
                    break;
                case EDieReason.Fire:
                    PlayAnim(dieFireAnimationName, false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDieFire != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroDieFire);
                    break;
                case EDieReason.Despair:
                    dieEffect.SetActive(false);
                    PlayAnim(failAnimationName, true);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroCry != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroCry);
                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => { PlayAnim(fail1AnimationName, true); })
                        .AddTo(this);
                    break;
                case EDieReason.Ice:
                    PlayAnim(freezeAnimationName, false);
                    dieEffect.SetActive(false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDieFire != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heroDieFire);
                    break;
                case EDieReason.Rat:
                    PlayAnim(swoonAnimationName, false);
                    dieEffect.SetActive(false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDieFire != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heoroScream);
                    break;
                case EDieReason.Bee:
                    PlayAnim(beeAnimationName, true);
                    dieEffect.SetActive(false);
                    if (SoundManager.Instance != null && SoundManager.Instance.heroDieFire != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.heoroScream);
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

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ChangeDir"))
        {
            if (collision.transform.localScale.x > 0)
            {
                _isMoveLeft = false;
                _isMoveRight = true;
            }
            else
            {
                _isMoveLeft = true;
                _isMoveRight = false;
            }

            if (IsTakeSword || IsTakeHolyWater) beginMove = true;
            skeleton.skeleton.ScaleX = collision.transform.localScale.x > 0 ? 1 : -1;
            if (realPet != null && IsRealPetActive())
            {
                realPet.skeleton.ScaleX = skeleton.skeleton.ScaleX;
            }

            var result = HeroData.HeroHasPetByIndex(Data.currentHero);
            if (result.Item1)
            {
                if (skeleton.skeleton.ScaleX > 0)
                {
                    pet.skeleton.ScaleX = -1;
                    pet.transform.localPosition = positionLeft;
                }
                else
                {
                    pet.skeleton.ScaleX = 1;
                    pet.transform.localPosition = positionRight;
                }
            }
        }
        else if (collision.CompareTag("Trap_Other"))
        {
            if (IsTakeHolyWater) return;

            if (!GameManager.instance.FlagCharacterDie)
            {
                GameManager.instance.FlagCharacterDie = true;
                GameManager.instance.FailureButton();
            }

            if (state == EUnitState.Playing || state == EUnitState.Running)
            {
                if (GameManager.instance.gameState != EGameState.Win)
                {
                    OnPlayerDie(EDieReason.Normal);
                }
            }
        }
        else if (collision.CompareTag("arrow"))
        {
            if (IsTakeHolyWater) return;
            if (GameManager.instance.targetCollects.Exists(collision.transform))
            {
                GameManager.instance.targetCollects.Remove(collision.transform);
            }

            if (!GameManager.instance.FlagCharacterDie)
            {
                GameManager.instance.FlagCharacterDie = true;
                GameManager.instance.FailureButton();
            }

            collision.gameObject.SetActive(false);
            if (state == EUnitState.Playing || state == EUnitState.Running)
            {
                if (GameManager.instance.gameState != EGameState.Win)
                {
                    OnPlayerDie(EDieReason.Normal);
                }
            }
        }

        else if (collision.CompareTag("Tag_Win") && !instance.FlagAttackAffterCollect)
        {
            switch (MapLevelManager.Instance.eQuestType)
            {
                case EQuestType.Collect:
                    instance.CallWin = false;
                    break;
                case EQuestType.OpenChest:
                    instance.CallWin = true;
                    break;
            }

            if (GameManager.instance.gameState == EGameState.Playing)
            {
                if (instance.skeleton.AnimationName.Equals(instance.attackAnimationName))
                {
                    instance.FlagAttackAffterCollect = true;
                    Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => { Collect(); })
                        .AddTo(instance.gameObject);
                    return;
                }

                Collect();
            }
            else if (GameManager.instance.gameState == EGameState.Win &&
                     MapLevelManager.Instance.eQuestType == EQuestType.OpenChest)
            {
                Collect();
            }

            void Collect()
            {
                if (state == EUnitState.Die) return;
                if (!GameManager.instance.FlagCharacterCollectGem)
                {
                    PlaySound();
                    GameManager.instance.FlagCharacterCollectGem = true;
                    GameManager.instance.SuccessButton();
                    instance.StartCollectGem();
                    //Utils.SetTaskProcess(ETaskType.CollectGold, Utils.GetTaskProcess(ETaskType.CollectGold) + 1);
                    //Debug.Log(Utils.GetTaskProcess(ETaskType.CollectGold));
                }

                if (!instance.CallWin)
                {
                    instance.CallWin = true;
                    instance.OnWin(true);
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
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
        DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
        {
            if (state != EUnitState.Die)
            {
                state = EUnitState.Win;
            }

            // StartCoroutine(IsShowWin());
            if (GameManager.instance.isShowLosing == false)
            {
                MapLevelManager.Instance.OnWin();
            }
        });
    }

    private bool _win;

    public void OnWin(bool playcollect, bool saveMyWife = false)
    {
        if (state == EUnitState.Die) return;
        //DOTween.KillAll();
        Observer.StopCheckPinLoseCondition?.Invoke();
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        transform.rotation = Quaternion.identity;
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        beginMove = false;
        CallWin = true;
        if (HostageManager.instance != null)
        {
            skeleton.skeleton.ScaleX =
                Utils.ChangeScale(transform.position, HostageManager.instance.transform.position);
            var result = HeroData.HeroHasPetByIndex(Data.currentHero);
            if (result.Item1)
            {
                if (skeleton.skeleton.ScaleX > 0)
                {
                    pet.skeleton.ScaleX = -1;
                    pet.transform.localPosition = positionLeft;
                }
                else
                {
                    pet.skeleton.ScaleX = 1;
                    pet.transform.localPosition = positionRight;
                }
            }
        }

        if (playcollect)
        {
            if (!_win)
            {
                // PlayerManager.instance.time?.Cancel();
                PlayOpenAnimation(saveMyWife);
                _win = true;
                _isMoveLeft = false;
                _isMoveRight = false;
                // PlayerManager.instance.time?.Cancel();
                // Debug.Log(1);
                // StartCoroutine(DelayWin());
            }
        }
        else
        {
            StartCoroutine(IsShowWin());
        }

        PlayAnimWinPet();
    }

    #endregion

    private void PlayAnim(string anim, bool isLoop)
    {
        if (!skeleton.AnimationName.Equals(anim))
        {
            skeleton.AnimationState.SetAnimation(0, anim, isLoop);
        }
    }

    public bool IsBombTriggering()
    {
        return true;
    }

    public void OnExplodedAt(BombItem bomb)
    {
        if (!bomb || IsTakeHolyWater) return;
        OnDeath(EDieReason.Normal);
    }


    public void OnDeath(EDieReason reason)
    {
        if (state == EUnitState.Playing || state == EUnitState.Running)
        {
            if (GameManager.instance.gameState != EGameState.Win)
            {
                OnPlayerDie(reason);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartCollectGem()
    {
        if (!_flagCollectGem)
        {
            _flagCollectGem = true;
            _disposableCollectGem = Observable.FromCoroutine(IeStartColelctGem).Subscribe().AddTo(this);
        }
    }

    private IEnumerator IeStartColelctGem()
    {
        yield return new WaitForSeconds(0.1f);

        var arr = MapLevelManager.Instance.GemSpawnObject.gems.Where(_ => _.gameObject.activeSelf).ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            (arr[i] as Gems)?.CollectByPlayer(gemGroupTransform, durationMoveCollectGem + durationIncreasePerGem * i);
            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisposeCollectGem()
    {
        if (MapLevelManager.Instance.GemSpawnObject != null)
        {
            _disposableCollectGem?.Dispose();

            try
            {
                var arr = MapLevelManager.Instance.GemSpawnObject.gems.Where(_ => _.gameObject.activeSelf).ToArray();
                foreach (var item in arr)
                {
                    if (item != null)
                    {
                        item.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }
    }

    private bool IsStop()
    {
        return skeleton.AnimationName == winAnimationName || skeleton.AnimationName == win2AnimationName ||
               skeleton.AnimationName == win3AnimationName ||
               skeleton.AnimationName == winSuperManAnimationName ||
               skeleton.AnimationName == win2SuperManAnimationName ||
               skeleton.AnimationName == win3SuperManAnimationName || skeleton.AnimationName == lootAnimationName ||
               skeleton.AnimationName == openAnimationName ||
               skeleton.AnimationName == openSuperManAnimationName;
    }
}

public enum EDieReason
{
    Normal,
    Fire,
    Despair,
    Ice,
    Rat,
    Bee,
}

public enum EWinReason
{
    Normal,
    Collect,
    SavePrincess,
}