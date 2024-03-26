using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Worldreaver.Utility;

public class PortalItem : MonoBehaviour
{
    [SerializeField] private PortalItem linkedPortal;
    [SerializeField] private float durationTelePort = 0.5f;
    [SerializeField] private float durationMagnet = 0.3f;
    [SerializeField] private PortalDirection direction;
    private Vector3 _origin;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("BodyPlayer") || other.CompareTag("Wolf") || other.CompareTag("Enemy") || other.CompareTag("Hostage") || other.CompareTag("Chest") || other.CompareTag("Sword") || other.CompareTag("Tag_Stone") || other.CompareTag("SpecialItem") || other.CompareTag("Item") || other.CompareTag("Trap_Other") || other.CompareTag("Meat") || other.CompareTag("arrow") || other.CompareTag("Bullet")) && !GameManager.instance.targetCollects.Exists(other.transform.parent))
        {
            Transform t;
            if (other.CompareTag("Hostage") || other.CompareTag("Item") || other.CompareTag("Chest") || other.CompareTag("Sword") || other.CompareTag("Tag_Stone") || other.CompareTag("Trap_Other") || other.CompareTag("Meat") && other.GetComponent<WolfMeat>() != null || other.CompareTag("arrow") || other.CompareTag("Bullet") || other.CompareTag("SpecialItem"))
            {
                t = other.transform;
                _origin = other.gameObject.transform.localScale;
            }
            else
            {
                t = other.transform.parent;
                _origin = other.gameObject.transform.parent.localScale;
            }

            if (GameManager.instance.targetCollects.Exists(t)) return;

            GameManager.instance.targetCollects.Add(t);

            if (other.CompareTag("Bullet") || other.GetComponent<CircleCollider2D>() != null)
            {
                t.TryGetComponent(out Collider2D col);
                if (col != null)
                {
                    col.enabled = false;
                }
            }
            t.TryGetComponent(out Rigidbody2D rid);
            if (rid != null)
            {
                rid.velocity = Vector2.zero;
                rid.angularVelocity = 0;
            }
            t.DOScale(Vector3.zero, durationMagnet);
            t.DOMove(transform.position, durationMagnet);
            if (!(SoundManager.Instance is null)) SoundManager.Instance.PlaySound(SoundManager.Instance.teleportSound);
            Observable.Timer(TimeSpan.FromSeconds(durationTelePort)).Subscribe(_ => { linkedPortal.Recive(t, linkedPortal.transform.position, _origin); }).AddTo(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="position"></param>
    public void Recive(Transform target, Vector3 position, Vector3 originScale)
    {
        target.localScale = Vector3.zero;
        var scale = originScale;
        Vector2 velocity;
        target.position = position;

        var enemy = target.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.target = null;
            enemy.PlayIdle();
            switch (direction)
            {
                case PortalDirection.Left:
                    enemy.saPlayer.skeleton.ScaleX = 1;
                    break;
                case PortalDirection.Right:
                    enemy.saPlayer.skeleton.ScaleX = -1;
                    break;
            }
        }

        var character = target.GetComponent<PlayerManager>();
        if (character != null)
        {
            switch (direction)
            {
                case PortalDirection.Left:
                    character.skeleton.skeleton.ScaleX = -1;
                    break;
                case PortalDirection.Right:
                    character.skeleton.skeleton.ScaleX = 1;
                    break;
            }
        }

        var hostage = target.GetComponent<HostageManager>();
        if (hostage != null)
        {
            switch (direction)
            {
                case PortalDirection.Left:
                    hostage.skeleton.skeleton.ScaleX = -1;
                    break;
                case PortalDirection.Right:
                    hostage.skeleton.skeleton.ScaleX = 1;
                    break;
            }
        }


        if (target.GetComponent<CircleCollider2D>() != null)
        {
            target.TryGetComponent(out Collider2D col);
            if (col != null) col.enabled = true;
        }

        if (target.CompareTag("arrow"))
        {
            switch (direction)
            {
                case PortalDirection.Up:
                    velocity = Vector2.up * 6;
                    target.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case PortalDirection.Down:
                    velocity = Vector2.down * 6;
                    scale.Set(-1, 1, 1);
                    target.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case PortalDirection.Left:
                    velocity = Vector2.left * 6;
                    target.eulerAngles = new Vector3(0, 0, 180);
                    break;
                case PortalDirection.Right:
                    velocity = Vector2.right * 6;
                    scale.Set(-1, 1, 1);
                    target.eulerAngles = new Vector3(0, 0, 180);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Observable.Timer(TimeSpan.FromSeconds(durationMagnet)).Subscribe(_ => { target.GetComponent<Rigidbody2D>().velocity = velocity; }).AddTo(this);
        }
        else if (target.CompareTag("Bullet"))
        {
            scale = new Vector3(0.4f, 0.4f, 0.4f);
            switch (direction)
            {
                case PortalDirection.Up:
                    velocity = Vector2.up * 5;
                    break;
                case PortalDirection.Down:
                    velocity = Vector2.down * 5;
                    break;
                case PortalDirection.Left:
                    velocity = Vector2.left * 5;
                    break;
                case PortalDirection.Right:
                    velocity = Vector2.right * 5;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            target.TryGetComponent(out Rigidbody2D rigid);
            if (rigid != null)
            {
                rigid.velocity = velocity;
            }

            target.TryGetComponent(out Collider2D col);
            if (col != null)
            {
                col.enabled = true;
            }
        }
        else if (target.CompareTag("Sword") && !target.GetComponent<HolyWaterItem>())
        {
            scale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (target.CompareTag("Tag_Stone") && target.GetComponent<BombItem>() && !target.GetComponent<MineItem>())
        {
            scale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        else if (target.CompareTag("Trap_Other") && target.parent.GetComponent<TrapArrow>() != null)
        {
            target.SetParent(MapLevelManager.Instance.transform, true);
            switch (direction)
            {
                case PortalDirection.Up:
                    velocity = Vector2.up * 6;
                    scale.Set(-1, 1, 1);
                    target.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case PortalDirection.Down:
                    velocity = Vector2.down * 6;
                    target.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case PortalDirection.Left:
                    velocity = Vector2.left * 6;
                    scale.Set(-1, 1, 1);
                    target.eulerAngles = new Vector3(0, 0, 180);
                    break;
                case PortalDirection.Right:
                    velocity = Vector2.right * 6;
                    target.eulerAngles = new Vector3(0, 0, 180);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Observable.Timer(TimeSpan.FromSeconds(durationMagnet)).Subscribe(_ => { target.GetComponent<Rigidbody2D>().velocity = velocity; }).AddTo(this);
        }

        target.DOScale(scale, durationMagnet);
    }

    private enum PortalDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
