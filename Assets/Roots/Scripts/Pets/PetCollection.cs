using System;
using System.Collections.Generic;
using System.Linq;
using Lance.Common;
using Spine.Unity;
using UnityEngine;
using Random = System.Random;

public class PetCollection : ScriptableObject
{
    private static PetCollection instance;
    private static PetCollection Instance => instance ? instance : (instance = Resources.Load<PetCollection>("PetCollection"));

    public SkeletonDataAsset[] petDataAssets;
    public PetInfo[] infos;

    public int[] costUpgradeLevel2;
    public int[] costUpgradeLevel3;
    public int[] costUpgradeLevel4;
    public int[] costUpgradeLevel5;

    public static readonly Random Rnd = new Random();

    public static int Length => Instance.infos.Length;

    public static SkeletonDataAsset GetSkeletonAsset(int index)
    {
        if (index > Instance.petDataAssets.Length - 1)
        {
            index = Instance.petDataAssets.Length - 1;
        }

        return Instance.petDataAssets[index];
    }

    public static int GetNumberShardNeedToBreak(int id)
    {
        for (int i = 0; i < Length; i++)
        {
            if (instance.infos[i].id == id)
            {
                return instance.infos[i].numberShardBreak;
            }
        }

        return 0;
    }

    public static int PickRandomEggShard()
    {
        var pets = new List<PetInfo>();
        for (int i = 0; i < 6; i++)
        {
            if (DataController.instance.petDataController.CurrentNumberShard(instance.infos[i].id) < instance.infos[i].numberShardBreak)
            {
                pets.Add(instance.infos[i]);
            }
        }

        bool flagNewPetAdded = false;
        if (pets.Count == 0 || pets.Count == 1 && pets[0].id == 5)
        {
            for (int i = 6; i < Instance.infos.Length; i++)
            {
                if (DataController.instance.petDataController.CurrentNumberShard(instance.infos[i].id) < instance.infos[i].numberShardBreak)
                {
                    flagNewPetAdded = true;
                    pets.Add(instance.infos[i]);
                }
            }

            if (pets.Count == 0) return -1;
        }

        var (rateReCalculate, id) = CheckMaximum(ref pets);

        // if (id == 5 && flagNewPetAdded)
        // {
        //     
        // }

        // Calculate the summa of all portions.
        int poolSize = 0;
        for (int i = 0; i < pets.Count; i++)
        {
            if (pets[i].id == id)
            {
                poolSize += rateReCalculate;
            }
            else
            {
                poolSize += pets[i].rateCollect;
            }
        }

        // Get a random integer from 0 to PoolSize.
        int randomNumber = Rnd.Next(0, poolSize) + 1;

        // Detect the item, which corresponds to current random number.
        int accumulatedProbability = 0;
        for (int i = 0; i < pets.Count; i++)
        {
            if (pets[i].id == id)
            {
                accumulatedProbability += rateReCalculate;
            }
            else
            {
                accumulatedProbability += pets[i].rateCollect;
            }

            if (randomNumber <= accumulatedProbability)
            {
                return pets[i].id;
            }
        }

        return -1; // this code will never come while you use this programm right :)
    }

    /// <summary>
    /// rate calculate / id
    /// </summary>
    /// <param name="petCollects"></param>
    /// <returns></returns>
    private static (int, int) CheckMaximum(ref List<PetInfo> petCollects)
    {
        int MakeRate(ref List<PetInfo> petCollectLocals, int index)
        {
            var tempRate = instance.infos[index].rateCollect;
            var result1 = CalculateRate(index, 0);
            tempRate += result1.Item2;
            if (result1.Item1)
            {
                foreach (var collect in petCollectLocals.Where(collect => collect.id == 0))
                {
                    petCollectLocals.Remove(collect);
                    break;
                }
            }

            var result2 = CalculateRate(index, 1);
            tempRate += result2.Item2;
            if (result2.Item1)
            {
                foreach (var collect in petCollectLocals.Where(collect => collect.id == 1))
                {
                    petCollectLocals.Remove(collect);
                    break;
                }
            }

            var result3 = CalculateRate(index, 2);
            tempRate += result3.Item2;
            if (result3.Item1)
            {
                foreach (var collect in petCollectLocals.Where(collect => collect.id == 2))
                {
                    petCollectLocals.Remove(collect);
                    break;
                }
            }

            var result4 = CalculateRate(index, 3);
            tempRate += result4.Item2;
            if (result4.Item1)
            {
                foreach (var collect in petCollectLocals.Where(collect => collect.id == 3))
                {
                    petCollectLocals.Remove(collect);
                    break;
                }
            }

            var result5 = CalculateRate(index, 4);
            tempRate += result5.Item2;
            if (result5.Item1)
            {
                foreach (var collect in petCollectLocals.Where(collect => collect.id == 4))
                {
                    petCollectLocals.Remove(collect);
                    break;
                }
            }

            var result6 = CalculateRate(index, 5);
            tempRate += result6.Item2;
            if (result6.Item1)
            {
                foreach (var collect in petCollectLocals.Where(collect => collect.id == 5))
                {
                    petCollectLocals.Remove(collect);
                    break;
                }
            }

            // todo return
            return tempRate;
        }

        (bool, int) CalculateRate(int id1, int id2)
        {
            if (instance.infos[id1].maximumNumberCollects[id2] == -1)
            {
                return (false, 0);
            }

            if (DataController.instance.petDataController.CurrentNumberShard(id2) >= instance.infos[id1].maximumNumberCollects[id2])
            {
                return (true, instance.infos[id2].rateCollect);
            }

            return (false, 0);
        }

        int rate = 0;
        PetDataController pet = DataController.instance.petDataController;
        if (!pet.IsAvaiableClaim(0))
        {
            return (MakeRate(ref petCollects, 0), 0);
        }

        if (!pet.IsAvaiableClaim(1))
        {
            return (MakeRate(ref petCollects, 1), 1);
        }

        if (!pet.IsAvaiableClaim(2))
        {
            return (MakeRate(ref petCollects, 2), 2);
        }

        if (!pet.IsAvaiableClaim(3))
        {
            return (MakeRate(ref petCollects, 3), 3);
        }

        if (!pet.IsAvaiableClaim(4))
        {
            return (MakeRate(ref petCollects, 4), 4);
        }

        // if (!pet.IsAvaiableClaim(5))
        // {
        //     return (MakeRate(ref petCollects, 5), 5);
        // }

        return (instance.infos[5].rateCollect, 5);
    }

    public static int GetCostUpgrade(int level, int segment)
    {
        switch (level)
        {
            case 1:
                return Instance.costUpgradeLevel2[segment];
            case 2:
                return Instance.costUpgradeLevel3[segment];
            case 3:
                return Instance.costUpgradeLevel4[segment];
            case 4:
                return Instance.costUpgradeLevel5[segment];
        }

        return 5000;
    }
}

[Serializable]
public struct PetInfo
{
    public int id;
    public string name;
    public int numberShardBreak;
    public int rateCollect;
    public int[] maximumNumberCollects;
}