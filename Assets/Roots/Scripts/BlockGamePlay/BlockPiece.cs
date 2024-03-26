using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockPiece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer pieceShadow;
    private int initialImageSortingOrder = 1;
    [SerializeField] private SpriteRenderer pieceImage;

    [Header("UseForEditor")] [HideInInspector]
    public int pieceNumber; // only for debugging

    [SerializeField] private Cell cell; // cell prefab for generate

    [SerializeField] private List<Cell> listCell = new List<Cell>();
    [SerializeField] private PieceState pieceState = PieceState.NotPlaced;
    [SerializeField] private PolygonCollider2D pieceTrigger;
    [SerializeField] private Vector3 startPosition;

    public List<Cell> ListCell => listCell;
    public PieceState PieceState => pieceState;
    public PolygonCollider2D PieceTrigger => pieceTrigger;
    public SpriteRenderer PieceImage => pieceImage;
    public Vector2 Center => pieceImage.bounds.center;

    public Vector3 StartPosition => startPosition;
    //public SpriteRenderer Image;

    public float OffsetDragY => (transform.position - pieceImage.bounds.center).y + pieceImage.bounds.extents.y;

    public float LeftRange => transform.position.x - pieceImage.bounds.min.x;
    public float RightRange => pieceImage.bounds.max.x - transform.position.x;

    void Start()
    {
        pieceImage.sortingLayerName = "UI";
        pieceImage.sortingOrder = initialImageSortingOrder;
        pieceImage.gameObject.AddComponent<BoxCollider2D>();
        SetupShadow();
    }

    void SetupShadow()
    {
        pieceShadow.transform.localScale = pieceImage.transform.localScale;
        pieceShadow.sprite = pieceImage.sprite;
        pieceShadow.sortingOrder = initialImageSortingOrder - 1;
        pieceShadow.gameObject.SetActive(false);
    }

    public void Dragging(bool canPlace, Vector3 shadowPos)
    {
        pieceShadow.gameObject.SetActive(canPlace);
        pieceShadow.transform.position = shadowPos;
    }

    public void BeginDrag()
    {
        pieceState = PieceState.NotPlaced;
        // Observer.TakeBlock?.Invoke();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acTakeBlock);
        pieceImage.sortingOrder = initialImageSortingOrder + 1;
    }

    public void EndDrag()
    {
        pieceShadow.gameObject.SetActive(false);
        pieceImage.sortingOrder = initialImageSortingOrder;
    }

    public void PlacePiece()
    {
        pieceState = PieceState.Placed;
    }

    public void ReturnPiece()
    {
        pieceState = PieceState.NotPlaced;
    }

    public void CalcStartPosition() // used to calculate center
    {
        startPosition = transform.position;
    }

#if UNITY_EDITOR

    public void UpdateCell()
    {
        Cell[] newListCell = GetComponentsInChildren<Cell>(true);
        listCell = new List<Cell>();
        foreach (Cell item in newListCell)
        {
            var cellPosition = item.transform.position;
            int x = Mathf.RoundToInt(cellPosition.x);
            int y = Mathf.RoundToInt(cellPosition.y);
            int z = Mathf.RoundToInt(cellPosition.z);
            cellPosition = new Vector3(x, y, z);
            item.transform.position = cellPosition;
            item.Text.text = pieceNumber.ToString();
            listCell.Add(item);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public int Round(float x, float c)
    {
        return Mathf.RoundToInt((x - c) * 1.1f);
    }

    public void GenerateCell()
    {
        Cell[] newListCell = GetComponentsInChildren<Cell>(true);
        listCell = new List<Cell>();
        for (int i = newListCell.Length - 1; i >= 0; i--)
        {
            DestroyImmediate(newListCell[i].gameObject);
        }

        Bounds bounds = pieceImage.bounds;
        float left = bounds.center.x - bounds.extents.x;
        float right = bounds.center.x + bounds.extents.x;
        float top = bounds.center.y + bounds.extents.y;
        float bottom = bounds.center.y - bounds.extents.y;
        var position = transform.position;
        Vector2 bottomLeft = new Vector2(Round(bottom, position.y), Round(left, position.x));
        Vector2 topRight = new Vector2(Round(top, position.y), Round(right, position.x));
        for (int j = (int)bottomLeft.y; j < topRight.y; j++)
        {
            for (int k = (int)bottomLeft.x; k < topRight.x; k++)
            {
                var item1 = (Cell)PrefabUtility.InstantiatePrefab(cell, transform);
                item1.transform.localPosition = new Vector3(j, k, 0);
                item1.Text.text = pieceNumber.ToString();
                listCell.Add(item1);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public void AddBox()
    {
        while (pieceImage.gameObject.GetComponent<PolygonCollider2D>() != null)
        {
            DestroyImmediate(pieceImage.gameObject.GetComponent<PolygonCollider2D>());
        }

        pieceTrigger = pieceImage.gameObject.AddComponent<PolygonCollider2D>();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}