using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] int count;
    [SerializeField] float space = 5f;
    [SerializeField] GameObject text;
    public int Count => count;
    [SerializeField] bool isDisableBall = false;
    

#if UNITY_EDITOR
    public void InitEditor() 
    {
        //var ball = balls.FirstOrDefault(_ => _.Type.Equals(type));
        var ball = this.ball;
        int lengt = Mathf.CeilToInt(Mathf.Sqrt(count));
        int countCache = 0;
        for (int i = 0; i < lengt; i++)
        {
            for (int j = 0; j< lengt; j++) 
            {
                var r = UnityEngine.Random.Range(0, 5);
                //if (r == 0) ball.Fx.gameObject.SetActive(true);
                //else ball.Fx.gameObject.SetActive(false);
                //ball.UpdateModel();
                if (countCache < count)
                {
                    countCache++;
                    Vector2 newVector2 = new Vector2(j - (int)(lengt / 2), i - (int)(lengt / 2)) * space;
                    var ballPrefab = (GameObject)PrefabUtility.InstantiatePrefab(ball.gameObject, this.transform);
                    ballPrefab.transform.localPosition = newVector2;
                }
                else break;
            }

        }
    }
    public void ClearEditor() 
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if(transform.GetChild(i).gameObject.activeInHierarchy)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnManager))]
[CanEditMultipleObjects]
public class SpawnManagerEditor : Editor
{
    SpawnManager spawnManager;
    private MapLevelManager levelMap;
    void OnEnable()
    {
        spawnManager = target as SpawnManager;
        levelMap = spawnManager.GetComponentInParent<MapLevelManager>();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Update", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            spawnManager.ClearEditor();
            spawnManager.InitEditor();
            if(levelMap != null) EditorUtility.SetDirty(levelMap.gameObject);
        }
        if (GUILayout.Button("Clear", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            spawnManager.ClearEditor();
            if (levelMap != null) EditorUtility.SetDirty(levelMap.gameObject);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
