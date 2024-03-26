using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;
using Random = Pancake.Random;

public class TaskDataController : MonoBehaviour
{
    [SerializeField] private TaskDataResources taskDataResources;
    [SerializeField] private int normalTaskNumber = 2;
    public List<TaskData> normalTaskDataList;
    private List<int> tempValueList;

    private void Start()
    {
        Observer.ResetTaskTempValue += ResetTaskTempValue;
        Observer.UpdateTaskValue += UpdateTaskValue;
        Observer.UpdateTempValue += UpdateTempValue;
    }

    public MainTaskData GetCurrentMainTask()
    {
        foreach (var mainTaskData in taskDataResources.mainTaskDataList)
        {
            if (!mainTaskData.IsClaimed)
            {
                return mainTaskData;
            }
        }

        return null;
    }

    public void CompleteCurrentMainTask()
    {
        GetCurrentMainTask().CurrentTask++;
    }

    public Vector2 GetTaskProgress()
    {
        MainTaskData mainTaskData = GetCurrentMainTask();
        if (mainTaskData == null)
            return new Vector2(0, 0);
        return new Vector2(mainTaskData.CurrentTask, mainTaskData.collectionPage.CollectionItemList.Count);
    }

    private void ResetTaskTempValue()
    {
        for (int i = 0; i < tempValueList.Count; i++)
        {
            tempValueList[i] = 0;
        }
    }

    private void UpdateTaskValue()
    {
        if (Utils.CurrentLevel > 8)
        {
            for (int i = 0; i < normalTaskDataList.Count; i++)
            {
                normalTaskDataList[i].TaskCount += tempValueList[i];
            }

            DataController.instance.CheckWarningForTask();
            ResetTaskTempValue();
        }
    }
    private void UpdateTempValue(ETaskType taskType)
    {
        if (Utils.CurrentLevel > 8)
        {
            for (int i = 0; i < normalTaskDataList.Count; i++)
            {
                if (normalTaskDataList[i].taskType == taskType)
                {
                    tempValueList[i]++;
                }
            }
        }

    }

    public void InitData()
    {
        normalTaskDataList = new List<TaskData>();
        tempValueList = new List<int>();
        for (int i = 0; i < normalTaskNumber; i++)
        {
            normalTaskDataList.Add(null);
            tempValueList.Add(0);
        }

        foreach (TaskData taskData in taskDataResources.normalTaskDataList)
        {
            if (taskData.TaskCount != -1) // currentlyUse
            {
                int id = GetNullTask();
                if (id != -1)
                {
                    normalTaskDataList[id] = taskData;
                }
            }
        }

        while (GetNullTask() != -1)
        {
            AddNewTask();
        }
    }

    public void AddNewTask(ETaskType taskType = ETaskType.None)
    {
        int lowestTaskNumber = -1;
        foreach (var taskData in taskDataResources.normalTaskDataList)
        {
            if (taskData.TaskCount == -1 && taskData.taskType != taskType)
            {
                if (lowestTaskNumber == -1)
                    lowestTaskNumber = taskData.CurrentTask;
                else
                    lowestTaskNumber = Math.Min(lowestTaskNumber, taskData.CurrentTask);
            }
        }

        List<TaskData> newTaskDataList = new List<TaskData>();

        foreach (var taskData in taskDataResources.normalTaskDataList)
        {
            if (taskData.TaskCount == -1 && taskData.taskType != taskType && taskData.CurrentTask == lowestTaskNumber)
            {
                newTaskDataList.Add(taskData);
            }
        }

        int id = Random.Range(0, newTaskDataList.Count);
        Debug.Log(id);
        var newTaskData = newTaskDataList[id];
        if (newTaskData.CurrentTask >= newTaskData.taskDataList.Count)
        {
            newTaskData.CurrentTask = 0;
        }

        newTaskData.TaskCount = 0;
        int curId = GetNullTask();
        normalTaskDataList[curId] = newTaskData;
    }

    private int GetNullTask()
    {
        for (int i = 0; i < normalTaskDataList.Count; i++)
        {
            if (normalTaskDataList[i] == null)
            {
                return i;
            }
        }

        return -1;
    }
}
