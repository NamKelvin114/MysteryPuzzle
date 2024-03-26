using UnityEngine;
using UnityEngine.UI;

public class CollectionItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;

    public void SetupUnlockState(Sprite BgSprite, Sprite iconSprite)
    {
        iconImage.gameObject.SetActive(true);
        bgImage.sprite = BgSprite; 
        iconImage.sprite = iconSprite;
        iconImage.SetNativeSize();
    }

    public void SetupDefaultState(Sprite BgSprite)
    {
        iconImage.gameObject.SetActive(false);
        bgImage.sprite = BgSprite;
    }
}
