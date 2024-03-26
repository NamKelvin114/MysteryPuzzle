using UnityEngine;

public class HolyWaterItem : MonoBehaviour
{
    public Rigidbody2D rig2d;
    public GameObject effectUsed;
    private EnemyBase _enemy;
    private BaseCannon _cannon;

    private bool _flag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        void PlaySound()
        {
            if (!_flag)
            {
                _flag = true;
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.useHolyWater);
            }
        }

        if (PlayerManager.instance.state == EUnitState.Die) return;
        if (collision.CompareTag("BodyPlayer"))
        {
            rig2d.bodyType = RigidbodyType2D.Kinematic;
            PlayerManager.instance.OnTakeHolyWater(transform);
            effectUsed.transform.SetParent(GameManager.instance.Root.transform, true);
            effectUsed.SetActive(true);
            PlaySound();
            return;
        }

        _enemy = collision.GetComponentInParent<EnemyBase>();
        if (_enemy != null)
        {
            // use
            _enemy.OnTakeHolyWater(transform);
            effectUsed.transform.SetParent(GameManager.instance.Root.transform, true);
            effectUsed.SetActive(true);
            PlaySound();
            return;
        }

        _cannon = collision.GetComponentInParent<BaseCannon>();
        if (_cannon != null && !collision.gameObject.name.Equals("SearchCollider")) // bad
        {
            // use
            _cannon.OnTakeHolyWater(transform);
            effectUsed.transform.SetParent(GameManager.instance.Root.transform, true);
            effectUsed.SetActive(true);
            PlaySound();
            return;
        }
    }
}