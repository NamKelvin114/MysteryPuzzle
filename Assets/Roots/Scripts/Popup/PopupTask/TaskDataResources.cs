using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/TaskDataResources")]
public class TaskDataResources : ScriptableObject
{
    public List<MainTaskData> mainTaskDataList;
    public List<TaskData> normalTaskDataList;
}

[Serializable]
public class MainTaskData
{
    public string description;
    public int reward = 1000;
    public CollectionPage collectionPage;
    public int CurrentTask
    {
        get => Data.GetInt($"MAIN_TASK_DATA_{collectionPage.id}", 0);
        set => Data.SetInt($"MAIN_TASK_DATA_{collectionPage.id}", value);
    }

    public bool IsClaimed
    {
        get => Data.GetBool($"MAIN_TASK_DATA_CLAIM_{collectionPage.id}", false);
        set => Data.SetBool($"MAIN_TASK_DATA_CLAIM_{collectionPage.id}", value);
    }
}
