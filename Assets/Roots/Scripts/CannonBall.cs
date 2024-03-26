using System;
using UnityEngine;

public class CannonBall : LevelObject
{
    public Rigidbody2D rigidbody2D;
    public GameObject explodeEffect;

    private bool _flagVibrate;

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        CheckCollision(other);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collider2D"></param>
    private void CheckCollision(
        Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("ChangeDir") || collider2D.gameObject.CompareTag("Portal"))
        {
            return;
        }

        var character = collider2D.GetComponentInParent<PlayerManager>();
        if (character != null && !character.IsTakeHolyWater)
        {
            if (!GameManager.instance.FlagCharacterDie)
            {
                GameManager.instance.FlagCharacterDie = true;
                GameManager.instance.FailureButton();
            }

            character.OnDeath(EDieReason.Normal);
        }

        var hostage = collider2D.GetComponentInParent<CharsBase>();
        if (hostage != null && !hostage.IsTakeHolyWater)
        {
            if (!GameManager.instance.FlagHostageDie)
            {
                GameManager.instance.FlagHostageDie = true;
                GameManager.instance.FailureButton();
            }

            hostage.OnDie(true);
        }
        
        var enemy = collider2D.GetComponentInParent<EnemyBase>();
        if (enemy != null && !enemy.IsTakeHolyWater)
        {
            if (enemy._charStage == EnemyBase.CHAR_STATE.DIE)
            {
                return;
            }

            if (!_flagVibrate)
            {
                _flagVibrate = true;
                GameManager.instance.WarningButton();
            }

            enemy.OnDie(EDieReason.Normal);
        }
        
        Explode();
    }

    public void Explode()
    {
        if (explodeEffect != null)
        {
            explodeEffect.transform.SetParent(transform.parent);
            explodeEffect.SetActive(true);
            Destroy(explodeEffect.gameObject, 1);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.fireBallExplode);
        }

        Destroy(gameObject);
    }
}