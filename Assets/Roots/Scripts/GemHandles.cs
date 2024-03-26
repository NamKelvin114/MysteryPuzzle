using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GemHandles : MonoBehaviour
{
    public Gems[] gems;
    
    private void Start()
    {
        GameManager.instance.TotalGems = gems.Length - (int) (gems.Length * 0.3f);
        PlaySoundGem();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ranIndex"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundGemApear(int ranIndex, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(SoundManager.Instance.acCoinApear[ranIndex]);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlaySoundGem()
    {
        if (SoundManager.Instance == null) return;

        var index = Random.Range(0, SoundManager.Instance.acCoinApear.Length);
        StartCoroutine(PlaySoundGemApear(index));
    }
}
