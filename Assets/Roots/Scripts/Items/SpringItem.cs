using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;

public class SpringItem : BaseItem
{
    [SerializeField, Range(0.1f, 1)] private float radius;
    [SerializeField] private Transform springCenter;
    [SerializeField] private LayerMask springMask;
    [SerializeField] private SkeletonAnimation springSke;
    [SpineAnimation] public string pushAnimation;
    [SerializeField] private float delayTime;
    // private void OnCollisionEnter2D(Collision2D col)
    // {
    //     if (col.collider != null)
    //     {
    //         Debug.Log("ColSpribg");
    //         StartCoroutine(WaitToPush());
    //     }
    // }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject != null)
        {
            StartCoroutine(WaitToPush());
            var getBox = GetComponent<BoxCollider2D>();
            getBox.enabled = false;
        }
    }
    IEnumerator WaitToPush()
    {
        yield return new WaitForSeconds(delayTime);
        springSke.AnimationState.SetAnimation(0, pushAnimation, false);
        PlayActionAudio();
        yield return new WaitForSeconds(0.1f);
        PushTarget();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = ColorUtils.orange;
        Gizmos.DrawWireSphere(springCenter.position, radius);
    }
    void PushTarget()
    {
        var cols = Physics2D.OverlapCircleAll(springCenter.position, radius, springMask.value);
        var receivers = cols.Where(n => n.gameObject.name != "SearchCollider")
            .Select(c => c.GetComponentInParent<IPushAway>())
            .Where(r => r != null)
            .Distinct()
            .ToList();
        foreach (var receiver in receivers)
        {
            receiver.GetPushed(this);
        }
    }
}
