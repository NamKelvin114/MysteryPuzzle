using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbbellBar : MonoBehaviour
{
    // Start is called before the first frame update
    public int singelDumbbleCount = 0;
    
    [SerializeField] private GameObject DumbbellBarComplete;
    private List<SingleDumbble> _singleDumbbles = new List<SingleDumbble>();
    public void ActiveComplete()
    {
        DumbbellBarComplete.SetActive(true);
        this.gameObject.gameObject.SetActive(false);
        foreach (var variaSingleDumbble in _singleDumbbles)
        {
            variaSingleDumbble.gameObject.SetActive(false);
        }
    }
    
    public void AddSingleDumbleToList(SingleDumbble singleDumbble)
    {
        _singleDumbbles.Add(singleDumbble);
    }
}
