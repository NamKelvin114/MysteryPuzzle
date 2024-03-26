using Spine.Unity;
using UnityEngine;

public class Trap4 : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeleton;
    [SerializeField] private AudioClip actionAudio;

    private bool _canUse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canUse)
        {
            return;
        }

        void Use()
        {
            if (actionAudio != null)
            {
                SoundManager.Instance.PlaySound(actionAudio);
            }
            _canUse = false;
            GetComponent<Collider2D>().enabled = false;
            skeleton.AnimationName = "Attack";
            skeleton.Initialize(true);
        }

        var player = other.GetComponentInParent<PlayerManager>();
        if (player != null && !player.IsTakeHolyWater)
        {
            if ((player.state == EUnitState.Playing || player.state == EUnitState.Running))
            {
                Use();
                if (!player.IsTakeHolyWater)
                {
                    player.OnDeath(EDieReason.Normal);
                }
                return;
            }
        }

        var enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy != null)
        {
            if ((enemy._charStage == EnemyBase.CHAR_STATE.PLAYING || enemy._charStage == EnemyBase.CHAR_STATE.RUNNING))
            {
                Use();
                if (!enemy.IsTakeHolyWater)
                {
                    enemy.OnDie(EDieReason.Normal);
                }
                return;
            }
        }

        var hostage = other.GetComponentInParent<CharsBase>();
        if (hostage == null) return;
        if ((hostage.state == EUnitState.Playing || hostage.state == EUnitState.Running))
        {
            Use();
            if (!hostage.IsTakeHolyWater)
            {
                hostage.OnDie(true);
            }
            return;
        }
    }
}
