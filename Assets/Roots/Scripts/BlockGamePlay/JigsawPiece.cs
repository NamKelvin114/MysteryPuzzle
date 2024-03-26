using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class JigsawPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Setup
    [Range(-1, 1)] public int left = 0;
    [Range(-1, 1)] public int right = 0;
    [Range(-1, 1)] public int top = 0;
    [Range(-1, 1)] public int bottom = 0;

    [HideInInspector] public Image pieceVisualImage;
    [HideInInspector] public Image pieceShadowImage;

    // InGame
    [SerializeField] private Transform completedParent;
    [SerializeField] private Transform selectedParent;
    [SerializeField] private Vector3 tracerScale;
    [SerializeField] private bool isComplete;
    [SerializeField] private int pieceRow;
    [SerializeField] private int pieceColumn;

    public bool PieceIsComplete => isComplete;
    public int PieceRow => pieceRow;
    public int PieceColumn => pieceColumn;

    [HideInInspector] public RectTransform _rectTransform;

    public Vector3 completedPosition;
    private Vector3 _selectedScale;
    private Vector3 _completedScale;
    private Transform _tracerSlot;
    private float _flySpeed;
    private float _scaleDuration;
    private float _magnetDistance;
    private float _maxBrightness;
    private float _flyDistance;
    private float _flyDuration;
    private Material _newMaterial;
    private float _duration = 1.0f;
    private bool _isSelected = false;
    public bool canMove = true;
    private Action _onDragStart;
    private Action _onDragEnd;
    private Sequence _sequence;
    private Vector2 _startDragPosition;
    private float _dragOffset;
    private Vector3 _velocity;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        completedPosition = transform.position;
        _completedScale = transform.localScale;
        _selectedScale = _completedScale + Vector3.one * 0.05f;
        _flySpeed = 25.0f;
        _scaleDuration = 0.1f;
        _magnetDistance = 1.0f;
        _maxBrightness = 0.1f;
        // _dragOffset = 1.5f * (_rectTransform.rect.height / 2f);
        _dragOffset = 1.5f * (_rectTransform.rect.height * GameManager.instance.CanvasUI.localScale.x / 2.0f);
        _velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        if (!_isSelected) return;
        PieceTracerState();
        _isSelected = false;
    }

    public void InitPiece(int row, int column, Material glowMaterial, Action onDragStart, Action onDragEnd)
    {
        pieceRow = row;
        pieceColumn = column;
        _onDragStart = onDragStart;
        _onDragEnd = onDragEnd;
        pieceVisualImage.material = glowMaterial;

        if (pieceVisualImage.material != null)
        {
            _newMaterial = new Material(pieceVisualImage.material);

            pieceVisualImage.material = _newMaterial;
        }
    }

    public void SetTracerSlot(Transform slotTransform)
    {
        _tracerSlot = slotTransform;

        PieceTracerState();
    }

    private void CheckCompletePos()
    {
        if ((transform.position - completedPosition).sqrMagnitude < _magnetDistance * _magnetDistance)
        {
            //SoundController.Instance.PlayFX(EnumPack.SoundType.BLOCK_PLACE);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acPlaceBlock);
            PieceCompleteState();
        }
        else
        {
            //SoundController.Instance.PlayFX(EnumPack.SoundType.BLOCK_PLACE_FAIL);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acPlaceBlockFalse);
            PieceTracerState();
        }
    }

    private void PieceTracerState()
    {
        pieceShadowImage.gameObject.SetActive(true);
        pieceVisualImage.raycastTarget = false;

        _flyDistance = (transform.position - _tracerSlot.position).magnitude;
        _flyDuration = _flyDistance / _flySpeed;
        PieceScale(tracerScale, _flyDuration);
        transform.DOMove(_tracerSlot.position, _flyDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            pieceShadowImage.gameObject.SetActive(false);
            pieceVisualImage.raycastTarget = true;
            transform.SetParent(_tracerSlot, true);
            transform.position = _tracerSlot.position;
        });
    }

    private void PieceMoveState()
    {
        pieceShadowImage.gameObject.SetActive(true);
        pieceVisualImage.raycastTarget = false;

        PieceScale(_selectedScale, _scaleDuration);
        transform.SetParent(selectedParent);
    }

    private void PieceCompleteState()
    {
        pieceShadowImage.gameObject.SetActive(false);
        pieceVisualImage.raycastTarget = false;
        isComplete = true;

        PieceScale(_completedScale, _scaleDuration);
        transform.SetParent(completedParent);
        transform.position = completedPosition;
        _tracerSlot.gameObject.SetActive(false);

        //GameEvents.PieceInPlace(this);
        Observer.PieceInPlace(this);
    }

    public void GlowPiece(float distance, float delayGlow = 0, float duration = .6f, float stayGlowTime = 0.2f)
    {
        DOTween.Sequence().AppendInterval(delayGlow).AppendCallback(() =>
        {
            Color outlineColor = pieceVisualImage.material.GetColor("_OutlineColor");

            float newAlpha = outlineColor.a;

            float brightness = distance * _maxBrightness;

            DOTween.To(() => newAlpha, x => newAlpha = x, brightness, duration / 2)
                .OnUpdate(() => UpdateOutlineAlpha(newAlpha))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    DOTween.Sequence().AppendInterval(stayGlowTime).AppendCallback(() =>
                    {
                        DOTween.To(() => newAlpha, x => newAlpha = x, 0f, duration / 2)
                            .OnUpdate(() => UpdateOutlineAlpha(newAlpha))
                            .SetEase(Ease.Linear);
                    });
                });
        });
    }

    private void UpdateOutlineAlpha(float alpha)
    {
        Color outlineColor = pieceVisualImage.material.GetColor("_OutlineColor");
        outlineColor.a = alpha;
        pieceVisualImage.material.SetColor("_OutlineColor", outlineColor);
    }

    private void PieceScale(Vector3 endValue, float scaleDuration)
    {
        _sequence.Kill();
        _sequence = DOTween.Sequence().Append(transform.DOScale(endValue, scaleDuration).SetEase(Ease.Linear));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canMove == false)
            return;
        //SoundController.Instance.PlayFX(EnumPack.SoundType.BLOCK_PICKUP);
        Observer.HidePopupTutorialJigSaw?.Invoke();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeBlock);
        _isSelected = true;
        _onDragStart?.Invoke();

        PieceMoveState();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isSelected == false) return;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, eventData.position,
                eventData.pressEventCamera, out var globalMousePostion))
        {
            globalMousePostion = new Vector3(globalMousePostion.x, globalMousePostion.y + _dragOffset,
                globalMousePostion.z);

            _rectTransform.position =
                Vector3.SmoothDamp(_rectTransform.position, globalMousePostion, ref _velocity, 0.0f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isSelected == false)
            return;
        _isSelected = false;
        CheckCompletePos();
        _onDragEnd?.Invoke();
    }

    public void PieceSetup(Transform completeParent, Transform selectParent, float tracerScale)
    {
        completedParent = completeParent;
        selectedParent = selectParent;
        this.tracerScale = Vector3.one * tracerScale;
    }
}