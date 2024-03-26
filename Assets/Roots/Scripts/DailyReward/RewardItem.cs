#pragma warning disable 649
using Spine.Unity;
using TMPro;
using UnityEngine;
using Worldreaver.Timer;

public class RewardItem : MonoBehaviour
{
    [SerializeField] private PopupDailyReward dailyReward;
    [SerializeField] private GameObject avaiableItem;
    [SerializeField] private DailyItemData _dailyItemData;
    [SerializeField] private SkeletonGraphic _skeletonGraphic;
    [SerializeField, SpineAnimation(dataField = "giftBox")]
    private string actionIdle0;
    [SerializeField, SpineAnimation(dataField = "giftBox")]
    private string actionIdle1;

    public GameObject tick;
    public GameObject iconCoin;
    public int dayIndex;
    public TextMeshProUGUI txtDay;
    public DailyItemData.DayType day;
    public TMP_Text coinValue;
    public DailyItemData DailyItemData { get => _dailyItemData; set => _dailyItemData = value; }
    public bool CanClaim = false;

    public void DisplayAgain()
    {
        if (_skeletonGraphic != null && _skeletonGraphic.AnimationState != null)
        {
            _skeletonGraphic.AnimationState.AddAnimation(0, actionIdle0, true, 0);
        }
        
        //set coin value belong to dailyItemData
        if (coinValue != null)
        {
            coinValue.SetText(_dailyItemData.coin.ToString());
        }

        if (dayIndex == Utils.curDailyGift && !Utils.cantakegiftdaily && !Utils.IsClaimReward())
        {
            if (iconCoin != null)
            {
                iconCoin.SetActive(true);
            }
            if (Utils.curDailyGift == 7)
            {
                Observer.AddFromPosiGenerationCoin?.Invoke(gameObject);
            }

            dailyReward.Day = dayIndex;
            if (avaiableItem != null)
            {
                avaiableItem.SetActive(true);
            }
            if (_skeletonGraphic != null)
            {
                _skeletonGraphic.AnimationState.AddAnimation(0, actionIdle1, true, 0);
            }
            tick.SetActive(false);
            dailyReward.BtnClaimByVideo.gameObject.SetActive(dayIndex != 7);
            // dailyReward.BtnClaim.gameObject.SetActive(true);
            // // dailyReward.CanClaim = true;
            CanClaim = true;
        }
        else if (dayIndex == Utils.curDailyGift && Utils.cantakegiftdaily)
        {
            dailyReward.BtnClaimByVideo.gameObject.SetActive(false);
            tick.SetActive(false);
            if (iconCoin != null)
            {
                iconCoin.SetActive(true);
            }
            // dailyReward.BtnClaim.gameObject.SetActive(false);
            // // dailyReward.CanClaim = false;
            CanClaim = false;
        }
        else if (dayIndex == Utils.curDailyGift - 1 && !Utils.cantakegiftdaily && Utils.IsClaimReward())
        {
            Debug.Log(1 + " " + this.gameObject.name);
            dailyReward.BtnClaimByVideo.gameObject.SetActive(false);
            // dailyReward.BtnClaim.gameObject.SetActive(false);
            // // dailyReward.CanClaim = true;
            CanClaim = true;
            if (avaiableItem != null)
            {
                avaiableItem.SetActive(false);
            }
            tick.SetActive(true);
            Observer.AddFromPosiGenerationCoin?.Invoke(gameObject);
            if (iconCoin != null)
            {
                iconCoin.SetActive(false);
            }
        }
        else if (dayIndex < Utils.curDailyGift)
        {
            if (avaiableItem != null)
            {
                avaiableItem.SetActive(false);
            }
            tick.SetActive(true);

            Observer.AddFromPosiGenerationCoin?.Invoke(gameObject);
            if (iconCoin != null)
            {
                iconCoin.SetActive(false);
            }
        }
        else
        {
            if (avaiableItem != null)
            {
                avaiableItem.SetActive(false);
            }
            tick.SetActive(false);
            if (iconCoin != null)
            {
                iconCoin.SetActive(true);
            }
        }
        
        //Debug.Log(dayIndex + " : " + CanClaim);
    }

    private void OnEnable() { DisplayAgain(); }
}
