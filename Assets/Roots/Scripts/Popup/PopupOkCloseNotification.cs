using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupOkCloseNotification : UniPopupBase
{
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private UniButton btnNo;
    [SerializeField] private UniButton btnClose;
    [SerializeField] private Sprite greenArrow;
    [SerializeField] private Sprite redArrow;

    [Header("server")] [SerializeField] private TextMeshProUGUI txtLevelLeft;
    [SerializeField] private TextMeshProUGUI txtCoinLeft;
    [SerializeField] private TextMeshProUGUI txtTotalSkinLeft;
    [SerializeField] private TextMeshProUGUI txtTitleLeft;

    [Header("device")] [SerializeField] private TextMeshProUGUI txtLevelRight;
    [SerializeField] private Image arrowLevelRigth;
    [SerializeField] private TextMeshProUGUI txtCoinRight;
    [SerializeField] private Image arrowCoinRigth;
    [SerializeField] private TextMeshProUGUI txtTotalSkinRight;
    [SerializeField] private Image arrowSkinRigth;
    [SerializeField] private TextMeshProUGUI txtTitleRight;


    private Action _actionOk;
    private Action _actionClose;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionOk"></param>
    /// <param name="actionClose"></param>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="serverLevel"></param>
    /// <param name="serverCoin"></param>
    /// <param name="serverTotalSkin"></param>
    /// <param name="titleRight"></param>
    /// <param name="titleLeft"></param>
    /// <param name="isBackup"></param>
    public void Initialized(
        Action actionOk,
        Action actionClose,
        string message,
        string title,
        int serverLevel,
        int serverCoin,
        int serverTotalSkin,
        string titleRight,
        string titleLeft,
        bool isBackup)
    {
        _actionOk = actionOk;
        _actionClose = actionClose;
        txtMessage.text = message;
        txtTitle.text = title;
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);

        if (isBackup)
        {
            txtLevelRight.text = $"Level {serverLevel + 1}";
            txtCoinRight.text = $"{serverCoin}";
            txtTotalSkinRight.text = $"{serverTotalSkin}";

            txtLevelLeft.text = $"Level {Utils.CurrentLevel + 1}";
            txtCoinLeft.text = $"{Utils.currentCoin}";
            txtTotalSkinLeft.text = $"{DataController.instance.TotalSkinUnlocked()}";

            if (Utils.CurrentLevel > serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = greenArrow;
            }
            else if (Utils.CurrentLevel < serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = redArrow;
            }
            else
            {
                arrowLevelRigth.gameObject.SetActive(false);
            }

            if (Utils.currentCoin > serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = greenArrow;
            }
            else if (Utils.currentCoin < serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = redArrow;
            }
            else
            {
                arrowCoinRigth.gameObject.SetActive(false);
            }

            if (DataController.instance.TotalSkinUnlocked() > serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = greenArrow;
            }
            else if (DataController.instance.TotalSkinUnlocked() < serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = redArrow;
            }
            else
            {
                arrowSkinRigth.gameObject.SetActive(false);
            }
        }
        else
        {
            arrowCoinRigth.gameObject.SetActive(false);
            arrowLevelRigth.gameObject.SetActive(false);
            arrowSkinRigth.gameObject.SetActive(false);

            txtLevelLeft.text = $"Level {serverLevel + 1}";
            txtCoinLeft.text = $"{serverCoin}";
            txtTotalSkinLeft.text = $"{serverTotalSkin}";

            txtLevelRight.text = $"Level {Utils.CurrentLevel + 1}";
            txtCoinRight.text = $"{Utils.currentCoin}";
            txtTotalSkinRight.text = $"{DataController.instance.TotalSkinUnlocked()}";

            if (Utils.CurrentLevel > serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = redArrow;
            }
            else if (Utils.CurrentLevel < serverLevel)
            {
                arrowLevelRigth.gameObject.SetActive(true);
                arrowLevelRigth.sprite = greenArrow;
            }
            else
            {
                arrowLevelRigth.gameObject.SetActive(false);
            }

            if (Utils.currentCoin > serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = redArrow;
            }
            else if (Utils.currentCoin < serverCoin)
            {
                arrowCoinRigth.gameObject.SetActive(true);
                arrowCoinRigth.sprite = greenArrow;
            }
            else
            {
                arrowCoinRigth.gameObject.SetActive(false);
            }

            if (DataController.instance.TotalSkinUnlocked() > serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = redArrow;
            }
            else if (DataController.instance.TotalSkinUnlocked() < serverTotalSkin)
            {
                arrowSkinRigth.gameObject.SetActive(true);
                arrowSkinRigth.sprite = greenArrow;
            }
            else
            {
                arrowSkinRigth.gameObject.SetActive(false);
            }
        }

        txtTitleLeft.text = titleLeft;
        txtTitleRight.text = titleRight;

        btnNo.onClick.RemoveAllListeners();
        btnNo.onClick.AddListener(OnCloseButtonPressed);

        btnClose.onClick.RemoveListener(OnCloseButtonPressed);
        btnClose.onClick.AddListener(OnCloseButtonPressed);
    }

    private void OnCloseButtonPressed()
    {
        _actionClose?.Invoke();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed()
    {
        _actionOk?.Invoke();
        gameObject.SetActive(false);
    }
}