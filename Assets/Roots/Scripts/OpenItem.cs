using System;
using System.Reflection;
using DG.Tweening;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Worldreaver.UniUI;

public class OpenItem : MonoBehaviour
{
    [SerializeField] private GameObject spin;
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private UniButton btnOpen;
    [SerializeField] private SkeletonGraphic skeleton;

    private Action _action;
    private Vector3 _position;

    public void Initialized(Action action, string nameSkin, string nameItem, Vector3 pos)
    {
        _position = pos;
        _action = action;
        btnOpen.onClick.RemoveAllListeners();
        btnOpen.onClick.AddListener(OnOpenButtonPressed);
        txtMessage.gameObject.SetActive(true);
        txtMessage.text = $"YOU FOUND A {nameItem.ToUpper()}";
        //skeleton.ChangeSkin(nameSkin);
    }

    private void OnOpenButtonPressed()
    {
        GameManager.instance.SoundClickButton();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.openBoxFX);
        spin.SetActive(false);
        txtMessage.gameObject.SetActive(false);
        btnOpen.gameObject.SetActive(false);
        GetComponent<Image>().enabled = false;
        skeleton.transform.DOScale(Vector3.one, 0.5f);
        skeleton.AnimationState.SetAnimation(0, "animation", false);

        skeleton.transform.DOMove(_position, 0.5f).OnComplete(
            () =>
            {
                gameObject.SetActive(false);
                _action?.Invoke();
            });
    }

    private void OnDisable()
    {
        skeleton.transform.localScale = new Vector3(3, 3, 3);
        skeleton.transform.localPosition = new Vector3(0, -106f);
        btnOpen.gameObject.SetActive(true);
        GetComponent<Image>().enabled = true;
        spin.SetActive(true);
    }
}