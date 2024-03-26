using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTray : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tray;
    public SpriteRenderer Tray => tray;
    private float pointDistance = 1f;
    public float PointDistance => pointDistance;
    public float StartPos => tray.bounds.min.x + pointDistance * transform.localScale.x * 0.25f; 
    public float LastPos => tray.bounds.max.x - pointDistance * transform.localScale.x * 0.25f; 
    int _lastPointIndexOnScreen;

    public void Init(Camera cam)
    {
        transform.localScale = Vector3.one;
        float screenWidth = cam.orthographicSize * 2 * cam.aspect;
        float objectWidth = tray.bounds.size.x;
        float objectHeight = tray.bounds.size.y;
        float normalSize = 0.2f * Screen.safeArea.height * 2 * cam.orthographicSize / Screen.safeArea.height;
        transform.localScale = new Vector3(screenWidth / objectWidth, normalSize / objectHeight, 1);
        float worldDistance = 0.25f * Screen.safeArea.height * 2 * cam.orthographicSize / Screen.safeArea.height;
        transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y - worldDistance);
    }
}
