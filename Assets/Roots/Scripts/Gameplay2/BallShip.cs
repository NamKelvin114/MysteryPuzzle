using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShip : MonoBehaviour, IExplodeReceiver
{
    // Start is called before the first frame update
    [SerializeField] GameObject fxCollection;
    [SerializeField] GameObject fx;
    public void OnExplodedAt(BombItem bomb)
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            gameObject.SetActive(false);
            Vector3 collisionPoint = col.contacts[0].point;
            Instantiate(fx, collisionPoint, Quaternion.identity);
        }
    }
}
