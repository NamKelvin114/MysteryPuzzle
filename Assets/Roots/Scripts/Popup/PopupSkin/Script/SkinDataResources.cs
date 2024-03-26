using System.Collections.Generic;
using Dat22.Attributes;
using UnityEditor;
using UnityEngine;
using Worldreaver.Root.Attribute;

[CreateAssetMenu(fileName = "SkinDataResources", menuName = "ScriptableObject/SkinDataResources")]
public class SkinDataResources : ScriptableObject
{
    [SerializeField, ReadOnly] public string id;
    [SerializeField] public SkinItemType skinItemType;
    [SerializeField] public List<SkinData> skinDataList;

    public bool Contains(string skinName)
    {
        return (skinDataList.Find(item => (item.skinName == skinName)) != null);
    }

    public string CurrentSkin
    {
        get => PlayerPrefs.GetString(id, "");
        set
        {
            PlayerPrefs.SetString(id, value);
            Observer.CurrentSkinChanged?.Invoke();
            Observer.CurrentSkinPinChanged?.Invoke();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        id = NamedIdAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

public enum SkinItemType
{
    Hat,
    Shirt,
    Shoe,
    Pin,
    None,
}