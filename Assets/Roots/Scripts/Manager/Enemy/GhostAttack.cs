using System;
using Spine.Unity;
using UniRx;
using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;
    private IDisposable _disposable;

    public void CancleAfterAttack() { _disposable?.Dispose(); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        void Attack()
        {
            enemyBase.PlayAttack();

            _disposable?.Dispose();
            _disposable = Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => { enemyBase.PlayIdle(); }).AddTo(this);
        }

        if (other.CompareTag("BodyPlayer"))
        {
            var player = other.gameObject.GetComponentInParent<PlayerManager>();
            if (player != null && player.state == EUnitState.Playing)
            {
                if (!player.IsTakeHolyWater)
                {
                    Attack();
                    Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ => { player.OnDeath(EDieReason.Normal); }).AddTo(this);
                }
                else
                {
                    PlayerManager.instance.OnAttackEnemy(enemyBase);
                }
            }
        }

        if (other.TryGetComponent(out HostageManager hostage))
        {
            if (hostage != null && hostage.state == EUnitState.Playing)
            {
                if (!hostage.IsTakeHolyWater)
                {
                    Attack();
                    Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ => { hostage.OnDie(true); }).AddTo(this);
                }
                else
                {
                    HostageManager.instance.OnAttackEnemy(enemyBase);
                    //enemyBase.OnDie(EDieReason.Normal);
                }
            }
        }

        if (other.CompareTag("Enemy") || other.CompareTag("Wolf"))
        {
            other.transform.parent.TryGetComponent(out EnemyBase enemy);
            if (enemy != null && enemy._charStage == EnemyBase.CHAR_STATE.PLAYING && !enemy.IsTakeHolyWater)
            {
                Attack();
                Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ => { enemy.OnDie(EDieReason.Normal); }).AddTo(this);
            }
        }
    }
}