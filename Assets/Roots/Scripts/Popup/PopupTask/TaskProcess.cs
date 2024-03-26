using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskProcess : MonoBehaviour
{
    [SerializeField] private Image contentText;
    [SerializeField] private TextMeshProUGUI processText;
    [SerializeField] private SkeletonGraphic chest;

    [SerializeField, SpineAnimation(dataField = "chest")]
    private string close;

    [SerializeField, SpineAnimation(dataField = "chest")]
    private string open;

    [SerializeField] private Image imageProcess;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos;
    [SerializeField] private GameObject completeState;
    [SerializeField] private GameObject onDoingState;

    public void Setup(Vector2 fillAmountData, Sprite _contentText = null)
    {
        completeState.SetActive(false);
        onDoingState.SetActive(false);

        if (fillAmountData.y == 0)
        {
            completeState.SetActive(true);
        }
        else
        {
            onDoingState.SetActive(true);
        }

        if (contentText != null)
        {
            contentText.sprite = _contentText;
        }

        float fillAmount = (float)fillAmountData.x / fillAmountData.y;
        imageProcess.DOFillAmount(fillAmount, 0.5f);
        processText.text = fillAmountData.x + "/" + fillAmountData.y;
        if (fillAmountData.x == fillAmountData.y)
        {
            chest.AnimationState.SetAnimation(0, open, false);
        }
        else
        {
            chest.AnimationState.SetAnimation(0, close, false);
        }
    }

    public Vector3 CalculateFillPosition()
    {
        float curFill = imageProcess.fillAmount;
        float len = endPos.transform.position.x - startPos.transform.position.x;
        float targetX = startPos.transform.position.x + len * curFill;
        return new Vector3(targetX, startPos.transform.position.y, startPos.transform.position.z);
    }
}