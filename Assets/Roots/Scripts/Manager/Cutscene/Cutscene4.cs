using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene4 : MonoBehaviour
{
    // Start is called before the first frame update
    public void endCutscene()
    {
        Observer.endCutscene4?.Invoke();
    }
    public void DoTrans()
    {
        GamePopup.Instance.ShowPopupTransition();
    }
    public void EnableSkip()
    {
        GameManager.instance.CutsceneController.skipBtn.gameObject.SetActive(true);
    }
}
