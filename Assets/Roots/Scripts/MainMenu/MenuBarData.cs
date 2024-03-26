using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class MenuBarData : MonoBehaviour
{
    // Start is called before the first frame update
    public static MenuBarData instance;

    [SerializeField] private UniButton btnSkin;
    [SerializeField] private UniButton btnMap;
    [SerializeField] private UniButton btnHome;
    [SerializeField] private UniButton btnCollection;
    [SerializeField] private UniButton btnPet;
    [SerializeField] private List<Image> imageButtonChoosed;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetupData(out UniButton buttonSkin, out UniButton buttonMap, out UniButton buttonHome, 
        out UniButton buttonPet, out UniButton buttonCollection,out List<Image> imagesButton)
    {
        buttonSkin = btnSkin;
        buttonMap = btnMap;
        buttonHome = btnHome;
        buttonPet = btnPet;
        buttonCollection = btnCollection;
        imagesButton = imageButtonChoosed;
    }
}
