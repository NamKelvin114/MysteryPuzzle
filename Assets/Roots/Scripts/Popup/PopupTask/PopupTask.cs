using System;
using System.Collections.Generic;
using DG.Tweening;
using GoogleMobileAds.Api;
using Pancake;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupTask : UniPopupBase
{
    [SerializeField] private TaskProcess taskProcess;
    [SerializeField] private MainTask mainTask;
    [SerializeField] private List<SubTask> normalTask;
    [SerializeField] private float timeWaitToReloadTask = 1f;
    [SerializeField] private VerticalLayoutGroup taskContainer;
    [SerializeField] private RectTransform leftBound;
    [SerializeField] private RectTransform rightBound;
    [SerializeField] private RewardCollection rewardBoard;
    [SerializeField] private GameObject maskForTutorial;
    [SerializeField] private GameObject btnBack;

    private bool _isCallFromTutorial;
    private Action _actionBack;
    private TaskDataController _taskDataController;
    private bool _isFirstOpen = false;
    private bool _canClickBtn = true;

    public MainTask GetMainTask => mainTask;
    public List<SubTask> GetNormalsTask => normalTask;

    public void Initialized(Action actionBack, bool isCallFromTutorial)
    {
        _isCallFromTutorial = isCallFromTutorial;
        if (_isCallFromTutorial)
        {
            maskForTutorial.SetActive(true);
            btnBack.SetActive(false);
        }
        else
        {
            maskForTutorial.SetActive(false);
            btnBack.SetActive(true);
        }
        _actionBack = actionBack;

        if (_isFirstOpen == false)
        {
            _taskDataController = DataController.instance.taskDataController;
            InitData();
        }

        Refresh();
        if (_taskDataController.GetCurrentMainTask() == null)
            taskProcess.Setup(_taskDataController.GetTaskProgress());
        else
            taskProcess.Setup(_taskDataController.GetTaskProgress(),
                _taskDataController.GetCurrentMainTask().collectionPage.Content);
    }

    private void InitData()
    {
        if (_taskDataController.GetCurrentMainTask() != null)
        {
            mainTask.Init(_taskDataController.GetCurrentMainTask(), OnClickClaimReward, OnClickActionDoIt);
        }
        else
        {
            mainTask.gameObject.SetActive(false);
        }

        for (int i = 0; i < normalTask.Count; i++)
        {
            normalTask[i].Init(i, _taskDataController.normalTaskDataList[i], OnClickClaimReward, OnClickActionDoIt);
        }
    }

    private void Refresh()
    {
        mainTask.Refresh();
        foreach (var task in normalTask)
        {
            task.Refresh();
        }
    }

    bool _isfrstclaimed
    {
        get => PlayerPrefs.GetInt("IsFirstClaim", 0) == 1;
        set => PlayerPrefs.SetInt("IsFirstClaim", value ? 1 : 0);
    }

    public void OnClickClaimReward(SubTask claimTask)
    {
        if (_canClickBtn == true)
        {
            if (_isCallFromTutorial)
            {
                Observer.ShowHideButtonTutorial?.Invoke(false);
            }
            taskContainer.enabled = false;
            _canClickBtn = false;
            Observer.AddFromPosiGenerationCoin(claimTask.CoinSpawnPos);
            Utils.UpdateCoin(claimTask.RewardCoin);
            float defaultPos = claimTask.GetComponent<RectTransform>().anchoredPosition.x;
            DOTween.Sequence().AppendInterval(timeWaitToReloadTask).AppendCallback(() =>
            {
                MoveTaskOut(claimTask, () =>
                {
                    int id = claimTask.TaskID;
                    ETaskType preTaskType = _taskDataController.normalTaskDataList[id].taskType;
                    _taskDataController.normalTaskDataList[id].OnTaskCompleted();
                    DataController.instance.CheckWarningForTask();
                    _taskDataController.normalTaskDataList[id] = null;
                    _taskDataController.AddNewTask(preTaskType);
                    claimTask.Init(id, _taskDataController.normalTaskDataList[id], OnClickClaimReward,
                        OnClickActionDoIt);
                    MoveTaskIn(claimTask, defaultPos, () =>
                    {
                        taskContainer.enabled = true;
                        _canClickBtn = true;
                        Refresh();
                    });
                });
            });
        }
    }

    public void OnClickClaimReward(MainTask claimTask)
    {
        if (_canClickBtn == true)
        {
            if (_isCallFromTutorial)
            {
                Observer.ShowHideButtonTutorial?.Invoke(false);
                Observer.BackToMenuInTutorual?.Invoke();
                maskForTutorial.SetActive(false);
            }
            taskContainer.enabled = false;
            _canClickBtn = false;
            float defaultPos = claimTask.GetComponent<RectTransform>().anchoredPosition.x;
            if (_isfrstclaimed == false)
            {
                Observer.ContinueTutorial?.Invoke();
                _isfrstclaimed = true;
            }
            RotateObject star = claimTask.StarSpawnPos;
            Vector3 defaultPos2 = star.transform.localPosition;
            Quaternion defaultRotate = star.transform.rotation;
            Vector3 targetPos = taskProcess.CalculateFillPosition();
            star.isRotate = true;
            star.transform.DOMove(targetPos, timeWaitToReloadTask).OnComplete(() =>
            {
                star.gameObject.SetActive(false);
                star.isRotate = false;
                _taskDataController.CompleteCurrentMainTask();
                DataController.instance.CheckWarningForTask();
                taskProcess.Setup(_taskDataController.GetTaskProgress(),
                    _taskDataController.GetCurrentMainTask().collectionPage.Content);
                var taskProgress = _taskDataController.GetTaskProgress();
                if (taskProgress.x == taskProgress.y)
                {
                    MainTaskData currentMainTask = _taskDataController.GetCurrentMainTask();
                    rewardBoard.SetupCoinReward(() =>
                    {
                        currentMainTask.IsClaimed = true;
                        MoveTaskOut(claimTask, () =>
                        {
                            star.gameObject.SetActive(true);
                            star.transform.localPosition = defaultPos2;
                            star.transform.rotation = defaultRotate;
                            claimTask.Init(_taskDataController.GetCurrentMainTask(), OnClickClaimReward,
                                OnClickActionDoIt);
                            MoveTaskIn(claimTask, defaultPos, () =>
                            {
                                taskContainer.enabled = true;
                                _canClickBtn = true;
                                if (_taskDataController.GetCurrentMainTask() == null)
                                    taskProcess.Setup(_taskDataController.GetTaskProgress());
                                else
                                    taskProcess.Setup(_taskDataController.GetTaskProgress(),
                                        _taskDataController.GetCurrentMainTask().collectionPage.Content);
                                Refresh();
                            });
                        });
                    }, currentMainTask.reward);
                    rewardBoard.gameObject.SetActive(true);
                    if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.giftOpen);
                }
                else
                {
                    DOTween.Sequence().AppendInterval(timeWaitToReloadTask).AppendCallback(() =>
                    {
                        MoveTaskOut(claimTask, () =>
                        {
                            star.gameObject.SetActive(true);
                            star.transform.localPosition = defaultPos2;
                            star.transform.rotation = defaultRotate;
                            claimTask.Init(_taskDataController.GetCurrentMainTask(), OnClickClaimReward,
                                OnClickActionDoIt);
                            MoveTaskIn(claimTask, defaultPos, () =>
                            {
                                taskContainer.enabled = true;
                                _canClickBtn = true;
                                Refresh();
                            });
                        });
                    });
                }
            });
        }
    }


    private void MoveTaskOut(SubTask subTask, Action actionDone)
    {
        RectTransform taskTransform = subTask.GetComponent<RectTransform>();
        float targetPosX = rightBound.localPosition.x + taskTransform.rect.width + 100f; // 100 is offset
        taskTransform.DOAnchorPosX(targetPosX, 0.5f).OnComplete(() => actionDone?.Invoke());
    }

    private void MoveTaskIn(SubTask subTask, float defaultPos, Action actionDone)
    {
        RectTransform taskTransform = subTask.GetComponent<RectTransform>();
        float startPosX = leftBound.localPosition.x - taskTransform.rect.width - 100f; // 100 is offset
        // subTask.gameObject.SetActive(false);
        // taskTransform.DOAnchorPosX(startPosX, 0).OnComplete(() => subTask.gameObject.SetActive(true));
        taskTransform.anchoredPosition = new Vector2(startPosX, taskTransform.anchoredPosition.y);
        taskTransform.DOAnchorPosX(defaultPos, 0.5f).OnComplete(() => actionDone?.Invoke());
    }

    public void OnClickActionDoIt()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.btnStart);
        if (_canClickBtn == true)
        {
            if(GameManager.instance != null) GameManager.instance.isShowEvent = false;
            GamePopup.Instance.HidePopupTutorial();
            gameObject.SetActive(false);
            
            var menuController = MenuController.instance;
            if (menuController != null)
            {
                menuController.InternalNextLevel();
            }

            if (GameManager.instance != null && GameManager.instance.gPanelWin.activeSelf) return;
            GamePopup.Instance.HidePopupMoney();
        }
    }

    public void OnClickBack()
    {
        var game = GameManager.instance;
        if (game != null)
        {
            game.isShowEvent = false;
        }

        if (_canClickBtn == true)
        {
            GamePopup.Instance.HidePopupTutorial();
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
            gameObject.SetActive(false);
            if (game == null || !game.gPanelWin.activeSelf)
            {
                GamePopup.Instance.HidePopupMoney();
            }
        }

        _actionBack?.Invoke();
    }
}
