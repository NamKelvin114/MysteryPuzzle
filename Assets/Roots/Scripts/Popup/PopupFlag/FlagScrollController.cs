using Lance.Common;
using Lance.UI;
using UnityEngine;

public class FlagScrollController : ScrollBaseController<FlagScrollData>
{
    [SerializeField] private CountryCode countryCode;

    public CountryCode CountryCode => countryCode;

    public void ForeInitialized()
    {
        scroller.Initialize(this);
    }
    
    protected override void Initialized()
    {
        if (data != null) data.Clear();
        else data = new FasterList<FlagScrollData>();
        
        for (int i = 0; i < countryCode.countryCodes.Count; i++)
        {
            data.Add(
                new FlagScrollData(countryCode.GetIcon(BridgeData.Instance.countryCodes[i]), BridgeData.Instance.countryName[i], BridgeData.Instance.countryCodes[i]));
        }
    }

    public override float GetCellSize(Scroller scroller, int dataIndex) { return 64f; }

    public override void SetCellView(ICellView cell, int index)
    {
        var item = cell as FlagScrollView;
        if (item != null) item.Initialized(DataByIndex, index, index);
    }

    public override ICellView GetCellView(Scroller scroller, int dataIndex, int cellIndex) { throw new System.NotImplementedException(); }
}