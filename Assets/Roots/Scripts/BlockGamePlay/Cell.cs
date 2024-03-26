using TMPro;
using UnityEngine;
using Utilities;

public class Cell : MonoBehaviour
{
    public Transform darkCell;
    public Renderer Renderer;
    public TextMeshPro Text;
    public BoxCollider2D CellBox;

    private void OnEnable()
    {
        darkCell.gameObject.SetActive(false);
        Text.gameObject.SetActive(false);
        CellBox.enabled = false;
    }
}