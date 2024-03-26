using Spine.Unity;
using UnityEngine;

public class RedWolf : EnemyBase
{
    [SpineAnimation] [SerializeField] public string eatAnimationName;
    [SerializeField] private LayerMask layerMaskMeat;
    private bool _flagEat;
    private RaycastHit2D _hitMeat;

    private bool _flagMove;

    public override void Start()
    {
        if (MapLevelManager.Instance != null)
        {
            MapLevelManager.Instance.lstAllEnemies.Add(this);
        }

        eventHandle.actionAttack += () =>
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.wolfAttack);
            if (playerTarget != null && playerTarget.state != EUnitState.Die)
            {
                if (!GameManager.instance.FlagCharacterDie)
                {
                    GameManager.instance.FlagCharacterDie = true;
                    GameManager.instance.FailureButton();
                }

                PlayerManager.instance.OnPlayerDie(EDieReason.Normal);
            }
            else if (hostageTarget != null && hostageTarget.state != EUnitState.Die)
            {
                if (!GameManager.instance.FlagHostageDie)
                {
                    GameManager.instance.FlagHostageDie = true;
                    GameManager.instance.FailureButton();
                }

                HostageManager.instance.OnDie(true);
                //HitPlayer.collider.gameObject.GetComponent<HostageManager>().OnDie_(true);
            }
        };

        saPlayer.AnimationState.Start += HandleEndAnimation;
    }

    public override void FixedUpdate()
    {
        if (_flagEat) return;

        if (_charStage == CHAR_STATE.PLAYING && GameManager.instance.gameState != EGameState.Win && GameManager.instance.gameState != EGameState.Lose)
        {
            HitDownMapObject();

            if (hitDown)
            {
                vStart = center.position;
                vEnd = left.position;

                vStartForward = center.position;
                vEndForward = right.position;

#if UNITY_EDITOR
                Debug.DrawLine(vStart, vEnd, Color.red);
                Debug.DrawLine(vStartForward, vEndForward, Color.green);
#endif
                Hit2D1 = Physics2D.Linecast(vStartForward, vEndForward, lmColl);
                Hit2D = Physics2D.Linecast(vStart, vEnd, lmColl);

                if (HitPlayer.collider == null)
                {
                    if (Hit2D.collider != null)
                    {
                        _flagMove = false;
                        if (!Hit2D.collider.gameObject.CompareTag(Utils.TAG_STICKBARRIE) && !Hit2D.collider.gameObject.CompareTag("Chan"))
                        {
                            if (Hit2D.collider.gameObject.name == "Hostage_Female")
                            {
                                _isCanMoveToTarget = Hit2D.collider.gameObject.GetComponent<HostageManager>().state != EUnitState.Die;
                                target = Hit2D.collider.gameObject;
                                _flagMove = true;
                            }
                            else if (Hit2D.collider.gameObject.CompareTag("BodyPlayer") && PlayerManager.instance._checkattack==PlayerManager.CHECKATTACK.idle)
                            {
                                _isCanMoveToTarget = PlayerManager.instance.state != EUnitState.Die;
                                target = Hit2D.collider.gameObject;
                                _flagMove = true;
                            }
                            else if (Hit2D.collider.gameObject.CompareTag("Meat"))
                            {
                                _isCanMoveToTarget = true;
                                target = Hit2D.collider.gameObject;
                                _flagMove = true;
                            }
                            else
                            {
                                _isCanMoveToTarget = false;
                                PlayIdle();
                            }

                            if (_isCanMoveToTarget)
                            {
                                OnMoveToTarget();
                            }
                        }
                    }

                    if (Hit2D1.collider != null)
                    {
                        if (!Hit2D1.collider.gameObject.CompareTag(Utils.TAG_STICKBARRIE) && !Hit2D1.collider.gameObject.CompareTag("Chan"))
                        {
                            if (Hit2D1.collider.gameObject.name == "Hostage_Female")
                            {
                                _isCanMoveToTarget = Hit2D1.collider.gameObject.GetComponent<HostageManager>().state != EUnitState.Die;
                                target = Hit2D1.collider.gameObject;
                            }
                            else if (Hit2D1.collider.gameObject.CompareTag("BodyPlayer") && PlayerManager.instance._checkattack==PlayerManager.CHECKATTACK.idle)
                            {
                                _isCanMoveToTarget = PlayerManager.instance.state != EUnitState.Die;
                                target = Hit2D1.collider.gameObject;
                            }
                            else if (Hit2D1.collider.gameObject.CompareTag("Meat"))
                            {
                                _isCanMoveToTarget = true;
                                target = Hit2D1.collider.gameObject;
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
                        else
                        {
                            if (_isCanMoveToTarget && !_flagMove)
                            {
                                _isCanMoveToTarget = false;
                                rig.velocity = Vector2.zero;
                                PlayIdle();
                            }
                        }
                    }
                    else
                    {
                        if (_isCanMoveToTarget && !_flagMove)
                        {
                            _isCanMoveToTarget = false;
                            rig.velocity = Vector2.zero;
                            PlayIdle();
                        }
                    }
                }

                #region Check Hit Player

                _vStart = center.position;
                _vEnd = saPlayer.skeleton.ScaleX < 0
                    ? new Vector2(rightAttack.transform.position.x, rightAttack.transform.position.y)
                    : new Vector2(leftAttack.transform.position.x, leftAttack.transform.position.y);
                HitPlayer = Physics2D.Linecast(_vStart, _vEnd, lmPlayer);
                if (HitPlayer.collider != null)
                {
                    OnPrepareAttack();
                    return;
                }

                #endregion

                #region check hit meat

                _hitMeat = Physics2D.Linecast(_vStart, _vEnd, layerMaskMeat);
                if (_hitMeat.collider != null && !_flagEat)
                {
                    _flagEat = true;
                    rig.velocity = Vector2.zero;
                    PlayAnim(eatAnimationName, true);
                    _hitMeat.collider.gameObject.SetActive(false);
                }

                #endregion
            }
        }
    }

    public override void OnMoveToTarget()
    {
        if (target != null)
        {
            saPlayer.skeleton.ScaleX = Utils.ChangeScale(transform.position, target.transform.position);
            var direction = transform.position.x < target.transform.position.x ? Vector2.right : Vector2.left;
            rig.velocity = moveSpeed * direction;
            PlayMove();
        }
    }

    public override void OnPrepareAttack()
    {
        // Debug.Log("dog");
        rig.velocity = Vector2.zero;

        if (HitPlayer.collider != null && _charStage != CHAR_STATE.DIE && !DeathMark)
        {
            hostageTarget = HitPlayer.collider.GetComponentInParent<HostageManager>();
            playerTarget = HitPlayer.collider.GetComponentInParent<PlayerManager>();

            var checkPlayer = playerTarget != null && (playerTarget.IsTakeSword || playerTarget.IsTakeHolyWater);
            var checkHostage = hostageTarget != null && hostageTarget.IsTakeHolyWater;
            if (checkPlayer)
            {
                if (!PlayerManager.instance.IsTakeHolyWater)
                {
                    PlayerManager.instance._checkattack = PlayerManager.CHECKATTACK.Attack;
                    DeathMark = true;
                    PlayerManager.instance.OnAttackEnemy(this);
                    StartCoroutine(PlayerManager.instance.CountToEndAttack(1));
                    Debug.Log("dog");
                   
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
        else
        {
            PlayIdle();
        }
    }

    public override void OnDie(EDieReason dieReason)
    {
        if (_charStage != CHAR_STATE.DIE)
        {
            switch (dieReason)
            {
                case EDieReason.Normal:
                    PlayAnim(dieAnimation, false);
                    Timer?.Cancel();
                    break;
                case EDieReason.Fire:
                    PlayAnim(dieFireAnimation, false);
                    break;
                case EDieReason.Ice:
                    PlayAnim(dieIceAnimation, false);
                    break;
            }

            _charStage = CHAR_STATE.DIE;
            rig.velocity = Vector2.zero;
            rig.constraints = RigidbodyConstraints2D.FreezePositionX;
            transform.rotation = Quaternion.identity;
            rig.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.SetActive(false);
            head.SetActive(false);
            skull.SetActive(true);
            GameManager.instance.EnemyKill++;
            MapLevelManager.Instance.lstAllEnemies.Remove(this);
            gGround.gameObject.layer = LayerMask.NameToLayer("DeadBody");
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.wolfDie);

            if (MapLevelManager.Instance.eQuestType == EQuestType.Kill && MapLevelManager.Instance.lstAllEnemies.Count == 0 &&
                MapLevelManager.Instance.allCannonEnemies.Count == 0)
            {
                PlayerManager.instance.OnWin(false);
            }
        }
    }
}