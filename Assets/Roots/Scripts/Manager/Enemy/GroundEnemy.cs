using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public EnemyBase enemyBase;
    public bool dieByFire = true;
    public bool dieByIce = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyBase.IsTakeHolyWater) return;
        
        if (collision.gameObject.CompareTag("Trap_Lava"))
        {
            if (dieByFire) enemyBase.OnDie(EDieReason.Fire);
        }
        else if (collision.gameObject.CompareTag("Ice"))
        {
            if (dieByIce) enemyBase.OnDie(EDieReason.Ice);
        }
    }
}
