
using TMPro;
using System;
using UnityEngine;
using Worldreaver.UniUI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
public class PopupLeaderboard : UniPopupBase
{
    [SerializeField] protected Transform content;
    [SerializeField] protected Transform content2;
    [SerializeField] protected Transform itemRank;
    [SerializeField] protected Transform itemRank2;
    [SerializeField] protected ItemRankPopup myItemRank;
    [SerializeField] protected GameObject popLevelRank;
    [SerializeField] protected GameObject popStarRank;
    [SerializeField] protected Transform btnLevelRank;
    [SerializeField] protected Transform btnStarRank;
    [SerializeField] protected Transform btnLeft;
    [SerializeField] protected Transform btnRight;
    [SerializeField] protected TextMeshProUGUI textCurrentPage;
    protected List<PlayFab.ClientModels.PlayerLeaderboardEntry> ListEntryLevel = new List<PlayFab.ClientModels.PlayerLeaderboardEntry>();
    protected List<PlayFab.ClientModels.PlayerLeaderboardEntry> ListEntryCountry = new List<PlayFab.ClientModels.PlayerLeaderboardEntry>();
    protected List<PlayFab.ClientModels.PlayerLeaderboardEntry> ListEntryCurrent = new List<PlayFab.ClientModels.PlayerLeaderboardEntry>();
    protected string currentStaticName;
    int _currentPage1 = 0;
    int _currentPage2 = 0;
    int _currentPage = 0;
    int _currentStack = 0;
    /// <param name="actionBack"></param>

    Action _actionBack;
    private void Start()
    {
        ListEntryCurrent = ListEntryLevel;
    }
    public void Initialized(Action actionBack)
    {
        ListEntryCountry.Clear();
        ListEntryLevel.Clear();
        ListEntryCurrent.Clear();

        this._currentPage = 0;

        _actionBack = actionBack;

        InitData();

        UpdateBottom();

    }

    protected virtual void InitData()
    {
        currentStaticName = PlayfabConstants.LEVEL_STATISTIC_NAME;
        getMyRankInLeaderboard(currentStaticName);
        getLeaderBoard(currentStaticName, 0);

        var str = Utils.getCodeLeaderBoardCountry(Playfab.displayName, PlayfabConstants.LEVEL_STATISTIC_NAME);
        getLeaderBoard(str, 1);
    }
    protected void getLeaderBoard(string StatisticName, int stack)
    {
        int start = 10 * this._currentPage;
        if (stack == 0)
            Playfab.GetLeaderboard(StatisticName, start, 100, CallbackGetNextDataLeaderBoardCountry);
        else
        {
            Playfab.GetLeaderboard(StatisticName, start, 100, CallbackGetNextDataLeaderBoardCountry2);
        }
    }
    protected void getLeaderBoard(string StatisticName)
    {
        int start = 10 * this._currentPage;
        if (_currentStack == 0)
            Playfab.GetLeaderboard(StatisticName, start, 100, CallbackGetNextDataLeaderBoardCountry);
        else
        {
            Playfab.GetLeaderboard(StatisticName, start, 100, CallbackGetNextDataLeaderBoardCountry2);
        }
    }
    void CallbackGetNextDataLeaderBoardCountry(GetLeaderboardResult result)
    {
        var dataEntry = result.Leaderboard;

        if (dataEntry.Count == 0) return;

        ListEntryLevel.AddRange(dataEntry);
        UpdateListBoard(ListEntryLevel, _currentPage * 10, _currentPage * 10 + 10, content, this.itemRank);
    }
    void CallbackGetNextDataLeaderBoardCountry2(GetLeaderboardResult result)
    {
        var dataEntry = result.Leaderboard;

        if (dataEntry.Count == 0) return;

        ListEntryCountry.AddRange(dataEntry);
        UpdateListBoard(ListEntryCountry, _currentPage * 10, _currentPage * 10 + 10, content2, this.itemRank2);
    }

