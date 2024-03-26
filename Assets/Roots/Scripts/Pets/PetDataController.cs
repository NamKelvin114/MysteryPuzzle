using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class PetDataController : MonoBehaviour
{
    public const string EGG_SHARE_STORE_KEY = "egg_shard_store";
    public List<UserEggShard> saveEggShards = new List<UserEggShard>();
    private string _stringDataEggShard;
    private JsonData _jsonData;

    public void LoadDataEggShard()
    {
        saveEggShards.Clear();

        for (int i = 0; i < PetCollection.Length; i++)
        {
            saveEggShards.Add(new UserEggShard()
            {
                id = i,
                level = 1,
                number = 0,
                segment = 0,
                unlocked = false
            });
        }

        _stringDataEggShard = PlayerPrefs.GetString(EGG_SHARE_STORE_KEY);
        if (!string.IsNullOrEmpty(_stringDataEggShard))
        {
            _jsonData = JsonMapper.ToObject(_stringDataEggShard);
            var count = _jsonData.Count;
            if (count > PetCollection.Length)
            {
                count = PetCollection.Length;
            }

            for (int i = 0; i < count; i++)
            {
                if (_jsonData[i] != null)
                {
                    saveEggShards[i] = JsonMapper.ToObject<UserEggShard>(_jsonData[i].ToJson());
                }
            }
        }
    }

    public void SaveDataEggShard() { PlayerPrefs.SetString(EGG_SHARE_STORE_KEY, JsonMapper.ToJson(saveEggShards)); }

    public int CurrentNumberShard(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id) return saveEggShard.number;
        }

        return 0;
    }

    public int GetSegment(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id) return saveEggShard.segment;
        }

        return 0;
    }

    public (int, int) GetLevelAndSegement(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id) return (saveEggShard.level, saveEggShard.segment);
        }

        return (0, 0);
    }
    
    public int GetLevel(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id) return saveEggShard.level;
        }

        return 0;
    }

    public void UnlockPet(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id) saveEggShard.unlocked = true;
        }
    }

    public void SetLevelAndSegement(int id, int level, int segement)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id)
            {
                saveEggShard.level = level;
                saveEggShard.segment = segement;
            }
        }
    }

    public bool IsUnlocked(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id) return saveEggShard.unlocked;
        }

        return false;
    }

    public bool IsAvaiableClaim(int id)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id)
            {
                return saveEggShard.number >= PetCollection.GetNumberShardNeedToBreak(id);
            }
        }

        return false;
    }

    public bool HasNotification()
    {
        var flag = false;
        for (int i = 0; i < saveEggShards.Count; i++)
        {
            if (!saveEggShards[i].unlocked && saveEggShards[i].number >= PetCollection.GetNumberShardNeedToBreak(i))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }

    public void AddEggShard(int id, int count = 1)
    {
        foreach (var saveEggShard in saveEggShards)
        {
            if (saveEggShard.id == id)
            {
                saveEggShard.number += count;
                break;
            }
        }
    }
}