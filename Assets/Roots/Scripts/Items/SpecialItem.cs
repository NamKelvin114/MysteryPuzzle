using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialItem : MonoBehaviour,IExplodeReceiver
{
    public void OnExplodedAt(BombItem bomb)
    {
        Destroy(gameObject);
    }
}
