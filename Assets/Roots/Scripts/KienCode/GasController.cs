using UnityEngine;

public class GasController : Unit
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BodyPlayer"))
        {
            if (PlayerManager.instance.IsTakeHolyWater) return;
            
            if (PlayerManager.instance.state == EUnitState.Playing || PlayerManager.instance.state == EUnitState.Running)
            {
                if (GameManager.instance.gameState != EGameState.Win)
                {
                    PlayerManager.instance.OnPlayerDie(EDieReason.Normal);
                }
            }
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        
        timeFly -= deltaTime;

        if (timeFly <= 0)
        {
            rid.velocity = transform.up * speedMove;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-360, 360)));
            timeFly = 1f;
        }
    }
}