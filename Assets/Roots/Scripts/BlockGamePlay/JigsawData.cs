using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/JigsawData")]
public class JigsawData : ScriptableObject
{
    public List<Sprite> spriteList;
    public Sprite finalImage;
    public float baseSize = 201f;
    public float subSize;
    public float subSizeOffset;
    public float pieceTracerScale;
    public Material glowMaterial;
}