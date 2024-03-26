using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class TravelStoryItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI textChapterNumber;
    [SerializeField] private TextMeshProUGUI textCountryName;
    [SerializeField] private UniButton btnChooseRoom;

    public void SetupUnlockState(Sprite BgSprite, Sprite iconSprite, int chapterNumber, string countryName,
        Action chooseRoom)
    {
        iconImage.gameObject.SetActive(true);
        textChapterNumber.gameObject.SetActive(true);
        textCountryName.gameObject.SetActive(true);
        btnChooseRoom.interactable = true;
        bgImage.sprite = BgSprite;
        iconImage.sprite = iconSprite;
        textChapterNumber.text = "Chap " + chapterNumber;
        textCountryName.text = countryName;
        btnChooseRoom.onClick.RemoveAllListeners();
        btnChooseRoom.onClick.AddListener(() =>
        {
            chooseRoom?.Invoke();
            if (MenuController.instance != null)
            {
                MenuController.instance.SoundClickButton();
            }
        });
    }

    public void SetupDefaultState(Sprite BgSprite)
    {
        iconImage.gameObject.SetActive(false);
        textChapterNumber.gameObject.SetActive(false);
        textCountryName.gameObject.SetActive(false);
        bgImage.sprite = BgSprite;
        btnChooseRoom.interactable = false;
    }
}