using UnityEngine;

namespace Lance.Common
{
    public static partial class Util
    {
        /// thanks for @carloswilkes
        /// <summary>This method allows you to destroy the target object in game and in edit mode, and it returns null.</summary>
        public static T Destroy<T>(T o) where T : Object
        {
            if (o != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Object.Destroy(o);
                }
                else
                {
                    Object.DestroyImmediate(o);
                }
#else
				Object.Destroy(o);
#endif
            }

            return null;
        }

        /// <summary>
        /// Instantiate object and connect prefab if possible
        /// </summary>
        public static T Instantiate<T>(T prefab, Transform parent, bool connectPrefab = true) where T : Object
        {
#if UNITY_EDITOR
            if (!connectPrefab || Application.isPlaying) return Object.Instantiate(prefab, parent);
            return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent) as T;
#else
            return Object.Instantiate(prefab, parent);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static Bounds GetRendererBounds(GameObject go, bool includeInactive = true) { return GetBounds<Renderer>(go, includeInactive); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="includeInactive"></param>
        /// <param name="getBounds"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Bounds GetBounds<T>(GameObject go, bool includeInactive = true, System.Func<T, Bounds> getBounds = null) where T : Component
        {
            if (getBounds == null)
                getBounds = (t) => (t as Collider)?.bounds ?? (t as Collider2D)?.bounds ?? (t as Renderer)?.bounds ?? default;
            var comps = go.GetComponentsInChildren<T>(includeInactive);

            Bounds bound = default;
            bool found = false;

            foreach (var comp in comps)
            {
                if (comp)
                {
                    if (!includeInactive)
                    {
                        if (!(comp as Collider)?.enabled ?? false) continue;
                        if (!(comp as Collider2D)?.enabled ?? false) continue;
                        if (!(comp as Renderer)?.enabled ?? false) continue;
                        if (!(comp as MonoBehaviour)?.enabled ?? false) continue;
                    }

                    if (!found || bound.size == Vector3.zero)
                    {
                        bound = getBounds(comp);
                        found = true;
                    }
                    else bound.Encapsulate(getBounds(comp));
                }
            }

            return bound;
        }
    }
}