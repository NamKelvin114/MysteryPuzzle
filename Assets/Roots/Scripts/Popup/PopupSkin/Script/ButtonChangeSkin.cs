using System.Collections;
using System.Collections.Generic;
using Pancake.UI;
using UnityEngine;

public class ButtonChangeSkin : MonoBehaviour
{
    [SerializeField] private SkinItemType skinItemType; 
    [SerializeField] private GameObject active;
    [SerializeField] private GameObject deactive;
    [SerializeField] private UIButton gameButton;
    public UIButton UIButton => gameButton;
    public SkinItemType SkinItemType => skinItemType;
    public void Refresh(SkinItemType _skinItemType)
    {
        if (_skinItemType == skinItemType)
        {
            Active();
        }
        else
        {
            Deactive();   
        }
    }
    
    public void Active() 
    {
        active.gameObject.SetActive(true);
        deactive.gameObject.SetActive(false);
    }

    public void Deactive() 
    {
        active.gameObject.SetActive(false);
        deactive.gameObject.SetActive(true);
    }
    
    
}
