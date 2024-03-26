using System;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "TaskConfig", menuName = "ScriptableObject/TaskConfig")]
public class TaskConfig : ScriptableObject
{
    public List<TaskItem> TaskItems;

    public TaskItem GetRandomTaskItem()
    {
        var TaskItemsCanTake = TaskItems.Where(taskItem => taskItem.isAppeared == false).ToArray();
        if (TaskItemsCanTake.Length == 0) return TaskItems[0];
        int pos = Random.Range(0, TaskItemsCanTake.Length);
        foreach (var taskItem in TaskItems)
        {
            if (taskItem == TaskItemsCanTake[pos])
            {
                taskItem.isAppeared = true;
            }
        }
        return TaskItemsCanTake[pos];
    }
    public TaskItem GetTaskById(int _id)
    {
        foreach (var task in TaskItems)
        {
            if (task.id == _id)
                return task;
        }

        return null;
    }

    public void ResetListTask()
    {
        foreach (var taskItem in TaskItems)
        {
            taskItem.isAppeared = false;
        }
    }

    private void OnValidate()
    {
        TaskItems[TaskItems.Count - 1].id = TaskItems.Count;
    }
}

[Serializable]
public class TaskItem
{
    public TypeTask typeTask;
    [ShowIf(nameof(typeTask), TypeTask.Collective)] public ItemCollective ItemCollective;
    public Sprite icon;
    public int numberToClaim = 0;
    public int coinReward = 0;
    public bool isAppeared = false;
    private static int nextID = 0;
    [ReadOnly] public int id;
    [HideInInspector] public int currentProgressCount = 0;
    public TaskItem()
    {
        id = nextID;
        nextID++;
    }

}
public enum TypeTask
{
    Collective,
    Destroy,
    Rescue,
    Building
}

public enum ItemCollective
{
    Pin,
    Coin,
    Ads
}
