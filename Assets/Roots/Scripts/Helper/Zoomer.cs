using DG.Tweening;
using UnityEngine;

public class Zoomer : MonoBehaviour
{
    public float time = 0.35f;
    public float delay = 0;
    
    private Vector3 _cachedScale;

    private void OnEnable()
    {
        DOTween.Kill(this);
        _cachedScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(_cachedScale, time)
            .SetDelay(delay)
            .SetId(this);
    }

    private void OnDisable()
    {
        if (DOTween.Kill(this) > 0)
        {
            transform.localScale = _cachedScale;
        }
    }
}