using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class PanelWinHandle : MonoBehaviour
{
    public RectTransform winBtnAds;
    public RectTransform winBtnContinue;
    public RectTransform winProgress;

    public RectTransform normalLoseBtnSkip;
    public RectTransform normalLoseBtnReset;

    public RectTransform hardLoseBtnSkip;
    public RectTransform hardLoseBtnBackNormal;
    public RectTransform hardLoseBtnReset;
    public RectTransform hardLoseText;

    [SerializeField] private GameObject _gachaObject;
    [SerializeField] private TextMeshProUGUI bonusRewardAds;
    [SerializeField] private ArrowGachha _arrowGacha;
    [SerializeField] private Button btnHome;

    private void Start()
    {
        float aspect = (float)Screen.height / (float)Screen.width; // Portrait
        //aspect = (float)Screen.width / (float)Screen.height; // Landscape
        if (aspect >= 1.87)
        {
            //Debug.Log("19.5:9"); // iPhone X                  
        }
        else if (aspect >= 1.74) // 16:9
        {
            //Debug.Log("16:9");
        }
        else if (aspect > 1.6) // 5:3
        {
            //Debug.Log("5:3");
        }
        else if (Math.Abs(aspect - 1.6) < Mathf.Epsilon) // 16:10
        {
            //Debug.Log("16:10");
        }
        else if (aspect >= 1.5) // 3:2
        {
            //Debug.Log("3:2");
        }
        else
        {
            // 4:3
            //Debug.Log("4:3 or other");

            // winBtnContinue.localPosition = new Vector3(0, -104, 0);
            // winBtnAds.anchoredPosition3D = new Vector3(0, 72, 0);
            // winProgress.localPosition = new Vector3(0, 232);
            //
            // //normalLoseBtnSkip.anchoredPosition3D = new Vector3();
            // normalLoseBtnReset.anchoredPosition3D = new Vector3(0, -360);
            //
            // hardLoseBtnSkip.anchoredPosition3D = new Vector3(198, -192);
            // hardLoseBtnBackNormal.anchoredPosition3D = new Vector3(-184, -192);
            // hardLoseText.localPosition = new Vector3(0, -20);
        }

        Observer.UpdateBonusAdsButton += UpdateTextBonusAdsButon;
        btnHome.onClick.RemoveAllListeners();
        btnHome.onClick.AddListener(OnButtonHomeClick);
        if (Utils.CurrentLevel <= 2) btnHome.gameObject.SetActive(false);
    }

    void OnButtonHomeClick()
    {
        GameManager.instance.SoundClickButton();
        GameManager.instance.GoToMenu();
    }

    private void UpdateTextBonusAdsButon(int bonus)
    {
        bonusRewardAds.text = "+" + bonus * Utils.CoinReward;
    }

    private void OnDestroy()
    {
        Observer.UpdateBonusAdsButton -= UpdateTextBonusAdsButon;
    }

    public void ClaimByVideo()
    {
        winBtnContinue.gameObject.SetActive(false);
        _arrowGacha.isMoving = false;
    }

    public void OutGameOnClaimVideo()
    {
        winBtnContinue.gameObject.SetActive(true);
        _arrowGacha.isMoving = true;
    }
}