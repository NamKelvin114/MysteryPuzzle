using UnityEngine;
using Worldreaver.Utility;

public class EnemyLava : EnemyBase
{
    protected override void InitSoundAttack()
    {
        if (SoundManager.Instance != null)
        {
            attackAudio = SoundManager.Instance.punch;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_charStage == CHAR_STATE.PLAYING && !IsTakeHolyWater)
        {
            if (collision.CompareTag("Trap_Other") || collision.CompareTag(Utils.TAG_GAS) || collision.CompareTag("arrow") || collision.CompareTag(Utils.TAG_ICE_WATER))
            {
                var dieReason = EDieReason.Normal;

                if (collision.CompareTag(Utils.TAG_ICE_WATER))
                {
                    dieReason = EDieReason.Ice;
                }
                if (collision.GetComponent<Trap4>())
                {
                    return;
                }

                if (!flagVibrateDie)
                {
                    flagVibrateDie = true;
                    GameManager.instance.SoftButton();
                }

                OnDie(dieReason);
                if (collision.CompareTag("arrow"))
                {
                    collision.gameObject.SetActive(false);
                    if (GameManager.instance.targetCollects.Exists(collision.transform))
                    {
                        GameManager.instance.targetCollects.Remove(collision.transform);
                    }
                }
            }
        }
    }
}