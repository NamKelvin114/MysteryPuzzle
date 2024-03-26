#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using Pancake;

[CustomEditor(typeof(JigsawData)), CanEditMultipleObjects]
public class JigsawDataEditor : Editor
{
    JigsawData jigsawSO;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        jigsawSO = (JigsawData)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Load Level Resources"))
        {
            LoadResources();

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }

    private static int GetNum(string s)
    {
        int x = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if ('0' <= s[i] && s[i] <= '9')
                x = x * 10 + (s[i] - '0');
        }

        return x;
    }

    private void LoadResources()
    {
        Sprite[] spriteList = Resources.LoadAll<Sprite>(jigsawSO.name);
        List<Sprite> imageList = new List<Sprite>();

        string pieceRegex = @"^\d+$";

        jigsawSO.spriteList.Clear();

        for (int i = 0; i < spriteList.Length; i++)
        {
            if (Regex.IsMatch(spriteList[i].name, pieceRegex))
            {
                jigsawSO.spriteList.Add(spriteList[i]);
            }
            else
            {
                imageList.Add(spriteList[i]);
            }
        }

        jigsawSO.spriteList = jigsawSO.spriteList.OrderBy(piece => GetNum(piece.name)).ToList();

        Sprite finalImage = imageList[0];

        // Detect check by "Final image" is square ot not
        for (int i = 0; i < imageList.Count; i++)
        {
            if (imageList[i].rect.width.RoundToInt() == imageList[i].rect.height.RoundToInt())
            {
                finalImage = imageList[i];
            }
        }

        // Detect check by "Final Image" have "_" in name
        // string finalImageRegex = @"_";
        // for (int i = 0; i < imageList.Count; i++)
        // {
        //     if ()
        // }

        jigsawSO.finalImage = finalImage;
        jigsawSO.baseSize = finalImage.rect.width / Mathf.Sqrt(jigsawSO.spriteList.Count);

        float biggerOffset = jigsawSO.spriteList[0].rect.width - jigsawSO.baseSize;
        float smallerOffset = jigsawSO.spriteList[0].rect.height - jigsawSO.baseSize;
        if (smallerOffset > biggerOffset)
        {
            float temp = smallerOffset;
            smallerOffset = biggerOffset;
            biggerOffset = temp;
        }

        jigsawSO.subSize = biggerOffset + smallerOffset;
        jigsawSO.subSizeOffset = jigsawSO.subSize - 2 * smallerOffset;
        jigsawSO.pieceTracerScale = Mathf.Sqrt(jigsawSO.spriteList.Count) / 4;

        EditorUtility.SetDirty(jigsawSO);
    }
}
#endif