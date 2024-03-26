using UnityEngine;
using UnityTimer;

public class Cannon : BaseCannon, IDieByFallingStone, IExplodeReceiver
{
    protected override void Start()
    {
        base.Start();
        if (!isEnemy) return;
        if (MapLevelManager.Instance != null) MapLevelManager.Instance.allCannonEnemies.Add(this);
    }

    public void OnExplodedAt(
        BombItem bomb)
    {
        if (bomb && !IsDisable)
        {
            PlayDeathAnimation();
            DoBreak();
        }
    }
    
    protected override void PlayIdleAnimation()
    {
        handler.SkeletonAnimation.loop = true;
        handler.SkeletonAnimation.AnimationName = IsTakeHolyWater ? idleHolyWater : idleName;
    }

    protected override void PlayAttackAnimation()
    {
        if (handler.SkeletonAnimation != null)
        {
            handler.SkeletonAnimation.loop = false;
            handler.SkeletonAnimation.AnimationName = IsTakeHolyWater ? attackHolyWater : attackName;
        }
    }

    protected virtual void PlayApear()
    {
        handler.SkeletonAnimation.loop = false;
        handler.SkeletonAnimation.AnimationName = eatHolyWater;
    }

    public override void OnTakeHolyWater(Transform holyWater)
    {
        base.OnTakeHolyWater(holyWater);
        PlayApear();
        Timer.Register(handler.SkeletonAnimation.DurationAnimation(eatHolyWater), PlayIdleAnimation);
    }
}