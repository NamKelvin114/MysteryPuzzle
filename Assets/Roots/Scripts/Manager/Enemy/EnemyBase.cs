using System;
using Spine;
using UnityEngine;
using Spine.Unity;
using UniRx;
using UnityTimer;
using Worldreaver.Utility;
using System.Collections;
using DG.Tweening;


// bad class architecture
public class EnemyBase : MonoBehaviour, IExplodeReceiver, IPushAway
{
    private bool _shoot;

    public enum ENEMY_TYPE
    {
        MELEE,
        RANGE,
        MONSTER
    }

    public enum CHAR_STATE
    {
        PLAYING,
        DIE,
        WIN,
        RUNNING
    }

    public enum CHECKATTACK
    {
        Attack,
        idle
    }

    public GameObject skull;
    [SerializeField] public ENEMY_TYPE enemyType;
    [SerializeField] public CHAR_STATE _charStage;
    [SerializeField] public CHECKATTACK _checkattack;

    public SkeletonAnimation saPlayer;

    [SpineAnimation] public string str_idle, str_Att, str_Run;
    [SpineAnimation, SerializeField] protected string dieAnimation;
    [SpineAnimation, SerializeField] protected string dieFireAnimation;
    [SpineAnimation, SerializeField] protected string dieIceAnimation;

    [SpineAnimation, SerializeField] protected string eatHolyWater;
    [SpineAnimation, SerializeField] protected string idleHolyWater;
    [SpineAnimation, SerializeField] protected string attackHolyWater;
    [SpineAnimation, SerializeField] protected string runHolyWater;
    [SerializeField] private bool isRat;

    //Edit AnimationCurve when Level need to use Spring
    [Header("AnimationCurve")] [SerializeField]
    private AnimationCurve animationCurve;
    private float _durationCurve;
    private Guid _guid;
    Sequence _pushSequence;

    private float _startValueCurve = 0;
    [SerializeField] private float durationCurve;

    public float moveSpeed;
    [SerializeField] public Rigidbody2D rig;
    public LayerMask lmColl, lmPlayer, lmMapObject;
    public LayerMask layerWolf;
    public bool isRangerAtt;
    public GameObject gGround, body, head, centerBox;

    [SerializeField] private RaycastHit2D hit2D, hit2D_1, hitPlayer;
    private RaycastHit2D _hitWolf;
    protected bool hitDown;

    protected RaycastHit2D HitPlayer
    {
        get => hitPlayer;
        set => hitPlayer = value;
    }

    protected RaycastHit2D Hit2D
    {
        get => hit2D;
        set => hit2D = value;
    }

    protected RaycastHit2D Hit2D1
    {
        get => hit2D_1;
        set => hit2D_1 = value;
    }

    public bool DeathMark { get; set; }

    //  private bool isContinueDetect = true;
    protected Vector2 vEnd, vStart, _vEnd, _vStart;
    protected Vector2 vStartForward, vEndForward;
    protected bool _isCanMoveToTarget;
    protected Vector2 vStartHitDown, vEndHitDown;

    [SerializeField] protected EnemyHandleEvent eventHandle;

    //  private string targetName;
    private EnemyBase ebTarget;
    private HostageManager hmTarget;
    private PlayerManager pmTarget;
    private bool _isPushing;
    public GameObject target;
    protected bool flagVibrateDie;
    public AudioClip attackAudio;
    public bool isLoopSoundIdle;
    public AudioClip idleAudio;
    public AudioClip dieAudio;
    public bool isNotUseSoundAttack;
    private bool _isStopAudio;

    protected PlayerManager playerTarget;
    protected HostageManager hostageTarget;
    public Transform center, left, right, ground, leftAttack, rightAttack;
    private bool beginMove = false;
    private Vector2 direction;
    private Timer time;

    public Timer Timer
    {
        get { return time; }
        set => time = value;
    }

    private IDisposable d;

    public bool IsTakeHolyWater { get; private set; }

    protected void PlayAnim(string anim, bool isLoop)
    {
        if (!saPlayer.AnimationName.Equals(anim))
        {
            saPlayer.AnimationState.SetAnimation(0, anim, isLoop);
        }
    }

