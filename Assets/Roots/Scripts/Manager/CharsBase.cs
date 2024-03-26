using UnityEngine;
using Spine.Unity;

public class CharsBase : MonoBehaviour, IExplodeReceiver
{
    #region properties

    public SkeletonAnimation skeleton;
    public EUnitState state;
    [SerializeField] protected GameObject skull;
    [SerializeField, SpineAnimation] protected string winAnimationName;
    [SerializeField, SpineAnimation] protected string win2AnimationName;
    [SerializeField, SpineAnimation] protected string loseAnimationName;
    [SerializeField, SpineAnimation] protected string freezyAnimationName;
    [SerializeField, SpineAnimation] protected string fireAnimationName;
    [SpineAnimation] public string idleAnimationName;
    [SpineAnimation] public string idleWonderWomanAnimationName;
    [SpineAnimation] public string attackWonderWomanAnimationName;
    [SpineAnimation] public string apearWonderWomanAnimationName;

    public bool IsTakeHolyWater { get; set; }
    #endregion
    
    #region function

    protected void PlayAnim(string nameAnim, bool isLoop) { skeleton.AnimationState.SetAnimation(0, nameAnim, isLoop); }

    public virtual void OnDie(bool effect)
    {
        if (GameManager.instance.gameState != EGameState.Win)
        {
            state = EUnitState.Die;
            PlayAnim(loseAnimationName, false);

            if (PlayerManager.instance != null) PlayerManager.instance.OnPlayerDie(EDieReason.Despair);
            MapLevelManager.Instance.OnLose();
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(SoundManager.Instance.acPrincessDie);
    }

    public void OnExplodedAt(BombItem bomb) { OnDie(true); }

    #endregion
}