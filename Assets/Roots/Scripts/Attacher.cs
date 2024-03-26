using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif

[ExecuteInEditMode]
public class Attacher : MonoBehaviour
{
    public Attachment attach1 = new Attachment();
    public Attachment attach2 = new Attachment();
    public AttachType attachType = AttachType.Pivotable;


    private void Awake()
    {
        if (Application.isPlaying)
        {
            AttachAndDie();
        }
    }

    public Attachment GetJointLocation()
    {
        if (attach1 == null) return attach2;
        if (attach2 == null) return attach1;
        return attach2.GetJointPriorityScore() > attach1.GetJointPriorityScore() ? attach2 : attach1;
    }


    public void AttachAndDie()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Cant attach when not in playmode", this);
            return;
        }

        if (attachType == AttachType.None)
        {
            Destroy(gameObject);
            return;
        }

        bool attached = false;
        var targetAtm = GetJointLocation();
        var targetRb = targetAtm?.GetTargetBody();
        if (targetRb)
        {
            var otherAtm = targetAtm == attach1 ? attach2 : attach1;
            var otherRb = otherAtm?.GetTargetBody();
            if (otherRb == targetRb) otherRb = null;

            AnchoredJoint2D joint = null;
            if (attachType == AttachType.Pivotable) joint = targetRb.gameObject.AddComponent<HingeJoint2D>();
            else if (attachType == AttachType.Fixed) joint = targetRb.gameObject.AddComponent<FixedJoint2D>();

            if (joint)
            {
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = targetAtm.localOffset;
                joint.connectedBody = otherRb;
                joint.connectedAnchor = otherRb ? otherAtm.localOffset : (Vector2) targetRb.transform.TransformPoint(targetAtm.localOffset);
                attached = true;

                if (otherRb)
                {
                    var dummy = DummyBehaviour.Add(joint.gameObject, "joint-destroy-waiter");
                    dummy.TWLEWait(new WaitUntil(() => !otherRb),
                        () =>
                        {
                            Destroy(joint);
                            Destroy(dummy);
                        });
                }
            }
        }

        if (!attached)
        {
            Debug.LogError("Failed to attach. Attacher still alive", this);
        }
        else Destroy(gameObject);
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            if (CenterPosition())
            {
                EditorUtility.SetDirty(transform);
            }
        }
    }
#endif

    public bool CenterPosition()
    {
        var rb1 = attach1?.GetTargetBody();
        var rb2 = attach2?.GetTargetBody();
        var p1nlb = rb1?.transform.TransformPoint(attach1.localOffset);
        var p2nlb = rb2?.transform.TransformPoint(attach2.localOffset);
        if (p1nlb.HasValue || p2nlb.HasValue)
        {
            var pos0 = transform.position;
            var p1 = p1nlb ?? p2nlb ?? pos0;
            var p2 = p2nlb ?? p1nlb ?? pos0;
            var newPos = (p1 + p2) / 2f;
            if ((newPos - pos0).sqrMagnitude > 0.1f)
            {
                transform.position = newPos;
                return true;
            }
        }

        return false;
    }


    private void OnDrawGizmos()
    {
        var jointLocation = GetJointLocation();

        var rb1 = attach1?.GetTargetBody();
        if (rb1)
        {
            var p1o = (Vector3) rb1.position;
            var p1 = rb1.transform.TransformPoint(attach1.localOffset);
            Gizmos.color = ColorUtils.orange;
            Gizmos.DrawWireSphere(p1, 0.1f);
            Gizmos.color *= new Color(1, 1, 1, 0.5f);
            Gizmos.DrawLine(p1, p1o);
        }

        var rb2 = attach2?.GetTargetBody();
        if (rb2)
        {
            var p2o = (Vector3) rb2.position;
            var p2 = rb2.transform.TransformPoint(attach2.localOffset);
            Gizmos.color = ColorUtils.orange;
            Gizmos.DrawWireSphere(p2, 0.1f);
            Gizmos.color *= new Color(1, 1, 1, 0.5f);
            Gizmos.DrawLine(p2, p2o);
        }

        if (rb1 && rb2)
        {
            var p1 = rb1.transform.TransformPoint(attach1.localOffset);
            var p2 = rb2.transform.TransformPoint(attach2.localOffset);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawWireSphere(jointLocation == attach1 ? p1 : p2, 0.2f);
        }
    }


    public enum AttachType
    {
        None,
        Fixed,
        Pivotable
    }

    public enum TargetType
    {
        None,
        Common,
        RopeEnd1,
        RopeEnd2,
    }

    [System.Serializable]
    public class Attachment
    {
        public Component baseTarget;
        public TargetType targetType;
        public Vector2 localOffset;
        public bool jointPriority;

        public int GetJointPriorityScore()
        {
            int score = 0;

            if (!baseTarget) score = -1000;
            else if (targetType == TargetType.RopeEnd2) score = 10000;
            else if (targetType == TargetType.RopeEnd1) score = 9000;
            else if (targetType == TargetType.Common) score = 8000;

            if (jointPriority) score += 1;
            return score;
        }

        public Rigidbody2D GetTargetBody()
        {
            if (!baseTarget) return null;

            T castOrGet<T>(
                Component tt)
                where T : Component
            {
                return tt && tt is T ? tt as T : tt.GetComponent<T>();
            }

            if (targetType == TargetType.Common)
            {
                return castOrGet<Rigidbody2D>(baseTarget);
            }
            else if (targetType == TargetType.RopeEnd1 || targetType == TargetType.RopeEnd2)
            {
                var rope = castOrGet<Rope>(baseTarget);
                var nodes = rope?.GetNodes();
                if (nodes?.Count > 0)
                {
                    var targetNode = targetType == TargetType.RopeEnd1 ? nodes[0] : nodes[nodes.Count - 1];
                    return targetNode?.rb;
                }
            }

            return null;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Attacher))]
