using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UniRx;
using UnityEngine;
using UnityTimer;
using Worldreaver.Utility;

public class HostageManager : CharsBase
{
    public static HostageManager instance;
    public ParticleSystem pheart;
    [SpineAnimation] public string untieAnimationName;
    [SpineEvent, SerializeField] private string eventAttack;
    private bool _win;

    private EnemyBase _enemyBase;
    private BaseCannon _baseCannon;

    private void Awake()
    {
        instance = this;

        ChangeSkin();
    }

    public void ChangeSkin() { skeleton.ChangeSkin(HeroData.SkinPrincessByIndex(Data.currentPrincess)); }

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            StartCoroutine(PlaySoundApear());
        }

        if (MapLevelManager.Instance != null)
        {
            MapLevelManager.Instance.trTarget = transform;
        }

        skeleton.AnimationState.Event += Event;
        skeleton.AnimationState.Complete += delegate
        {
            if (skeleton.AnimationName.Equals(untieAnimationName))
            {
                if (Data.idEasterEgg != -1 && Utils.isHardMode != true)
                {
                    var result = HeroData.GetAnimationPetGiveEgg();
                    try
                    {
                        skeleton.GetComponent<MeshRenderer>().sortingOrder = 11;
                        skeleton.AnimationState.SetAnimation(0, result.Item1, false);

                        Timer.Register(0.8f,
                            () =>
                            {
                                skeleton.AnimationState.SetAnimation(0, result.Item2, true);
                                Timer.Register(0.5f, () => PlayerManager.instance.OnWin(true, true));
                            });
                    }
                    catch (Exception)
                    {
                        PlayerManager.instance.OnWin(true, true);
                    }
                }
                else
                {
                    skeleton.AnimationState.SetAnimation(0, winAnimationName, true);
                    Observable.Timer(TimeSpan.FromSeconds(1.5f))
                        .Subscribe(_ =>
                        {
                            try
                            {
                                skeleton.AnimationState.SetAnimation(0, win2AnimationName, true);
                                PlayerManager.instance.OnWin(true, true);
                            }
                            catch (Exception)
                            {
                                PlayerManager.instance.OnWin(true, true);
                            }
                        })
                        .AddTo(this);
                }
            }
        };


        if (PlayerManager.instance != null)
        {
            if (transform.position.x > PlayerManager.instance.transform.position.x)
            {
                skeleton.skeleton.ScaleX = -1;
            }
            else skeleton.skeleton.ScaleX = 1;
        }
        else
        {
            GameManager.instance.gTargetFollow = gameObject;
        }
    }

    private IEnumerator PlaySoundApear()
    {
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.acPrincessApear);
    }

    public void PlayWin()
    {
        if (!_win)
        {
            StartCoroutine(IEWaitToIdle());
            _win = true;
            if (transform.position.x > PlayerManager.instance.transform.position.x)
            {
                skeleton.skeleton.ScaleX = -1;
            }
            else skeleton.skeleton.ScaleX = 1;
        }
    }

    public void PlayDie(EDieReason reason)
    {
        if (IsTakeHolyWater)
        {
            return;
        }

        switch (reason)
        {
            case EDieReason.Normal:
                skull.SetActive(true);
                skeleton.AnimationState.SetAnimation(0, loseAnimationName, false);
                break;
            case EDieReason.Fire:
                skull.SetActive(true);
                skeleton.AnimationState.SetAnimation(0, fireAnimationName, false);
                break;
            case EDieReason.Despair:
                skull.SetActive(true);
                skeleton.AnimationState.SetAnimation(0, loseAnimationName, false);
                break;
            case EDieReason.Ice:
                skeleton.AnimationState.SetAnimation(0, freezyAnimationName, false);
                break;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acPrincessDie);
        }
    }

    public override void OnDie(bool effect)
    {
        if (!IsTakeHolyWater)
        {
            if (GameManager.instance.gameState != EGameState.Win && PlayerManager.instance.state != EUnitState.Die)
            {
                state = EUnitState.Die;
                PlayAnim(loseAnimationName, false);
                skull.SetActive(true);
                if (PlayerManager.instance != null) PlayerManager.instance.OnPlayerDie(EDieReason.Despair);
                MapLevelManager.Instance.OnLose();
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.acPrincessDie);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BodyPlayer"))
        {
            if (GameManager.instance.gameState != EGameState.Win && GameManager.instance.gameState != EGameState.Lose)
            {
                if (PlayerManager.instance.state != EUnitState.Die)
                {
                    if (!GameManager.instance.FlagHostageMeetCharacter)
                    {
                        GameManager.instance.FlagHostageMeetCharacter = true;
                        GameManager.instance.SuccessButton();
                    }

                    GameManager.instance.gameState = EGameState.Win;
                    //Utils.SetTaskProcess(ETaskType.SaveCat, Utils.GetTaskProcess(ETaskType.SaveCat) + 1);
                    PlayerManager.instance.OnWin(true, true);
                    PlayWin();
                }
            }
        }
        else if (collision.CompareTag(Utils.TAG_ICE_WATER) || collision.gameObject.CompareTag(Utils.TAG_LAVA) || collision.gameObject.CompareTag("Trap_Other") ||
            collision.gameObject.CompareTag("arrow"))
        {
            if (IsTakeHolyWater) return;
            EDieReason dieReason = EDieReason.Normal;

            if (collision.CompareTag(Utils.TAG_LAVA))
            {
                dieReason = EDieReason.Fire;
            }
            else if (collision.CompareTag(Utils.TAG_ICE_WATER))
            {
                dieReason = EDieReason.Ice;
            }

            if (!GameManager.instance.FlagHostageDie)
            {
                if (collision.gameObject.CompareTag(Utils.TAG_LAVA) || collision.gameObject.CompareTag(Utils.TAG_ICE_WATER))
                {
                    if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
                }

                GameManager.instance.FlagHostageDie = true;
                GameManager.instance.FailureButton();
            }

            if (PlayerManager.instance.state == EUnitState.Playing || PlayerManager.instance.state == EUnitState.Running)
            {
                if (GameManager.instance.gameState != EGameState.Win)
                {
                    PlayerManager.instance.OnPlayerDie(EDieReason.Despair);
                    PlayDie(dieReason);
                    if (collision.gameObject.CompareTag("arrow"))
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

        else if (collision.gameObject.CompareTag(Utils.TAG_GAS))
        {
            if (IsTakeHolyWater) return;
            if (PlayerManager.instance.state == EUnitState.Playing || PlayerManager.instance.state == EUnitState.Running)
            {
                if (GameManager.instance.gameState != EGameState.Win)
                {
                    PlayerManager.instance.OnPlayerDie(EDieReason.Despair);
                    PlayDie(EDieReason.Normal);
                }
            }
        }
    }

    private IEnumerator IEWaitToIdle()
    {
        yield return new WaitForSeconds(0.66f);

        if (!pheart.gameObject.activeSelf)
        {
            pheart.gameObject.SetActive(true);

            skeleton.AnimationState.SetAnimation(0, untieAnimationName, false);
            if (SoundManager.Instance != null && SoundManager.Instance.acPrincessSave != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acPrincessSave);
        }
    }

    public void OnAttackEnemy(EnemyBase enemy)
    {
        if (state == EUnitState.Die) return;

        _enemyBase = enemy;
        if (_enemyBase != null) skeleton.skeleton.ScaleX = Utils.ChangeScale(transform.position, _enemyBase.transform.position);

        PlayAttackAnimation();
        Observable.FromCoroutine(IeWaitToIdleAfterAttack).Subscribe().AddTo(this);
    }

    private void PlayAttackAnimation()
    {
        if (IsTakeHolyWater)
        {
            PlayAnim(attackWonderWomanAnimationName, false);
        }
        else
        {
            PlayAnim(idleAnimationName, true);
        }
    }

    private void PlayIdleAnimation(bool isLoop = true)
    {
        if (IsTakeHolyWater)
        {
            PlayAnim(idleWonderWomanAnimationName, isLoop);
        }
        else
        {
            PlayAnim(idleAnimationName, isLoop);
        }
    }

    private IEnumerator IeWaitToIdleAfterAttack()
    {
        yield return new WaitForSeconds(0.75f);
        OnIdleState();
    }

    private void Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name.Equals(eventAttack))
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acMeleeAttack);

            if (_enemyBase != null && _enemyBase._charStage != EnemyBase.CHAR_STATE.DIE) _enemyBase.OnDie(EDieReason.Normal);
        }
    }

    public void OnIdleState()
    {
        state = EUnitState.Playing;
        PlayIdleAnimation();
    }

    public void OnTakeHolyWater(Transform holyWater)
    {
        IsTakeHolyWater = true;
        holyWater.gameObject.SetActive(false);

        skeleton.ChangeSkin(HeroData.TransfromWonder());
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeSword);
        // play Apear
        state = EUnitState.Playing;
        PlaySuperManApear();
        Timer.Register(0.67f, () => PlayIdleAnimation());
    }

    private void PlaySuperManApear() { PlayAnim(apearWonderWomanAnimationName, false); }
}
