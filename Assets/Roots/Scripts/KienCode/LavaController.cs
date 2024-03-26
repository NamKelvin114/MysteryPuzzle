using System.Collections;
using UnityEngine;

public class LavaController : Unit
{
    private bool _flagVibrate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activeChangeStone) return;
        if (!MapLevelManager.Instance.isGameplay1)
        {
            var target = collision.gameObject.GetComponentInParent<Target>();
            if (target != null)
            {
                collision.gameObject.layer = 0;
                collision.gameObject.SetActive(false);
                var randomEffect = Random.Range(0, 100);
                if (randomEffect <= 10)
                {
                    if (ObjectPoolerManager.Instance == null) return;
                    var destroyEffect = ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
                    destroyEffect.transform.position = transform.position;
                    destroyEffect.SetActive(true);
                }

                MapLevelManager.Instance.OnTargetOut(target);
            }
        }
        else
        {
            if (collision.CompareTag("Tag_Win"))
            {
                if (!GameManager.instance.FlagGemMeetLava)
                {
                    GameManager.instance.FlagGemMeetLava = true;
                    GameManager.instance.WarningButton();
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
                }

                GameManager.instance.TotalGems--;

                if (GameManager.instance.TotalGems <= 0)
                {
                    if (PlayerManager.instance.state == EUnitState.Playing ||
                        PlayerManager.instance.state == EUnitState.Running)
                    {
                        if (GameManager.instance.gameState != EGameState.Win)
                            PlayerManager.instance.OnPlayerDie(EDieReason.Despair);
                    }
                }

                collision.gameObject.layer = 0;
                collision.gameObject.SetActive(false);
                var randomEffect = Random.Range(0, 100);
                if (randomEffect <= 10)
                {
                    if (ObjectPoolerManager.Instance == null) return;
                    var destroyEffect = ObjectPoolerManager.Instance.effectDestroyPooler.GetPooledObject();
                    destroyEffect.transform.position = transform.position;
                    destroyEffect.SetActive(true);
                }
            }
        }

        if (!activeChangeStone && collision.CompareTag("Tag_Stone") && collision.gameObject.name != "BigStone" &&
            collision.gameObject.name != "BigStoneBomb")
        {
            if (collision.CompareTag("Tag_Stone") && collision.GetComponentInParent<MineItem>())
            {
                return;
            }

            ChangeStone();
            GameManager.instance.VibrateStone();
        }


        if (MapLevelManager.Instance.isGameplay1)
            if (collision.gameObject.CompareTag("BodyPlayer"))
            {
                if (PlayerManager.instance.IsTakeHolyWater) return;

                if (!GameManager.instance.FlagCharacterDie)
                {
                    GameManager.instance.FlagCharacterDie = true;
                    GameManager.instance.FailureButton();
                }

                if (GameManager.instance.gameState != EGameState.Win)
                {
                    PlayerManager.instance.OnPlayerDie(EDieReason.Fire);
                }
            }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (GameManager.instance.FlagSoundLavaStickbarrie) return;

        if (other.gameObject.CompareTag("StickBarrie") &&
            other.gameObject.GetComponent<StickBarrier>()?.beginMove == true)
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

        StartCoroutine(PlaySoundLavaApear());
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