    int UpdateListBoard(List<PlayerLeaderboardEntry> ListEntry, int start, int end, Transform _content, Transform prefab)
    {
        // ListEntry.RemoveRange(start, end - start);
        int index = 0;

        for (; start < end && start < ListEntry.Count; index++)
        {
            var element = ListEntry[start];

            Transform item;
            if (index < _content.childCount)
                item = _content.GetChild(index);
            else
            {
                item = Instantiate(prefab, _content);
            }
            item.gameObject.SetActive(true);
            var comp = item.GetComponent<ItemRankPopup>();
            var split = Utils.SplitCountry(element.DisplayName);
            var _name = Utils.SplitLongString(split.name, 12);
            comp.InitInfo(_name, element.StatValue, element.Position, split.countryCodes);
            start++;
        };
        int check = index;
        for (; index < _content.childCount; index++)
        {
            var any = _content.GetChild(index);
            any.gameObject.SetActive(false);
        }
        return check;
    }
    protected void getMyRankInLeaderboard(string StatisticName)
    {
        Playfab.getMyRankLeadBoard(StatisticName, callBackDataMyRank);
    }
    void callBackDataMyRank(GetLeaderboardAroundPlayerResult result)
    {

        for (var index = 0; index < result.Leaderboard.Count; index++)
        {
            var element = result.Leaderboard[index];
            var split = Utils.SplitCountry(element.DisplayName);
            myItemRank.InitInfo(split.name, element.StatValue, element.Position, split.countryCodes);
        }
    }
    public void OnClickTapLevel()
    {
        getMyRankInLeaderboard(PlayfabConstants.LEVEL_STATISTIC_NAME);
        // getLeaderBoard(PlayfabConstants.LEVEL_STATISTIC_NAME, 0);

        popLevelRank.SetActive(true);
        popStarRank.SetActive(false);
        btnLevelRank.GetChild(0).gameObject.SetActive(true);
        btnStarRank.GetChild(0).gameObject.SetActive(false);

        this._currentPage = this._currentPage1;
        currentStaticName = PlayfabConstants.LEVEL_STATISTIC_NAME;
        _currentStack = 0;
        ListEntryCurrent = ListEntryLevel;
        // UpdateUi();
        UpdateBottom();
    }
    public void OnClickTapCountry()
    {
        string str = Utils.getCodeLeaderBoardCountry(Playfab.displayName, PlayfabConstants.LEVEL_STATISTIC_NAME);
        getMyRankInLeaderboard(str);
        // getLeaderBoard(str, 1);

        popLevelRank.SetActive(false);
        popStarRank.SetActive(true);
        btnLevelRank.GetChild(0).gameObject.SetActive(false);
        btnStarRank.GetChild(0).gameObject.SetActive(true);

        this._currentPage = this._currentPage2;
        currentStaticName = str;
        _currentStack = 1;
        ListEntryCurrent = ListEntryCountry;
        UpdateBottom();
    }
    public void onClickLeft()
    {
        this._currentPage--;
        if (this._currentPage <= 0)
        {
            this._currentPage = 0;
        }
        // let start = 100 * this._currentPage;
        // this.UpdateListBoard(this.ListEntryLeadBoard,start,start+RankConfig.LB_COUNT_IN_PAGE)
        // this.updateUi();
        UpdateUi();
    }

    public void onClickRight()
    {

        int check = (this._currentPage + 1) * 10;
        this._currentPage++;
        bool isupdate = true;
        if (CheckNumCanNext(check, this.ListEntryCurrent.Count) > 0)
        {

            UpdateUi();
        }
        else
            isupdate = false;
        if (check >= this.ListEntryCurrent.Count && this.ListEntryCurrent.Count > 0)
        {
            this.getLeaderBoard(currentStaticName);
        }

        if (!isupdate) this._currentPage--;

    }
    private int CheckNumCanNext(int start, int end)
    {
        if (start < end)
        {
            return end - start;
        }
        return 0;
    }
    public void UpdateUi()
    {
        var start = 10 * this._currentPage;
        if (_currentStack == 0)
        {
            this.UpdateListBoard(this.ListEntryCurrent, start, start + 10, content, itemRank);
            this._currentPage1 = this._currentPage;
        }

        else
        {
            this.UpdateListBoard(this.ListEntryCurrent, start, start + 10, content2, itemRank2);
            this._currentPage2 = this._currentPage;
        }
        UpdateBottom();
    }
    public void UpdateBottom()
    {
        if (_currentPage <= 0)
        {
            btnLeft.gameObject.SetActive(false);
        }
        else btnLeft.gameObject.SetActive(true);
        textCurrentPage.text = "Page:" + (_currentPage + 1);

    }
    public void OnBackButtonPressed() { _actionBack?.Invoke(); }
}
public class CountryCodeEF
{
    public CountryCodeEF(string _name, string _countryCodes)
    {
        name = _name;
        countryCodes = _countryCodes;
    }
    public string name;
    public string countryCodes;
}
