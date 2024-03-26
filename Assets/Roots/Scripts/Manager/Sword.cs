using UnityEngine;

public class Sword : Target
{
    public LayerMask lmMapObject;
    public Rigidbody2D rig2d;
    [SerializeField] private ParticleSystem swordParticle;

    private RaycastHit2D hitDown;
    
    private HostageManager _hHostage;
    private EnemyBase _eEnemy;
    private Vector3 _vStartHitDown, _vEndHitDown;
    private void HitDownMapObject()
    {
        _vStartHitDown = new Vector3(transform.localPosition.x, transform.localPosition.y - 2.35f, transform.localPosition.z);
        _vEndHitDown = new Vector3(_vStartHitDown.x, _vStartHitDown.y - 0.25f, _vStartHitDown.z);
        hitDown = Physics2D.Linecast(_vStartHitDown, _vEndHitDown, lmMapObject);

        Debug.DrawLine(_vStartHitDown, _vEndHitDown, Color.red);
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (swordParticle != null)
        {
            swordParticle.gameObject.SetActive(true);
        }
    }

    public bool IsCanKilling() { return hitDown.collider == null; }

    private void FixedUpdate() { HitDownMapObject(); }

    private void OnTriggerEnter2D(
        Collider2D collision)
    {
        if (MapLevelManager.Instance.isGameplay1) if (PlayerManager.instance.state == EUnitState.Die) return;
        if (collision.CompareTag("BodyPlayer"))
        {
            rig2d.bodyType = RigidbodyType2D.Kinematic;
            PlayerManager.instance.OnTakeSword(transform);
        }

        _hHostage = collision.gameObject.GetComponent<HostageManager>();
        if (_hHostage != null)
        {
            if (IsCanKilling())
            {
                if (!PlayerManager.instance.IsTakeSword) _hHostage.OnDie(false);
            }
        }

        _eEnemy = collision.gameObject.GetComponent<EnemyBase>();
        if (_eEnemy != null)
        {
            if (IsCanKilling())
            {
                _eEnemy.OnDie(EDieReason.Normal);
            }
        }
    }

    
}