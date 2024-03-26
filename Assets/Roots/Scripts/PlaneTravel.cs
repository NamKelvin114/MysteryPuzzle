using System;
using UniRx;
using UnityEngine;

public class PlaneTravel : MonoBehaviour
{
    public float timeBezier;
    public Vector2 offsetAOnTop;
    public Vector2 offsetBOnTop;
    [Space] public Vector2 offsetAOnBottom;
    public Vector2 offsetBOnBottom;
    public RectTransform[] countries;
    private IDisposable _disposable;

    private Action _onCompleted;

    public void StartTravel(int nextCountry, Action onComplete)
    {
        _onCompleted = onComplete;
        switch (nextCountry)
        {
            case 1:
                transform.localScale = Vector3.one;
                DoBezier(0, 1, offsetAOnTop, offsetBOnTop);
                break;
            case 2:
                transform.localScale = new Vector3(-1, 1, 1);
                DoBezier(1,
                    2,
                    offsetAOnBottom,
                    offsetBOnBottom,
                    -180);
                break;
            case 3:
                transform.localScale = Vector3.one;
                DoBezier(2, 3, offsetAOnTop, offsetBOnTop);
                break;
            case 4:
                transform.localScale = Vector3.one;
                DoBezier(3, 4, offsetAOnTop, offsetBOnTop);
                break;
            case 5:
                transform.localScale = new Vector3(-1, 1, 1);
                DoBezier(4,
                    5,
                    offsetAOnBottom,
                    offsetBOnBottom,
                    -180);
                break;
            case 6:
                transform.localScale = Vector3.one;
                DoBezier(5, 6, offsetAOnTop, offsetBOnTop);
                break;
            case 7:
                transform.localScale = new Vector3(-1, 1, 1);
                DoBezier(6,
                    7,
                    offsetAOnBottom,
                    offsetBOnBottom,
                    -180);
                break;
            case 8:
                transform.localScale = Vector3.one;
                DoBezier(7, 8, offsetAOnTop, offsetBOnTop);
                break;
            case 9:
                transform.localScale = new Vector3(-1, 1, 1);
                DoBezier(8,
                    9,
                    offsetAOnBottom,
                    offsetBOnBottom,
                    -180);
                break;
            case 10:
                transform.localScale = Vector3.one;
                DoBezier(9, 10, offsetAOnTop, offsetBOnTop);
                break;
            case 11:
                transform.localScale = Vector3.one;
                DoBezier(10, 11, offsetAOnTop, offsetBOnTop);
                break;
        }
    }

    /// <summary>
    /// move item to character
    /// </summary>
    private void DoBezier(int currentCountry, int nextCountry, Vector2 offsetA, Vector2 offsetB, int a = 0)
    {
        _disposable?.Dispose();
        void SetNewPosition(Bezier localBezier, float f) { transform.localPosition = localBezier.GetPointAtTime(f / timeBezier); }

        transform.localPosition = countries[currentCountry].localPosition;
        //gameObject.SetActive(true);
        var controlA = transform.localPosition + new Vector3(offsetA.x, offsetA.y, 0);
        var controlB = countries[nextCountry].localPosition + new Vector3(offsetB.x, offsetB.y, 0);

        // Debug.Log("1: " + transform.localPosition);
        // Debug.Log("2: " + controlA);
        // Debug.Log("3: " + controlB);
        // Debug.Log("4: " + countries[nextCountry].localPosition);
        var bezier = new Bezier(transform.localPosition, controlA, controlB, countries[nextCountry].localPosition);

        float t = 0;
        _disposable = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                t += Time.unscaledDeltaTime;

                if (t >= timeBezier)
                {
                    t = timeBezier;
                    SetNewPosition(bezier, t);

                    //gameObject.SetActive(false);
                    _onCompleted?.Invoke();
                    _disposable?.Dispose();
                    _disposable = null;
                    return;
                }

                SetNewPosition(bezier, t);

                Vector3 dir = countries[nextCountry].localPosition - transform.localPosition;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                transform.localRotation = Quaternion.Euler(0, 0, angle + a);
            })
            .AddTo(this);
    }
}