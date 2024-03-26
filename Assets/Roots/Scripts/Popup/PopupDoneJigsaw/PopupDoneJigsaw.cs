using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lance.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Worldreaver.UniUI;

public class PopupDoneJigsaw : UniPopupBase
{
    [SerializeField] private ContentWinJigsaw contentWinJigsaw;
    [SerializeField] private TextMeshProUGUI textContent;
    public void SetUp(ETpyeContent eTpyeContent)
    {
        var getContent = contentWinJigsaw.setUpContent.Where(g => g.eTpyeContent == eTpyeContent).First();
        textContent.text = getContent.ContentText;
    }
    public void ClickContinue()
    {
        Utils.CurrentLevel += 1;
        Data.CountPlayLevel += 1;
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
    }
}
