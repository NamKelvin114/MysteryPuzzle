using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TravelStoryDataItem", menuName = "ScriptableObject/TravelStoryDataItem")]
public class TravelStoryDataItem : ScriptableObject
{
    // Start is called before the first frame update
    [SerializeField] private int id;
    [SerializeField] private Sprite icon;
    [SerializeField] private string countryName;

    public int Id => id;
    public Sprite Icon => icon;
    public string CountryName => countryName;
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId(int Id)
    {
        id = Id;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}