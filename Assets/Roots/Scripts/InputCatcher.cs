using UnityEngine;
using Lean.Touch;

public class InputCatcher : MonoBehaviour
{
    private static LeanFinger currentFinger;
    private static FingerSlicer Slicer => GameManager.instance.Root.Slicer;

    private void OnEnable()
    {
        StopSlicing();
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
        StopSlicing();
    }

    private void OnFingerDown(LeanFinger obj)
    {
        if (currentFinger == null && !obj.StartedOverGui)
        {
            currentFinger = obj;
            StartSlicing();
        }
    }

    private void OnFingerUp(LeanFinger obj)
    {
        if (obj == currentFinger)
        {
            currentFinger = null;
            StopSlicing();
        }
    }


    private void StartSlicing()
    {
        if (currentFinger == null) return;
        if (!Slicer) return;

        var cam = Camera.main;
        if (!cam) return;

        Slicer.StartSlicing();
        var mp = currentFinger.ScreenPosition;
        var wp = cam.ScreenToWorldPoint(mp);
        wp.z = 0;
        Slicer.MoveTo(wp, false);
    }

    private void StopSlicing()
    {
        if (Slicer != null) Slicer.StopSlicing();
    }


    private void Update()
    {
        if (currentFinger == null) return;
        if (!Slicer) return;

        var cam = Camera.main;
        if (!cam) return;

        var mp = currentFinger.ScreenPosition;
        var wp = cam.ScreenToWorldPoint(mp);
        wp.z = 0;
        Slicer.MoveTo(wp);
    }
}