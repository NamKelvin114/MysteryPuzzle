using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

//[SerializeField]
public class MovementPlatform : LevelObject, IMapObject, IExplodeReceiver, IBombTrigger
{
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public Rigidbody2D rig;

    [SerializeField] [HideInInspector] public float moveSpeed = 0;
    [SerializeField] [HideInInspector] public float rotateSpeed = 0;
    [SerializeField] [HideInInspector] public Vector3 vPosStart;
    [SerializeField] [HideInInspector] public Vector3 vPosEnd;
    [SerializeField] [HideInInspector] private Vector3 dir;
    [SerializeField] [HideInInspector] private bool isMoveToEndPos;
    [SerializeField] public float waitTime = 2.5f;


    private void Start() { wWaitToMove = new WaitForSeconds(waitTime); }

    private void FixedUpdate() { Moving(); }

    private void Moving()
    {
        if (moveSpeed != 0)
        {
            if (Vector3.Distance(transform.localPosition, vPosEnd) <= 0.1f)
            {
                //isMoveToEndPos = true;
                StartCoroutine(IeWaitToMove(true));
            }
            else if (Vector3.Distance(transform.localPosition, vPosStart) <= 0.1f)
            {
                //isMoveToEndPos = false;
                StartCoroutine(IeWaitToMove(false));
            }

            if (!isMoveToEndPos)
            {
                if (Vector3.Distance(transform.localPosition, vPosEnd) > 0.09f)
                {
                    dir = (vPosEnd - transform.position).normalized;
                    rig.MovePosition(transform.position + dir * (moveSpeed * Time.fixedDeltaTime));
                }
            }
            else
            {
                dir = (vPosStart - transform.position).normalized;
                rig.MovePosition(transform.position + dir * (moveSpeed * Time.fixedDeltaTime));
            }
        }
    }

    private bool _moveToEnd, _moveToStart;
    public WaitForSeconds wWaitToMove;

    private IEnumerator IeWaitToMove(bool _bl)
    {
        yield return wWaitToMove;
        isMoveToEndPos = _bl;
    }

    public bool IsBombTriggering() { return true; }

    public void OnExplodedAt(BombItem bomb) { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovementPlatform))]
//[CanEditMultipleObjects]
public class MovementPlatformEditor : Editor
{
    private MovementPlatform mp;

    private void OnSceneGUI()
    {
        //var movementPlatform = target as MovementPlatform;
        mp = (MovementPlatform) target;

        if (!mp || !mp.sr) return;

        float hsize = HandleUtility.GetHandleSize(mp.transform.position) * 0.1f;
        var lmin = mp.sr.size * -0.5f;
        var lmax = mp.sr.size * 0.5f;
        var lmid = (lmax + lmin) / 2f;

        var lleft = new Vector2(lmin.x, 0);
        var lright = new Vector2(lmax.x, 0);

        //var lleft = new Vector2(/*lmin.x*/-mp.sr.size.x, 0);
        //var lright = new Vector2(/*lmax.x*/mp.sr.size.x, 0);

        var ltop = new Vector2(0, lmax.y);
        var lbot = new Vector2(0, lmin.y);

        Vector2 Move(Vector2 local, ref bool change, Vector2 direction, out Vector3 worldOffset)
        {
            var world = mp.transform.TransformPoint(local);
            var newWorld = Handles.FreeMoveHandle(world,
                Quaternion.identity,
                hsize,
                Vector3.one * 0.1f,
                Handles.DotHandleCap);
            worldOffset = mp.transform.TransformVector(mp.transform.InverseTransformVector(newWorld - world).Mult(direction));
            change |= worldOffset != Vector3.zero;
            return mp.transform.InverseTransformPoint(newWorld);
        }

        bool changed = false;
        Handles.color = Color.magenta;
        var newlleft = Move(lleft, ref changed, new Vector2(1, 0), out var wleftchange);
        var newlright = Move(lright, ref changed, new Vector2(1, 0), out var wrightchange);
        var newltop = Move(ltop, ref changed, new Vector2(0, 1), out var wtopchange);
        var newlbot = Move(lbot, ref changed, new Vector2(0, 1), out var wbotchange);

        if (changed)
        {
            Undo.RegisterCompleteObjectUndo(mp, "resize");
            Undo.RegisterCompleteObjectUndo(mp.sr, "resize");
            Undo.RegisterCompleteObjectUndo(mp.transform, "resize");
            mp.sr.size = new Vector2(newlright.x - newlleft.x, newltop.y - newlbot.y);
            mp.transform.position += (wleftchange + wrightchange + wtopchange + wbotchange) / 2f;
            EditorUtility.SetDirty(mp);
        }
    }

    private void DrawGUIButton()
    {
        GUIStyle sectionNameStyle = new GUIStyle();
        sectionNameStyle.fontSize = 16;
        sectionNameStyle.normal.textColor = Color.blue;

        #region Check Can Rotate

        EditorGUILayout.LabelField("Move Speed", sectionNameStyle);
        mp.moveSpeed = EditorGUILayout.Slider(mp.moveSpeed, 0, 10);

        #endregion


        EditorGUILayout.LabelField("\tMoving Platform", sectionNameStyle);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            if (GUILayout.Button("Save Start Position", GUILayout.Height(50)))
            {
                mp.vPosStart = mp.gameObject.transform.position;
                EditorUtility.SetDirty(mp);
            }

            if (GUILayout.Button("Save End Position", GUILayout.Height(50)))
            {
                mp.vPosEnd = mp.gameObject.transform.position;
                mp.gameObject.transform.position = mp.vPosStart;
                EditorUtility.SetDirty(mp);
            }

            if (GUILayout.Button("Move to Start Position", GUILayout.Height(50)))
            {
                //mp.gameObject.transform.position = mp.vPosStart;
                Vector3 vSwap = mp.vPosStart;
                mp.vPosStart = mp.vPosEnd;
                mp.vPosEnd = vSwap;
                mp.gameObject.transform.localPosition = mp.vPosStart;
                EditorUtility.SetDirty(mp);
            }
        }
        EditorGUILayout.EndVertical();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (!mp) return;
        DrawGUIButton();
    }

    private void OnDisable()
    {
        if (!mp) return;
        EditorUtility.SetDirty(mp);
    }
}
#endif