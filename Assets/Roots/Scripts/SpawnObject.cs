using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public List<Unit> gGems;
    public MapLevelManager.SPAWNTYPE _spawnType;
    private int _randomDisplayEffect;

    private void Start()
    {
        if (_spawnType == MapLevelManager.SPAWNTYPE.LAVA)
        {
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

    
    private void Update()
    {
        if (_spawnType != MapLevelManager.SPAWNTYPE.GAS) return;

        var deltaTime = Time.deltaTime;
        for (int i = 0; i < gGems.Count; i++) gGems[i].OnUpdate(deltaTime);
    }
}