using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class Target : MonoBehaviour, IExplodeReceiver
{
    public TargetType TargetType;
    public ExpectedType ExpectedType;
    public void OnExplodedAt(BombItem bomb)
    {
        gameObject.SetActive(false);
        MapLevelManager.Instance.OnTargetOut(this);
    }
}

