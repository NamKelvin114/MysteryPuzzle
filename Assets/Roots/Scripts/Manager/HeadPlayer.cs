using System;
using Pancake;
using UnityEngine;

public class HeadPlayer : MonoBehaviour
{
    [SerializeField] private PlayerManagerGameplay2 playerManagerGameplay2;
    [SerializeField] private BoxCollider2D boxPot;
    [SerializeField] private PlayerManager pPlayer;

    private void Start()
    {
        if (!MapLevelManager.Instance.isGameplay1)
            boxPot.enabled = playerManagerGameplay2.PotActive();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!MapLevelManager.Instance.isGameplay1)
        {
            if (playerManagerGameplay2.IsTakeHolyWater) return;
            if (collision.gameObject.name == "Sword")
            {
                if (GameManager.instance.gameState != EGameState.Lose) playerManagerGameplay2.OnPlayerDie(EDieReason.Normal);
            }

            if (collision.gameObject.CompareTag(Utils.TAG_STONE))
            {
                if (GameManager.instance.gameState != EGameState.Lose) playerManagerGameplay2.OnPlayerDie(EDieReason.Normal);
            }
            if (collision.gameObject.CompareTag(Utils.TAG_LAVA))
            {
                if (GameManager.instance.gameState != EGameState.Lose) playerManagerGameplay2.OnPlayerDie(EDieReason.Fire);
            }
        }
        else
        {
            if (pPlayer.IsTakeHolyWater) return;
            if (collision.gameObject.CompareTag(Utils.TAG_STONE) || collision.gameObject.name == "Sword")
            {
                if (GameManager.instance.gameState != EGameState.Lose) pPlayer.OnPlayerDie(EDieReason.Normal);
            }

            if (collision.gameObject.CompareTag(Utils.TAG_LAVA))
            {
                if (GameManager.instance.gameState != EGameState.Lose) pPlayer.OnPlayerDie(EDieReason.Fire);
            }
        }

    }


    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Tag_Stone"))
        {
            if (GameManager.instance.gameState != EGameState.Lose) pPlayer.OnPlayerDie(EDieReason.Normal);
        }
    }
}
