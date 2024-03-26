using Cysharp.Threading.Tasks;
using Pancake.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTransaction : MonoBehaviour
{
    public void ChangeScene()
    {
        Utils.showNewWorld = false;
        Data.SetStateCutScene(Data.CurrentWorld, true);
        if (Data.CurrentWorld == 0)
        {
            SceneManager.LoadSceneAsync("MainMenu");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            SceneManager.LoadSceneAsync("MainGame");
        }
    }
    
    private async void WaitingDone()
    {
        await UniTask.WaitUntil(() => BridgeData.Instance.nextLevelLoaded != null && BridgeData.Instance.menuRoomPrefab != null);
    }

    private async void WaitInitializeIap() { await UniTask.WaitUntil(() => IAPManager.Instance.IsInitialize); }

    //private void LoadMenu() { loader.Load("MainMenu", OnSceneLoaded, WaitingDone, WaitInitializeIap); }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        BridgeData.Instance.showUpdatePopupAction?.Invoke();
        
        // remove it
        //SoundManager.Instance.PlayBackgroundMusic();
    }
}