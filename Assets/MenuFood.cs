using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuFood : MonoBehaviour
{
    // Start is called before the first frame update
    public float menuFoodTimeScale = .5f;
    public float numberRotation = 1f;
    public float sizeMax = 0.5f;
    [SerializeField] private Image imageItem;
    [SerializeField] private TextMeshProUGUI textDecription;
    [SerializeField] private TextMeshProUGUI textTitle;

    public void Setup(Sprite item, string decription, string title)
    {
        imageItem.sprite = item;
        textDecription.text = decription;
        textTitle.text = title;
    }
}
