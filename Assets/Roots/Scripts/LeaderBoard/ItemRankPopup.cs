using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemRankPopup : MonoBehaviour
{
    [SerializeField] Sprite[] lisIconTop;
    [SerializeField] TextMeshProUGUI lbName;
    [SerializeField] TextMeshProUGUI lbPos;
    [SerializeField] TextMeshProUGUI lbLevel;
    [SerializeField] Image iconCountry;
    [SerializeField] Image iconTop;
    public void InitInfo(string _name, int level, int position, string codeContry)
    {
        this.lbName.text = _name;
        this.lbLevel.text = level + "";
        this.lbPos.text = (position + 1) + "";
        iconCountry.sprite = I2.Loc.ResourceManager.pInstance.LoadFromResources<UnityEngine.Sprite>("Img/CountryIcon/" + codeContry);
        // ResUtil.getIconCountry(codeContry, (err, res) =>
        // {
        //     this.iconCountry.spriteFrame = res;
        // });
        iconTop.gameObject.SetActive(false);
        switch (position)
        {
            case 0:
                iconTop.gameObject.SetActive(true);
                iconTop.sprite = lisIconTop[0];
                break;
            case 1:
                iconTop.gameObject.SetActive(true);
                iconTop.sprite = lisIconTop[1];
                break;
            case 2:
                iconTop.gameObject.SetActive(true);
                iconTop.sprite = lisIconTop[2];
                break;
            default:

                break;
        }

    }

}

