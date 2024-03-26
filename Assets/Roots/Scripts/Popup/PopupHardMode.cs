using System;
using UnityEngine;
using Worldreaver.UniUI;

public class PopupHardMode : UniPopupBase
{
    [SerializeField] private UniButton btnClose;
    [SerializeField] private UniButtonTMP[] btnHardLevel; // need improve

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite currentSprite;
    [SerializeField] private Sprite lockedSprite;

    private Action _actionClose;

    public void Initialized(Action actionClose)
    {
        _actionClose = actionClose;
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(OnCloseButtonPressed);

        for (int i = 0; i < btnHardLevel.Length; i++)
        {
            var level = i;

            btnHardLevel[i].Text.text = $"{i + 1}";
            btnHardLevel[i].onClick.RemoveAllListeners();
            btnHardLevel[i].onClick.AddListener(() => OnLevelButtonPressed(level));
        }

        DisplayRefresh();
    }

    public void DisplayRefresh()
    {
        for (int i = 0; i < btnHardLevel.Length; i++)
        {
            btnHardLevel[i].interactable = true;
            if (i <= Utils.MaxHardLevel)
            {
                btnHardLevel[i].image.sprite = normalSprite;
            }
            else
            {
                btnHardLevel[i].interactable = false;
                btnHardLevel[i].image.sprite = lockedSprite;
            }

            if (i == Utils.CurrentHardLevel)
            {
                btnHardLevel[i].interactable = true;
                btnHardLevel[i].image.sprite = currentSprite;
            }
        }
    }
    
    
    private void OnCloseButtonPressed() { _actionClose?.Invoke(); }

    private void OnLevelButtonPressed(int level)
    {
        _actionClose?.Invoke();
        Utils.isHardMode = true;
        Utils.CurrentHardLevel = level;
        MenuController.instance.InternalNextLevel();
        SoundManager.Instance.PlaySound(SoundManager.Instance.btnStart);
    }
}