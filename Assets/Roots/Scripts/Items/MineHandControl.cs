using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityTimer;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class MineHandControl : LevelObject, IMapObject
{
    public int leverID;
    public Transform handleRotRoot;
    public float activateAngle = 30;

    public bool IsActivated { get; private set; }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsActivated)
        {
            var enemy = collision.GetComponentInParent<EnemyBase>();
            if (enemy)
            {
                Activate();
                return;
            }

            var player = collision.GetComponentInParent<PlayerManager>();
            if (player)
            {
                Activate();
                return;
            }

            var hostage = collision.GetComponentInParent<HostageManager>();
            if (hostage)
            {
                Activate();
                return;
            }

            var item = collision.GetComponent<BaseItem>();
            if (item)
            {
                Activate();
                return;
            }

            var dragon = collision.GetComponentInParent<BaseCannon>();
            if (dragon && !collision.gameObject.name.Equals("SearchCollider"))
            {
                Activate();
                return;
            }
        }
    }

    public void Activate()
    {
        if (IsActivated) return;
        IsActivated = true;

        DOTween.Kill(handleRotRoot);
        handleRotRoot.DOLocalRotate(new Vector3(0, 0, activateAngle), 0.1f);
        Timer.Register(0.1f,
            () =>
            {
                var bombs = GameManager.instance.mapLevel.GetComponentsInChildren<MineItem>();
                foreach (var bomb in bombs.Where(b => b.IsLeverActivated && b.controlID == leverID))
                {
                    if (bomb) bomb.RemoteActivate(this);
                }
            });

        // play sound
        if (SoundManager.Instance.mineActiveClip != null) SoundManager.Instance.PlaySound(SoundManager.Instance.mineActiveClip);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var bombs = FindObjectsOfType<MineItem>();
        Gizmos.color = ColorUtils.orange;
        foreach (var b in bombs)
        {
            if (b.IsLeverActivated && b.controlID == leverID)
            {
                Gizmos.DrawLine(transform.position, b.transform.position);
            }
        }
    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(MineHandControl))]
[CanEditMultipleObjects]
public class MineHandControlEditor : Editor
{
    private void OnSceneGUI()
    {
        var mineControl = target as MineHandControl;
        if (mineControl != null)
        {
            var bounds = ObjectUtils.GetRendererBounds(mineControl.gameObject, false);
            var p = bounds.center;
            p.y = bounds.max.y;
            var hsize = HandleUtility.GetHandleSize(p) * 0.35f;
            p.y += hsize * 0.5f;

            Handles.color = new Color(1, 1, 1, 0.5f);
            Handles.DrawSolidDisc(p, Vector3.back, hsize);
            Handles.Label(p, mineControl.leverID.ToString(), new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter});
        }
    }
}

#endif