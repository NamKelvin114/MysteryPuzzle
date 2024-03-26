using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[Serializable]
public class SaveHero
{
    public bool unlock;
}

[Serializable]
public class SaveCastle
{
    public bool unlock;
    public int star;
}

[Serializable]
public class SaveItem
{
    public bool unlock;
}

public class DataController : MonoBehaviour
{
    public static DataController instance;

    public List<SaveHero> SaveHero { get; } = new List<SaveHero>();

    public List<SaveItem> SaveItems { get; } = new List<SaveItem>();

    public List<SaveHero> SavePrincess { get; } = new List<SaveHero>();

    public List<SaveCastle> SaveCastle { get; } = new List<SaveCastle>();
    public PetDataController petDataController;
    public TaskDataController taskDataController;

    public bool[] CheckCastleNotification()
    {
        bool[] regions = { false, false, false, false, false, false };

        for (int i = 0; i < SaveCastle.Count; i++)
        {
            var index = i / 7;

            if (index >= 0 && index <= 5)
            {
                int currentStar = SaveCastle[i].star;
                if (currentStar > 5)
                {
                    currentStar = 5;
                }

                var star = currentStar + index * 5;

                if (star < 0)
                {
                    star = 0;
                }
                else if (star >= Config.CostPurchaseByStars.Length)
                {
                    star = Config.CostPurchaseByStars.Length - 1;
                }

                if (Utils.currentCoin >= Config.CostPurchaseByStars[star] && currentStar < 5)
                {
                    if (index == 0 || index == 1 && Utils.CurrentLevel >= 200 || index == 2 && Utils.CurrentLevel >= 500 || index == 3 && Utils.CurrentLevel >= 1000 ||
                        index == 4 && Utils.CurrentLevel >= 1500 || index == 5 && Utils.CurrentLevel >= 2000)
                    {
                        regions[index] = true;
                    }
                }
            }
        }

        return regions;
    }

    public void CheckWarningForTask()
    {
        // check main task complete
        var mainTask = taskDataController.GetCurrentMainTask();
        if (mainTask != null)
        {
            int id = mainTask.CurrentTask;
            var collectionItemList = mainTask.collectionPage.CollectionItemList;
            int curCount = 0;
            if (id > collectionItemList.Count - 1) curCount = 0;
            else curCount = collectionItemList[id].IsUnlocked ? 1 : 0;
            
            if (curCount == 1)
            {
                Observer.ShowHideTaskWarning?.Invoke(true);
                return;
            }
        }

        // check normal task complete
        var listNormalTask = taskDataController.normalTaskDataList;
        foreach (var taskData in listNormalTask)
        {
            int id = taskData.CurrentTask;
            int maxNumber = taskData.taskDataList[Math.Min(id, taskData.taskDataList.Count - 1)].number;
            int curCount = Math.Min(taskData.TaskCount, maxNumber);
            if (curCount == maxNumber)
            {
                Observer.ShowHideTaskWarning?.Invoke(true);
                return;
            }
        }
        
        Observer.ShowHideTaskWarning?.Invoke(false);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
            DestroyImmediate(gameObject);
    }

    private TextAsset _ta;
    private JsonData _jsonData;
    private string _stringDataHero;
    private string _stringDataPrincess;
    private string _stringDataItem;
    private string _stringDataCastle;
    private int _currentLevel;

    private void LoadData()
    {
        Data.currentHero = PlayerPrefs.GetInt(Data.CURRENTHERO);
        Data.currentPrincess = PlayerPrefs.GetInt(Data.CURRENTPRINCESS);
        Data.currentPet = PlayerPrefs.GetInt(Data.CURRENT_PET, -1);
        Data.petLevel = PlayerPrefs.GetInt(Data.CURRENT_PET_LEVEL, 1);
        Utils.currentCoin = PlayerPrefs.GetInt(Data.TOTALCOIN, 0);
        Data.TotalGoldMedal = PlayerPrefs.GetInt(Data.TOTALGOLDMEDAL, 0);
        Data.firsttime = PlayerPrefs.GetInt(Data.FIRSTTIME);
        LoadHero();
        LoadPrincess();
        LoadCastle();
        LoadItem();
        petDataController.LoadDataEggShard();
        taskDataController.InitData();
    }

    private void LoadCastle()
    {
        for (int i = 0; i < 42; i++)
        {
            SaveCastle.Add(new SaveCastle());
        }

        _stringDataCastle = PlayerPrefs.GetString(Data.SAVECASTLE);
        if (!string.IsNullOrEmpty(_stringDataCastle))
        {
            try
            {
                _jsonData = JsonMapper.ToObject(_stringDataCastle);
                for (int i = 0; i < _jsonData.Count; i++)
                {
                    if (_jsonData[i] != null)
                    {
                        SaveCastle[i] = JsonMapper.ToObject<SaveCastle>(_jsonData[i].ToJson());
                    }
                }
            }
            catch (Exception)
            {
                for (int i = 0; i < 42; i++)
                {
                    SaveCastle.Add(new SaveCastle());
                }
            }
        }
    }