    public virtual void PlayIdle()
    {
        if (_charStage == CHAR_STATE.DIE) return;
        PlayAnim(IsTakeHolyWater ? idleHolyWater : str_idle, true);
        PlaySoundIdle();
    }
    public void PlaySoundIdle()
    {
        if (SoundManager.Instance != null && idleAudio != null)
        {
            SoundManager.Instance.StopSound();
            if (!isLoopSoundIdle)
            {
                SoundManager.Instance.PlaySound(idleAudio);
            }
            else
            {
                SoundManager.Instance.PlaySoundContinously(idleAudio);
            }
            idleAudio = null;
        }
    }
    public void StopSoundIdle()
    {
        if (!_isStopAudio)
        {
            _isStopAudio = true;
            if (isLoopSoundIdle)
            {
                SoundManager.Instance.StopSound();
            }
            else
            {
                SoundManager.Instance.StopSoundContinously();
            }
        }
    }

    public virtual void PlayMove(bool isLoop = true)
    {
        StopSoundIdle();
        PlayAnim(IsTakeHolyWater ? runHolyWater : str_Run, isLoop);
    }

    public virtual void PlayAttack(bool isLoop = false)
    {
        StopSoundIdle();
        if (_charStage == CHAR_STATE.DIE) return;
        PlayAnim(IsTakeHolyWater ? attackHolyWater : str_Att, isLoop);
    }

    public virtual void PlayApear()
    {
        PlayAnim(eatHolyWater, false);
        StopSoundIdle();
    }

    protected void PlaySoundAttack()
    {
        if (SoundManager.Instance != null && attackAudio != null && !isNotUseSoundAttack)
        {
            SoundManager.Instance.PlaySound(attackAudio);
        }
    }

    protected virtual void InitSoundAttack()
    {
        if (SoundManager.Instance != null && attackAudio == null)
        {
            attackAudio = SoundManager.Instance.acMeleeAttack;
        }
    }

    public virtual void Start()
    {
        _durationCurve = durationCurve;
        _checkattack = CHECKATTACK.idle;
        DeathMark = false;
        InitSoundAttack();
        if (MapLevelManager.Instance != null && !MapLevelManager.Instance.lstAllEnemies.Contains(this))
        {
            MapLevelManager.Instance.lstAllEnemies.Add(this);
        }
        PlayIdle();

        saPlayer.AnimationState.Event += delegate
        {
            if (saPlayer.AnimationName.Equals(str_Att))
            {
                if (enemyType == ENEMY_TYPE.RANGE)
                {
                    rig.velocity = Vector2.zero;
                    rig.angularVelocity = 0;
                    GameObject arrow = ObjectPoolerManager.Instance.arrowPooler.GetPooledObject();
                    var scale = saPlayer.skeleton.ScaleX * -1;
                    arrow.transform.position =
                        scale < 0 ? leftAttack.transform.position : rightAttack.transform.position;
                    arrow.transform.localScale = new Vector2(scale, arrow.transform.localScale.y);
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 180);
                    arrow.SetActive(true);
                    arrow.GetComponent<Rigidbody2D>().velocity = scale > 0 ? Vector2.left * 6 : Vector2.right * 6;
                    PlaySoundAttack();
                    Observable.Timer(TimeSpan.FromSeconds(0.45f))
                        .Subscribe(_ =>
                        {
                            if (_charStage == CHAR_STATE.PLAYING)
                            {
                                PlayIdle();
                            }
                        })
                        .AddTo(this);
                }
            }
        };

