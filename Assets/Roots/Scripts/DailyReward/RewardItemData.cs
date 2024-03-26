using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Reward Item Data", order = 1)]
public class RewardItemData: ScriptableObject
{
    public List<DailyItemData> listDailyItemData;
}

[System.Serializable]
public class DailyItemData
{
    public enum DayType
    {
        DAY1,
        DAY2,
        DAY3,
        DAY4,
        DAY5,
        DAY6,
        DAY7
    }
    public enum ItemType
    {
        COIN,
        DRESS
    }

    public DayType day;
    public ItemType type;
    public int coin;
}
