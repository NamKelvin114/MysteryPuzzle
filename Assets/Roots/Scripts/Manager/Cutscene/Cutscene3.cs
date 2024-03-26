using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene3 : MonoBehaviour
{
    // Start is called before the first frame update
    public void endCutscene()
    {
        Observer.endCutscene3?.Invoke();
    }
    public void DoTrans()
    {
        GamePopup.Instance.ShowPopupTransition();
    }
}
