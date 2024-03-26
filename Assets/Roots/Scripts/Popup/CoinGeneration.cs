using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;

public class CoinGeneration : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject from;
    [SerializeField] private GameObject to;
    [SerializeField] private int numberCoin;
    [SerializeField] private int delay;
    [SerializeField] private float durationNear;
    [SerializeField] private float durationTarget;
    [SerializeField] private Ease easeNear;
    [SerializeField] private Ease easeTarget;
    [SerializeField] private float scale = 1;

    private GameObject _from;
    private int numberCoinMoveDone;
    private System.Action moveOneCoinDone;
    private System.Action moveAllCoinDone;
    public void SetNumberCoin(int numberCoin)
    {
        this.numberCoin = numberCoin;
    }

    public void SetFromGameObject(GameObject from)
    {
        _from = from;
    }

    public void SetToGameObject(GameObject to)
    {
        this.to = to;
    }

    private void Start()
    {
        if (overlay != null)
        {
            overlay.SetActive(false);
        }
        Observer.AddFromPosiGenerationCoin += SetFromGameObject;
    }
    public async void GenerateCoin(System.Action moveOneCoinDone, System.Action moveAllCoinDone, GameObject from = null, GameObject to = null, int numberCoin = -1)
    {
        this.moveOneCoinDone = moveOneCoinDone;
        this.moveAllCoinDone = moveAllCoinDone;
        _from = from == null ? _from : this.from;
        this.to = to == null ? this.to : to;
        this.numberCoin = numberCoin < 0 ? this.numberCoin : numberCoin;
        numberCoinMoveDone = 0;
        SoundManager.Instance.PlaySound(SoundManager.Instance.coinGain);
        if (overlay != null)
        {
            overlay.SetActive(true);

        }
        for (int i = 0; i < this.numberCoin; i++)
        {
            await Task.Delay(Random.Range(0, delay));
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.transform.localScale = Vector3.one * scale;
            if (_from != null)
            {
                coin.transform.position = _from.transform.position;
            }
            else coin.transform.position = this.from.transform.position;
            MoveCoin(coin);
        }
    }
    // public async void GenerateGoldMedal(System.Action moveOneCoinDone, System.Action moveAllCoinDone, GameObject from = null, GameObject to = null, int numberCoin = -1)
    // {
    //     this.moveOneCoinDone = moveOneCoinDone;
    //     this.moveAllCoinDone = moveAllCoinDone;
    //     this.from = from == null ? this.from : from;
    //     this.to = to == null ? this.to : to;
    //     this.numberGoldMedal = numberCoin < 0 ? this.numberGoldMedal : numberCoin;
    //     numberCoinMoveDone = 0;
    //     overlay.SetActive(true);
    //     for (int i = 0; i < this.numberGoldMedal; i++)
    //     {
    //         await Task.Delay(Random.Range(0, delay));
    //         GameObject GoldMedal = Instantiate(GoldMedalPrefab, transform);
    //         GoldMedal.transform.localScale = Vector3.one * scaleGoldMedal;
    //         GoldMedal.transform.position = this.from.transform.position;
    //         MoveCoin(GoldMedal);
    //     }
    // }

    private void MoveCoin(GameObject coin)
    {
        MoveToNear(coin).OnComplete(() =>
        {
            MoveToTarget(coin).OnComplete(() =>
            {
                numberCoinMoveDone++;
                Destroy(coin);
                moveOneCoinDone?.Invoke();
                if (numberCoinMoveDone >= numberCoin)
                {
                    moveAllCoinDone?.Invoke();
                    overlay.SetActive(false);
                }
            });
        });
    }

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveTo(Vector3 endValue, GameObject coin, float duration, Ease ease)
    {
        return coin.transform.DOMove(endValue, duration).SetEase(ease);
    }

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveToNear(GameObject coin)
    {
        return MoveTo(coin.transform.position + (Vector3)Random.insideUnitCircle, coin, durationNear, easeNear);
    }

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveToTarget(GameObject coin)
    {
        return MoveTo(to.transform.position, coin, durationTarget, easeTarget);
    }
}
