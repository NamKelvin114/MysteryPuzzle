#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Linq;
using Pancake;
using Coffee.UIExtensions;
using UnityEditor.VersionControl;

[CustomEditor(typeof(PiecesGenerator)), CanEditMultipleObjects]
public class PiecesGeneratorEditor : Editor
{
    PiecesGenerator piecesGenerator;
    JigsawData jigsawSO;
    List<JigsawData> jigsawSOList = new List<JigsawData>();
    Tracer tracerPrefab;
    readonly string localPath = "Assets/Roots/DataStorages/JigsawData/";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        piecesGenerator = (PiecesGenerator)target;
        tracerPrefab = piecesGenerator.GetTracerPrefab();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Level"))
        {
            GenerateLevel();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void RemoveChildrenGameObject(Transform transform)
    {
        var children = transform.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject.GetComponent<JigsawController>();
            if (child != null)
                UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject, true);
        }
    }

    private void GenerateLevel()
    {
        jigsawSOList.Clear();
        RemoveChildrenGameObject(piecesGenerator.transform); //.RemoveChildrenGameObject();

        string[] guids = AssetDatabase.FindAssets(piecesGenerator.transform.name + " t:JigsawData",
            new String[] { localPath });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            JigsawData jigsawAsset = AssetDatabase.LoadAssetAtPath<JigsawData>(path);

            if (jigsawAsset.name == piecesGenerator.transform.name) jigsawSOList.Add(jigsawAsset);

            int lastSpaceIndex = jigsawAsset.name.LastIndexOf(' ');

            if (lastSpaceIndex != -1)
            {
                if (jigsawAsset.name.Substring(0, lastSpaceIndex) == piecesGenerator.transform.name)
                {
                    jigsawSOList.Add(jigsawAsset);
                }
            }

            jigsawSOList = jigsawSOList.OrderBy(jigsawAssetOrder =>
            {
                string jigSawName = jigsawAssetOrder.name;
                string numberString = jigSawName.Replace(piecesGenerator.transform.name, "").Trim();

                if (int.TryParse(numberString, out int result))
                {
                    return result;
                }

                return int.MinValue;
            }).ToList();
        }

        for (int i = 0; i < jigsawSOList.Count; i++)
        {
            jigsawSO = jigsawSOList[i];

            // Create ElementController 
            GameObject elementController = new GameObject(jigsawSO.name);
            RectTransform elementControllerRectTrans = elementController.AddComponent<RectTransform>();
            elementControllerRectTrans.SetParent(piecesGenerator.transform);
            elementControllerRectTrans.anchorMin = Vector2.zero;
            elementControllerRectTrans.anchorMax = Vector2.one;
            elementControllerRectTrans.offsetMin = Vector2.zero;
            elementControllerRectTrans.offsetMax = Vector2.zero;
            JigsawController elementControllerScript = elementController.AddComponent<JigsawController>();

            //level.ElementControllerList.Add(elementControllerScript);

            //Create Tracer Bar
            Tracer tracer = (Tracer)PrefabUtility.InstantiatePrefab(tracerPrefab, elementController.transform);
            tracer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -500);

            //Create Pieces Parent
            GameObject pieceHolder = new GameObject("Pieces");
            RectTransform pieceHolderRectTrans = pieceHolder.AddComponent<RectTransform>();
            pieceHolderRectTrans.SetParent(elementControllerScript.transform);
            pieceHolderRectTrans.anchoredPosition = new Vector2(0.0f, 200);
            Image pieceHolderImage = pieceHolder.AddComponent<Image>();
            pieceHolderImage.sprite = piecesGenerator.GetPieceBg(jigsawSO.spriteList.Count);
            pieceHolderImage.SetNativeSize();

            int mapSize = (int)Mathf.Sqrt(jigsawSO.spriteList.Count);

            RectTransform previousPiece = null;
            RectTransform startRowPiece = null;

            for (int index = 0; index < jigsawSO.spriteList.Count; index++)
            {
                // Create Piece
                int pieceRow = index / mapSize;
                int pieceColumn = index % mapSize;

                GameObject newPiece = new GameObject(jigsawSO.name + " - " + pieceRow + "x" + pieceColumn);
                RectTransform newPieceRectTrans = newPiece.AddComponent<RectTransform>();
                JigsawPiece newPieceScript = newPiece.AddComponent<JigsawPiece>();
                newPieceRectTrans.SetParent(pieceHolder.transform);

                // Create PieceShadow
                GameObject pieceShadow = new GameObject("PieceShadow");
                RectTransform pieceShadowRectTrans = pieceShadow.AddComponent<RectTransform>();
                pieceShadowRectTrans.SetParent(newPiece.transform);
                Image pieceShadowImage = pieceShadow.AddComponent<Image>();
                pieceShadowImage.sprite = jigsawSO.spriteList[index];
                pieceShadowImage.SetNativeSize();
                pieceShadowRectTrans.localScale = Vector3.one * 1.05f;
                pieceShadowRectTrans.anchoredPosition = new Vector2(30, -30);
                pieceShadowImage.raycastTarget = false;
                pieceShadowImage.maskable = false;
                Color shadowColor = Color.black;
                shadowColor.a = 0.6f;
                pieceShadowImage.color = shadowColor;
                pieceShadowRectTrans.anchorMin = Vector2.zero;
                pieceShadowRectTrans.anchorMax = Vector2.one;
                pieceShadowRectTrans.offsetMin = new Vector2(30, -30);
                pieceShadowRectTrans.offsetMax = new Vector2(30, -30);

                // Create PieceImage
                GameObject pieceVisual = new GameObject("PieceImage");
                RectTransform pieceImageRectTrans = pieceVisual.AddComponent<RectTransform>();
                pieceImageRectTrans.SetParent(newPiece.transform);
                pieceImageRectTrans.anchoredPosition = Vector2.zero;
                Image pieceVisualImage = pieceVisual.AddComponent<Image>();
                pieceVisualImage.sprite = jigsawSO.spriteList[index];
                pieceVisualImage.material = piecesGenerator.glowMaterial;
                pieceVisualImage.SetNativeSize();
                pieceVisualImage.raycastTarget = false;
                pieceVisualImage.maskable = false;
                pieceImageRectTrans.anchorMin = Vector2.zero;
                pieceImageRectTrans.anchorMax = Vector2.one;
                pieceImageRectTrans.offsetMin = Vector2.zero;
                pieceImageRectTrans.offsetMax = Vector2.zero;

                // SetupPieceSript
                newPieceRectTrans.sizeDelta =
                    new Vector2(jigsawSO.spriteList[index].rect.width, jigsawSO.spriteList[index].rect.height);
                newPieceRectTrans.pivot = new Vector2(0, 1);
                newPieceScript.pieceShadowImage = pieceShadowImage;
                newPieceScript.pieceVisualImage = pieceVisualImage;
                newPieceScript.pieceShadowImage.gameObject.SetActive(false);
                CalculatePieceOffset(newPieceScript, index, mapSize);
                ReCalculatePiecePivot(newPieceScript);

                elementControllerScript.AddPiece(newPieceScript);

                if (pieceColumn == 0)
                {
                    if (pieceRow == 0)
                    {
                        // newPieceRectTrans.anchoredPosition = Vector2.zero;
                        float xPos = jigsawSO.finalImage.rect.width / 2.0f - jigsawSO.baseSize / 2.0f;
                        float yPos = jigsawSO.finalImage.rect.height / 2.0f - jigsawSO.baseSize / 2.0f;
                        newPieceRectTrans.anchoredPosition = new Vector2(-xPos, yPos);
                    }
                    else
                    {
                        newPieceRectTrans.anchoredPosition = new Vector2(startRowPiece.anchoredPosition.x,
                            startRowPiece.anchoredPosition.y - jigsawSO.baseSize);
                    }

                    startRowPiece = newPieceRectTrans;
                }
                else
                {
                    newPieceRectTrans.anchoredPosition = new Vector2(
                        previousPiece.anchoredPosition.x + jigsawSO.baseSize, previousPiece.anchoredPosition.y);
                }

                previousPiece = newPieceRectTrans;
            }

            //Create Completed Parent
            GameObject completedHolder = new GameObject("Completed");
            RectTransform completedHolderRectTrans = completedHolder.AddComponent<RectTransform>();
            completedHolderRectTrans.SetParent(elementControllerScript.transform);
            completedHolderRectTrans.anchoredPosition = Vector2.zero;

            // Create Selected Parent
            GameObject selectedHolder = new GameObject("Selected");
            RectTransform selectedHolderRectTrans = selectedHolder.AddComponent<RectTransform>();
            selectedHolderRectTrans.SetParent(elementControllerScript.transform);
            selectedHolderRectTrans.anchoredPosition = Vector3.zero;

            // Create Final Image
            GameObject finalImageHolder = new GameObject("FinalImage");
            RectTransform finalImageRectTrans = finalImageHolder.AddComponent<RectTransform>();
            finalImageRectTrans.SetParent(elementControllerScript.transform);
            finalImageRectTrans.anchoredPosition = pieceHolderRectTrans.anchoredPosition;
            Image finalImage = finalImageHolder.AddComponent<Image>();
            var finalImageUIShiny = finalImageHolder.AddComponent<UIShiny>();
            finalImageUIShiny.effectFactor = 0.0f;
            finalImageUIShiny.brightness = 0.5f;
            finalImageUIShiny.Play();
            finalImage.sprite = jigsawSO.finalImage;
            finalImage.SetNativeSize();
            finalImageHolder.SetActive(false);

            // Setup For Level
            elementControllerScript.SetUp(jigsawSO, finalImage);

            // Setup For Pieces
            List<JigsawPiece> pieceList = elementControllerScript.GetPieceList();
            for (int index = 0; index < pieceList.Count; index++)
            {
                pieceList[index].PieceSetup(completedHolder.transform, selectedHolder.transform,
                    jigsawSO.pieceTracerScale);
            }
        }
    }

    public void CalculatePieceOffset(JigsawPiece piece, int index, int mapSize)
    {
        float pieceHeight = piece.GetComponent<RectTransform>().rect.height;
        float pieceWidth = piece.GetComponent<RectTransform>().rect.width;

        int pieceRow = index / mapSize;
        int piececColumn = index % mapSize;

        int subElementValue = (pieceHeight - jigsawSO.baseSize >= jigsawSO.subSizeOffset) ? 1 : -1;

        if (pieceRow == 0)
        {
            piece.bottom = subElementValue;
        }
        else if (pieceRow == mapSize - 1)
        {
            piece.top = subElementValue;
        }
        else
        {
            piece.top = piece.bottom = subElementValue;
        }

        subElementValue = (pieceWidth - jigsawSO.baseSize >= jigsawSO.subSizeOffset) ? 1 : -1;

        if (piececColumn == 0)
        {
            piece.right = subElementValue;
        }
        else if (piececColumn == mapSize - 1)
        {
            piece.left = subElementValue;
        }
        else
        {
            piece.right = piece.left = subElementValue;
        }
    }

    public void ReCalculatePiecePivot(JigsawPiece piece)
    {
        float xPivot = 0;
        float yPivot = 0;
        float smallOffset = (jigsawSO.subSize - jigsawSO.subSizeOffset) / 2.0f;
        RectTransform pieceRect = piece.GetComponent<RectTransform>();

        if (piece.top - piece.bottom == 0)
        {
            pieceRect.pivot = new Vector2(pieceRect.pivot.x, 0.5f);
        }
        else
        {
            if (piece.bottom > 0)
            {
                yPivot = jigsawSO.subSize - smallOffset + jigsawSO.baseSize / 2.0f;
            }
            else
            {
                yPivot = -1 * piece.bottom * smallOffset + jigsawSO.baseSize / 2.0f;
            }

            pieceRect.pivot = new Vector2(pieceRect.pivot.x, yPivot / pieceRect.rect.height);
        }

        if (piece.left - piece.right == 0)
        {
            pieceRect.pivot = new Vector2(0.5f, pieceRect.pivot.y);
        }
        else
        {
            if (piece.left > 0)
            {
                xPivot = jigsawSO.subSize - smallOffset + jigsawSO.baseSize / 2.0f;
            }
            else
            {
                xPivot = -1 * piece.left * smallOffset + jigsawSO.baseSize / 2.0f;
            }

            pieceRect.pivot = new Vector2(xPivot / pieceRect.rect.width, pieceRect.pivot.y);
        }
    }
}
#endif