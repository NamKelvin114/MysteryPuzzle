using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class TrapArrow : LevelObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask searchMask;
    [SerializeField] private Collider2D searchCollider;
    [SerializeField] private Collider2D mainCollider;
    [SpineAnimation, SerializeField] private string idleName;
    [SpineAnimation, SerializeField] private string deathName;
    [SerializeField] private SkeletonAnimation skeleton;
    [SerializeField] private Rigidbody2D[] arrows;
    [SerializeField] private float timeCheck;
    [SerializeField] private AudioClip audioAction;
    private static List<Collider2D> cachedSearchCollider = new List<Collider2D>();

    private bool _isDeath;

    private Vector3 _directionVertical = Vector3.zero;

    private void Start()
    {
        if (transform.eulerAngles.z == 90) // floating point
        {
            _directionVertical = Vector3.up;
            if (transform.localScale.x == 1)
            {
                _directionVertical = Vector3.down;
            }
        }
    }

    public void DoBreak()
    {
        if (!_isDeath)
        {
            _isDeath = true;
            mainCollider.enabled = false;
            GameManager.instance.SoftButton();
        }
    }

    private void SearchShoot()
    {
        cachedSearchCollider = new List<Collider2D>();
        searchCollider.OverlapCollider(new ContactFilter2D() { layerMask = searchMask.value }, cachedSearchCollider);

        cachedSearchCollider.RemoveAll(_ => _.gameObject.CompareTag("Tag_Win")); // remove gems
        float length = 100;
        int index = 0;
        for (int i = 0; i < cachedSearchCollider.Count; i++)
        {
            var col1 = cachedSearchCollider[i];

            float d;
            if (col1.transform.parent.GetComponent<EnemyBase>() || col1.transform.parent.GetComponent<PlayerManager>() || col1.transform.parent.GetComponent<HostageManager>() || col1.gameObject.CompareTag("StickBarrie"))
            {
                if (_directionVertical != Vector3.zero)
                {
                    d = Mathf.Abs(col1.transform.parent.position.y - transform.position.y);
                }
                else
                {
                    d = Mathf.Abs(col1.transform.parent.position.x - transform.position.x);
                }
            }
            else
            {
                if (_directionVertical != Vector3.zero)
                {
                    d = Mathf.Abs(col1.transform.position.y - transform.position.y);
                }
                else
                {
                    d = Mathf.Abs(col1.transform.position.x - transform.position.x);
                }
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
        if (col == mainCollider || col.gameObject.CompareTag("Wall_Bottom") || col.gameObject.CompareTag("Chan") || col.gameObject.CompareTag("Rope") || col.gameObject.CompareTag("Tag_Win") || col.gameObject.CompareTag("StickBarrie"))
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

        if (player != null)
        {
            if ((player.state == EUnitState.Playing || player.state == EUnitState.Running))
            {
                TryShoot();
                return;
            }
        }

        var enemy = col.GetComponentInParent<EnemyBase>();
        if (enemy != null)
        {
            if ((enemy._charStage == EnemyBase.CHAR_STATE.PLAYING || enemy._charStage == EnemyBase.CHAR_STATE.RUNNING))
            {
                TryShoot();
                return;
            }
        }

        var hostage = col.GetComponentInParent<CharsBase>();
        if (hostage == null) return;
        if ((hostage.state == EUnitState.Playing || hostage.state == EUnitState.Running))
        {
            TryShoot();
            return;
        }
    }

    private void TryShoot()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(audioAction);

        var direction = Vector3.left;
        if (transform.eulerAngles.z == 90) // floating point
        {
            direction = Vector3.up;
            if (transform.localScale.x == 1)
            {
                direction = Vector3.down;
            }
        }

        foreach (var arrow in arrows)
        {
            arrow.velocity = direction.Mult(transform.localScale) * moveSpeed;
        }

        skeleton.AnimationName = deathName;
        DoBreak();
    }

    private float _t;

    private void Update()
    {
        if (_isDeath)
        {
            return;
        }

        if (_t <= 0)
        {
            _t = timeCheck;
            SearchShoot();
        }
        else
        {
            _t -= Time.deltaTime;
        }
    }
}