    private void LoadHero()
    {
        var heroLength = HeroData.Length;
        for (int i = 0; i < heroLength; i++)
        {
            SaveHero.Add(new SaveHero());
        }

        _stringDataHero = PlayerPrefs.GetString(Data.SAVEHERO);

        if (!string.IsNullOrEmpty(_stringDataHero))
        {
            _jsonData = JsonMapper.ToObject(_stringDataHero);
            for (int i = 0; i < _jsonData.Count; i++)
            {
                if (_jsonData[i] != null)
                {
                    SaveHero[i] = JsonMapper.ToObject<SaveHero>(_jsonData[i].ToJson());
                }
            }
        }

        SaveHero[0].unlock = true;
    }

    private void LoadItem()
    {
        var itemLength = ItemCollection.Length;
        for (int i = 0; i < itemLength; i++)
        {
            SaveItems.Add(new SaveItem());
        }

        _stringDataItem = PlayerPrefs.GetString(Data.SAVEITEM);
        if (!string.IsNullOrEmpty(_stringDataItem))
        {
            _jsonData = JsonMapper.ToObject(_stringDataItem);
            for (int i = 0; i < _jsonData.Count; i++)
            {
                if (_jsonData[i] != null)
                {
                    SaveItems[i] = JsonMapper.ToObject<SaveItem>(_jsonData[i].ToJson());
                }
            }
        }
    }

    private void LoadPrincess()
    {
        for (int i = 0; i < HeroData.LengthPrincess; i++)
        {
            SavePrincess.Add(new SaveHero());
        }

        _stringDataPrincess = PlayerPrefs.GetString(Data.SAVE_PRINCESS);

        if (!string.IsNullOrEmpty(_stringDataPrincess))
        {
            _jsonData = JsonMapper.ToObject(_stringDataPrincess);
            for (int i = 0; i < _jsonData.Count; i++)
            {
                if (_jsonData[i] != null)
                {
                    SavePrincess[i] = JsonMapper.ToObject<SaveHero>(_jsonData[i].ToJson());
                }
            }
        }

        SavePrincess[0].unlock = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnlockAllHero()
    {
        SaveHero.Clear();
        var heroLength = HeroData.Length;
        for (int i = 0; i < heroLength; i++)
        {
            SaveHero.Add(new SaveHero { unlock = true });
        }

        PlayerPrefs.SetString(Data.SAVEHERO, JsonMapper.ToJson(SaveHero));

        SavePrincess.Clear();
        for (int i = 0; i < HeroData.LengthPrincess; i++)
        {
            var princess = new SaveHero { unlock = true };
            SavePrincess.Add(princess);
        }

        PlayerPrefs.SetString(Data.SAVE_PRINCESS, JsonMapper.ToJson(SavePrincess));
    }

    public void SaveData()
    {
        PlayerPrefs.SetString(Data.SAVEHERO, JsonMapper.ToJson(SaveHero));
        PlayerPrefs.SetString(Data.SAVE_PRINCESS, JsonMapper.ToJson(SavePrincess));
        PlayerPrefs.SetInt(Data.CURRENTHERO, Data.currentHero);
        PlayerPrefs.SetInt(Data.CURRENTPRINCESS, Data.currentPrincess);
        PlayerPrefs.SetInt(Data.CURRENT_PET, Data.currentPet);
        PlayerPrefs.SetInt(Data.CURRENT_PET_LEVEL, Data.petLevel);
        PlayerPrefs.SetString(Data.SAVECASTLE, JsonMapper.ToJson(SaveCastle));
        SaveItem();
        PlayerPrefs.SetInt(Data.TOTALCOIN, Utils.currentCoin);
        PlayerPrefs.SetInt(Data.TOTALGOLDMEDAL, Data.TotalGoldMedal);
        PlayerPrefs.SetInt(Data.FIRSTTIME, Data.firsttime);
        petDataController.SaveDataEggShard();
    }

    public void SaveItem()
    {
        PlayerPrefs.SetString(Data.SAVEITEM, JsonMapper.ToJson(SaveItems));
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveData();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    public int TotalSkinUnlocked()
    {
        int count = 0;

        for (int i = 0; i < SaveHero.Count; i++)
        {
            if (SaveHero[i].unlock)
            {
                count++;
            }
        }

        for (int i = 0; i < SavePrincess.Count; i++)
        {
            if (SavePrincess[i].unlock)
            {
                count++;
            }
        }

        return count;
    }

    public void LoadByServer()
    {
        LoadHero();
        LoadPrincess();
        LoadCastle();
        LoadItem();
        petDataController.LoadDataEggShard();
    }
}