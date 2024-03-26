using UnityEngine;

public class PrefabResources : ScriptableObject
{
    private static PrefabResources instance;
    public static PrefabResources Instance => instance ? instance : instance = Resources.Load<PrefabResources>("PrefabResources");

    public LevelRoot root;
}