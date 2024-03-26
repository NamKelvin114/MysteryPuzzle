using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    private Vector3 originalPosition;
    private bool isShaking = false;

    public void Shake()
    {
        if (!isShaking)
            StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        originalPosition = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector3(x, y, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
        isShaking = false;
    }
}
