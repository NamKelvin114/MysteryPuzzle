using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JigsawController : MonoBehaviour
{
    [SerializeField] private JigsawData jigsawData;
    [SerializeField] protected List<JigsawPiece> pieceList = new List<JigsawPiece>();
    [SerializeField] private Image finalImage;

    private int numberOfPieces;
    private int currentPieces;
    private int maxDistance;
    private int mapSize;
    private float delayGlowTime;
    private float glowDuration;
    private float stayGlowTime;

    private void Awake()
    {
        numberOfPieces = pieceList.Count;
        currentPieces = 0;
        maxDistance = 2;
        delayGlowTime = 0.3f;
        glowDuration = 0.4f;
        stayGlowTime = 0.2f;

        mapSize = (int)Mathf.Sqrt(numberOfPieces);
    }

    private void Start()
    {
        //SetUpPiece();
        Observer.MoveToTracer(pieceList);
        for (int i = 0; i < pieceList.Count; i++)
        {
            pieceList[i].InitPiece(i / mapSize, i % mapSize, jigsawData.glowMaterial, OnPieceDragStart, OnPieceDragEnd);
        }

        Observer.PlayAnimIdle?.Invoke();
    }

    private void OnEnable()
    {
        Observer.PieceInPlace += GameEvents_PieceInPlace;
        // SetUpStartGame();
    }

    private void OnDisable()
    {
        Observer.PieceInPlace -= GameEvents_PieceInPlace;
    }

    private void GameEvents_PieceInPlace(JigsawPiece glowPiece)
    {
        currentPieces++;
        StartGlow(glowPiece, currentPieces == numberOfPieces);
    }

    protected virtual void OnPieceDragStart()
    {
    }

    protected virtual void OnPieceDragEnd()
    {
    }

    private void StartGlow(JigsawPiece glowPiece, bool isWin = false)
    {
        var glowDistance = isWin ? mapSize : maxDistance;
        float timeEndGlow = 0;

        foreach (var t in pieceList)
        {
            if (!t.PieceIsComplete) continue;
            float len = DistanceBetween(glowPiece, t);
            if (!(len < glowDistance)) continue;
            t.GlowPiece((glowDistance - len) / glowDistance, len * delayGlowTime, glowDuration,
                stayGlowTime);
            timeEndGlow = Mathf.Max(timeEndGlow, len * delayGlowTime + glowDuration + stayGlowTime);
        }

        if (!isWin) return;
        //Observer.ShowFinalImage?.Invoke();
        DOTween.Sequence().AppendInterval(timeEndGlow).AppendCallback(CompleteElement);
    }

    private int DistanceBetween(JigsawPiece firstPiece, JigsawPiece secondPiece)
    {
        return Mathf.Max(Mathf.Abs(firstPiece.PieceColumn - secondPiece.PieceColumn),
            Mathf.Abs(firstPiece.PieceRow - secondPiece.PieceRow));
    }

    public void CompleteElement()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acShiny);

        finalImage.gameObject.SetActive(true);

        const float shinyDuration = 1.5f;
        DOTween.Sequence().AppendInterval(shinyDuration).AppendCallback(() =>
        {
            gameObject.SetActive(false);
            MapLevelManager.Instance.OnWin();
            Observer.PlayAnimWin?.Invoke();
            //Observer.CompleteElement?.Invoke(this);
        });
    }

    public void AddPiece(JigsawPiece newPiece)
    {
        pieceList.Add(newPiece);
    }

    public List<JigsawPiece> GetPieceList()
    {
        return pieceList;
    }

    public void SetUp(JigsawData jigsawData, Image finalImage)
    {
        this.jigsawData = jigsawData;
        this.finalImage = finalImage;
    }
}