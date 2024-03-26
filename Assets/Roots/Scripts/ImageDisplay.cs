using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplay : MonoBehaviour
{
    [SerializeField] private float duration = 2;
    private Image _image;
    private Image _imageChild;

    private IDisposable _disposable;

    public void Initialize()
    {
        _image = GetComponent<Image>();
        if (transform.childCount > 0)
        {
            _imageChild = transform.GetChild(0).GetComponent<Image>();
            _imageChild.fillAmount = 0;
        }
        _image.fillAmount = 0;
    }
    
    public void Play()
    {
        float t = 0;
        _disposable?.Dispose();
        _disposable = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                t += Time.deltaTime;
                if (t >= duration)
                {
                    t = duration;
                    _image.fillAmount = t / duration;
                    if (_imageChild != null)
                    {
                        _imageChild.fillAmount = _image.fillAmount;
                    }
                    _disposable?.Dispose();
                    return;
                }

                _image.fillAmount = (t / duration);
            })
            .AddTo(this);
    }
}