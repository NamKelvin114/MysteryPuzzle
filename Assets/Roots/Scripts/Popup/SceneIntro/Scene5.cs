using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class Scene5 : MonoBehaviour
{
    [SerializeField] private TransScene transScene1;
    void DoChangeScene()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.intro5);
        transScene1.DoTransScene(Done);
    }
    void PLaySound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.intro5);
    }
    void Done()
    {
        //Observer.nextIntroScene?.Invoke(this.rectTransform());
        Observer.CompleteIntro?.Invoke();
    }
}
