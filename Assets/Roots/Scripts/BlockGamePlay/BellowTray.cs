using UnityEngine;

public class BellowTray : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tray;
    public SpriteRenderer Tray => tray;
    private float _pointDistance = 1f;
    public float PointDistance => _pointDistance;
    public float StartPos => tray.bounds.min.x + _pointDistance * transform.localScale.x * 0.25f; 
    public float LastPos => tray.bounds.max.x - _pointDistance * transform.localScale.x * 0.25f; 

    public void Init(Camera cam)
    {
        tray.sortingLayerName = "UI";
        var localScale = transform.localScale;
        float screenWidth = cam.orthographicSize * 2 * cam.aspect;
        float objectWidth = tray.bounds.size.x;
        float objectHeight = tray.bounds.size.y;
        var orthographicSize = cam.orthographicSize;
        float normalSize = 0.2f * Screen.safeArea.height * 2 * orthographicSize / Screen.safeArea.height;
        localScale = new Vector3(screenWidth / objectWidth, normalSize / objectHeight, 1);
        transform.localScale = localScale;
        float worldDistance = 0.25f * Screen.safeArea.height * 2 * orthographicSize / Screen.safeArea.height;
        var position = cam.transform.position;
        transform.position = new Vector2(position.x, position.y - worldDistance);
        tray.sortingOrder = 0;
    }
}