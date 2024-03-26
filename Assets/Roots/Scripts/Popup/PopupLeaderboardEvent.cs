using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PopupLeaderboardEvent : PopupLeaderboard
{
    Action _actionBack;
    // public void Initialized(Action actionBack)
    // {


    //     _actionBack = actionBack;

    //     InitData();

    //     UpdateBottom();

    // }
    protected override void InitData()
    {
        currentStaticName = PlayfabConstants.EVENT_STATISTIC_NAME;
        getMyRankInLeaderboard(PlayfabConstants.EVENT_STATISTIC_NAME);
        getLeaderBoard(PlayfabConstants.EVENT_STATISTIC_NAME);
    }
    public void OnClickTapLevel()
    {
        getMyRankInLeaderboard(PlayfabConstants.EVENT_STATISTIC_NAME);
        getLeaderBoard(PlayfabConstants.EVENT_STATISTIC_NAME);
        popLevelRank.SetActive(true);
        popStarRank.SetActive(false);
        btnLevelRank.GetChild(0).gameObject.SetActive(true);
        btnStarRank.GetChild(0).gameObject.SetActive(false);
    }
    public void OnClickTapCountry()
    {
        string str = Utils.getCodeLeaderBoardCountry(Playfab.displayName, PlayfabConstants.EVENT_STATISTIC_NAME);
        getMyRankInLeaderboard(str);
        getLeaderBoard(str);
        popLevelRank.SetActive(false);
        popStarRank.SetActive(true);
        btnLevelRank.GetChild(0).gameObject.SetActive(false);
        btnStarRank.GetChild(0).gameObject.SetActive(true);
    }
}
