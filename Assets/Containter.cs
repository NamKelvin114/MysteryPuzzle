using System;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using TMPro;
using UnityEngine;

public class Containter : MonoBehaviour
{
    // Start is called before the first frame update    
    private bool endGame;
    [SerializeField] private TargetType targetTypeToCollect;
    private EnityTarget enityTarget;
    [SerializeField] private TextMeshPro textPercent;
    private List<GameObject> _objectsCollide = new List<GameObject>();
    private Transform trans;

    private void Awake()
    {
        Observer.setPostionOfClothe += SetTransformOfClothe;
        Observer.playerEndGameplay2 += TurnOffPercent;
        Observer.playerDieGameplay2 += TurnOffPercent;
        Observer.CheckWinTargetPercent += SetUsingPercent;
    }

    private void Start()
    {
        enityTarget = MapLevelManager.Instance.GetETargetToWin();
        if (enityTarget != null)
        {
            textPercent.gameObject.SetActive(enityTarget.isUsePercent);
        }
    }

    private void TurnOffPercent(bool x)
    {
        textPercent.gameObject.SetActive(false);
    }

    private void SetTransformOfClothe(Transform transform)
    {
        trans = transform;
    }

    private float _currentPercent = 0;
    private Target _percentTarget;
    private Target _previousTarget;

