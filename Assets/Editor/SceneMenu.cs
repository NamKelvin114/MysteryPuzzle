#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneMenu
{
    const string LevelEditorScene = "LevelEditorScene";
    const string PrefabScene = "PrefabScene";
    const string LauncherScene = "Loading";
    const string GameScene = "MainGame";
    const string FarmScene = "FarmScene";
    const string HomeScene = "MainMenu";
    
    static void ChangeScene(string name)
    {
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene(Application.dataPath + "/Roots/Scenes/" + name + ".unity");
    }

    static bool CanChangeScene(string name)
    {
        return HasScene(name) && SceneManager.GetActiveScene().name != name;
    }

    static bool HasScene(string name)
    {
        return File.Exists(Application.dataPath + "/Roots/Scenes/" + name + ".unity");
    }
    
    [MenuItem("Scenes/Level Editor Scene", false, 11)]
    static void OpenLevelEditorScene()
    {
        ChangeScene(LevelEditorScene);
    }

    [MenuItem("Scenes/Level Editor Scene", true, 11)]
    static bool CanOpenLevelEditorScene()
    {
        return CanChangeScene(LevelEditorScene);
    }

    [MenuItem("Scenes/Prefab Scene", false, 11)]
    static void OpenPrefabScene()
    {
        ChangeScene(PrefabScene);
    }

    [MenuItem("Scenes/Prefab Scene", true, 11)]
    static bool CanOpenPrefabScene()
    {
        return CanChangeScene(PrefabScene);
    }

    [MenuItem("Scenes/Launcher Scene", false, 22)]
    static void OpenLauncherScene()
    {
        ChangeScene(LauncherScene);
    }

    [MenuItem("Scenes/Launcher Scene", true, 22)]
    static bool CanOpenLauncherScene()
    {
        return CanChangeScene(LauncherScene);
    }

    [MenuItem("Scenes/Game Scene", false, 22)]
    static void OpenGameScene()
    {
        ChangeScene(GameScene);
    }

    [MenuItem("Scenes/Game Scene", true, 22)]
    static bool CanOpenGameScene()
    {
        return CanChangeScene(GameScene);
    }

    [MenuItem("Scenes/Farm Scene", false, 33)]
    static void OpenFarmScene()
    {
        ChangeScene(FarmScene);
    }

    [MenuItem("Scenes/Farm Scene", true, 33)]
    static bool CanOpenFarmScene()
    {
        return CanChangeScene(FarmScene);
    }

    [MenuItem("Scenes/Home Scene", false, 44)]
    static void OpenHomeScene()
    {
        ChangeScene(HomeScene);
    }

    [MenuItem("Scenes/Home Scene", true, 44)]
    static bool CanOpenHomeScene()
    {
        return CanChangeScene(HomeScene);
    }
    
    [MenuItem("Scenes/Play", false, 44)]
    public static void PlayLauncherScene()
    {
        if (HasScene(LauncherScene))
        {
            EditorSceneManager.SaveOpenScenes();
            ChangeScene(LauncherScene);
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("Scenes/Play", true, 44)]
    static bool CanPlayLauncherScene()
    {
        return HasScene(LauncherScene) && !Application.isPlaying;
    }
}
#endif