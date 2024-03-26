using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int multiBonus = 1;
    [SerializeField] private TextMeshProUGUI textBonus;
    public int MultiBonus
    {
        get => multiBonus;
        set => multiBonus = value;
    }

    public void ChangeColorWhenEnterColider()
    {
        textBonus.color = new Color(1f, 1f, 1f);
    }
    public void ChangeColorWhenExitColider()
    {
        textBonus.color = new Color(0f, 0f, 0f);
    }
}
