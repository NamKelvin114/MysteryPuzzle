using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class MineItem : BombItem
{
    public int controlID;

    public void RemoteActivate(MineHandControl control)
    {
        if (control) DoExplode();
    }

    protected override void CheckCollision(Collider2D collision)
    {
        if (!IsLeverActivated) DoExplode();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(MineItem))]
[CanEditMultipleObjects]
public class BombEditor : Editor
{
    private void OnSceneGUI()
    {
        var b = target as MineItem;
        if (b.IsLeverActivated)
        {
            var bounds = ObjectUtils.GetRendererBounds(b.gameObject, false);
            var p = bounds.center;
            p.y = bounds.max.y;
            var hsize = HandleUtility.GetHandleSize(p) * 0.35f;
            p.y += hsize * 0.5f;

            Handles.color = new Color(1, 1, 1, 0.5f);
            Handles.DrawSolidDisc(p, Vector3.back, hsize);
            Handles.Label(p, b.controlID.ToString(), new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter});
        }
    }
}

#endif