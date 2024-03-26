using Dat22.Attributes;
using Pancake;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "ScriptableObject/SkinData")]
public class SkinData : ScriptableObject
{
    [NamedId, SerializeField] private string Id;
    [SerializeField] public SkeletonDataAsset skeletonDataAsset;

    [SerializeField, SpineSkin(dataField = "skeletonDataAsset")]
    public string skinName;

    [SerializeField] public Sprite spritePin;
    [SerializeField] public SkinBuyType skinBuyType;

    [ShowIf("skinBuyType", SkinBuyType.BuyCoin)]
    public int coinValue;

    [ShowIf("skinBuyType", SkinBuyType.Level)]
    public int levelUnlock;

    public Sprite shopIcon;

    //public bool isUnlock = false;
    public string skinNamePin => skeletonDataAsset ? "notpin" : Id;
    private string SkinId => skeletonDataAsset ? skinName : Id;

    public bool IsUnlocked
    {
        get => Data.GetBool($"SKINDATA_{SkinId}", skinBuyType == SkinBuyType.Default);
        set
        {
            Data.SetBool($"SKINDATA_{SkinId}", value);
            //isUnlock = value;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        Id = NamedIdAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

public enum SkinBuyType
{
    Default, // unlocked 
    BuyCoin,
    DailyReward,
    WatchAds,
    Task,
    Level,
}