using System;
using UnityEngine;

public class BigStoneItem : BaseItem, IFallingStone
{
    [SerializeField] private bool isGP2;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isGP2)
        {
            IDieByFallingStone obj = other.gameObject.GetComponentInParent<IDieByFallingStone>();


            if (obj != null)
            {
                var dragon = (BaseCannon)obj;
                if (!dragon.IsDisable)
                {
                    dragon.PlayDeathAnimation();
                    dragon.DoBreak();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isGP2)
        {
            var getTarget = col.gameObject.GetComponentInParent<IFallByStone>();
            if (getTarget != null)
                getTarget.GetStoneHit();
        }
    }
}
