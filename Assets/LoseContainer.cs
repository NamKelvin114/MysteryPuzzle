using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseContainer : MonoBehaviour
{
    // Start is called before the first frame update
    private Target previousTarget;
    public List<Target> Targetsl = new List<Target>();
    private void OnTriggerExit2D(Collider2D col)
    {
        var target = col.gameObject.GetComponentInParent<Target>();
        if (!Targetsl.Contains(target))
        {
            Targetsl.Add(target);
            MapLevelManager.Instance.OnTargetOut(target);
            previousTarget = target;
        }
    }
}
