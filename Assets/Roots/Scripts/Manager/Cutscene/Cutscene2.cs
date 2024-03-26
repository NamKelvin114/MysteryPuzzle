using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene2 : MonoBehaviour
{
    // Start is called before the first frame update
    public void endCutscene()
    {
        Observer.endCutscene2?.Invoke();
    }
    public void StopSound()
    {
        SoundManager.Instance.StopBGSound();
        SoundManager.Instance.StopSound();
    }
    public void PlaySound()
    {
        SoundManager.Instance.PlaySound(GameManager.instance.CutsceneController.startByPlane);
    }
}
