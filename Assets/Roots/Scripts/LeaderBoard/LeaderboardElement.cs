using System;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private Image countryIcon;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI userLevel;
    public Image itemImg;
    [SerializeField] private GameObject effect;
    public Image CountryIcon => countryIcon;

    public void Initialize(int page, int rank, Sprite icon, string userName, string userLevel, Color[] colors)
    {
        this.rank.text = $"{rank}";
        this.userName.text = userName;
        if (effect != null) effect.SetActive(rank == 1);

        switch (rank)
        {
            case 1:
                itemImg.color = colors[0];

                gameObject.TryGetComponent(out UIShiny shiny);
                gameObject.TryGetComponent(out Animator animator);
                if (shiny != null) shiny.enabled = true;
                if (animator != null) animator.enabled = true;

                countryIcon.gameObject.TryGetComponent(out UIShiny shiny1);
                countryIcon.gameObject.TryGetComponent(out Animator animator1);
                if (shiny1 != null) shiny1.enabled = true;
                if (animator1 != null) animator1.enabled = true;
                break;
            case 2:
                itemImg.color = colors[1];
                break;
            case 3:
                itemImg.color = colors[2];
                break;
            default:
                if (Data.UserName.Equals(userName))
                {
                    itemImg.color = colors[4];
                }
                else
                {
                    itemImg.color = colors[3];
                }

                gameObject.TryGetComponent(out UIShiny shiny2);
                gameObject.TryGetComponent(out Animator animator2);
                if (shiny2 != null) shiny2.enabled = false;
                if (animator2 != null) animator2.enabled = false;

                countryIcon.gameObject.TryGetComponent(out UIShiny shiny3);
                countryIcon.gameObject.TryGetComponent(out Animator animator3);
                if (shiny3 != null) shiny3.enabled = false;
                if (animator3 != null) animator3.enabled = false;
                break;
        }

        this.userLevel.text = userLevel;

        countryIcon.sprite = icon;
        countryIcon.gameObject.SetActive(true);
    }
}