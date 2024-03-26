using UnityEngine;

public class BaseItem : MonoBehaviour, IItem
{
    [SerializeField] protected AudioClip audioAction;
    public virtual void PlayActionAudio()
    {
        if (audioAction!=null)
        {
            SoundManager.Instance.PlaySound(audioAction);
        }
    }
}
