using System;
using Lance.Common;
using Lance.UI;
using TMPro;
using UnityEngine.UI;

public class FlagScrollView : CellView<FlagScrollData>
{
    public Image foreground;
    public Image imgIconCountry;
    public TextMeshProUGUI txtNameCountry;
    public Image checkmark;

    private Button _btn;
    private string _countryCode;
    
    private void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClickButtonPressed);
    }

    private void OnClickButtonPressed()
    {
        Utils.tempCountryCode = _countryCode;
        int count = transform.parent.childCount;
        for (int i = 0; i < count; i++)
        {
            transform.parent.GetChild(i).GetComponent<FlagScrollView>().Refresh();
        }

        checkmark.gameObject.SetActive(true);
        foreground.color = foreground.color.ChangeAlpha(0.3f);
    }

    public override void Initialized(Func<int, FlagScrollData> funcDataByIndex, int index, int dataIndex)
    {
        base.Initialized(funcDataByIndex, index, dataIndex);

        var data = FuncDataByIndex?.Invoke(DataIndex);
        if (data != null)
        {
            imgIconCountry.sprite = data.iconCountry;
            txtNameCountry.text = data.nameCountry;
            _countryCode = data.countryCode;
            checkmark.gameObject.SetActive(false);
            foreground.color = foreground.color.ChangeAlpha(0f);
            if (!string.IsNullOrEmpty(Utils.tempCountryCode) && Utils.tempCountryCode == data.countryCode)
            {
                checkmark.gameObject.SetActive(true);
                foreground.color = foreground.color.ChangeAlpha(0.3f);
            }
        }
    }

    public override void Refresh()
    {
        checkmark.gameObject.SetActive(false);
        if (foreground != null) foreground.color = foreground.color.ChangeAlpha(0f);
    }
}