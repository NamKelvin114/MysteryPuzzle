#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class EditorCommonUtils
{
    private static string StartScenePath => Constants.START_SCENE_PATH;

    public static System.Type GetInspectorWindowType()
    {
        var editorAsm = typeof(Editor).Assembly;
        var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
        return inspWndType;
    }

    public static void OpenSceneMenu()
    {
        GenericMenu menu = new GenericMenu();
        var scenes = AssetUtils.FindAllAssets<SceneAsset>();
        var groups = scenes.GroupBy(s =>
        {
            var path = AssetDatabase.GetAssetPath(s);
            return Directory.GetParent(path)
                .FullName;
        });

        foreach (var group in groups)
        {
            foreach (var scene in group)
            {
                menu.AddItem(new GUIContent(scene.name),
                    false,
                    obj =>
                    {
                        var sc = obj as SceneAsset;
                        var path = AssetDatabase.GetAssetPath(sc);
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(path);
                        }
                    },
                    scene);
            }

            menu.AddSeparator("");
        }

        menu.ShowAsContext();
    }

    public static void PlayStartScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.path.Equals(StartScenePath))
            {
                EditorApplication.isPlaying = true;
            }
            else
            {
                var scene = EditorSceneManager.OpenScene(StartScenePath);
                if (scene.IsValid() && scene.isLoaded)
                {
                    EditorApplication.isPlaying = true;
                }
                else
                {
                    Debug.LogErrorFormat("Start scene not valid {0}", StartScenePath);
                }
            }
        }
    }
}
#endif