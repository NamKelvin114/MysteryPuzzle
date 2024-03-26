using System.Collections.Generic;
using UnityEngine;
using Dat22.Attributes;
using Pancake;
using UnityEditor;

[CreateAssetMenu(fileName = "CollectionPage", menuName = "ScriptableObject/CollectionPage")]
public class CollectionPage : ScriptableObject
{
    [SerializeField] public string id;
    [SerializeField] public Sprite pageSprite;
    [SerializeField] private List<CollectionItemData> collectionItemList;
    [SerializeField] private Sprite content;
    [SerializeField] private int rewardMoney = 5000;
    public int RewardMoney => rewardMoney;
    public Sprite Content => content;
    public List<CollectionItemData> CollectionItemList => collectionItemList;

    public bool CheckUnlocked()
    {
        foreach (var item in collectionItemList)
        {
            if (item.IsUnlocked == false)
            {
                return false;
            }
        }
        return true;
    }

    public CollectionItemData GetLastestItem()
    {
        foreach (var item in collectionItemList)
        {
            if (item.IsUnlocked == false)
                return item;
        }
        return null;
    }

    public CollectionItemData GetItemById(int id)
    {
        if(id < 0 || id >= collectionItemList.Count)
            return null;
        return collectionItemList[id];
    }
    
    public bool IsCollected
    {
        get
        {
            Data.IdCollected = id;
            return Data.IsCollected;
        }

        set
        {
            Data.IdCollected = id;
            Data.IsCollected = value;
            Debug.Log(value);
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        id = NamedIdAttributeDrawer.ToSnakeCase(name);
        foreach (var item in collectionItemList)
        {
            item.ResetId(id);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
