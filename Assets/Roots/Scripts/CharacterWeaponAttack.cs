using UnityEngine;

public class CharacterWeaponAttack : MonoBehaviour
{
    private HostageManager _hostage;
    private EnemyBase _enemy;

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (PlayerManager.instance.state == EUnitState.Die)
        {
            return;
        }

        _enemy = other.gameObject.GetComponent<EnemyBase>();
        if (_enemy != null) _enemy.OnDie(EDieReason.Normal);
    }
}