using UnityEngine;

public class EnemyGhost : EnemyBase
{
    [SerializeField] private GhostAttack ghostAttack;
    public override void FixedUpdate() { }

    public override void OnPrepareAttack() { }
    public override void OnMoveToTarget() { }

    public override void Start()
    {
        InitSoundAttack();
        if (MapLevelManager.Instance != null)
        {
            MapLevelManager.Instance.lstAllEnemies.Add(this);
        }
    }

    protected override void OnDrawGizmos() { }

    public override void OnDie(EDieReason dieReason)
    {
        if (_charStage != CHAR_STATE.DIE)
        {
            ghostAttack.CancleAfterAttack();
            rig.gravityScale = 1;
            switch (dieReason)
            {
                case EDieReason.Normal:
                    PlayAnim(dieAnimation, false);
                    break;
                case EDieReason.Fire:
                    PlayAnim(dieFireAnimation, false);
                    break;
                case EDieReason.Ice:
                    PlayAnim(dieIceAnimation, false);
                    break;
            }
            _charStage = CHAR_STATE.DIE;
            rig.velocity = Vector2.zero;
            rig.constraints = RigidbodyConstraints2D.FreezePositionX;
            transform.rotation = Quaternion.identity;
            rig.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.SetActive(false);
            head.SetActive(false);

            gGround.gameObject.layer = LayerMask.NameToLayer("DeadBody");
            
            GameManager.instance.EnemyKill++;
            MapLevelManager.Instance.lstAllEnemies.Remove(this);

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.acEnemyDie);
            }

            if (MapLevelManager.Instance.eQuestType == EQuestType.Kill && MapLevelManager.Instance.lstAllEnemies.Count == 0 && MapLevelManager.Instance.allCannonEnemies.Count == 0)
            {
                PlayerManager.instance.OnWin(false);
            }
        }

        if (ghostAttack.gameObject.activeSelf) ghostAttack.gameObject.SetActive(false);
    }
}