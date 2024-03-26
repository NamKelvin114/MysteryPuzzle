using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNoti : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject noti;
    void Start()
    {
        Observer.ButtonTaskActiveNoti += ActiveNoti;
    }

    private void ActiveNoti(bool active)
    {
        noti.SetActive(active);
    }

    private void OnDestroy()
    {
        Observer.ButtonTaskActiveNoti -= ActiveNoti;
    }
}
