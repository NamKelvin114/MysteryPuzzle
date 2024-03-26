using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pancake.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PopupLogin : UniPopupBase
{
    [SerializeField] private CountryCode countryCode;
    [SerializeField] private TextMeshProUGUI waringName;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TextMeshProUGUI countryName;
    [SerializeField] private Image iconCountry;
    [SerializeField] private UniButton btnBack;
    [SerializeField] private UniButton btnOk;
    [SerializeField] private UniButton btnFlag;
    
    private Action _actionBack; 
    private Action _openPopupAction;

    public Image IconCountry => iconCountry;
    public TextMeshProUGUI CountryName => countryName;

    public void Initialized(Action actionBack, Action actionOpenPopup)
    {
        _actionBack = actionBack;
        _openPopupAction = actionOpenPopup;
        inputName.characterLimit = 16;
        inputName.onValueChanged.AddListener(InputNameCallback);
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(OnBackButtonPressed);

        waringName.gameObject.SetActive(false);

        inputName.text = "";
        inputName.ActivateInputField();
        inputName.Select();

        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(OnNameOkButtonPressed);

        btnFlag.onClick.RemoveAllListeners();
        btnFlag.onClick.AddListener(ShowFlagPopup);

        var code = BridgeData.Instance.GetCountryCode();
        countryName.text = BridgeData.Instance.GetCountryName(code);
        iconCountry.sprite = countryCode.GetIcon(code);
        iconCountry.gameObject.SetActive(true);
        btnOk.gameObject.SetActive(true);
    }

    public void ShowFlagPopup() { GamePopup.Instance.ShowFlagPopup(); }

    private void PunchWarringName() { waringName.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 0.5f, 5, 0.5f).Play(); }

    private void InputNameCallback(string value)
    {
        if (value.Length >= 16)
        {
            if (!waringName.gameObject.activeSelf)
            {
                waringName.gameObject.SetActive(true);
                waringName.text = "Name length cannot be longer than 16 characters";
                // punch
                PunchWarringName();
            }
        }
        else
        {
            waringName.gameObject.SetActive(false);
        }
    }

    private async void OnNameOkButtonPressed()
    {
        void Failure()
        {
            btnOk.interactable = true;
            waringName.text = "Check your network connection again!";
            Data.UserName = "";
            waringName.gameObject.SetActive(true);
            PunchWarringName();
        }

        btnOk.interactable = false;
        waringName.gameObject.SetActive(false);
        var str = inputName.text;

        if (string.IsNullOrEmpty(str))
        {
            btnOk.interactable = true;
            waringName.text = "Name can be empty!";
            inputName.Select();
            waringName.gameObject.SetActive(true);
            PunchWarringName();
            return;
        }

        if (!PlayfabHelper.Instance.CompletedRun && Utils.CheckInternetConnection())
        {
            if (!PlayfabHelper.Instance.StatusLogin)
            {
                PlayfabHelper.Instance.RequestLogin(); // was reset state completeRun
                await UniTask.WaitUntil(() => PlayfabHelper.Instance.CompletedRun);
                if (PlayfabHelper.Instance.StatusLogin)
                {
                    // todo login success
                    PlayfabHelper.Instance.UpdateUserDisplayName(str,
                        BridgeData.Instance.GetCountryCode(),
                        _ =>
                        {
                            Data.UserName = str;
                            PlayfabHelper.Instance.SubmitScore(Utils.CurrentLevel + 1, PlayfabHelper.Instance.NameTableLeaderboard, result => { }, error => { });
                            PlayfabHelper.Instance.SubmitScore(Data.CurrentEgg, PlayfabHelper.Instance.NameTableEvent1, result => { }, error => { });
                        },
                        error => { }); // update user display name

                    OnBackButtonPressed();
                    _openPopupAction?.Invoke();
                }
                else
                {
                    // todo login failure
                    PlayfabHelper.Instance.ResetRun();
                    Failure();
                }
            }
            else
            {
                // todo realy login success

                PlayfabHelper.Instance.UpdateUserDisplayName(str,
                    BridgeData.Instance.GetCountryCode(),
                    _ =>
                    {
                        Data.UserName = str;
                        PlayfabHelper.Instance.SubmitScore(Utils.CurrentLevel + 1, PlayfabHelper.Instance.NameTableLeaderboard, result => { }, error => { });
                        PlayfabHelper.Instance.SubmitScore(Data.CurrentEgg, PlayfabHelper.Instance.NameTableEvent1, result => { }, error => { });
                    },
                    error => { }); // update user display name

                OnBackButtonPressed();
                _openPopupAction?.Invoke();
            }
        }
        else
        {
            PlayfabHelper.Instance.ResetRun();
            Failure();
        }
    }

    /// <summary>
    /// back button pressed
    /// </summary>
    private void OnBackButtonPressed() { _actionBack?.Invoke(); }

    private void OnDisable() { inputName.onValueChanged.RemoveListener(InputNameCallback); }
}