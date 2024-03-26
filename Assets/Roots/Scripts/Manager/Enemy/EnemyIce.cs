using UnityEngine;
using Worldreaver.Utility;

public class EnemyIce : EnemyBase
{
    protected override void InitSoundAttack() { attackAudio = SoundManager.Instance.punch; }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_charStage == CHAR_STATE.PLAYING && !IsTakeHolyWater)
        {
            if (collision.gameObject.CompareTag(Utils.TAG_LAVA) || collision.gameObject.CompareTag("Trap_Other") || collision.gameObject.CompareTag(Utils.TAG_GAS) || collision.gameObject.CompareTag("arrow"))
            {
                var dieReason = EDieReason.Normal;
                if (collision.gameObject.CompareTag(Utils.TAG_LAVA))
                {
                    dieReason = EDieReason.Fire;
                }
                
                if (collision.gameObject.GetComponent<Trap4>())
                {
                    return;
                }
                
                if (!flagVibrateDie)
                {
                    flagVibrateDie = true;
                    GameManager.instance.SoftButton();
                }

                OnDie(dieReason);
                if (collision.gameObject.CompareTag("arrow"))
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