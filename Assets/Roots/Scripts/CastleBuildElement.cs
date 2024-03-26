#pragma warning disable 649
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Worldreaver.UniUI;
using Worldreaver.Utility;

public class CastleBuildElement : MonoBehaviour
{
    [SerializeField] private int idElement;
    [SerializeField] private Image imgCurrentLevel;
    [SerializeField] private Image imgNextLevel;
    [SerializeField] private Image imgArrow;
    [SerializeField] private TextMeshProUGUI txtCost;
    [SerializeField] private TextMeshProUGUI txtMax;
    [SerializeField] private GameObject groupInfo;
    [SerializeField] private UniButton btnBuild;

    [SerializeField] private Image[] slotStar;
    [SerializeField] private Sprite[] upgradeSprites;

    [SerializeField] private Vector3 positionMaxLevel;
    [SerializeField] private Vector3 positionStart;

    private Func<int, int> _funcBuild;
    private Func<int, int> _funcGetCostByStar;
    private Func<int, int> _funcGetStarById;
    private Action<int, Sprite> _actionCastBuild;
    private Func<(Sprite, Sprite)> _funcGetStarSprite;
    private Func<(Sprite, Sprite, Sprite)> _funcGetBuildButtonSprite;
    private Func<(Color, Color)> _funGetCostColor;
    private int _cost;

    private TextMeshProUGUI _txtBuild;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="build"></param>
    /// <param name="getCostByStar"></param>
    /// <param name="getStarById"></param>
    /// <param name="actionCastBuild"></param>
    /// <param name="funcGetStarSprite"></param>
    /// <param name="funcGetBuildButtonSprite"></param>
    /// <param name="funGetCostColor"></param>
    public void Initialized(
        Func<int, int> build,
        Func<int, int> getCostByStar,
        Func<int, int> getStarById,
        Action<int, Sprite> actionCastBuild,
        Func<(Sprite, Sprite)> funcGetStarSprite,
        Func<(Sprite, Sprite, Sprite)> funcGetBuildButtonSprite,
        Func<(Color, Color)> funGetCostColor)
    {
        _funcBuild = build;
        _funcGetCostByStar = getCostByStar;
        _funcGetStarById = getStarById;
        _actionCastBuild = actionCastBuild;
        _funcGetBuildButtonSprite = funcGetBuildButtonSprite;
        _funcGetStarSprite = funcGetStarSprite;
        _funGetCostColor = funGetCostColor;

        Refresh();

        btnBuild.onClick.RemoveAllListeners();
        btnBuild.onClick.AddListener(OnBuildCastleButtonPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="star"></param>
    private void UpdateCost(
        int star)
    {
        if (star >= 5)
        {
            txtCost.text = "";
            return;
        }

        if (_cost < 1000)
        {
            txtCost.text = $"{_cost}";
        }
        else
        {
            txtCost.text = $"{_cost:0,000}";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="star"></param>
    private void ActiveStar(
        int star)
    {
        var result = _funcGetStarSprite();
        switch (star)
        {
            default:
            case 0:
                foreach (var image in slotStar)
                {
                    image.sprite = result.Item2;
                }

                break;
            case 1:
                slotStar[0]
                    .sprite = result.Item1;
                for (int i = 1; i < slotStar.Length; i++)
                {
                    slotStar[i]
                        .sprite = result.Item2;
                }

                break;
            case 2:
                slotStar[0]
                    .sprite = result.Item1;
                slotStar[1]
                    .sprite = result.Item1;
                for (int i = 2; i < slotStar.Length; i++)
                {
                    slotStar[i]
                        .sprite = result.Item2;
                }

                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    slotStar[i]
                        .sprite = result.Item1;
                }

                slotStar[3]
                    .sprite = result.Item2;
                slotStar[4]
                    .sprite = result.Item2;
                break;
            case 4:
                for (int i = 0; i < 4; i++)
                {
                    slotStar[i]
                        .sprite = result.Item1;
                }

                slotStar[4]
                    .sprite = result.Item2;
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    slotStar[i]
                        .sprite = result.Item1;
                }

                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBuildCastleButtonPressed()
    {
        int star = _funcGetStarById(idElement);
        
        if (star >= 5)
        {
            return;
        }

        if (Utils.currentCoin >= _cost)
        {
            Utils.currentCoin -= _cost;
            _funcBuild(idElement);
            Refresh(true);
            _actionCastBuild.Invoke(idElement, upgradeSprites[_funcGetStarById(idElement) - 1]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Refresh(
        bool onAfterBuild = false)
    {
        var currentStar = _funcGetStarById(idElement);

        var buildSprite = _funcGetBuildButtonSprite.Invoke();
        
        if (currentStar == 0 || currentStar >= 5)
        {
            if (currentStar >= 5)
            {
                txtMax.gameObject.SetActive(true);
                groupInfo.SetActive(false);
                imgCurrentLevel.sprite = upgradeSprites[4];
            }
            else
            {
                imgCurrentLevel.sprite = upgradeSprites[0];
                txtMax.gameObject.SetActive(false);
                groupInfo.SetActive(true);
                
                btnBuild.image.sprite = buildSprite.Item1;
                if (_txtBuild == null)
                {
                    _txtBuild = btnBuild.GetComponentInChildren<TextMeshProUGUI>();
                }

                if (_txtBuild != null)
                {
                    _txtBuild.text = "Build";
                }
            }

            imgCurrentLevel.transform.localPosition = positionMaxLevel;
            imgNextLevel.gameObject.SetActive(false);
            imgArrow.gameObject.SetActive(false);
        }
        else
        {
            if (onAfterBuild)
            {
                imgCurrentLevel.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 0.5f, 5, 0.5f);
            }

            imgCurrentLevel.sprite = upgradeSprites[currentStar - 1];
            imgNextLevel.sprite = upgradeSprites[currentStar];
            imgCurrentLevel.transform.localPosition = positionStart;
            imgNextLevel.gameObject.SetActive(true);
            imgArrow.gameObject.SetActive(true);
            txtMax.gameObject.SetActive(false);
            groupInfo.SetActive(true);
            
            btnBuild.image.sprite = buildSprite.Item3;
            if (_txtBuild == null)
            {
                _txtBuild = btnBuild.GetComponentInChildren<TextMeshProUGUI>();
            }
            
            if (_txtBuild != null)
            {
                _txtBuild.text = "Upgrade";
            }
        }

        ActiveStar(currentStar);
        _cost = _funcGetCostByStar(currentStar);

        var s = _funcGetBuildButtonSprite();
        var colors = _funGetCostColor();
        if (Utils.currentCoin >= _cost)
        {
            btnBuild.interactable = true;
            txtCost.color = colors.Item1;
        }
        else
        {
            btnBuild.interactable = false;
            btnBuild.image.sprite = s.Item2;
            txtCost.color = colors.Item2;
        }

        UpdateCost(currentStar);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public (Sprite, bool, int) GetData()
    {
        var saveCastle = DataController.instance.SaveCastle[idElement];
        if (saveCastle.unlock)
        {
            int index = saveCastle.star >= 5 ? 4 : saveCastle.star - 1;
            if (index < 0)
            {
                index = 1;
                saveCastle.star = 1;
            }

            return (upgradeSprites[index], true, idElement);
        }

        return (null, false, idElement);
    }
}