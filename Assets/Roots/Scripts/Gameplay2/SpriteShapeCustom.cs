#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class SpriteShapeCustom : MonoBehaviour 
{
    [SerializeField] float width = 0.1f;
    [SerializeField] float wallSize = 1f;
    [SerializeField] GameObject coll;
    [SerializeField] SpriteShapeController ssc;
    public SpriteShapeController SSC => ssc ?? (ssc = GetComponent<SpriteShapeController>());
    public float Width => width;
    public float WallSize => wallSize;
    public GameObject Coll => coll;

    public void OnUpdate()
    {
        List<EdgeCollider2D> listEdge = new List<EdgeCollider2D>();

        listEdge = this.Coll.gameObject.GetComponents<EdgeCollider2D>().ToList();
        for (int i = listEdge.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(listEdge[i], true);
        }
        List<Vector2> points = new List<Vector2>();
        List<Vector2> pointsTotal = new List<Vector2>();
        pointsTotal = ssc.edgeCollider.points.ToList();

        EdgeCollider2D edge = null;
        int count = 0;
        bool isCreate = false;
        int pointSpaceCount = 0;
        int pointSpaceCountCache = 0;
        int pointSpaceValue = 0;
        bool isAddPoint = false;

        for (int i = 0; i < ssc.spline.GetPointCount(); i++)
        {
            ssc.spline.SetHeight(i, this.WallSize);
            if (ssc.spline.GetSpriteIndex(i) == 1) pointSpaceCount++;
        }

        var level = ssc.GetComponentInParent<LevelMap>();
        if (level == null) return;
        for (int i = 0; i < ssc.spline.GetPointCount(); i++)
        {
            if (pointSpaceCountCache < pointSpaceCount)
            {
                if (ssc.spline.GetSpriteIndex(i) == 1)
                {
                    pointSpaceCountCache++;
                    for (int j = count; j < pointsTotal.Count; j++)
                    {
                        points.Add(pointsTotal[j]);
                        if ((pointsTotal[j] - (Vector2)ssc.spline.GetPosition(i)).magnitude <= this.Width && !isCreate)
                        {
                            pointSpaceValue = i;
                            isCreate = true;
                            edge = ObjectFactory.AddComponent<EdgeCollider2D>(this.Coll.gameObject);
                            edge.edgeRadius = this.Width;
                            edge.points = points.ToArray();
                        }

                        if (i < ssc.spline.GetPointCount() - 1)
                        {
                            if (ssc.spline.GetSpriteIndex(i + 1) != 1)
                            {
                                if ((pointsTotal[j] - (Vector2)ssc.spline.GetPosition(i + 1)).magnitude <= this.Width)
                                {
                                    isCreate = false;
                                    count = j;
                                    points = new List<Vector2>();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (pointSpaceValue == 0)
                {
                    for (int j = count; j < pointsTotal.Count; j++)
                    {
                        points.Add(pointsTotal[j]);
                    }
                    edge = ObjectFactory.AddComponent<EdgeCollider2D>(this.Coll.gameObject);
                    edge.edgeRadius = this.Width;
                    edge.points = points.ToArray();
                    break;
                }
                else
                if (pointSpaceValue < ssc.spline.GetPointCount() - 1)
                {
                    points = new List<Vector2>();

                    for (int j = count; j < pointsTotal.Count; j++)
                    {
                        if ((pointsTotal[j] - (Vector2)ssc.spline.GetPosition(pointSpaceValue + 1)).magnitude <= this.Width)
                        {
                            isAddPoint = true;
                        }

                        if (isAddPoint)
                        {
                            points.Add(pointsTotal[j]);
                        }
                    }
                    if (!isCreate)
                    {
                        if (!ssc.spline.isOpenEnded) points.Add(ssc.spline.GetPosition(0));
                        isCreate = true;
                        edge = ObjectFactory.AddComponent<EdgeCollider2D>(this.Coll.gameObject);
                        edge.edgeRadius = this.Width;
                        edge.points = points.ToArray();
                    }
                }
            }
        }
    }
}
[CustomEditor(typeof(SpriteShapeCustom))]
[CanEditMultipleObjects]
public class CustomSpriteShapeEditor : Editor
{
    SpriteShapeCustom spriteShape;
    SpriteShapeController ssc;

    void OnEnable()
    {
        spriteShape = target as SpriteShapeCustom;
        ssc = spriteShape.SSC;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Update", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            OnUpdate();
        }
        serializedObject.ApplyModifiedProperties();
    }
    public void OnUpdate() 
    {
        List<EdgeCollider2D> listEdge = new List<EdgeCollider2D>();

        listEdge = spriteShape.Coll.gameObject.GetComponents<EdgeCollider2D>().ToList();
        for (int i = listEdge.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(listEdge[i]);
        }
        List<Vector2> points = new List<Vector2>();
        List<Vector2> pointsTotal = new List<Vector2>();
        pointsTotal = ssc.edgeCollider.points.ToList();

        EdgeCollider2D edge = null;
        int count = 0;
        bool isCreate = false;
        int pointSpaceCount = 0;
        int pointSpaceCountCache = 0;
        bool isSpacePoint = false;

        for (int i = 0; i < ssc.spline.GetPointCount(); i++)
        {
            ssc.spline.SetHeight(i, spriteShape.WallSize);
            if (ssc.spline.GetSpriteIndex(i) == 1) pointSpaceCount++;
        }

        var level = ssc.GetComponentInParent<LevelMap>();
        if (level == null) return;
        for (int i = 0; i < ssc.spline.GetPointCount(); i++)
        {
            if (isSpacePoint && ssc.spline.GetSpriteIndex(i) == 0)
            {
                isSpacePoint = false;
                for (int j = count; j < pointsTotal.Count; j++)
                {
                    if ((pointsTotal[j] - (Vector2)ssc.spline.GetPosition(i)).magnitude <= spriteShape.Width)
                    {
                        isCreate = false;
                        count = j;
                        points = new List<Vector2>();
                        break;
                    }
                }
            }

            if (pointSpaceCount == 0)
            {
                for (int j = count; j < pointsTotal.Count; j++)
                {
                    points.Add(pointsTotal[j]);
                }
                edge = ObjectFactory.AddComponent<EdgeCollider2D>(spriteShape.Coll.gameObject);
                edge.edgeRadius = spriteShape.Width;
                edge.points = points.ToArray();
                break;
            }
            else
            if (pointSpaceCountCache < pointSpaceCount)
            {
                if (ssc.spline.GetSpriteIndex(i) == 1)
                {
                    isSpacePoint = true;
                    pointSpaceCountCache++;
                    if (!isCreate)
                    {
                        for (int j = count; j < pointsTotal.Count; j++)
                        {
                            points.Add(pointsTotal[j]);
                            if ((pointsTotal[j] - (Vector2)ssc.spline.GetPosition(i)).magnitude <= spriteShape.Width)
                            {
                                count = j;
                                isCreate = true;
                                edge = ObjectFactory.AddComponent<EdgeCollider2D>(spriteShape.Coll.gameObject);
                                edge.edgeRadius = spriteShape.Width;
                                edge.points = points.ToArray();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                points = new List<Vector2>();

                for (int j = count; j < pointsTotal.Count; j++)
                {
                    points.Add(pointsTotal[j]);
                }
                if (!isCreate)
                {
                    if (!ssc.spline.isOpenEnded) points.Add(ssc.spline.GetPosition(0));
                    isCreate = true;
                    edge = ObjectFactory.AddComponent<EdgeCollider2D>(spriteShape.Coll.gameObject);
                    edge.edgeRadius = spriteShape.Width;
                    edge.points = points.ToArray();
                    break;
                }
            }
        }
        EditorUtility.SetDirty(level.gameObject);
    }
}
#endif