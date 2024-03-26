#pragma warning disable 649
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BombItem : BaseItem
{
    [SerializeField] private float radiusForExplore = 4f;
    [SerializeField] private float radiusForForce = 4f;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private LayerMask explodeMask;
    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject iconBomp;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private bool isLeverActivated;
    [SerializeField] private bool isHang;
    [SerializeField] private float force;

    private bool _exploded;
    public Vector3 ExplosionCenter => collider2d ? collider2d.bounds.center : transform.position;
    public bool IsLeverActivated => isLeverActivated;

    private void OnDrawGizmosSelected()
    {
        DrawGizos(radiusForExplore);
        DrawGizos(radiusForForce);
    }
    void DrawGizos(float radius)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ExplosionCenter, radius);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("StickBarrie") || other.gameObject.CompareTag("Map"))
        {
            if (isHang == false)
            {
                return;
            }
        }
        if (other.gameObject.CompareTag("Rope"))
        {
            return;
        }

        CheckCollision(other.collider);
    }

    protected virtual void CheckCollision(Collider2D collision) { DoExplode(); }

    protected virtual void DoExplode()
    {
        if (!_exploded)
        {
            _exploded = true;
        }

        effect.transform.SetParent(transform.parent);
        effect.SetActive(true);
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        if (GameManager.instance.gameState != EGameState.Win && GameManager.instance.gameState != EGameState.Lose)
        {
            Observer.ShakeCamera?.Invoke();
            if (Application.platform == RuntimePlatform.Android && Data.UserVibrate)
            {
                Handheld.Vibrate();
            }
        }

        var center = ExplosionCenter;
        CheckExplode(center);
        CheckForce(center);
        iconBomp.gameObject.SetActive(false);
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.mineExplodeClip);
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback((() =>
        {
            collider2d.enabled = false;
            Destroy(gameObject);
        }));

        //SoundController.Current.PlayBombExplode();
    }
    void CheckExplode(Vector3 center)
    {
        var cols = Physics2D.OverlapCircleAll(center, radiusForExplore, explodeMask.value);
        var receivers = cols.Where(n => n.gameObject.name != "SearchCollider")
            .Select(c => c.gameObject)
            .Where(r => r != null)
            .Distinct()
            .ToList();
        foreach (var receiversexpl in receivers)
        {
            var setReceiver = receiversexpl.GetComponentInParent<IExplodeReceiver>();
            if (setReceiver != null) setReceiver.OnExplodedAt(this);
        }
    }
    void CheckForce(Vector3 center)
    {
        var cols = Physics2D.OverlapCircleAll(center, radiusForForce, explodeMask.value);
        var receivers = cols.Where(n => n.gameObject.name != "SearchCollider")
            .Select(c => c.gameObject)
            .Where(r => r != null)
            .Distinct()
            .ToList();
        foreach (var receiver in receivers)
        {
            var direction = receiver.transform.position - this.transform.position;
            if (receiver.GetComponentInParent<Rigidbody2D>() != null)
            {
                receiver.GetComponentInParent<Rigidbody2D>().AddForce(direction * force);
            }
        }
    }
    private void OnDestroy()
    {
        SoundManager.Instance.StopSound();
    }
}
