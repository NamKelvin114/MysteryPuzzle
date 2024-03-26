using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Clothe : Target
{
    // Start is called before the first frame update
    float speedMove = 5f;
    [SerializeField] private float scale = .5f;
    private Vector3 scaleDefaut = Vector3.one;
    private Rigidbody2D rig;
    void Start()
    {
        rig = this.GetComponentInChildren<Rigidbody2D>();
        scaleDefaut = Vector3.one * scale;
    }

    // Update is called once per frame
    public void Applly(Vector3 newPos, Action ActionCompleted = null)
    {
        this.GetComponent<SpriteRenderer>().sortingLayerName = "Popup";
        this.transform.DOKill();
        rig.simulated = false;
        var distane = (newPos - this.transform.position).magnitude;
        this.transform.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);
        this.transform.DOScale(scaleDefaut * 1.5f, 1f).SetEase(Ease.OutBack).OnComplete(() => 
        {
            this.transform.DOMove(newPos, distane / speedMove).OnComplete(() => ActionCompleted?.Invoke());
            this.transform.DOScale(scaleDefaut, distane / speedMove);
        });
    }
}
