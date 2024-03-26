using System;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupGiftCodeComplete : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private SkeletonGraphic bossSkeleton;
    [SerializeField] private SkeletonGraphic wifeSkeleton;

    private Action _actionBack;
    private Action _actionOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    /// <param name="idBossSkin"></param>
    /// <param name="idWifeSkin"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    public void Initialized(Action actionBack, Action actionOk, int idBossSkin, int idWifeSkin, string title, string message)
    {
        _actionBack = actionBack;
        _actionOk = actionOk;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);

        txtTitle.text = title;
        txtMessage.text = message;

        if (idBossSkin == -1)
        {
            bossSkeleton.gameObject.SetActive(false);
        }
        else
        {
            bossSkeleton.gameObject.SetActive(true);
            bossSkeleton.ChangeSkin(HeroData.SkinHeroByIndex(idBossSkin).Item1);
        }

        if (idWifeSkin == -1)
        {
            wifeSkeleton.gameObject.SetActive(false);
        }
        else
        {
            wifeSkeleton.gameObject.SetActive(true);
            wifeSkeleton.ChangeSkin(HeroData.SkinPrincessByIndex(idWifeSkin));
        }
    }


    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed() { _actionBack?.Invoke(); }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed() { _actionOk?.Invoke(); }
}