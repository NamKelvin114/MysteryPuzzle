using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Worldreaver.UniUI;

public class EventRewardItem : MonoBehaviour
{
    [SerializeField] private GameObject btnDisable;
    [SerializeField] private PopupEvent popupEvent;
    [SerializeField] public GameObject imageDay7;

    public GameObject tick;
    public int dayIndex;
    public TextMeshProUGUI txtDay;


    public void DisplayAgain()
    {
        if (dayIndex == Utils.curEventDailyGift && !Utils.canTakeEventGiftDaily && !Utils.IsClaimEventReward() && Utils.curEventDailyGift<=7)
        {  
            popupEvent.Day = dayIndex;
            popupEvent.BtnClaim[dayIndex-1].gameObject.SetActive(true); 
            tick.SetActive(false);
        }
        else if (dayIndex == Utils.curEventDailyGift && Utils.canTakeEventGiftDaily)
        {
            // popupEvent.BtnClaim[dayIndex-1].gameObject.SetActive(false);
            tick.SetActive(false);
            btnDisable.gameObject.SetActive(true);
        }
        else if (dayIndex == Utils.curEventDailyGift-1 && !Utils.canTakeEventGiftDaily && Utils.IsClaimEventReward() )
        {
            tick.SetActive(true);
            btnDisable.gameObject.SetActive(false);
            popupEvent.BtnClaim[dayIndex-1].gameObject.SetActive(false);
        }
        else if (dayIndex < Utils.curEventDailyGift)
        {
            tick.SetActive(true);
            btnDisable.gameObject.SetActive(false);
            popupEvent.BtnClaim[dayIndex-1].gameObject.SetActive(false);
        }
        else
        { 
            if(dayIndex==7 && !Utils.canTakeEventGiftDaily && !Utils.IsClaimEventReward() && Utils.curEventDailyGift>7)
            {
                tick.SetActive(true);
                btnDisable.gameObject.SetActive(false);
                popupEvent.BtnClaim[dayIndex-1].gameObject.SetActive(false);
            }
            else tick.SetActive(false); 
        }
        if(MenuController.instance!=null)MenuController.instance.CheckDisplayWarningDailyGiftEvent();
        if(GameManager.instance!=null)GameManager.instance.CheckDisplayWarningDailyGiftEvent();
    }

    private void OnEnable() { DisplayAgain(); }
}
