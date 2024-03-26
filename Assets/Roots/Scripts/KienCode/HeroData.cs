using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class HeroData : ScriptableObject
{
    private static HeroData instance;
    public static HeroData Instance => instance ? instance : (instance = Resources.Load<HeroData>("SkinDatas"));
    [SerializeField] private SkeletonDataAsset heroDataAsset;

    [SerializeField] public Info[] infos;

    [SerializeField] private Info[] infoPrincess;

    #region open api

    public static SkeletonDataAsset HeroDataAsset => Instance.heroDataAsset;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string NameHeroWithIndex(int index) => $"Player_{index + 1}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string NamePrincessWithIndex(int index) => $"Wife{index + 1}";

    /// <summary>
    /// (name skin normal, name skin with stick)
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static (string, string) SkinHeroByIndex(int index)
    {
        if (index > Instance.infos.Length - 1) return ("", "");
        if (index == -1)
            return ("Main","");
        return ($"Main{index + 1}", $"Main{index + 1}/Stick");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string SkinPrincessByIndex(int index)
    {
        
        return $"Pet{index+1}";
        // return "Pet1";
        // if (index > Instance.infoPrincess.Length - 1) return "";
        // return $"Wife{index + 1}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Info HeroInfoByIndex(int index)
    {
        if (index > Instance.infos.Length - 1) return default;

        return Instance.infos[index];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Info PrincessInfoByIndex(int index) { return index > Instance.infoPrincess.Length - 1 ? default : Instance.infoPrincess[index]; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Dictionary<int, Info> HeroCanCollect()
    {
        var caches = new Dictionary<int, Info>();
        for (int i = 0; i < Instance.infos.Length; i++)
        {
            var info = Instance.infos[i];
            if ((info.typeUnlock == EHeroTypeUnlock.Coin || info.typeUnlock == EHeroTypeUnlock.VideoReward) && !DataController.instance.SaveHero[i].unlock)
                caches.Add(i, info);
        }

        return caches;
    }

    public static (bool, string) HeroHasPetByIndex(int index)
    {
        var info = HeroInfoByIndex(index);
        bool hasPet = info.hasPet;
        string nameSkinPet;
        switch (index)
        {
            case 3:
                nameSkinPet = "Eagle";
                break;
            case 4:
                nameSkinPet = "Angel";
                break;
            case 5:
                nameSkinPet = "Demon";
                break;
            case 32:
                nameSkinPet = "Egg";
                break;
            default:
                nameSkinPet = "";
                break;
        }

        return (hasPet, nameSkinPet);
    }

    public static string TransformSuperMan() => "SuperMan/Main";
    public static string TransfromWonder() => "Wonder";

    public static (string, string) GetAnimationPetGiveEgg() { return ($"Egg{Data.idEasterEgg + 1}", $"Egg{Data.idEasterEgg + 1}{Data.idEasterEgg + 1}"); }

    /// <summary>
    /// number hero
    /// </summary>
    public static int Length => Instance.infos.Length;

    /// <summary>
    /// number princess
    /// </summary>
    public static int LengthPrincess => Instance.infoPrincess.Length;

    #endregion

#if UNITY_EDITOR
    [ContextMenu("Execute")]
    public void Execute()
    {
        var listOfSkins = heroDataAsset.GetSkeletonData(false).Skins;
        foreach (var skin in listOfSkins)
        {
            Debug.Log(skin.Name);
        }
    }
#endif
}

[System.Serializable]
public struct Info
{   
    public EventValentine ValentineType;
    public EHeroTypeUnlock typeUnlock;
    public int price;
    public int NumBerGoldEvent;
    public bool hasPet;
    public bool isAvailable; // default is avaiable
    
}
