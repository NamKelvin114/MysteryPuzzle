using System;
using System.Text.RegularExpressions;
using Pancake;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public enum MapType
{
    BRAZIL_AMAZONE,
    EGYPT,
    GREEK
}
public class LevelMap : LevelObject
{
    public MapType mapType;
    public int CurrentLevelIndex;
    public bool isBlockGameplay = false;
    [ShowIf(nameof(isBlockGameplay))] 
    [SerializeField] private BlockController blockController;
    public void SetLevelLoaded(int levelIndex) { CurrentLevelIndex = levelIndex; }

    private void OnEnable()
    {
        if (isBlockGameplay)
        {
            blockController.Init(GameManager.instance._camFollow._myCam);
        }
        else
        {
            NormalCameraSetting(GameManager.instance._camFollow._myCam);
        }
    }

    public void NormalCameraSetting(Camera cam)
    {
        cam.transform.localPosition = Vector3.zero;
        cam.orthographicSize = 5;
    }
    
    public override void StartPlaying()
    {
        base.StartPlaying();
        var objs = GetComponentsInChildren<LevelObject>();
        foreach (var levelObject in objs)
        {
            if (levelObject != this)
            {
                levelObject.StartPlaying();
            }
        }
    }
    
    public override void StopPlaying()
    {
        base.StopPlaying();
        var objs = GetComponentsInChildren<LevelObject>();
        foreach (var levelObject in objs)
        {
            if (levelObject != this)
            {
                levelObject.StopPlaying();
            }
        }
    }
    private void OnDrawGizmos()
    {
        float verticalHeightSeen = 19.2f * 5 / 11;
        float verticalWidthSeen = 10.8f * 5 / 11;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(verticalWidthSeen, verticalHeightSeen, 0));
    }
#if UNITY_EDITOR
    [Button]
    public void StartLevel()
    {
        if (isBlockGameplay)
        {
            Utils.IsJigsawMode = true;
            Utils.CurrentJigsawLevel = GetNumberInAString(gameObject.name) - 1;
        }
        else
        {
            Utils.IsJigsawMode = false;
            Utils.CurrentLevel = GetNumberInAString(gameObject.name) - 1;
        }

        Debug.Log("Start level " + GetNumberInAString(gameObject.name) + " " + Utils.IsJigsawMode);
        EditorApplication.isPlaying = true;
    }
    public static int GetNumberInAString(string str)
    {
        try
        {
            var getNumb = Regex.Match(str.Split(' ')[0], @"\d+").Value;
            return Int32.Parse(getNumb);
        }
        catch (Exception e)
        {
            return -1;
        }

        return -1;
    }
#endif    
}