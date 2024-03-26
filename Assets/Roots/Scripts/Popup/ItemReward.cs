using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemReward : MonoBehaviour
{
    [SerializeField] private Image icon;
    public void SetIcon(Sprite getIcon)
    {
        icon.sprite = getIcon;
    }
}
