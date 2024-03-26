public class EnemyRange : EnemyBase
{
    protected override void InitSoundAttack()
    {
        if (SoundManager.Instance != null)
        {
            attackAudio = SoundManager.Instance.audioEnemyShoot;
        }
    }
}
