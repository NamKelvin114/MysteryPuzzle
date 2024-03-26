using UnityEngine;
using UnityEngine.SceneManagement;

public class EventPopupDone : MonoBehaviour
{
    /// <summary>
    /// using in animator LoadingPanel
    /// </summary>
    public void EventLoadScene()
    {
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE_NAME);
        if (GamePopup.Instance.menuRoom != null) GamePopup.Instance.menuRoom.gameObject.SetActive(false);
    }

    /// <summary>
    /// using in animator Loading
    /// </summary>
    public void EventDisableLoading()
    {
        gameObject.SetActive(false);
        GameManager.instance.gameState = EGameState.Playing;
    }

    /// <summary>
    /// using in animator BouderCoinFly
    /// </summary>
    public void EventCoinFly()
    {
        GameManager.instance.CoinTemp += Utils.CoinReward / 5;
        GameManager.instance.txtCoinWin.text = GameManager.instance.CoinTemp.ToString();

        GameManager.instance.SoftButton();
    }
}