using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene1 : MonoBehaviour
{
    // Start is called before the first frame update
    public void endCutscene()
    {
        Observer.endCutscene1?.Invoke();
    }
}