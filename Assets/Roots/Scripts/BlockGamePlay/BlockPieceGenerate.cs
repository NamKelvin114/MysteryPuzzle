using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BlockPieceGenerate : MonoBehaviour
{
    [SerializeField] private BlockPiece piece;
    [SerializeField] private List<Sprite> listPieceSprite;
    [SerializeField] private BlockController blockController;

    [SerializeField] SpriteRenderer frameImage;
    [SerializeField] Vector2 spawnPiecesOffset;
    public List<BlockPiece> ListPiece;
    [ContextMenu("Find path")]
    public void FindAsset()
    {

    }
#if UNITY_EDITOR
    string getIndex(string s)
    {
        string sum = "";
        for (int i = 0; i < s.Length; i++)
        {
            if ('0' <= s[i] && s[i] <= '9')
            {
                sum += s[i];
            }
        }

        return sum;
    }

    public void GeneratePiece()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        listPieceSprite = new List<Sprite>();
        string regex = @"\w+\s*\(\d+\)$";
        // string regex2 = @"\w+\s*_+\d+$";

        var folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(frameImage.sprite));
        var directoryInfo = new DirectoryInfo(folderPath);
        if (directoryInfo.Exists)
        {
            var fileInfos = directoryInfo.GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                var s = fileInfo.FullName.IndexOf("Assets", System.StringComparison.Ordinal);
                var path = fileInfo.FullName.Substring(s);
                var sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
                if (sprite && Regex.IsMatch(sprite.name, regex))
                {
                    listPieceSprite.Add(sprite);
                }
            }
        }

        List<BlockPiece> pieces = new List<BlockPiece>();
        int piecesPerEdge = Mathf.CeilToInt(listPieceSprite.Count * 1.0f / 4);
        Vector2 maxHalfSizeOfAPiece = Vector2.zero;
        Vector2 imageHalfSize = frameImage.bounds.size * 0.5f;
        List<Vector2> edges = new List<Vector2>()
            { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

        for (int i = 0; i < listPieceSprite.Count; i++)
        {
            var index = i;

            var piece = (BlockPiece)PrefabUtility.InstantiatePrefab(this.piece, transform);
            piece.PieceImage.sprite = listPieceSprite[i];
            piece.pieceNumber = index;
            piece.PieceImage.transform.localPosition = new Vector3(0, 0, 0);
            maxHalfSizeOfAPiece = new Vector2(Mathf.Max(maxHalfSizeOfAPiece.x, piece.PieceImage.bounds.size.x),
                Mathf.Max(maxHalfSizeOfAPiece.y, piece.PieceImage.bounds.size.y));

            pieces.Add(piece);

            piece.GenerateCell();
        }

        maxHalfSizeOfAPiece *= 0.5f;


        Vector2 maxWidthHeight = maxHalfSizeOfAPiece * 2 * piecesPerEdge + spawnPiecesOffset * (piecesPerEdge - 1) -
                                 maxHalfSizeOfAPiece;
        Vector2 maxDistanceUpperRightBetweenEgdes = maxHalfSizeOfAPiece * 2 + spawnPiecesOffset;
        Vector2 maxUpperRightFrameImage = imageHalfSize + spawnPiecesOffset + maxHalfSizeOfAPiece;

        for (int i = 0; i < 4; i++)
        {
            Vector2 centerOfEdge = (Vector2)frameImage.bounds.center + edges[i] * maxUpperRightFrameImage;
            Vector2 startPos = centerOfEdge - new Vector2(maxWidthHeight.x * (1 - Mathf.Abs(edges[i].x)),
                                   maxWidthHeight.y * (1 - Mathf.Abs(edges[i].y))) * 0.5f
                               + new Vector2(edges[i].x * maxHalfSizeOfAPiece.x, edges[i].y * maxHalfSizeOfAPiece.y);

            for (int j = 0; j < piecesPerEdge; j++)
            {
                int pieceIndex = i * piecesPerEdge + j;

                if (pieceIndex >= listPieceSprite.Count) break;

                pieces[pieceIndex].transform.position = startPos + j * new Vector2(
                    maxDistanceUpperRightBetweenEgdes.x * (1 - Mathf.Abs(edges[i].x)),
                    maxDistanceUpperRightBetweenEgdes.y * (1 - Mathf.Abs(edges[i].y)));
            }
        }

        blockController.UpdatePiece();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public void AddBox()
    {
        BlockPiece[] listBlockPiece = GetComponentsInChildren<BlockPiece>(true);
        foreach (var blockPiece in listBlockPiece)
        {
            blockPiece.AddBox();
        }

        while (frameImage.gameObject.GetComponent<PolygonCollider2D>() != null)
        {
            DestroyImmediate(frameImage.gameObject.GetComponent<PolygonCollider2D>());
        }

        blockController.SetupBox(frameImage.gameObject.AddComponent<PolygonCollider2D>());
        EditorUtility.SetDirty(this);
    }

    public void RoundUp()
    {
        BlockPiece[] listBlockPiece = GetComponentsInChildren<BlockPiece>(true);
        ListPiece.Clear();
        foreach (var blockPiece in listBlockPiece)
        {
            ListPiece.Add(blockPiece);
            int X = Mathf.RoundToInt(blockPiece.transform.position.x);
            int Y = Mathf.RoundToInt(blockPiece.transform.position.y);
            Debug.Log(X + " " + Y);
            blockPiece.transform.position = new Vector3(X, Y, 0);
            var item = blockPiece.PieceImage.transform;
            blockPiece.PieceImage.transform.localPosition = new Vector3(0, 0, 0);
            blockPiece.UpdateCell();
        }

        int x = Mathf.RoundToInt(frameImage.gameObject.transform.position.x);
        int y = Mathf.RoundToInt(frameImage.gameObject.transform.position.y);
        frameImage.gameObject.transform.position = new Vector3(x, y, 0);
        blockController.UpdatePiece();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(BlockPieceGenerate))]
public class CustomSpawnBlockPieceEditor : Editor
{
    BlockPieceGenerate spawn;
    private void OnEnable()
    {
        spawn = target as BlockPieceGenerate;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Generate", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            spawn.GeneratePiece();
        }
        if (GUILayout.Button("Update", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            spawn.RoundUp();
        }
        if (GUILayout.Button("AddBox", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            spawn.AddBox();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif