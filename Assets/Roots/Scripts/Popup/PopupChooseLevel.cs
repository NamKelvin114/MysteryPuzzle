using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTimer;
using Worldreaver.UniUI;

public class PopupChooseLevel : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private GameObject[] lockeds;
    [SerializeField] private UniButton[] btns;

    private Action _actionBack;

    public void Initialized(Action actionBack)
    {
        _actionBack = actionBack;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnbBackButtonPressed);

        foreach (var uniButton in btns)
        {
            uniButton.interactable = false;
        }

        btns[0].interactable = true;
        var countChapter = Utils.CurrentLevel / 40;

        if (countChapter > 11) countChapter = 11;

        for (int i = 1; i <= countChapter; i++)
        {
            lockeds[i]?.SetActive(false);
            btns[i].interactable = true;
        }
    }

    public async void UseRoom(int index)
    {
        // todo
        Data.CurrentMenuWorld = index;
        if (GamePopup.Instance.menuRoom != null) Destroy(GamePopup.Instance.menuRoom.gameObject);
        //MenuController.instance.Block.SetActive(true);
        GamePopup.Instance.menuRoom = null;
        var room = await BridgeData.Instance.GetRoom(index);
        if (room != null) BridgeData.Instance.menuRoomPrefab = room.GetComponent<BaseRoom>();

        GamePopup.Instance.ShowRoom(isDance: true);
        _actionBack?.Invoke();
    }

    private void OnbBackButtonPressed()
    {
        _actionBack?.Invoke();
    }
}