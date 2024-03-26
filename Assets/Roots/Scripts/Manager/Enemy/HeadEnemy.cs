using UnityEngine;

public class HeadEnemy : MonoBehaviour
{
    public EnemyBase enemyBase;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyBase.IsTakeHolyWater) return;
        
        if (collision.gameObject.CompareTag(Utils.TAG_STONE) || collision.gameObject.CompareTag(Utils.TAG_CHEST) || collision.gameObject.CompareTag(Utils.TAG_SWORD) || collision.gameObject.CompareTag(Utils.TAG_ITEM)
            || collision.gameObject.CompareTag(Utils.TAG_DUMBBELL))
        {
            if (collision.CompareTag(Utils.TAG_STONE))
            {
                var bomb = collision.GetComponent<BombItem>();
                if (bomb != null && bomb.IsLeverActivated)
                {
                    return;
                }
            }
            enemyBase.OnDie(EDieReason.Normal);
        }
    }
}
