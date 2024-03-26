using UnityEngine;
public interface IExplodeReceiver
{
    void OnExplodedAt(BombItem bomb);
}

public interface IBombTrigger
{
    bool IsBombTriggering();
}
public interface IBeeTringger
{
    void OnGetBeeAttack();
}
public interface IPushAway
{
    void GetPushed(SpringItem spring);
}
public interface IFallByStone
{
    void GetStoneHit();
}