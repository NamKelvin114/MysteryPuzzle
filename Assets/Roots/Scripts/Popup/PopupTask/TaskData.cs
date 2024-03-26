using System;
using System.Collections.Generic;
using Dat22.Attributes;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/TaskData")]
public class TaskData : ScriptableObject
{
    public string taskID;
    public List<TaskInfo> taskDataList;
    public string description;
    public Sprite taskIcon;
    public ETaskType taskType;
    public int currentTask = 0;
    public int taskCount = 0;
    public int CurrentTask
    {
        get => Data.GetInt($"TASK_DATA_{taskID}", 0);
        set
        {
            currentTask = value;
            Data.SetInt($"TASK_DATA_{taskID}", value);
        }
    }

    public int TaskCount
    {
        get => Data.GetInt($"TASK_DATA_COUNT_{taskID}", -1);
        set
        {
            taskCount = value;
            Data.SetInt($"TASK_DATA_COUNT_{taskID}", value);
        }
    }

    public void OnTaskCompleted()
    {
        TaskCount = -1;
        CurrentTask++;
    }
    
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        taskID = NamedIdAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

[Serializable]
public class TaskInfo
{
    public int number;
    public int reward;
}
public enum ETaskType
{
    PullPin,
    WinLevel,
    EatFoods,
    DestroyEnemy,
    None,
    // CollectKey,
    // SaveCat,
    // Drink,
    // CollectChest,
}