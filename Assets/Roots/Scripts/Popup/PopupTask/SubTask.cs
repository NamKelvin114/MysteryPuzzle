using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubTask : MonoBehaviour
{
    [SerializeField] protected GameObject doneState;
    [SerializeField] protected GameObject doingState;
    [SerializeField] private TextMeshProUGUI textCoin;
    [SerializeField] protected TextMeshProUGUI textProcess;
    [SerializeField] protected TextMeshProUGUI description;
    [SerializeField] protected Image processImage;
    [SerializeField] protected Image taskIcon;

    private TaskData _taskData;
    private int _taskID;
    private Action<SubTask> _actionClaim;
    protected Action _actionDoit;
    protected int rewardCoin = 0;

    public GameObject CoinSpawnPos => textCoin.gameObject;
    public int RewardCoin => rewardCoin;
    public int TaskID => _taskID;
    
    public void Init(int taskID, TaskData taskData, Action<SubTask> actionClaim = null, Action actionDoit = null)
    {
        _taskID = taskID;
        _taskData = taskData;
        _actionDoit = actionDoit;
        _actionClaim = actionClaim;
        taskIcon.sprite = taskData.taskIcon;
        taskIcon.SetNativeSize();
        int id = taskData.CurrentTask;
        textCoin.text = taskData.taskDataList[id].reward.ToString();
        rewardCoin = taskData.taskDataList[id].reward;
        description.text = String.Format(taskData.description, taskData.taskDataList[id].number);
        Refresh();
    }

    public virtual void Refresh()
    {
        int id = _taskData.CurrentTask;
        int maxNumber = _taskData.taskDataList[id].number;
        int curCount = Math.Min(_taskData.TaskCount, maxNumber);
        processImage.fillAmount = (float)curCount / maxNumber;
        textProcess.text = curCount + "/" + maxNumber;
        
        if(Utils.DoAllTask)
        {
            doneState.gameObject.SetActive(true);
            doingState.gameObject.SetActive(false);
            return;
        }
        
        doneState.gameObject.SetActive(curCount == maxNumber);
        doingState.gameObject.SetActive(curCount != maxNumber);
    }

    public virtual void OnClickBtnClaim()
    {
        _actionClaim?.Invoke(this);
    }

    public void OnClickBtnDoIt()
    {
        _actionDoit?.Invoke();
    }
    
}