[CanEditMultipleObjects]
public class AttacherEditor : Editor
{
    private BaseEditMode _editMode;


    public override void OnInspectorGUI()
    {
        if (serializedObject.isEditingMultipleObjects || Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }

        serializedObject.UpdateIfRequiredOrScript();
        var firstProp = serializedObject.GetIterator();
        firstProp.isExpanded = EditorGUILayout.Foldout(firstProp.isExpanded, "Attacher", true);
        if (firstProp.isExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            EditorGUI.EndChangeCheck();
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();

        var at = target as Attacher;

        EditorUtilEx.SubSection("Attacher editor");

        EditorUtilEx.MiniBoxedSection("Editing",
            () =>
            {
                void editModeBtn(
                    BaseEditMode mode)
                {
                    if (EditorUtilEx.MyButton(ObjectNames.NicifyVariableName(mode.ToString()), mode == _editMode ? (Color?) ColorUtils.lightGreen : null))
                    {
                        if (mode == _editMode) _editMode = BaseEditMode.None;
                        else _editMode = mode;
                        SceneView.RepaintAll();
                    }
                }

                ;
                EditorGUILayout.BeginHorizontal();
                editModeBtn(BaseEditMode.Offset);
                if (EditorUtilEx.MyButton("Reset offset"))
                {
                    Undo.RegisterCompleteObjectUndo(at, "Edit");
                    at.attach1.localOffset = at.attach2.localOffset = Vector2.zero;
                    EditorUtility.SetDirty(at);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                editModeBtn(BaseEditMode.Target1);
                if (EditorUtilEx.MyButton("Clear T1"))
                {
                    Undo.RegisterCompleteObjectUndo(at, "Edit");
                    at.attach1.baseTarget = null;
                    at.attach1.jointPriority = false;
                    EditorUtility.SetDirty(at);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                editModeBtn(BaseEditMode.Target2);
                if (EditorUtilEx.MyButton("Clear T2"))
                {
                    Undo.RegisterCompleteObjectUndo(at, "Edit");
                    at.attach2.baseTarget = null;
                    at.attach2.jointPriority = false;
                    EditorUtility.SetDirty(at);
                }

                EditorGUILayout.EndHorizontal();

                var newAttachType = (Attacher.AttachType) EditorGUILayout.EnumPopup("Attach type", at.attachType);
                if (newAttachType != at.attachType)
                {
                    Undo.RegisterCompleteObjectUndo(at, "Edit");
                    at.attachType = newAttachType;
                    EditorUtility.SetDirty(at);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    var jointLoc = at.GetJointLocation();
                    EditorGUILayout.LabelField("Current joint location", jointLoc == at.attach1 ? "Target 1" : "Target 2");
                    if (EditorUtilEx.MyButton("Try switch"))
                    {
                        if (at.attach1 != null && at.attach2 != null)
                        {
                            Undo.RegisterCompleteObjectUndo(at, "Edit");
                            if (at.attach1.jointPriority && at.attach2.jointPriority) at.attach1.jointPriority = false;
                            else if (!at.attach1.jointPriority && !at.attach2.jointPriority) at.attach1.jointPriority = true;
                            else
                            {
                                at.attach1.jointPriority = !at.attach1.jointPriority;
                                at.attach2.jointPriority = !at.attach2.jointPriority;
                            }

                            EditorUtility.SetDirty(at);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            });
    }


    private void OnSceneGUI()
    {
        var at = target as Attacher;
        var rb1 = at.attach1?.GetTargetBody();
        var rb2 = at.attach2?.GetTargetBody();
        if (rb1) Handles.Label(rb1.transform.position, "Target 1");
        if (rb2) Handles.Label(rb2.transform.position, "Target 2");

        if (_editMode == BaseEditMode.None) return;
        else if (_editMode == BaseEditMode.Offset)
        {
            void editOffset(
                Attacher.Attachment atm)
            {
                if (atm == null || !atm.baseTarget) return;

                var body = atm.GetTargetBody();
                var tr = body ? body.transform : atm.baseTarget.transform;

                var rot = Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : tr.rotation;
                var pos = tr.TransformPoint(atm.localOffset);
                var newPos = Handles.PositionHandle(pos, rot);
                if (newPos != pos)
                {
                    var newLPos = tr.InverseTransformPoint(newPos);
                    Undo.RegisterCompleteObjectUndo(at, "Edit");
                    atm.localOffset = newLPos;
                    EditorUtility.SetDirty(at);
                }
            }

            ;
            editOffset(at.attach1);
            editOffset(at.attach2);
        }
        else if (_editMode == BaseEditMode.Target1 || _editMode == BaseEditMode.Target2)
        {
            if (SceneViewUtils.Get2DMouseScenePosition(out var mp))
            {
                var hsize = HandleUtility.GetHandleSize(mp) * 2f;

                Handles.color = ColorUtils.orange;
                Handles.DrawWireDisc(mp, Vector3.back, hsize);

                var atm = SearchForAttachment(mp, hsize, at.gameObject.scene.GetPhysicsScene2D());
                if (atm != null)
                {
                    ref var targetAtm = ref (_editMode == BaseEditMode.Target1 ? ref at.attach1 : ref at.attach2);
                    var otherAtm = _editMode == BaseEditMode.Target1 ? at.attach2 : at.attach1;

                    var ec = Event.current;
                    if (ec.type == EventType.MouseDown && ec.button == 0)
                    {
                        HandlesUtils.SkipEvent();
                        Undo.RegisterCompleteObjectUndo(at, "Edit");
                        atm.localOffset = targetAtm?.localOffset ?? atm.localOffset;
                        targetAtm = atm;
                        EditorUtility.SetDirty(at);
                    }

                    Vector2 targetPos = Vector2.zero;
                    Vector2 otherPos = Vector2.zero;
                    var newTargetBody = atm?.GetTargetBody();
                    var otherTargetBody = otherAtm?.GetTargetBody();

                    if (newTargetBody)
                    {
                        targetPos = newTargetBody.transform.TransformPoint(targetAtm.localOffset);
                        Handles.DrawSolidDisc(targetPos, Vector3.back, hsize * 0.1f);
                    }

                    if (otherTargetBody)
                    {
                        otherPos = otherAtm.GetTargetBody()
                            .transform.TransformPoint(otherAtm.localOffset);
                        Handles.DrawSolidDisc(otherPos, Vector3.back, hsize * 0.1f);
                    }

                    if (newTargetBody && otherTargetBody) Handles.DrawLine(targetPos, otherPos);
                }

                SceneView.RepaintAll();
            }
        }
    }


    public static Attacher.Attachment SearchForAttachment(
        Vector3 position,
        float radius,
        PhysicsScene2D? physicsScene = null)
    {
        Collider2D col = null;
        if (physicsScene.HasValue)
        {
            var results = new List<Collider2D>();
            int count = physicsScene.Value.OverlapCircle(position, radius, new ContactFilter2D() {useTriggers = true}, results);
            col = results.Take(count)
                .OrderBy(c => (c.bounds.center - position).sqrMagnitude)
                .FirstOrDefault();
        }
        else
        {
            col = Physics2D.OverlapCircleAll(position, radius)
                .OrderBy(c => (c.bounds.center - position).sqrMagnitude)
                .FirstOrDefault();
        }

        if (col)
        {
            Attacher.Attachment atm = null;
            var rope = col.GetComponentInParent<Rope>();
            if (rope)
            {
                var nodes = rope.GetNodes();
                if (nodes?.Count > 0)
                {
                    var nE1 = nodes[0];
                    var nE2 = nodes[nodes.Count - 1];
                    var d1 = (nE1.transform.position - position).sqrMagnitude;
                    var d2 = (nE2.transform.position - position).sqrMagnitude;
                    var nTarget = d1 < d2 ? nE1 : nE2;
                    atm = new Attacher.Attachment() {baseTarget = rope, jointPriority = true, targetType = d1 < d2 ? Attacher.TargetType.RopeEnd1 : Attacher.TargetType.RopeEnd2,};
                }
            }

            if (atm == null)
            {
                var rb = col.GetComponentInParent<Rigidbody2D>();
                if (rb)
                {
                    atm = new Attacher.Attachment() {baseTarget = rb, jointPriority = false, targetType = Attacher.TargetType.Common,};
                }
            }

            return atm;
        }

        return null;
    }


    enum BaseEditMode
    {
        None,
        Offset,
        Target1,
        Target2
    }
}
#endif