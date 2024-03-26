using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using PlayFab.ClientModels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using DG.Tweening;

public class PopupTutotial : UniPopupBase
{
    private Action _actionBack;
    [SerializeField] private Transform handTur;
    [SerializeField] private Transform btnTask;
    [SerializeField] private Transform destinationTask;
    [SerializeField] private Transform btnBackTask;
    [SerializeField] Transform destinationStart;
    [SerializeField] private GameObject deadLockStart;
    [SerializeField] private GameObject bg;
    [SerializeField] private GameObject deadLock;
    [SerializeField] private GameObject btnCollection;
    [SerializeField] private GameObject btnShowTut;
    private bool _isInMenu;
    [SerializeField] Transform btnHome;

    public void Initialized(Action getActionBack)
    {
        Observer.ShowHideButtonTutorial += OnShowHideButtonTutorial;

        _actionBack = getActionBack;
        if (_isInMenu == false)
        {
            btnBackTask.gameObject.SetActive(false);
            deadLockStart.gameObject.SetActive(false);
            deadLock.gameObject.SetActive(false);
            btnTask.gameObject.SetActive(false);
            btnHome.gameObject.SetActive(false);
            btnShowTut.SetActive(false);
            handTur.gameObject.SetActive(false);
            ChangeStateBtn(btnTask.gameObject, false);

            ChangeStateBtn(btnCollection, false);
            DoHandTurPosition(btnTask);

            //DoHandTurPosition(btnHome);
            OnClickHome();
            Observer.ContinueTutorial += ContinueTutorial;
            Observer.ShowTaskToturial += ShowTaskTutorial;
            Observer.BackToMenuInTutorual += OnBackToMenu;

            Sequence sq = DOTween.Sequence();
            sq.AppendInterval(0.5f).AppendCallback(() =>
            {
                ChangeStateBtn(btnTask.gameObject, true);
            });
        }
    }
    

    private void ChangeStateBtn(GameObject button, bool isActive)
    {
        button.GetComponent<Image>().enabled = isActive;
        foreach (Transform child in button.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    private void OnDisable()
    {
        Observer.ContinueTutorial -= ContinueTutorial;
        Observer.ShowTaskToturial -= ShowTaskTutorial;
        Observer.ShowHideButtonTutorial -= OnShowHideButtonTutorial;
    }

    private void OnBackToMenu()
    {
        //PlaySoundButtonClicked();
        DoHandTurPosition(btnBackTask.transform);
        handTur.gameObject.SetActive(true);
        bg.SetActive(true);
        btnBackTask.gameObject.SetActive(true);
    }

    private void OnShowHideButtonTutorial(bool isEnable)
    {
        handTur.gameObject.SetActive(isEnable);
    }

    void DoHandTurPosition(Transform destination)
    {
        handTur.gameObject.SetActive(true);
        if (!destination.gameObject.activeSelf)
        {
            destination.gameObject.SetActive(true);
        }

        handTur.position = destination.position;
    }

    public void OnClickHome()
    {
        // GameManager.instance.SoundClickButton();
        GameManager.instance.GoToMenu();
        _isInMenu = true;
    }

    void ShowTaskTutorial()
    {
        if (gameObject.activeSelf)
        {
            DoHandTurPosition(btnTask);
        }
    }

    public void OnClickTask()
    {
        PlaySoundButtonClicked();
        GameManager.instance.SoundClickButton();
        ChangeStateBtn(btnTask.gameObject, false);
        MenuController.instance.ShowTask(true);
        gameObject.SetActive(false);
        GamePopup.Instance.ShowPopupTutorial();
        handTur.gameObject.SetActive(false);
        deadLock.gameObject.SetActive(true);
        bg.gameObject.SetActive(false);
        DoHandTurPosition(destinationTask);
    }

    public void OnClickBackTask()
    {
        PlaySoundButtonClicked();
        GamePopup.Instance.HidePopupMoney();
        ChangeStateBtn(btnTask.gameObject, false);
        ChangeStateBtn(btnCollection, true);

        GameManager.instance.SoundClickButton();
        GamePopup.Instance.HidePopupTask();
        btnBackTask.gameObject.SetActive(false);
        //deadLockStart.gameObject.SetActive(true);
        DoHandTurPosition(btnCollection.transform);
    }

    public void OnClickBtnCollection()
    {
        PlaySoundButtonClicked();
        ChangeStateBtn(btnCollection, false);
        MenuController.instance.ShowCollection();
        gameObject.SetActive(false);
        GamePopup.Instance.ShowPopupTutorial();
        btnShowTut.SetActive(true);
        DoHandTurPosition(btnShowTut.transform);
    }

    void ContinueTutorial()
    {
        bg.gameObject.SetActive(false);
        handTur.gameObject.SetActive(false);
        StartCoroutine(WaitToHand());
        deadLock.gameObject.SetActive(false);
    }

    public void OnClickShowTutorialCollection()
    {
        PlaySoundButtonClicked();
        handTur.gameObject.SetActive(false);
        GamePopup.Instance.ShowPopupCollectionTutorial();
    }

    IEnumerator WaitToHand()
    {
        yield return new WaitForSeconds(1.8f);
        DoHandTurPosition(destinationTask);
    }

    private void PlaySoundButtonClicked()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
    }
}