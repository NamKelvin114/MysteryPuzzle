using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupTutorialCollection : UniPopupBase
{
    private Action _actionBack;
    [SerializeField] private Transform handTur;
    [SerializeField] private Transform btnCollection;
    [SerializeField] Transform destinationGift;
    [SerializeField] private GameObject bg;
    private bool _isInMenu;
    [SerializeField] Transform btnHome;
    public void Initialized(Action getActionBack)
    {
        _actionBack = getActionBack;
        Observer.ShowCollectionTutorial += ShowCollectionBtn;
        if (_isInMenu == false)
        {
            btnCollection.gameObject.SetActive(false);
            OnClickHome();
        }
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
        GameManager.instance.SoundClickButton();
        btnHome.gameObject.SetActive(false);
        btnCollection.gameObject.SetActive(false);
        handTur.gameObject.SetActive(false);
        GameManager.instance.GoToMenu();
        //DoHandTurPosition(btnCollection);
        _isInMenu = true;
    }
    void ShowCollectionBtn()
    {
        if (gameObject.activeSelf)
        {
            DoHandTurPosition(btnCollection);
        }
    }
    public void OnClickCollection()
    {
        GameManager.instance.SoundClickButton();
        btnCollection.gameObject.SetActive(false);
        MenuController.instance.ShowCollection();
        gameObject.SetActive(false);
        GamePopup.Instance.ShowPopupTutorialCollection();
        Observer.HightLightGiftBoxCollection?.Invoke();
        handTur.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
        DoHandTurPosition(destinationGift);
    }
    public void OnClickBackTask()
    {
        GamePopup.Instance.HidePopupTask();

        bg.gameObject.SetActive(false);

    }
    void ContinueTutorial()
    {
        bg.gameObject.SetActive(false);
        handTur.gameObject.SetActive(false);
        StartCoroutine(WaitToHand());
    }
    IEnumerator WaitToHand()
    {
        yield return new WaitForSeconds(1.5f);
    }
}
