using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.U2D;
public class WallBGCollider : MonoBehaviour
{
    [SerializeField] SpriteShapeController spriteShapeController;
    [SerializeField] GameObject colliderGO;
    [SerializeField] EdgeCollider2D spriteShapeCollider;
    [SerializeField] float edgeRadius;
    [SerializeField] float heightSpriteShape;
    Vector2 Round(Vector2 vector2, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector2(
            Mathf.Round(vector2.x * multiplier) / multiplier,
            Mathf.Round(vector2.y * multiplier) / multiplier);
    }
    bool IsBetween(Vector2 minPoint, Vector2 maxPoint, Vector2 checkPoint)
    {
        return checkPoint.x >= minPoint.x && checkPoint.x <= maxPoint.x && checkPoint.y >= minPoint.y && checkPoint.y <= maxPoint.y;
    }
    private void OnValidate()
    {
        transform.localScale = Vector3.one;
    }
#if UNITY_EDITOR
    public void ApplyCollider()
    {
        for (int idx = 0; idx < spriteShapeController.spline.GetPointCount(); idx++) spriteShapeController.spline.SetHeight(idx, heightSpriteShape);
        while (colliderGO.GetComponentsInChildren<EdgeCollider2D>().Length > 0)
        {
            DestroyImmediate(colliderGO.GetComponent<EdgeCollider2D>());
        }
        int[] mapSpriteToColl = new int[spriteShapeController.spline.GetPointCount()];
        for (int i = 0; i < spriteShapeController.spline.GetPointCount(); i++)
        {
            mapSpriteToColl[i] = -1;
            for (int j = i > 0 ? mapSpriteToColl[i - 1] + 1 : 0; j < spriteShapeCollider.pointCount; j++)
            {
                if (Vector2.Distance(spriteShapeController.spline.GetPosition(i), spriteShapeCollider.points[j]) <= 0.1f)
                {
                    mapSpriteToColl[i] = j;
                    break;
                }
            }
        }
        int lastSpriteIndex = 1;
        int lastColliderIndex = 0;
        List<Vector2> edgeColliderPoints = new List<Vector2>();
        for (int i = 0; i < spriteShapeController.spline.GetPointCount(); i++)
        {
            int currentSpriteIndex = spriteShapeController.spline.GetSpriteIndex(i);
            Vector2 currentSpritePoint = spriteShapeController.spline.GetPosition(i);
            if (edgeColliderPoints.Count + currentSpriteIndex == 0)
            {
                edgeColliderPoints.Add(currentSpritePoint);
                if (mapSpriteToColl[i] != -1) lastColliderIndex = mapSpriteToColl[i] + 1;
                else lastColliderIndex++;
                lastSpriteIndex = 0;
                continue;
            }
            else if (edgeColliderPoints.Count == 0 && currentSpriteIndex == 1) continue;
            List<Vector2> checkBetWeenList = new List<Vector2>();
            checkBetWeenList.Add(spriteShapeController.spline.GetPosition(i) + spriteShapeController.spline.GetLeftTangent(i));
            checkBetWeenList.Add(spriteShapeController.spline.GetPosition(i - 1));
            checkBetWeenList.Add(spriteShapeController.spline.GetPosition(i - 1) + spriteShapeController.spline.GetRightTangent(i - 1));
            Vector2 minPoint = currentSpritePoint, maxPoint = currentSpritePoint;
            foreach (var point in checkBetWeenList)
            {
                minPoint.x = Mathf.Min(minPoint.x, point.x);
                minPoint.y = Mathf.Min(minPoint.y, point.y);
                maxPoint.x = Mathf.Max(maxPoint.x, point.x);
                maxPoint.y = Mathf.Max(maxPoint.y, point.y);
            }
            if (lastSpriteIndex == 0) // 0 -> 1, 0 -> 0
            {
                for (; lastColliderIndex < spriteShapeCollider.pointCount; lastColliderIndex++)
                {
                    if (mapSpriteToColl[i] != -1 && lastColliderIndex >= mapSpriteToColl[i]) break;
                    if (IsBetween(Round(minPoint), Round(maxPoint), Round(spriteShapeCollider.points[lastColliderIndex])))
                    {
                        if (spriteShapeCollider.points[lastColliderIndex] != edgeColliderPoints[^1]) edgeColliderPoints.Add(spriteShapeCollider.points[lastColliderIndex]);
                    }
                    else break;
                }
                if (currentSpritePoint != edgeColliderPoints[^1]) edgeColliderPoints.Add(currentSpritePoint);
                if (currentSpriteIndex == 1)
                {
                    EdgeCollider2D edge = ObjectFactory.AddComponent<EdgeCollider2D>(colliderGO);
                    edge.edgeRadius = edgeRadius;
                    edge.points = edgeColliderPoints.ToArray();
                    edgeColliderPoints.Clear();
                }
                else
                {
                    if (mapSpriteToColl[i] != -1) lastColliderIndex = mapSpriteToColl[i] + 1;
                }
            }
            lastSpriteIndex = currentSpriteIndex;
        }
        if (lastSpriteIndex == 0)
        {
            for (; lastColliderIndex < spriteShapeCollider.pointCount; lastColliderIndex++)
            {
                edgeColliderPoints.Add(spriteShapeCollider.points[lastColliderIndex]);
            }
            EdgeCollider2D edge = ObjectFactory.AddComponent<EdgeCollider2D>(colliderGO);
            edge.edgeRadius = edgeRadius;
            edge.points = edgeColliderPoints.ToArray();
        }
        spriteShapeCollider.enabled = false;
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(WallBGCollider))]
[CanEditMultipleObjects]
public class CustomSpriteShapeEditor2 : Editor
{
    WallBGCollider spriteShape;
    SpriteShapeController ssc;
    void OnEnable()
    {
        spriteShape = target as WallBGCollider;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Update", GUILayout.MinHeight(40), GUILayout.MinWidth(100)))
        {
            spriteShape.ApplyCollider();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif