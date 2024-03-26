using UnityEngine;

public class Saw : MonoBehaviour
{
    public float RotationSpeed;

    private void Update() { transform.Rotate(new Vector3(0, 0, 10) * RotationSpeed * Time.deltaTime); }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponentInParent<PlayerManager>();
        if (collision.CompareTag("BodyPlayer") && player != null && !player.IsTakeHolyWater)
        {
            if (GameManager.instance.gameState != EGameState.Win)
            {
                PlayerManager.instance.OnPlayerDie(EDieReason.Normal);
            }
            return;
        }
        
        var hostage = collision.GetComponentInParent<HostageManager>();
        if (hostage != null && hostage.CompareTag("Hostage") && hostage != null && !hostage.IsTakeHolyWater)
        {
            if (GameManager.instance.gameState != EGameState.Win)
            {
                HostageManager.instance.OnDie(true);
            }
        }
    }
}