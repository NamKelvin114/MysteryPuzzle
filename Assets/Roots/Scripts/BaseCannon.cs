using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class BaseCannon : LevelObject
{
    public GameObject stunEffect;
    public float shootSpeed;
    public float cooldownTime = 0.75f;
    public Collider2D searchCollider;
    public Collider2D mainCollider;
    public LayerMask searchMask;
    public CannonBall cannonPrefab;
    public Transform shootLocate;
    [SerializeField] protected bool isEnemy;
    [SerializeField] protected Rigidbody2D rig;
    [SerializeField] protected HeadDragon headDragon;
    public CannonAttackHandler handler;
    [SpineAnimation] public string idleName;
    [SpineAnimation] public string deathName;
    [SpineAnimation, SerializeField] protected string deathIceName;
    [SpineAnimation] public string attackName;
    [SpineAnimation, SerializeField] protected string eatHolyWater;
    [SpineAnimation, SerializeField] protected string idleHolyWater;
    [SpineAnimation, SerializeField] protected string attackHolyWater;
    private static List<Collider2D> cachedSearchCollider = new List<Collider2D>();

    public bool IsTakeHolyWater { get; set; }
    public bool IsDisable { get; private set; }
    public float ShootCooldown { get; private set; } = 0;

    private bool _flagVibrateDie;


    private bool _isDeath;

    protected virtual void Start()
    {
        handler.onBulletAction = TryShoot;
        handler.endAttackAction = PlayIdleAnimation;
    }

    public void DoBreak()
    {
        IsDisable = true;
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.dragonDeath);
        if (!_isDeath)
        {
            _isDeath = true;
            mainCollider.gameObject.layer = LayerMask.NameToLayer("DeadBody");
            GameManager.instance.SoftButton();
            if (!isEnemy) return;
            GameManager.instance.EnemyKill++;
            if (MapLevelManager.Instance != null) MapLevelManager.Instance.allCannonEnemies.Remove(this);
            if (MapLevelManager.Instance.eQuestType == EQuestType.Kill && MapLevelManager.Instance.lstAllEnemies.Count == 0 &&
                MapLevelManager.Instance.allCannonEnemies.Count == 0)
            {
                PlayerManager.instance.OnWin(false);
            }

            rig.velocity = Vector2.zero;
            rig.constraints = RigidbodyConstraints2D.FreezePositionX;
            transform.rotation = Quaternion.identity;
            rig.constraints = RigidbodyConstraints2D.FreezeRotation;

            headDragon.gameObject.SetActive(false);
            stunEffect.SetActive(true);
        }

        // todo
    }

    private void SearchShoot()
    {
        cachedSearchCollider = new List<Collider2D>();
        searchCollider.OverlapCollider(new ContactFilter2D() {layerMask = searchMask.value}, cachedSearchCollider);

        cachedSearchCollider.RemoveAll(_ => _.gameObject.CompareTag("Tag_Win")); // remove gems
        float length = 100;
        int index = 0;
        for (int i = 0; i < cachedSearchCollider.Count; i++)
        {
            var col1 = cachedSearchCollider[i];

            float d;
            if (col1.transform.parent.GetComponent<EnemyBase>() || col1.transform.parent.GetComponent<PlayerManager>() ||
                col1.transform.parent.GetComponent<HostageManager>() || col1.gameObject.CompareTag("StickBarrie"))
            {
                d = Mathf.Abs(col1.transform.parent.position.x - transform.position.x);
            }
            else
            {
                d = Mathf.Abs(col1.transform.position.x - transform.position.x);
            }

            if (d < length)
            {
                var enemy1 = col1.GetComponentInParent<EnemyBase>();
                if (enemy1 != null && enemy1._charStage == EnemyBase.CHAR_STATE.DIE)
                {
                    continue;
                }

                var character1 = col1.GetComponentInParent<PlayerManager>();
                if (character1 != null && character1.state == EUnitState.Die)
                {
                    continue;
                }

                var hostage1 = col1.GetComponentInParent<HostageManager>();
                if (hostage1 != null && hostage1.state == EUnitState.Die)
                {
                    continue;
                }

                length = d;
                index = i;
            }
        }

        var col = cachedSearchCollider[index];
        if (col == mainCollider || col.gameObject.CompareTag("Wall_Bottom") || col.gameObject.CompareTag("Chan") || col.gameObject.CompareTag("Rope") ||
            col.gameObject.CompareTag("Tag_Win") || col.gameObject.CompareTag("StickBarrie"))
        {
            return;
        }

        var enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null && enemy._charStage == EnemyBase.CHAR_STATE.DIE)
        {
            return;
        }

        var character = col.GetComponentInParent<PlayerManager>();
        if (character != null && character.state == EUnitState.Die)
        {
            return;
        }

        var hostage = col.GetComponentInParent<HostageManager>();
        if (hostage != null && hostage.state == EUnitState.Die)
        {
            return;
        }

        if (cachedSearchCollider[index])
        {
            Search(cachedSearchCollider[index]);
        }
    }

    private void Search(Collider2D col)
    {
        var player = col.GetComponentInParent<PlayerManager>();

        if (player != null && !player.IsTakeHolyWater)
        {
            if ((player.state == EUnitState.Playing || player.state == EUnitState.Running) && ShootCooldown <= 0)
            {
                PlayAttackAnimation();
                return;
            }
        }

        var enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null && !enemy.IsTakeHolyWater)
        {
            if ((enemy._charStage == EnemyBase.CHAR_STATE.PLAYING || enemy._charStage == EnemyBase.CHAR_STATE.RUNNING) && ShootCooldown <= 0)
            {
                PlayAttackAnimation();
                return;
            }
        }

        var hostage = col.GetComponentInParent<CharsBase>();
        if (hostage == null || hostage.IsTakeHolyWater) return;
        if ((hostage.state == EUnitState.Playing || hostage.state == EUnitState.Running) && ShootCooldown <= 0)
        {
            PlayAttackAnimation();
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (IsDisable || IsTakeHolyWater) return;
        var ice = other.gameObject.GetComponentInParent<IceWaterController>();
        if (ice != null)
        {
            PlayDeathIceAnimation();
            DoBreak();
        }
    }

    private void TryShoot()
    {
        if (ShootCooldown > 0)
        {
            return;
        }

        ShootCooldown = cooldownTime;
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.dragonAttack);
        var ball = Instantiate(cannonPrefab, transform.parent);
        ball.transform.position = shootLocate.position;
        ball.gameObject.SetActive(true);
        ball.rigidbody2D.velocity = transform.TransformDirection(Vector3.left.Mult(transform.localScale)) * -1 * shootSpeed;

        var myCols = GetComponentsInChildren<Collider2D>();
        var otherCols = ball.rigidbody2D.GetComponentsInChildren<Collider2D>();
        foreach (var c1 in myCols)
        foreach (var c2 in otherCols)
            Physics2D.IgnoreCollision(c1, c2, true);
    }

    protected virtual void PlayIdleAnimation()
    {
        handler.SkeletonAnimation.loop = true;
        handler.SkeletonAnimation.AnimationName = idleName;
    }

    protected virtual void PlayAttackAnimation()
    {
        if (handler.SkeletonAnimation != null)
        {
            handler.SkeletonAnimation.loop = false;
            handler.SkeletonAnimation.AnimationName = attackName;
        }
    }

    public void PlayDeathIceAnimation()

    {
        if (!IsDisable)
        {
            handler.SkeletonAnimation.loop = false;
            handler.SkeletonAnimation.AnimationName = deathIceName;
        }
    }

    public void PlayDeathAnimation()
    {
        // if (!IsDisable)
        // {
        handler.SkeletonAnimation.loop = false;
        handler.SkeletonAnimation.AnimationName = deathName;
        handler.SkeletonAnimation.Initialize(true);
        //}
    }

    private void Update()
    {
        if (IsDisable || GameManager.instance.gameState == EGameState.Win || GameManager.instance.gameState == EGameState.Lose) return;

        ShootCooldown = Mathf.Max(0, ShootCooldown - Time.deltaTime);
        if (ShootCooldown <= 0)
        {
            SearchShoot();
        }
    }

    public virtual void OnTakeHolyWater(Transform holyWater)
    {
        IsTakeHolyWater = true;
        holyWater.gameObject.SetActive(false);

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeSword);
    }
}