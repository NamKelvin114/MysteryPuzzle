using System;
using Spine.Unity;
using UnityEngine;
using UnityTimer;

public class PetFollowerDance : MonoBehaviour
{
    public SkeletonGraphic skeleton;
    public Transform target;
    public float idleTime;

    private float _tempScale;
    private bool _flagChangeDirection;
    private Timer _timer;
    private float _currentTime;

    private void Update()
    {
        if (_flagChangeDirection)
        {
            if (_currentTime < idleTime)
            {
                _currentTime += Time.deltaTime;
                transform.localPosition += new Vector3(Time.deltaTime * 100 * (transform.localScale.x > 0 ? -1 : 1), 0, 0);
            }
            else
            {
                _currentTime = 0;
                _flagChangeDirection = false;
                // skeleton.startingLoop = true;
                // skeleton.AnimationState.SetAnimation(0, "Walk", true);
                transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * (target.transform.localScale.x > 0 ? 1 : -1),
                    transform.localScale.y,
                    transform.localScale.z);
            }

            return;
        }

        if (target.localScale.x > 0)
        {
            if (transform.localScale.x < 0)
            {
                ChangeDirection();
            }
        }
        else if (target.localScale.x < 0)
        {
            if (transform.localScale.x > 0)
            {
                ChangeDirection();
            }
        }

        transform.localPosition += new Vector3(Time.deltaTime * 100 * (transform.localScale.x > 0 ? 1 : -1.8f), 0, 0);
    }

    public void ChangeDirection() { _flagChangeDirection = true; }
}