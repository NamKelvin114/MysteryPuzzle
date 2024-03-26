using System.Collections;
using UniRx;
using UnityEngine;

public class IceWaterController : Unit
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activeChangeStone) return;
        if (!activeChangeStone &&
            (collision.CompareTag("Tag_Stone") && collision.gameObject.name != "BigStone" && collision.gameObject.name != "BigStoneBomb" &&
                !collision.gameObject.name.Contains("FallingStone") || collision.CompareTag("Trap_Lava")))
        {
            if (collision.CompareTag("Tag_Stone") && collision.GetComponentInParent<MineItem>())
            {
                return;
            }
            
            var b = false;
            if (!collision.CompareTag("Trap_Lava"))
            {
                var unit = collision.GetComponentInParent<Unit>();
                if (unit && unit.StoneState == EStoneState.IceStone)
                {
                    b = true;
                }
            }

            ChangeStone(b);
            GameManager.instance.VibrateStone();
        }

        // if (collision.CompareTag("Tag_Win"))
        // {
        //     if (!GameManager.instance.FlagGemMeetLava)
        //     {
        //         GameManager.instance.FlagGemMeetLava = true;
        //         GameManager.instance.WarningButton();
        //         if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
        //     }
        //
        //     GameManager.instance.TotalGems--;
        //
        //     if (GameManager.instance.TotalGems <= 0)
        //     {
        //         if (PlayerManager.Instance.state == EUnitState.Playing || PlayerManager.Instance.state == EUnitState.Running)
        //         {
        //             if (GameManager.instance.gameState != GameManager.GAMESTATE.WIN) PlayerManager.Instance.OnPlayerDie(EDieReason.Despair);
        //         }
        //     }
        //     
        //     collision.gameObject.layer = 0;
        //     collision.gameObject.SetActive(false);
        //     var randomEffect = Random.Range(0, 100);
        //     if (randomEffect <= 10)
        //     {
        //         if (ObjectPoolerManager.Instance == null) return;
        //         var destroyEffect = ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
        //         destroyEffect.transform.position = transform.position;
        //         destroyEffect.SetActive(true);
        //     }
        // }

        if (collision.gameObject.CompareTag("BodyPlayer"))
        {
            if (PlayerManager.instance.IsTakeHolyWater) return;

            if (!GameManager.instance.FlagCharacterDie)
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
                GameManager.instance.FlagCharacterDie = true;
                GameManager.instance.FailureButton();
            }

            if (PlayerManager.instance.state == EUnitState.Playing || PlayerManager.instance.state == EUnitState.Running)
            {
                if (GameManager.instance.gameState != EGameState.Win) PlayerManager.instance.OnPlayerDie(EDieReason.Ice);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (GameManager.instance.FlagSoundLavaStickbarrie) return;

        if (other.gameObject.CompareTag("StickBarrie") && other.gameObject.GetComponent<StickBarrier>()?.beginMove == true)
        {
            GameManager.instance.FlagSoundLavaStickbarrie = true;
            PlaySoundLava();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PlaySoundLava()
    {
        if (SoundManager.Instance == null) return;
        Observable.FromCoroutine(() => PlaySoundLavaApear()).Subscribe().AddTo(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundLavaApear(float delay = 0.2f)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaApear);
    }
}