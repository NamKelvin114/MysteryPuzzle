using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Spine.Unity;

public class Chest : MonoBehaviour, IPushAway
{
    public bool fallingChest;
    private bool hasDetect;
    public SkeletonAnimation saChest;
    [SpineAnimation] public string animOpen;
    [SpineAnimation] public string iceAnimationName;
    [SpineAnimation] public string animFire;
    [HideInInspector] public Rigidbody2D rig2d;
    [SerializeField] AnimationCurve animationCurve;
    private float _durationCurve;
    private float _startValueCurve = 0;
    [SerializeField] private float durationCurve;
    [SerializeField] private GemHandles spawnGem;
    [SerializeField] private ParticleSystem chestParticle;
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private Rigidbody2D rigid;
    private Guid _guid;
    Sequence _pushSequence;


    private void OnEnable()
    {
        rig2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _durationCurve = durationCurve;
        if (chestParticle != null) chestParticle.gameObject.SetActive(true);
        if (MapLevelManager.Instance.eQuestType == EQuestType.OpenChest)
        {
            if (!fallingChest)
                MapLevelManager.Instance.trTarget = transform;
        }

        saChest.AnimationState.Complete += delegate
        {
            if (saChest.AnimationName.Equals(animOpen))
            {
                chestParticle.Play();
                PlayerManager.instance.OnWin(true);
            }
        };
    }
    public void GetPushed(SpringItem spring)
    {
        DOTween.Kill(_guid);
        _pushSequence = null;
        durationCurve = _durationCurve;
        _startValueCurve = 0;
        //saChest.skeleton.ScaleX *= -1;
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
                    rigid.velocity = Vector2.zero;
                }))
                .OnUpdate((() =>
                {
                    if (GameManager.instance.gameState != EGameState.Win &&
                        GameManager.instance.gameState != EGameState.Lose)
                    {
                        rigid.velocity = new Vector2(speed.x * animationCurve.Evaluate(_startValueCurve), rigid.velocity.y);
                    }
                })).OnComplete((() =>
                {
                    _startValueCurve = 0;
                })));
            _guid = System.Guid.NewGuid();
            _pushSequence.id = _guid;
        }
        _pushSequence.Play();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Utils.TAG_LAVA))
        {
            if (!hasDetect)
            {
                if (PlayerManager.instance.state == EUnitState.Playing &&
                    GameManager.instance.gameState != EGameState.Win)
                {
                    saChest.AnimationName = animFire;
                    saChest.Initialize(true);

                    fireParticle.gameObject.SetActive(true);
                    PlayerManager.instance.OnPlayerDie(EDieReason.Despair);

                    if (ObjectPoolerManager.Instance != null)
                    {
                        GameObject destroyEffect = ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
                        destroyEffect.transform.position = collision.transform.position;
                        destroyEffect.SetActive(true);
                    }
                }
            }

            hasDetect = true;
        }
        // else if (collision.CompareTag(Utils.TAG_ICE_WATER))
        // {
        //     if (!hasDetect)
        //     {
        //         if (PlayerManager.Instance.state == EUnitState.Playing && GameManager.instance.gameState != GameManager.GAMESTATE.WIN)
        //         {
        //             saChest.AnimationName = iceAnimationName;
        //             saChest.Initialize(true);
        //
        //             PlayerManager.Instance.OnPlayerDie(EDieReason.Despair);
        //
        //             if (ObjectPoolerManager.Instance != null)
        //             {
        //                 var destroyEffect = ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
        //                 destroyEffect.transform.position = collision.transform.position;
        //                 destroyEffect.SetActive(true);
        //             }
        //         }
        //     }
        //
        //     hasDetect = true;
        // }

        if (collision.gameObject.CompareTag("BodyPlayer"))
        {
            if (GameManager.instance.gameState != EGameState.Lose)
            {
                rig2d.constraints = RigidbodyConstraints2D.FreezePositionX;
                PlayerManager.instance.OnWin(true);
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acOpenChest);
                MapLevelManager.Instance.FetchGemObject(spawnGem);
                spawnGem.gameObject.SetActive(true);

                StartCoroutine(IEOpenChest());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        IDieByFallingStone obj = other.gameObject.GetComponentInParent<IDieByFallingStone>();

        if (obj != null)
        {
            var dragon = (BaseCannon)obj;
            if (!dragon.IsDisable)
            {
                dragon.PlayDeathAnimation();
                dragon.DoBreak();
            }
        }
    }

    private IEnumerator IEOpenChest()
    {
        yield return new WaitForSeconds(0.2f);
        saChest.AnimationState.SetAnimation(0, animOpen, false);
    }

    private void PlayAnim(SkeletonAnimation saPlayer, string anim_, bool isLoop)
    {
        if (!saPlayer.AnimationName.Equals(anim_))
        {
            saPlayer.AnimationState.SetAnimation(0, anim_, isLoop);
        }
    }
}
