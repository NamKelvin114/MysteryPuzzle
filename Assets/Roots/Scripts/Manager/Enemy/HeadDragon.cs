using UnityEngine;

public class HeadDragon : MonoBehaviour
{
    public BaseCannon enemyBase;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Utils.TAG_STONE) || collision.gameObject.CompareTag(Utils.TAG_CHEST) || collision.gameObject.CompareTag(Utils.TAG_ITEM))
        {
            if (collision.CompareTag(Utils.TAG_STONE))
            {
                var bomb = collision.GetComponent<BombItem>();
                if (bomb != null && bomb.IsLeverActivated)
                {
                    return;
                }
            }
            
            if (!enemyBase.IsDisable)
            {
                enemyBase.PlayDeathAnimation();
                enemyBase.DoBreak();
            }
        }
    }
}