    private void OnTriggerEnter2D(Collider2D col)
    {
        var target = col.gameObject.GetComponentInParent<Target>();
        if (target != null && col.gameObject.layer != 0 && _previousTarget != target)
        {
            if (GameManager.instance.gameState == EGameState.Lose ||
                GameManager.instance.gameState == EGameState.Win) return;
            if (endGame) return;
            if (!_objectsCollide.Contains(col.gameObject))
            {
                _objectsCollide.Add(col.gameObject);
                _percentTarget = col.gameObject.GetComponentInParent<Target>();
                // if (MapLevelManager.Instance != null)
                //     MapLevelManager.Instance.GetComponentInChildren()
                if (_percentTarget.TargetType == enityTarget.TargetType)
                {
                    if (enityTarget.disappearWhenCollider) _percentTarget.gameObject.SetActive(false);
                    enityTarget.countCurent++;
                    if (enityTarget.isUsePercent)
                    {
                        _currentPercent = (float)enityTarget.countCurent / (float)enityTarget.countMax * 100f;
                        if (MapLevelManager.Instance.GetETargetToWin().TargetType == enityTarget.TargetType)
                        {
                            UpdateText((int)_currentPercent);
                            enityTarget.countAlive--;
                            MapLevelManager.Instance.CheckRunOutOfTarget(_percentTarget);
                        }
                    }
                    else
                    {
                        if (enityTarget.countCurent >= enityTarget.numberToWin)
                        {
                            if (!endGame)
                                if (_percentTarget is Clothe)
                                {
                                    var c = (Clothe)_percentTarget;
                                    c.Applly(trans.position, () =>
                                    {
                                        c.gameObject.SetActive(false);
                                        Observer.ActiveFxMatch?.Invoke();
                                        if (SoundManager.Instance != null)
                                            SoundManager.Instance.PlaySound(SoundManager.Instance.acFxMatch);
                                        MapLevelManager.Instance.endGameplay2?.Invoke(_percentTarget.TargetType,
                                            _percentTarget.ExpectedType);
                                        if (col.gameObject.GetComponent<SkinCollection>() != null)
                                        {
                                            var skin = col.gameObject.GetComponent<SkinCollection>();
                                            foreach (var skinData in skin.skinData)
                                            {
                                                skinData.IsUnlocked = true;
                                                Observer.UseSkin?.Invoke(skinData.skinName, skin.skinType);
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    DOTween.Sequence()
                                        .AppendInterval(MapLevelManager.Instance.isNotPullAllPins ? 1.5f : 0f)
                                        .OnComplete(() =>
                                            MapLevelManager.Instance.endGameplay2?.Invoke(_percentTarget.TargetType,
                                                _percentTarget.ExpectedType));
                                }

                            SetTargetType(enityTarget.TargetType);
                            textPercent.gameObject.SetActive(false);
                            endGame = true;
                        }
                    }
                }
                else
                {
                    enityTarget = MapLevelManager.Instance.GetETarget(target.TargetType);
                    if (enityTarget.disappearWhenCollider) _percentTarget.gameObject.SetActive(false);
                    enityTarget.countCurent++;
                    if (enityTarget.isUsePercent)
                    {
                        float currentPercent = (float)enityTarget.countCurent / (float)enityTarget.countMax * 100f;
                        if (currentPercent >= enityTarget.percentToWin)
                        {
                            if (!endGame)
                            {
                                textPercent.gameObject.SetActive(false);
                                DOTween.Sequence().AppendInterval(MapLevelManager.Instance.isNotPullAllPins ? 1.5f : 0f)
                                    .OnComplete(() =>
                                        MapLevelManager.Instance.endGameplay2?.Invoke(_percentTarget.TargetType,
                                            _percentTarget.ExpectedType));
                            }

                            textPercent.gameObject.SetActive(false);
                            endGame = true;
                        }
                    }
                    else
                    {
                        if (enityTarget.countCurent >= enityTarget.numberToWin)
                        {
                            textPercent.gameObject.SetActive(false);
                            if (!endGame)
                            {
                                DOTween.Sequence().AppendInterval(MapLevelManager.Instance.isNotPullAllPins ? 1.5f : 0f)
                                    .OnComplete(() =>
                                        MapLevelManager.Instance.endGameplay2?.Invoke(_percentTarget.TargetType,
                                            _percentTarget.ExpectedType));
                            }

                            endGame = true;
                        }
                    }
                }

                _previousTarget = target;
            }
        }
    }

    void SetUsingPercent()
    {
        float lastCheck = 0;
        lastCheck = (float)enityTarget.countCurent / (float)enityTarget.countMax * 100f;
        if (lastCheck >= enityTarget.percentToWin)
        {
            WaitToEndOrWinGame(true);
        }
        else
        {
            WaitToEndOrWinGame(false);
        }
    }

    void WaitToEndOrWinGame(bool isWin)
    {
        Observer.StopCheckPinLoseCondition?.Invoke();
        if (isWin)
        {
            DOTween.Sequence().AppendInterval(2).AppendCallback((() =>
            {
                if (!endGame)
                    MapLevelManager.Instance.endGameplay2?.Invoke(_percentTarget.TargetType,
                        _percentTarget.ExpectedType);
                SetTargetType(enityTarget.TargetType);
                endGame = true;
                textPercent.gameObject.SetActive(false);
            }));
        }
        else
        {
            DOTween.Sequence().AppendInterval(2).AppendCallback((() =>
            {
                Observer.playerEndGameplay2?.Invoke(false);
            }));
        }
    }

    void SetTargetType(TargetType targeType)
    {
        // if (targeType == TargetType.Hamberger || targeType == TargetType.Cheesesteak || targeType == TargetType.Pizza || targeType == TargetType.Sanwich ||
        //     targeType == TargetType.CheeseSanwich || targeType == TargetType.CrispyFries)
        // {
        //     Utils.SetTaskProcess(ETaskType.Eat, Utils.GetTaskProcess(ETaskType.Eat) + 1);
        // }
        // else if (targeType == TargetType.Water)
        // {
        //     Utils.SetTaskProcess(ETaskType.Drink, Utils.GetTaskProcess(ETaskType.Drink) + 1);
        // }
        // else if (targeType == TargetType.Gems)
        // {
        //     Utils.SetTaskProcess(ETaskType.CollectGold, Utils.GetTaskProcess(ETaskType.CollectGold) + 1);
        // }
        // else if (targeType == TargetType.Key)
        // {
        //     Utils.SetTaskProcess(ETaskType.CollectKey, Utils.GetTaskProcess(ETaskType.CollectKey) + 1);
        // }
        // else if (targeType == TargetType.Chest)
        // {
        //     Utils.SetTaskProcess(ETaskType.CollectChest, Utils.GetTaskProcess(ETaskType.CollectChest) + 1);
        // }
    }

    private void UpdateText(int _percent)
    {
        textPercent.text = _percent + "%";
    }

    private void OnDestroy()
    {
        Observer.setPostionOfClothe -= SetTransformOfClothe;
        Observer.playerEndGameplay2 -= TurnOffPercent;
        Observer.playerDieGameplay2 -= TurnOffPercent;
        Observer.CheckWinTargetPercent -= SetUsingPercent;
    }
}