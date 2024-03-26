using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TravelStoryData", menuName = "ScriptableObject/TravelStoryData")]
public class TravelStoryData : ScriptableObject
{
    [SerializeField] private List<TravelStoryDataItem> travelStoryDataItems;
    public int TravelStoryDataItemCount => travelStoryDataItems.Count;
    public TravelStoryDataItem GetTravelStoryById(int id)
    {
        if (id < 0 || id >= travelStoryDataItems.Count) return null;
        return travelStoryDataItems[id];
    }
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        for (int i = 0; i < travelStoryDataItems.Count; i++)
        {
            travelStoryDataItems[i].ResetId(i);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}