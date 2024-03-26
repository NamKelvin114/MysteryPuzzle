using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainTask : SubTask
{
    [SerializeField] private RotateObject starSpawnPos;
    [SerializeField] private ParticleSystem starTrail;
    public RotateObject StarSpawnPos => starSpawnPos;
    private MainTaskData _mainTaskData;
    private Action<MainTask> _actionClaim;

    public void Init(MainTaskData mainTaskData, Action<MainTask> actionClaim = null, Action actionDoit = null)
    {
        starTrail.Stop();
        
        _mainTaskData = mainTaskData;
        if (mainTaskData == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _actionClaim = actionClaim;
        _actionDoit = actionDoit;
        _mainTaskData = mainTaskData;
        int id = mainTaskData.CurrentTask;
        taskIcon.sprite = mainTaskData.collectionPage.CollectionItemList[id].ItemIcon;
        description.text = String.Format(_mainTaskData.description,
            mainTaskData.collectionPage.CollectionItemList[id].ItemName);
        Refresh();
    }

    public override void Refresh()
    {
        if (_mainTaskData == null)
            return;
        int id = _mainTaskData.CurrentTask;
        int curCount = _mainTaskData.collectionPage.CollectionItemList[id].IsUnlocked ? 1 : 0;
        processImage.fillAmount = curCount / 1;
        textProcess.text = curCount + "/" + 1;
        
        if(Utils.DoAllTask)
        {
            doneState.gameObject.SetActive(true);
            doingState.gameObject.SetActive(false);
            return;
        }
        
        doneState.gameObject.SetActive(curCount == 1);
        doingState.gameObject.SetActive(curCount != 1);
    }

    public override void OnClickBtnClaim()
    {
        starTrail.Play();
        _actionClaim?.Invoke(this);
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.mainTaskClaim);
    }
}