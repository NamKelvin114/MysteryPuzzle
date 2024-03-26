using System.Collections;
using System.Collections.Generic;
using PlayFab.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CameraResizer : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Cam;
    public void Init(Bounds bound)
    {
        // Bounds bound = LevelController.Instance.currentLevel.FrameImage.GetComponentInChildren<SpriteRenderer>().bounds;
        float objectHeight = bound.size.y;
        float objectWidth = bound.size.x;
        float screenHeightInWorldSpace = Cam.orthographicSize * 2.0f;
        float screenWidthInWorldSpace = screenHeightInWorldSpace * Cam.aspect;
        float objectHeightInScreenSpace = objectHeight / screenHeightInWorldSpace * Screen.safeArea.height;
        float objectWidthInScreenSpace = objectWidth / screenWidthInWorldSpace * Screen.safeArea.width;
        float targetHeightInScreenSpace = Screen.safeArea.height * 0.4f;
        float targetWidthInScreenSpace = Screen.safeArea.width * 0.75f;
        float newOrthographicSize = Cam.orthographicSize / targetHeightInScreenSpace * objectHeightInScreenSpace;
        float newOrthographicSize1 = Cam.orthographicSize / targetWidthInScreenSpace * objectWidthInScreenSpace;
        float newOrtho = Mathf.Max(newOrthographicSize, newOrthographicSize1);
        Cam.orthographicSize = newOrtho;
        float worldDistance = 0.1f * Screen.safeArea.height * 2 * Cam.orthographicSize / Screen.safeArea.height;
        Cam.transform.position = new Vector3(bound.center.x, bound.center.y - worldDistance, Cam.transform.position.z);
    }
}
