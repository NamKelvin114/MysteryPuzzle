using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Spine.Unity;
using UnityTimer;

public class EventItem : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeleton;
    public EventValentine Type;
    [SerializeField] private GameObject imageTextLetter;
    [SerializeField] private GameObject claimActiveButton;
    [SerializeField] private GameObject Lock;
    [SerializeField] private GameObject claimDisableButton;
    // [SerializeField] private Image progress;
    [SerializeField] private TextMeshProUGUI textProgress;

    [SerializeField] private GameObject iconDone;
    [SerializeField] private GameObject iconTop100;
    public int index;
    private Info _cacheDataInfo;
    private Info _PrincessInfo;


    private Info data;
    public PopupEvent popupEvent;

    public void Init(Info data, PopupEvent popupEvent)
    {
        this.data = data;
        this.popupEvent = popupEvent;
        _cacheDataInfo = HeroData.HeroInfoByIndex(index);
        _PrincessInfo = HeroData.PrincessInfoByIndex(index);
    }
    private void Start()
    {
        // _cacheDataInfo = HeroData.HeroInfoByIndex(index);
    }
    public void TryInitChangeSkinPlayer()
    {
        try
        {
            skeleton.ChangeSkin(HeroData.SkinHeroByIndex(index).Item1);
        }
        catch (Exception)
        {
            Timer.Register(0.1f, TryInitChangeSkinPlayer);
        }
    }
    public void TryInitChangeSkinPrincess()
    {
        try
        {
            skeleton.ChangeSkin(HeroData.SkinPrincessByIndex(index));
        }
        catch (Exception)
        {
            Timer.Register(0.1f, TryInitChangeSkinPlayer);
        }
    }

    public void ResetEvent()
    {

        if (Type == EventValentine.Hero || Type == EventValentine.Hero2 || Type == EventValentine.Hero3)
        {
            TryInitChangeSkinPlayer();

            // Debug.Log(Data.TotalGoldMedal+"____"+_cacheDataInfo.NumBerGoldEvent);
            if (Data.TotalGoldMedal >= _cacheDataInfo.NumBerGoldEvent)
            {
                if (DataController.instance.SaveHero[index].unlock)
                {
                    claimDisableButton.SetActive(false);
                    iconDone.SetActive(true);
                    Lock.SetActive(false);
                    claimActiveButton.SetActive(false);
                    textProgress.gameObject.SetActive(false);
                    imageTextLetter.SetActive(false);
                    textProgress.text = "Done";
                }
                else
                {
                    Lock.SetActive(true);
                    claimActiveButton.SetActive(true);
                    iconDone.SetActive(false);
                    textProgress.gameObject.SetActive(true);
                    textProgress.text = $"{_cacheDataInfo.NumBerGoldEvent}";
                }


            }
            else
            {
                Lock.SetActive(true);
                iconDone.SetActive(false);
                claimActiveButton.SetActive(false);
                textProgress.gameObject.SetActive(true);
                textProgress.text = $"{_cacheDataInfo.NumBerGoldEvent}";
            }
        }
        if (Type == EventValentine.Princess || Type == EventValentine.Princess1)
        {
            TryInitChangeSkinPrincess();
            textProgress.text = $"{_cacheDataInfo.NumBerGoldEvent}";
            if (Data.TotalGoldMedal >= _PrincessInfo.NumBerGoldEvent)
            {
                if (DataController.instance.SavePrincess[index].unlock)
                {
                    claimDisableButton.SetActive(false);
                    iconDone.SetActive(true);
                    Lock.SetActive(false);
                    claimActiveButton.SetActive(false);
                    textProgress.gameObject.SetActive(false);
                    imageTextLetter.SetActive(false);
                    textProgress.text = "Done";
                }
                else
                {
                    Lock.SetActive(true);
                    claimActiveButton.SetActive(true);
                    iconDone.SetActive(false);
                    textProgress.gameObject.SetActive(true);
                    textProgress.text = $"{_PrincessInfo.NumBerGoldEvent}";
                }
            }
            else
            {
                Lock.SetActive(true);
                iconDone.SetActive(false);
                claimActiveButton.SetActive(false);
                textProgress.gameObject.SetActive(true);
                textProgress.text = $"{_PrincessInfo.NumBerGoldEvent}";
            }
        }

        // if (Type == EventValentine.TOP100)
        // {
        //     TryInitChangeSkinPrincess();
        //     Playfab.getMyRankLeadBoard(PlayfabConstants.EVENT_STATISTIC_NAME, (e) =>
        //     {
        //         if (e.Leaderboard[0].Position <= 100)
        //         {
        //             if (DataController.instance.SavePrincess[index].unlock)
        //             {
        //                 claimDisableButton.SetActive(false);
        //                 iconDone.SetActive(true);
        //                 Lock.SetActive(false);
        //                 claimActiveButton.SetActive(false);
        //                 textProgress.gameObject.SetActive(false);
        //                 imageTextLetter.SetActive(false);
        //                 textProgress.text = "Done";
        //             }
        //             else
        //             {
        //                 Lock.SetActive(true);
        //                 claimActiveButton.SetActive(true);
        //                 iconDone.SetActive(false);
        //                 textProgress.gameObject.SetActive(true);
        //                 textProgress.text = $"{_PrincessInfo.NumBerGoldEvent}";
        //             }
        //             iconTop100.SetActive(false);
        //         }
        //     });
        // }

    }
    public void OnClickClaim()
    {

        if (Type == EventValentine.Hero || Type == EventValentine.Hero2 || Type == EventValentine.Hero3)
        {
            DataController.instance.SaveHero[index].unlock = true;
            Data.currentHero = index;
            GamePopup.Instance.menuRoom.ChangeSkin(HeroData.SkinHeroByIndex(Data.currentHero).Item1);
            popupEvent.ResetEvent();
            claimActiveButton.SetActive(false);

        }
        if (Type == EventValentine.Princess || Type == EventValentine.Princess1 || Type == EventValentine.TOP100)
        {
            DataController.instance.SavePrincess[index].unlock = true;
            Data.currentPrincess = index;
            popupEvent.ResetEvent();
            claimActiveButton.SetActive(false);
        }
        if (MenuController.instance != null) MenuController.instance.CheckDisplayWarningDailyGiftEvent();
        if (GameManager.instance != null) GameManager.instance.CheckDisplayWarningDailyGiftEvent();
    }
    public void OnlickTop100()
    {
        GamePopup.Instance.ShowLeaderboardEvent();
    }
}
public enum EventValentine
{
    None,
    Hero,
    Princess,
    Princess1,
    Hero2,
    Hero3,
    Noel,
    TOP100
}
