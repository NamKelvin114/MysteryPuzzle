using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyAmountText;
    [SerializeField] private int stepCount = 10;
    [SerializeField] private float delayTime = .01f;
    [SerializeField] private CoinGeneration coinGeneration;
    [SerializeField] PopupMoney popupMoney;
    private int _currentCoin;
    private void Start()
    {
        Observer.SaveCurrencyTotal += SaveCurrency;
        Observer.CurrencyTotalChanged += UpdateCurrencyAmountText;
        currencyAmountText.text = Utils.currentCoin.ToString();
    }
    private void OnEnable()
    {
        currencyAmountText.text = Utils.currentCoin.ToString();
    }

    private void SaveCurrency()
    {
        _currentCoin = Utils.currentCoin;
    }

    private void UpdateCurrencyAmountText(bool isIncreasing)
    {
        if(isIncreasing) IncreaseCurrency();
        else DecreaseCurrency();
    }

    private void IncreaseCurrency()
    {
        bool isFirstMove = false;
        coinGeneration.GenerateCoin(() =>
        {
            if (!isFirstMove)
            {
                isFirstMove = true;
                int currentCurrencyAmount = int.Parse(currencyAmountText.text);
                int nextAmount = (Utils.currentCoin - currentCurrencyAmount) / stepCount;
                int step = stepCount;
                CurrencyTextCount(currentCurrencyAmount, nextAmount, step);
            }
        }, () =>
        {
            // Observer.CoinMove?.Invoke();
        });
    }

    private void DecreaseCurrency()
    {
        int currentCurrencyAmount = int.Parse(currencyAmountText.text);
        int nextAmount = (Utils.currentCoin - currentCurrencyAmount) / stepCount;
        int step = stepCount;
        CurrencyTextCount(currentCurrencyAmount, nextAmount, step);
    }

    private void CurrencyTextCount(int currentCurrencyValue, int nextAmountValue, int stepCount)
    {
        if (stepCount == 0)
        {
            currencyAmountText.text = Utils.currentCoin.ToString();
            return;
        }
        int totalValue = (currentCurrencyValue + nextAmountValue);
        DOTween.Sequence().AppendInterval(delayTime).SetUpdate(isIndependentUpdate: true).AppendCallback(() =>
        {
            currencyAmountText.text = Utils.currentCoin.ToString();

        }).AppendCallback(() =>
        {
            CurrencyTextCount(totalValue, nextAmountValue, stepCount - 1);
        });
    }
}
