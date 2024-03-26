using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PiecesGenerator : MonoBehaviour
{
    [SerializeField] private JigsawData jigsawSO;
    [SerializeField] private float mapScale;
    [SerializeField] private Tracer tracerPrefab;

    public Material glowMaterial;
    public Sprite tracerSprite;
    public Sprite piecesBg4;
    public Sprite piecesBg9;
    public Sprite piecesBg16;

    public JigsawData GetJigsawSO()
    {
        return jigsawSO;
    }

    public float GetMapScale()
    {
        return mapScale;
    }

    public Tracer GetTracerPrefab()
    {
        return tracerPrefab;
    }

    public Sprite GetPieceBg(int numberOfPiece)
    {
        switch (numberOfPiece)
        {
            case 4:
                return piecesBg4;
            case 9:
                return piecesBg9;
            case 16:
                return piecesBg16;
            default:
                return null;
        }
    }
}