        if (enemyType == ENEMY_TYPE.MELEE)
        {
            // callback only for wolf to test
            eventHandle.actionAttack += () =>
            {
                PlaySoundAttack();
                if (playerTarget != null && playerTarget.state != EUnitState.Die)
                {
                    if (!GameManager.instance.FlagCharacterDie)
                    {
                        GameManager.instance.FlagCharacterDie = true;
                        GameManager.instance.FailureButton();
                    }

                    if (isRat)
                    {
                        PlayerManager.instance.OnPlayerDie(EDieReason.Rat);
                    }
                    else
                    {
                        PlayerManager.instance.OnPlayerDie(EDieReason.Normal);
                    }
                }
                else if (hostageTarget != null && hostageTarget.state != EUnitState.Die)
                {
                    if (!GameManager.instance.FlagHostageDie)
                    {
                        GameManager.instance.FlagHostageDie = true;
                        GameManager.instance.FailureButton();
                    }

                    HostageManager.instance.OnDie(true);
                }
                else if (_hitWolf.collider != null)
                {
                    var redWolf = _hitWolf.collider.gameObject.GetComponentInParent<RedWolf>();
                    if (redWolf._charStage != CHAR_STATE.DIE && !redWolf.IsTakeHolyWater)
                    {
                        redWolf.OnDie(EDieReason.Normal);
                    }
                }
            };

            saPlayer.AnimationState.Start += HandleEndAnimation;
        }
    }

    protected void HandleEndAnimation(TrackEntry trackentry)
    {
        if (GameManager.instance.gameState == EGameState.Win || GameManager.instance.gameState == EGameState.Lose)
        {
            saPlayer.AnimationState.Start -= HandleEndAnimation;
        }

        if (trackentry.Animation.Name.Equals(str_Att))
        {
            time = Timer.Register(saPlayer.DurationAnimation(str_Att), PlayIdle);
        }
        else if (trackentry.Animation.Name.Equals(attackHolyWater))
        {
            Timer.Register(saPlayer.DurationAnimation(attackHolyWater), PlayIdle);
        }
    }


    protected virtual void HitDownMapObject()
    {
        vStartHitDown = center.position;
        vEndHitDown = ground.position;
        hitDown = Physics2D.OverlapCircle(ground.position, 0.05f, lmMapObject);
    }

    public void GetPushed(SpringItem spring)
    {
        DOTween.Kill(_guid);
        _pushSequence = null;
        durationCurve = _durationCurve;
        _startValueCurve = 0;
        _isPushing = true;
        beginMove = false;
        //saPlayer.skeleton.ScaleX *= -1;
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
                    target = null;
                }))
                .OnUpdate((() =>
                {
                    if (_charStage != CHAR_STATE.DIE && GameManager.instance.gameState != EGameState.Win &&
                        GameManager.instance.gameState != EGameState.Lose)
                    {
                        rig.velocity = new Vector2(speed.x * animationCurve.Evaluate(_startValueCurve), rig.velocity.y);
                        PlayIdle();
                    }
                })).OnComplete((() =>
                {
                    _isCanMoveToTarget = false;
                    _isPushing = false;
                    _startValueCurve = 0;
                })));
            _guid = System.Guid.NewGuid();
            _pushSequence.id = _guid;
        }
        _pushSequence.Play();

    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ground.position, 0.05f);
    }

    public virtual void FixedUpdate()
    {
        if (_charStage == CHAR_STATE.PLAYING && _isPushing == false &&
            GameManager.instance.gameState != EGameState.Win && GameManager.instance.gameState != EGameState.Lose)
        {
            #region Check Hit Down
            HitDownMapObject();
            #endregion

            if (hitDown)
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

                hit2D_1 = Physics2D.Linecast(vStartForward, vEndForward, lmColl);
                hit2D = Physics2D.Linecast(vStart, vEnd, lmColl);

                if (hitPlayer.collider == null)
                {
                    if (hit2D.collider != null)
                    {
                        if (!hit2D.collider.gameObject.CompareTag(Utils.TAG_STICKBARRIE) &&
                            !hit2D.collider.gameObject.CompareTag("Chan"))
                        {
                            if (hit2D.collider.gameObject.name == "Hostage_Female")
                            {
                                _isCanMoveToTarget = hit2D.collider.gameObject.GetComponent<HostageManager>().state !=
                                    EUnitState.Die;
                                target = hit2D.collider.gameObject;
                            }
                            else if (hit2D.collider.gameObject.CompareTag("BodyPlayer") &&
                                _checkattack == CHECKATTACK.idle && PlayerManager.instance._checkattack ==
                                PlayerManager.CHECKATTACK.idle)
                            {
                                _isCanMoveToTarget = PlayerManager.instance.state != EUnitState.Die;
                                target = hit2D.collider.gameObject;
                            }
                            else if (hit2D.collider.gameObject.CompareTag("Wolf") && !_hitWolf)
                            {
                                _isCanMoveToTarget =
                                    hit2D.collider.gameObject.GetComponentInParent<RedWolf>()._charStage !=
                                    CHAR_STATE.DIE;
                                target = _isCanMoveToTarget == false ? null : hit2D.collider.gameObject;
                            }
                            else if (hit2D.collider.gameObject.CompareTag("Spring"))
                            {
                                _isCanMoveToTarget = true;
                                target = hit2D.collider.gameObject;
                            }
                            else
                            {
                                _isCanMoveToTarget = false;
                            }

                            if (_isCanMoveToTarget)
                            {
                                OnMoveToTarget();
                            }
                        }
                    }

                    if (hit2D_1.collider != null)
                    {
                        if (!hit2D_1.collider.gameObject.CompareTag(Utils.TAG_STICKBARRIE) &&
                            !hit2D_1.collider.gameObject.CompareTag("Chan"))
                        {
                            if (hit2D_1.collider.gameObject.name == "Hostage_Female")
                            {
                                _isCanMoveToTarget = hit2D_1.collider.gameObject.GetComponent<HostageManager>().state !=
                                    EUnitState.Die;
                                target = hit2D_1.collider.gameObject;
                            }
                            else if (hit2D_1.collider.gameObject.CompareTag("BodyPlayer") &&
                                _checkattack == CHECKATTACK.idle && PlayerManager.instance._checkattack ==
                                PlayerManager.CHECKATTACK.idle)
                            {
                                _isCanMoveToTarget = PlayerManager.instance.state != EUnitState.Die;
                                target = hit2D_1.collider.gameObject;
                            }
                            else if (hit2D_1.collider.gameObject.CompareTag("Wolf") && !_hitWolf)
                            {
                                _isCanMoveToTarget =
                                    hit2D_1.collider.gameObject.GetComponentInParent<RedWolf>()._charStage !=
                                    CHAR_STATE.DIE;
                                target = _isCanMoveToTarget == false ? null : hit2D_1.collider.gameObject;
                            }
                            else if (hit2D_1.collider.gameObject.CompareTag("Spring"))
                            {
                                _isCanMoveToTarget = true;
                                target = hit2D_1.collider.gameObject;
                            }
                            else
                            {
                                _isCanMoveToTarget = false;
                            }

                            if (_isCanMoveToTarget)
                            {
                                OnMoveToTarget();
                            }
                        }
                    }
                }
                #endregion

                #region Check Hit Player
                _vStart = center.position;

                _vEnd = saPlayer.skeleton.ScaleX < 0
                    ? new Vector2(rightAttack.transform.position.x, rightAttack.transform.position.y)
                    : new Vector2(leftAttack.transform.position.x, leftAttack.transform.position.y);
                hitPlayer = Physics2D.Linecast(_vStart, _vEnd, lmPlayer);
#if UNITY_EDITOR
                Debug.DrawLine(_vStart, _vEnd, Color.yellow);
#endif
                if (hitPlayer.collider != null)
                {
                    OnPrepareAttack();
                }
                #endregion

                #region check hit wolf
                _hitWolf = Physics2D.Linecast(_vStart, _vEnd, layerWolf);
                if (_hitWolf.collider != null)
                {
                    var redWolf = _hitWolf.collider.gameObject.GetComponentInParent<RedWolf>();
                    if (redWolf._charStage != CHAR_STATE.DIE)
                    {
                        // to bad code :(((
                        OnPrepareAttack();
                        float t = 0.56f;
                        if (enemyType == ENEMY_TYPE.RANGE)
                        {
                            t = 1.2f;
                        }

                        d = Observable.Timer(TimeSpan.FromSeconds(t))
                            .Subscribe(_ =>
                            {
                                if (_charStage == CHAR_STATE.PLAYING) PlayIdle();
                            })
                            .AddTo(this);
                    }
                }
                #endregion
            }

            checkMove();
        }
    }

    void checkMove()
    {
        var pos = moveSpeed * direction;
        if (beginMove) rig.velocity = new Vector2(pos.x, rig.velocity.y);
    }

    public virtual void OnMoveToTarget()
    {
        if (target != null)
        {
            saPlayer.skeleton.ScaleX = Utils.ChangeScale(transform.position, target.transform.position);
            beginMove = true;
            direction = transform.position.x < target.transform.position.x ? Vector2.right : Vector2.left;
            switch (enemyType)
            {
                case ENEMY_TYPE.MELEE:
                    rig.velocity = moveSpeed * direction;
                    PlayMove();
                    break;
                case ENEMY_TYPE.MONSTER:
                    PlayMove();
                    break;
                case ENEMY_TYPE.RANGE:
                    if (hostageTarget == null && HitPlayer.collider != null)
                    {
                        hostageTarget = HitPlayer.collider.GetComponentInParent<HostageManager>();
                        if (hostageTarget != null && hostageTarget.IsTakeHolyWater)
                        {
                            PlayIdle();
                            return;
                        }
                    }

                    PlayAttack();
                    break;
            }
        }
    }

    public virtual void OnPrepareAttack()
    {
        // Debug.Log("base");
        if (_charStage == CHAR_STATE.DIE) return;
        if (GameManager.instance.gameState == EGameState.Win ||
            GameManager.instance.gameState == EGameState.Lose) return;

        rig.velocity = Vector2.zero;
        if (enemyType == ENEMY_TYPE.RANGE)
        {
            if (hostageTarget == null)
            {
                hostageTarget = HitPlayer.collider.GetComponentInParent<HostageManager>();
                if (hostageTarget != null && hostageTarget.IsTakeHolyWater)
                {
                    PlayIdle();
                    return;
                }
            }

            PlayAttack();
        }
        else
        {
            if (hitPlayer.collider != null && _charStage != CHAR_STATE.DIE && !DeathMark)
            {
                _checkattack = CHECKATTACK.Attack;
                PlayerManager.instance._checkattack = PlayerManager.CHECKATTACK.Attack;
                hostageTarget = HitPlayer.collider.GetComponentInParent<HostageManager>();
                playerTarget = HitPlayer.collider.GetComponentInParent<PlayerManager>();

                var checkPlayer = playerTarget != null && (playerTarget.IsTakeSword || playerTarget.IsTakeHolyWater);
                var checkHostage = hostageTarget != null && hostageTarget.IsTakeHolyWater;

                if (checkPlayer)
                {
                    if (!PlayerManager.instance.IsTakeHolyWater)
                    {
                        // Debug.Log("stophere");
                        DeathMark = true;
                        PlayerManager.instance.OnAttackEnemy(this);
                        StartCoroutine(PlayerManager.instance.CountToEndAttack(0.833f));
                        Debug.Log("melee");
                    }

                    PlayIdle();
                }
                else if (checkHostage)
                {
                    if (HostageManager.instance.IsTakeHolyWater)
                    {
                        DeathMark = true;
                        HostageManager.instance.OnAttackEnemy(this);
                    }

                    PlayIdle();
                }
                else
                {
                    PlayAttack();
                    // bad
                    hostageTarget = HitPlayer.collider.GetComponentInParent<HostageManager>();
                    playerTarget = HitPlayer.collider.GetComponentInParent<PlayerManager>();
                }
            }
            else if (_hitWolf.collider != null && _charStage != CHAR_STATE.DIE)
            {
                _checkattack = CHECKATTACK.Attack;
                var wolf = _hitWolf.collider.GetComponentInParent<RedWolf>();
                if (wolf != null && !wolf.IsTakeHolyWater)
                {
                    PlayAttack();
                    StartCoroutine(CountToEndAttack(1));
                }
            }
            else
            {
                PlayIdle();
            }
        }
    }

    IEnumerator CountToEndAttack(float time)
    {
        yield return new WaitForSeconds(time);
        if (_charStage != CHAR_STATE.DIE)
        {
            _checkattack = CHECKATTACK.idle;
        }
        else
        {
            _charStage = CHAR_STATE.DIE;
        }
    }

    IEnumerator CountToEndAttackAndDieFire(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PlayDieSound();
        PlayAnim(dieFireAnimation, false);
    }
    void PlayDieSound()
    {
        if (SoundManager.Instance == null) return;
        if (dieAudio != null)
        {
            SoundManager.Instance.PlaySound(dieAudio);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acEnemyDie);
        }
    }

    public virtual void OnDie(EDieReason dieReason)
    {
        if (_charStage != CHAR_STATE.DIE)
        {
            Observer.UpdateTempValue?.Invoke(ETaskType.DestroyEnemy);
            head.gameObject.SetActive(false);
            body.gameObject.SetActive(false);
            switch (dieReason)
            {
                case EDieReason.Normal:
                    PlayAnim(dieAnimation, false);
                    time?.Cancel();
                    d?.Dispose();
                    break;
                case EDieReason.Fire:
                    if (_checkattack == CHECKATTACK.Attack)
                    {
                        _checkattack = CHECKATTACK.idle;
                        StartCoroutine(CountToEndAttackAndDieFire(0.5f));
                    }
                    else
                    {
                        PlayAnim(dieFireAnimation, false);
                    }

                    break;
                case EDieReason.Ice:
                    PlayAnim(dieIceAnimation, false);
                    break;
            }

            _charStage = CHAR_STATE.DIE;
            Utils.SetTaskProcess(ETaskType.DestroyEnemy, Utils.GetTaskProcess(ETaskType.DestroyEnemy) + 1);
            rig.velocity = Vector2.zero;
            rig.constraints = RigidbodyConstraints2D.FreezePositionX;
            transform.rotation = Quaternion.identity;
            rig.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (body != null) body.SetActive(false);
            if (head != null) head.SetActive(false);

            if (gGround != null)
            {
                gGround.layer = LayerMask.NameToLayer("DeadBody");
                var coll = gGround.GetComponent<BoxCollider2D>();
                if (coll != null)
                {
                    coll.size = new Vector2(0.1f, 0.01f);
                    coll.offset = new Vector2(0, -0.1f);
                }
            }

            if (dieReason != EDieReason.Ice)
            {
                if (skull != null) skull.SetActive(true);
            }

            GameManager.instance.EnemyKill++;
            MapLevelManager.Instance.lstAllEnemies.Remove(this);

            PlayDieSound();

            if (MapLevelManager.Instance.eQuestType == EQuestType.Kill &&
                MapLevelManager.Instance.lstAllEnemies.Count == 0 &&
                MapLevelManager.Instance.allCannonEnemies.Count == 0)
            {
                if (PlayerManager.instance.state != EUnitState.Die) PlayerManager.instance.OnWin(false);
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_charStage == CHAR_STATE.PLAYING && !IsTakeHolyWater)
        {
            if (collision.CompareTag(Utils.TAG_LAVA) || collision.CompareTag("Trap_Other") ||
                collision.CompareTag(Utils.TAG_GAS) || collision.CompareTag("arrow") ||
                collision.CompareTag(Utils.TAG_ICE_WATER))
            {
                var dieReason = EDieReason.Normal;
                if (collision.CompareTag(Utils.TAG_LAVA))
                {
                    dieReason = EDieReason.Fire;
                }
                else if (collision.CompareTag(Utils.TAG_ICE_WATER))
                {
                    dieReason = EDieReason.Ice;
                }

                if (collision.GetComponent<Trap4>())
                {
                    return;
                }

                if (!flagVibrateDie)
                {
                    if (collision.CompareTag(Utils.TAG_LAVA))
                    {
                        if (!(SoundManager.Instance is null))
                            SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
                    }

                    flagVibrateDie = true;
                    GameManager.instance.SoftButton();
                }

                OnDie(dieReason);
                if (collision.CompareTag(Utils.TAG_LAVA))
                {
                    if (ObjectPoolerManager.Instance == null)
                        return;
                    GameObject destroyEffect = ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
                    destroyEffect.transform.position = transform.position;
                    destroyEffect.SetActive(true);
                }
                else if (collision.CompareTag("arrow"))
                {
                    collision.gameObject.SetActive(false);
                    if (GameManager.instance.targetCollects.Exists(collision.transform))
                    {
                        GameManager.instance.targetCollects.Remove(collision.transform);
                    }
                }
            }
        }
    }

    public void OnExplodedAt(BombItem bomb)
    {
        if (IsTakeHolyWater) return;

        OnDie(EDieReason.Normal);
    }

    public void OnTakeHolyWater(Transform holyWater)
    {
        IsTakeHolyWater = true;
        holyWater.gameObject.SetActive(false);

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeSword);

        PlayApear();
        Timer.Register(saPlayer.DurationAnimation(eatHolyWater), PlayIdle);
    }
}
