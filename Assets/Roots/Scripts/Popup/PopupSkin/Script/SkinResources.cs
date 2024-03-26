using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinResources", menuName = "ScriptableObject/SkinResources")]
public class SkinResources : ScriptableObject
{
    [SerializeField] public SkinType skinType;
    [SerializeField] public List<SkinDataResources> skinDataResourcesList;

    public List<SkinData> GetLockedSkin(bool isShirt)
    {
        List<SkinData> listData = new List<SkinData>();
        foreach (var skinDataResource in skinDataResourcesList)
        {
            if (isShirt && skinDataResource.skinItemType == SkinItemType.Shirt)
            {
                foreach (var skinData in skinDataResource.skinDataList)
                {
                    if (skinData.IsUnlocked == false && skinData.skinBuyType == SkinBuyType.BuyCoin)
                    {
                        listData.Add(skinData);
                    }
                }
            }

            if (!isShirt && skinDataResource.skinItemType != SkinItemType.Shirt &&
                skinDataResource.skinItemType != SkinItemType.Pin)
            {
                foreach (var skinData in skinDataResource.skinDataList)
                {
                    if (skinData.IsUnlocked == false && skinData.skinBuyType == SkinBuyType.BuyCoin)
                    {
                        listData.Add(skinData);
                    }
                }
            }
        }

        return listData;
    }
}

public enum SkinType
{
    GirlSkin,
    Pin,
    None,
}