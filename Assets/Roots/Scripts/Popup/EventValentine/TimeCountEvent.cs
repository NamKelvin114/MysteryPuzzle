using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Spine;
using Spine.Unity;

public class TimeCountEvent : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI textCountdown;

    public void OnEnable()
    {
        CancelInvoke("Countdown");
        InvokeRepeating("Countdown", 0, 1);
    }

    public void Countdown()
    {
        if((int)Data.TimeToRescueParty.Milliseconds>0 && Data.isTimeValentine)
        {
            textCountdown.text = Utils.FormatTime(Data.TimeToRescueParty);
        }
        else
        {
            textCountdown.text="00:00";
        }
        // Debug.Log(textCountdown.text);
    }
    
}



