using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene7 : MonoBehaviour
{
    // Start is called before the first frame update
    public void EndCutscene7()
    {
        GameManager.instance.CutsceneController.CompletedIntro();
    }
}