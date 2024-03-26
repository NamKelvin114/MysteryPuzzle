using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectionBook", menuName = "ScriptableObject/CollectionBook")]
public class CollectionBook : ScriptableObject
{
    [SerializeField] private List<CollectionPage> listPages;
    public List<CollectionPage> ListPage { get => listPages; }
    
    public int GetLastestPageID()
    {
        for (int i = 0; i < listPages.Count; i++)
        {
            if (listPages[i].CheckUnlocked() == false)
            {
                return i;
            }
        }

        return listPages.Count - 1;
    }

    public CollectionPage GetLastestPage()
    {
        return GetPageByID(GetLastestPageID());
    }

    public CollectionPage GetPageByID(int pageID)
    {
        if (listPages.Count <= pageID || pageID < 0)
            return null;
        return listPages[pageID];
    }

    public int GetPageReadyToClaimReward()
    {
        for (int i = 0; i < listPages.Count; i++)
        {
            if (!listPages[i].IsCollected)
            {
                return i;
            }
        }
        return - 1;
    }

    public int GetBookSize()
    {
        return listPages.Count;
    }
}