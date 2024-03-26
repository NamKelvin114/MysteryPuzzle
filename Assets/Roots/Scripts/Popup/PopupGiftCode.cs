using System;
using TMPro;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupGiftCode : UniPopupBase
{
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private TMP_InputField txtGiftCode;
    [SerializeField] private TextMeshProUGUI txtWarning;

    [SerializeField] private UniButton btnFacebook;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite avaiableSprite;

    private Action _actionBack;
    private Action _actionOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionBack"></param>
    /// <param name="actionOk"></param>
    public void Initialized(Action actionBack, Action actionOk)
    {
        //block.gameObject.SetActive(false);
        txtGiftCode.text = "";
        _actionBack = actionBack;
        _actionOk = actionOk;

        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnOkButtonPressed);

        btnFacebook.onClick.RemoveAllListeners();
        btnFacebook.onClick.AddListener(OnFacebookButtonPressed);

        txtGiftCode.characterLimit = 12;
        txtGiftCode.onValidateInput -= OnValidateInput;
        txtGiftCode.onValidateInput += OnValidateInput;
        txtGiftCode.onValueChanged.AddListener(OnValueGiftCodeChange);

        btnOk.interactable = false;
        btnOk.image.sprite = lockedSprite;

        txtGiftCode.ActivateInputField();
        txtGiftCode.Select();
    }

    private void OnValueGiftCodeChange(string value)
    {
        if (value.Length >= 12)
        {
            btnOk.interactable = true;
            btnOk.image.sprite = avaiableSprite;
            txtWarning.text = "";
            txtWarning.gameObject.SetActive(false);
            return;
        }
        
        btnOk.interactable = false;
        btnOk.image.sprite = lockedSprite;
    }

    private char OnValidateInput(string text, int charindex, char addedchar) { return Validation(addedchar); }

    private char Validation(char c)
    {
        if (c >= 'a' && c <= 'z')
        {
            return (char) ((int) c - 'a' + 'A');
        }

        if (c >= 'A' && c <= 'Z' || c == '-')
        {
            return c;
        }

        return '\0';
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed() { _actionBack?.Invoke(); }

    /// <summary>
    /// ok button pressed
    /// </summary>
    private void OnOkButtonPressed()
    {
        // if (txtGiftCode.text.Equals("LUNARNEWYEAR"))
        // {
        //     if (!Data.GetStatusNewYearCode())
        //     {
        //         _actionBack?.Invoke();
        //         GamePopup.Instance.ShowNotificationPopup(null, "You have received\n+50000 coins!");
        //         Utils.currentCoin += 50000;
        //         if (GameManager.instance != null) GameManager.instance.txtCoin.text = Utils.currentCoin.ToString();
        //         if (MenuController.instance != null) MenuController.instance.TxtCoin.text = Utils.currentCoin.ToString();
        //         Data.SetStatusNewYearCode(true);
        //     }
        //     else
        //     {
        //         GamePopup.Instance.ShowNotificationPopup(null, "You can only use this code once!");
        //         txtGiftCode.text = "";
        //     }
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("HAPPYNEWYEAR"))
        // {
        //     if (!Data.GetStatusNewYear2Code())
        //     {
        //         _actionBack?.Invoke();
        //         Data.SetStatusNewYear2Code(true);
        //
        //         GamePopup.Instance.ShowNotificationPopup(null, "You have received\n+20000 coins!");
        //         Utils.currentCoin += 20000;
        //         if (GameManager.instance != null) GameManager.instance.txtCoin.text = Utils.currentCoin.ToString();
        //         if (MenuController.instance != null) MenuController.instance.TxtCoin.text = Utils.currentCoin.ToString();
        //     }
        //     else
        //     {
        //         GamePopup.Instance.ShowNotificationPopup(null, "You can only use this code once!");
        //         txtGiftCode.text = "";
        //     }
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("BESTWISHFORU"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idBoss = 35;
        //     if (idBoss < DataController.instance.SaveHero.Count)
        //     {
        //         DataController.instance.SaveHero[idBoss].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, idBoss, -1);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("GOODLUCKUALL"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 10;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("REDVALENTINE"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idBoss = 37;
        //     if (idBoss < DataController.instance.SaveHero.Count)
        //     {
        //         DataController.instance.SaveHero[idBoss].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, idBoss, -1);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("CHOCOLATEFOU"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 14;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("GREENBUGGIFT"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idBoss = 33;
        //     if (idBoss < DataController.instance.SaveHero.Count)
        //     {
        //         DataController.instance.SaveHero[idBoss].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, idBoss, -1);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("BERRYBUGGIFT"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 7;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("BROWNBUGGIFT"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 8;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("PUMKINCANDYY"))
        // {
        //     if (!Data.GetStatusCodeHalloween())
        //     {
        //         _actionBack?.Invoke();
        //         Data.SetStatusCodeHalloween(true);
        //
        //         DataController.instance.SavePrincess[1].unlock = true;
        //         DataController.instance.SavePrincess[2].unlock = true;
        //         DataController.instance.SavePrincess[3].unlock = true;
        //
        //         DataController.instance.SaveData();
        //         GamePopup.Instance.ShowNotificationPopup(CheckRefreshShop, "Every wife skin in the HALLOWEEN\nhas been unlocked");
        //     }
        //     else
        //     {
        //         GamePopup.Instance.ShowNotificationPopup(null, "You can only use this code once!");
        //         txtGiftCode.text = "";
        //     }
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("XMASSNOWBALL"))
        // {
        //     if (!Data.GetStatusCodeXmas())
        //     {
        //         _actionBack?.Invoke();
        //         Data.SetStatusCodeXmas(true);
        //
        //         DataController.instance.SavePrincess[4].unlock = true;
        //         DataController.instance.SavePrincess[5].unlock = true;
        //         DataController.instance.SavePrincess[6].unlock = true;
        //
        //         DataController.instance.SaveData();
        //         GamePopup.Instance.ShowNotificationPopup(CheckRefreshShop, "Every wife skin in the XMAS\nhas been unlocked");
        //     }
        //     else
        //     {
        //         GamePopup.Instance.ShowNotificationPopup(null, "You can only use this code once!");
        //         txtGiftCode.text = "";
        //     }
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("SORYABOUTBUG"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 9;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        //
        //
        // if (txtGiftCode.text.Equals("LUCKYCLOVERR111"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 18;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        //
        // if (txtGiftCode.text.Equals("FOURSHAMROCK111"))
        // {
        //     _actionBack?.Invoke();
        //
        //     const int idPrincess = 19;
        //     if (idPrincess < DataController.instance.SavePrincess.Count)
        //     {
        //         DataController.instance.SavePrincess[idPrincess].unlock = true;
        //     }
        //
        //     DataController.instance.SaveData();
        //     GamePopup.Instance.ShowGiftCodeCompletesPopup(CheckRefreshShop, CheckRefreshShop, -1, idPrincess);
        //
        //     return;
        // }
        
        _actionOk?.Invoke();
        GamePopup.Instance.ShowNotificationPopup(null, "The code is not correct\nPlease try another one!");
    }


    public void CheckRefreshShop()
    {
        if (GamePopup.Instance.popupSkinHandler != null)
        {
            if (GamePopup.Instance.popupSkinHandler.ThisGameObject.activeSelf)
            {
                var popupSkin = (PopupSkin) GamePopup.Instance.popupSkinHandler;
                if (popupSkin != null)
                {
                    // popupSkin.UpdateCurrencyDisplay();
                    // popupSkin.RefreshAllItem();
                    // popupSkin.RefreshAllWife();
                }
            }
        }

        if (GameManager.instance != null) GameManager.instance.txtCoin.text = Utils.currentCoin.ToString();

        if (MenuController.instance != null) MenuController.instance.TxtCoin.text = Utils.currentCoin.ToString();
    }

    private void OnFacebookButtonPressed() { GamePopup.Instance.ShowPopupGotoFacebook(); }
}