using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Data Item", menuName = "DataItem")]
public class ItemCollection : ScriptableObject
{
    private static ItemCollection instance;
    public static ItemCollection Instance => instance ? instance : (instance = Resources.Load<ItemCollection>("DataItem"));
    
    public string[] infos;

    public static int Length => Instance.infos.Length;
}