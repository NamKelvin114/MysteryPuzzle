using UnityEngine;

public class IntroGoalItemChange : MonoBehaviour
{
    //5, 6, 10, 9
    [SerializeField] private SpriteRenderer goalItem;
    [SerializeField] private SpriteRenderer pillar;
    [SerializeField] private Sprite[] goalItemSprites;
    [SerializeField] private Sprite[] pillarSprites;

    private void Start()
    {
        //
        if (Data.CurrentWorld > goalItemSprites.Length - 1)
        {
            goalItem.sprite = goalItemSprites[goalItemSprites.Length - 1];
            return;
        }

        goalItem.sprite = goalItemSprites[Data.CurrentWorld];

        if (Data.CurrentWorld == 4 || Data.CurrentWorld == 5 || Data.CurrentWorld == 8 || Data.CurrentWorld == 9)
        {
            pillar.sprite = pillarSprites[1];
        }
        else
        {
            pillar.sprite = pillarSprites[0];
        }
    }
}