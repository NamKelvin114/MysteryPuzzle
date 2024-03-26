using UnityEngine;
using Worldreaver.Root.Attribute;

public class LevelRoot : MonoBehaviour
{
    [SerializeField] private FingerSlicer slicer;
    [SerializeField, ReadOnly] private int levelIndex;
    public int LevelIndex { get => levelIndex; private set => levelIndex = value; }

    public FingerSlicer Slicer => slicer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public void Setup(int level) { LevelIndex = level; }

    /// <summary>
    /// 
    /// </summary>
    public void SetupDynamics() { }

    /// <summary>
    /// 
    /// </summary>
    public void StartPlaying() { }

    /// <summary>
    /// 
    /// </summary>
    public void StopPlaying() { }
}