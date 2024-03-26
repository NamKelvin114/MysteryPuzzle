using UnityEngine;

public abstract class LevelObject : MonoBehaviour
{
    public LevelRoot Root { get; protected set; }

    public void OnSpawnedInRoot(
        LevelRoot root)
    {
        Root = root;
        OnSpawned();
    }

    protected virtual void OnSpawned() { }
    public virtual void StartPlaying() { }
    public virtual void StopPlaying() { }
}