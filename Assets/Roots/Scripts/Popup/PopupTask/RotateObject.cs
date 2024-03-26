using DG.Tweening;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float _anglesPerSecond = 90;
    public bool isRotate = false;

    void Update()
    {
        if (isRotate)
        {
            Vector3 rotation = transform.localEulerAngles;
            rotation.z += _anglesPerSecond;
            transform.localEulerAngles = rotation;
        }
    }
}