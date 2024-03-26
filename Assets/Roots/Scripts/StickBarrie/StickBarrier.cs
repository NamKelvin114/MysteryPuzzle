using System;
using System.Collections;
using DG.Tweening;
using Pancake;
using UnityEngine;
using Random = Pancake.Random;


public class StickBarrier : MonoBehaviour
{
    public bool key;
    private const float speedAdd = 2;

    public enum MOVETYPE
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,
        FREE
    }

    [SerializeField] public MOVETYPE _moveType;
    public EPinType EPinType;
    private float shakeEndTime;
    [SerializeField] public Rigidbody2D _rig2D;
    [Range(0, 10)] [SerializeField] public float moveSpeed = 2;
    [SerializeField] public bool hasBlockGems;

    public bool isMove2Pos;

    [DrawIf("_moveType", MOVETYPE.FREE, ComparisonType.Equals, DisablingType.DontDraw)] [SerializeField]
    public Vector2 vEndPos;

    [SerializeField] public Vector2 vStartPos;
    public bool isTutorial;

    [ShowIf("isTutorial", true)] [SerializeField]
    GameObject handleTutorialGameObject;

    [SerializeField] private Vector2 vUpPos, vDownPos, vLeftPos, vRightPos;
    [HideInInspector] public bool beginMove;
    [HideInInspector] public bool moveBack;
    private bool doIncreaseProcess = true;
    private bool doDecreaseProcess = true;
    Sequence mysquence = DOTween.Sequence();

    private void OnValidate()
    {
        if (_rig2D == null)
        {
            _rig2D = GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        if (_moveType != MOVETYPE.FREE)
        {
            vStartPos = (Vector2)transform.localPosition;
        }

        MapLevelManager.Instance.lstStick.Add(this);
        if (Utils.CurrentLevel > Config.MaxLevelCanReach)
        {
            if (handleTutorialGameObject != null)
                handleTutorialGameObject.SetActive(false);
            isTutorial = false;
        }

        Observer.StopCheckPinLoseCondition += StopCheckLoseConditon;
    }

    void IncreaseProcess()
    {
        if (doIncreaseProcess)
        {
            doIncreaseProcess = false;
            doDecreaseProcess = true;
            //Utils.SetTaskProcess(ETaskType.PullPin, Utils.GetTaskProcess(ETaskType.PullPin) + 1);
        }
    }

    void DeCreaseProcess()
    {
        if (doDecreaseProcess)
        {
            doDecreaseProcess = false;
            //Utils.SetTaskProcess(ETaskType.PullPin, Utils.GetTaskProcess(ETaskType.PullPin) - 1);
        }
    }

    void StopCheckLoseConditon()
    {
        mysquence.Kill();
    }

    private void OnBecameInvisible()
    {
        if (GameManager.instance.gameState == EGameState.Playing)
        {
            if (!moveBack)
            {
                hasBlockGems = false;
                //gameObject.SetActive(false);
                Observer.UpdateTempValue(ETaskType.PullPin);
                try
                {
                    if (GameManager.instance.mapLevel.lstAllStick.Contains(this))
                    {
                        GameManager.instance.mapLevel.lstAllStick.Remove(this);
                    }

                    if (GameManager.instance.mapLevel.lstAllStick.Count <= 0) PlayerManager.instance.beginMove = true;
                    if (GameManager.instance.mapLevel.lstStick.Contains(this))
                    {
                        GameManager.instance.mapLevel.lstStick.Remove(this);
                        if (GameManager.instance.mapLevel.lstStick.Count == 0 &&
                            !MapLevelManager.Instance.isNotPullAllPins)
                            mysquence = DOTween.Sequence()
                                .AppendInterval(MapLevelManager.Instance.isGameplay1 ? 5f : 3f)
                                .OnComplete(() =>
                                {
                                    if (!MapLevelManager.Instance.isGameplay1)
                                        Observer.playerEndGameplay2?.Invoke(false);
                                    else PlayerManager.instance.OnPlayerDie(EDieReason.Despair);
                                });
                    }
                }
                catch (System.Exception)
                {
                }
            }
        }
    }

    private void MoveStick(
        Vector2 endPos)
    {
        if (moveBack)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, endPos, Time.deltaTime * moveSpeed);
        }
        else
        {
            _rig2D.velocity = -transform.up * moveSpeed * speedAdd;
        }
    }

    [SerializeField] private Vector2 dir;

    private void MoveStick2Pos(
        Vector2 endPos)
    {
        //_rig2D.velocity = -transform.up * moveSpeed * speedAdd;
        transform.position = Vector2.Lerp(transform.position, endPos, Time.deltaTime * moveSpeed);
    }

    private void MoveStickBarrie()
    {
        if (isTutorial && handleTutorialGameObject.gameObject.activeInHierarchy)
        {
            MapLevelManager.Instance.isShowTutorial = false;
            isTutorial = false;
            handleTutorialGameObject.gameObject.SetActive(false);
        }

        IncreaseProcess();
        StartMovePin();
    }

    public void StartMovePin()
    {
        switch (_moveType)
        {
            case MOVETYPE.FREE:
                if (isMove2Pos)
                {
                    if (beginMove && !moveBack)
                    {
                        if (Vector2.Distance(transform.position, vEndPos) > 0.03f)
                        {
                            MoveStick2Pos(vEndPos);
                        }
                        else
                        {
                            _rig2D.velocity = Vector2.zero;
                            beginMove = false;
                            moveBack = true;
                        }
                    }

                    if (moveBack && beginMove)
                    {
                        if (Vector2.Distance(transform.position, vStartPos) > 0.03f)
                        {
                            DeCreaseProcess();
                            MoveStick2Pos(vStartPos);
                        }
                        else
                        {
                            _rig2D.velocity = Vector2.zero;
                            beginMove = false;
                            moveBack = false;
                            doIncreaseProcess = true;
                        }
                    }
                }
                else
                {
                    if (beginMove && !moveBack)
                    {
                        if (Vector2.Distance(transform.localPosition, vEndPos) > 0.003f)
                        {
                            MoveStick(vEndPos);
                        }
                        else
                        {
                            beginMove = false;
                            moveBack = true;
                        }
                    }

                    if (moveBack && beginMove)
                    {
                        if (Vector2.Distance(transform.localPosition, vStartPos) > 0.003f)
                        {
                            DeCreaseProcess();
                            MoveStick(vStartPos);
                        }
                        else
                        {
                            beginMove = false;
                            moveBack = false;
                            doIncreaseProcess = true;
                        }
                    }
                }

                break;
            case MOVETYPE.LEFT:
                if (beginMove && !moveBack)
                {
                    _rig2D.velocity = Vector2.left * moveSpeed * speedAdd;
                }

                if (moveBack && beginMove)
                {
                    if (Vector2.Distance(transform.localPosition, vStartPos) > 0.003f)
                    {
                        DeCreaseProcess();
                        MoveStick(vStartPos);
                    }
                    else
                    {
                        beginMove = false;
                        moveBack = false;
                        doIncreaseProcess = true;
                    }
                }

                break;
            case MOVETYPE.RIGHT:
                if (beginMove && !moveBack)
                {
                    _rig2D.velocity = Vector2.right * moveSpeed * speedAdd;
                }

                if (moveBack && beginMove)
                {
                    if (Vector2.Distance(transform.localPosition, vStartPos) > 0.003f)
                    {
                        DeCreaseProcess();
                        MoveStick(vStartPos);
                    }
                    else
                    {
                        beginMove = false;
                        moveBack = false;
                        doIncreaseProcess = true;
                    }
                }

                break;
            case MOVETYPE.UP:
                if (beginMove && !moveBack)
                {
                    _rig2D.velocity = Vector2.up * moveSpeed * speedAdd;
                }

                if (moveBack && beginMove)
                {
                    if (Vector2.Distance(transform.localPosition, vStartPos) > 0.003f)
                    {
                        DeCreaseProcess();
                        MoveStick(vStartPos);
                    }
                    else
                    {
                        beginMove = false;
                        moveBack = false;
                        doIncreaseProcess = true;
                    }
                }

                break;
            case MOVETYPE.DOWN:
                if (beginMove && !moveBack)
                {
                    _rig2D.velocity = Vector2.down * moveSpeed * speedAdd;
                }

                if (moveBack && beginMove)
                {
                    if (Vector2.Distance(transform.localPosition, vStartPos) > 0.003f)
                    {
                        DeCreaseProcess();
                        MoveStick(vStartPos);
                    }
                    else
                    {
                        beginMove = false;
                        moveBack = false;
                        doIncreaseProcess = true;
                    }
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        if (beginMove)
        {
            MoveStickBarrie();
        }
    }

    private void OnCollisionEnter2D(
        Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Utils.TAG_WIN))
        {
            if (!hasBlockGems) hasBlockGems = true;
        }
    }

    #region Editor

    public void SaveEndPos()
    {
        if (isMove2Pos)
        {
            vEndPos = transform.localPosition;
            if (vStartPos == Vector2.zero)
            {
                vStartPos = vEndPos;
            }

            transform.localPosition = vStartPos;
        }
        else
        {
            vEndPos = transform.localPosition;
            if (vStartPos == Vector2.zero)
            {
                vStartPos = vEndPos;
            }

            transform.localPosition = vStartPos;
        }
    }

    public void SaveStartPos()
    {
        if (isMove2Pos)
        {
            vStartPos = transform.localPosition;
            transform.localPosition = vStartPos;
        }
        else
        {
            vStartPos = transform.localPosition;
            transform.localPosition = vStartPos;
        }
    }

    #endregion

    private void OnDestroy()
    {
        Observer.StopCheckPinLoseCondition -= StopCheckLoseConditon;
    }
}

public enum EPinType
{
    PullPin,
    PinMove
}