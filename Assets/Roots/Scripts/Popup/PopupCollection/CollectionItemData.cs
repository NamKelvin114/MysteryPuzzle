using UnityEditor;
using UnityEngine;
using Dat22.Attributes;
using Pancake;

[CreateAssetMenu(fileName = "CollectionItemData", menuName = "ScriptableObject/CollectionItemData")]
public class CollectionItemData : ScriptableObject
{
    public string id;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private string itemName = "";
    public Sprite ItemIcon => itemIcon;
    public string ItemName => itemName;
    
    public bool IsUnlocked
    {
        get => Data.GetBool($"COLLECTION_ITEM_DATA_{id}", false);
        set => Data.SetBool($"COLLECTION_ITEM_DATA_{id}", value);
    }
    
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId(string pageID)
    {
        id = pageID + "_" + NamedIdAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
    
}
