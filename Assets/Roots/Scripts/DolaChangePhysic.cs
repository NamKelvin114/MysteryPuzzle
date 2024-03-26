using System;
using System.Collections;
using System.Collections.Generic;
using Pancake.Tween;
using UnityEngine;
using DG.Tweening;
using Sequence = DG.Tweening.Sequence;

public class DolaChangePhysic : MonoBehaviour
{
    private void OnEnable()
    {
        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(2f).OnComplete(() =>
        {
            foreach (Transform dola in transform)
            {
                dola.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            }
        });
    }
}
