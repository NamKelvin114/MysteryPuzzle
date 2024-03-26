using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lance.Common
{
    public static partial class Util
    {
        public static void ShiftEnum<T>(ref T val) where T : struct, IConvertible { val = GetNextEnum(val); }

        public static T GetNextEnum<T>(T val) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (!type.IsEnum) return val;

            var enums = Enum.GetValues(type) as T[];
            int index = Array.IndexOf(enums, val);
            if (index >= 0)
            {
                index++;
                if (index >= enums.Length) index = 0;
                return enums[index];
            }

            return val;
        }
        
        /// <summary>
        /// return snapped value in a range. e.g. from 1 to 3, value 2.2547f, step 10 => 2.2
        /// </summary>
        public static float StepSnap(float from, float to, float value, int steps)
        {
            var ratio = Mathf.InverseLerp(from, to, value);
            ratio = Mathf.Round(ratio * (steps - 1)) / (steps - 1);
            return Mathf.Lerp(from, to, ratio);
        }

        public static float InverseLerpUnclamped(float a, float b, float x) { return b != a ? (x - a) / (b - a) : float.PositiveInfinity * (x - a); }

        /// <summary>
        /// count must be >=0, return from 0->count-1
        /// </summary>
        public static int Repeat(int value, int count)
        {
            if (count <= 0) return 0;
            while (value < 0) value += count;
            if (value < count) return value;
            return value % count;
        }

        /// <summary>
        /// pingpong with the ends happening half time, max must be >=0, return steps ...,2,1,0,1,2,...,max-1,max,max-1,...,2,1,0,1,2,...
        /// </summary>
        public static float PingPong(float value, int max)
        {
            if (max <= 0) return 0;
            var group = Mathf.CeilToInt(value / max);
            if (group % 2 == 0)
            {
                return max * group - value;
            }

            return value - max * (@group - 1);
        }

        /// <summary>
        /// Think of a simple jump trajectory / bell parabola. Highest point has height of <paramref name="topHeight"/>, jump is normalized from x=0->x=1.
        /// Evaluate the height at any given normalized <paramref name="x"/>
        /// </summary>
        public static float GetNormalizedBellHeight(float topHeight, float x) { return -topHeight * 4 * Mathf.Pow(x - 0.5f, 2) + topHeight; }
        
        public static bool CalculateIdealCount(float availableSpace, float minSize, float maxSize, int defaultCount, out int count, out float size)
        {
            int minCount = Mathf.FloorToInt(availableSpace / maxSize);
            int maxCount = Mathf.FloorToInt(availableSpace / minSize);
            bool goodness = defaultCount >= minCount && defaultCount <= maxCount;
            count = Mathf.Clamp(defaultCount, minCount, maxCount);
            size = availableSpace / count;
            return goodness;
        }

        #region Matrix

        public static Vector3 Position(this Matrix4x4 m) { return m.GetColumn(3); }

        public static Vector3 Scale(this Matrix4x4 m) { return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude); }

        #endregion


        #region Vector3

        public static bool IsZero(this Vector3 v) { return v == Vector3.zero; }

        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
        }

        public static Vector3 Flat(this Vector3 v) { return new Vector3(v.x, 0, v.z); }

        public static Vector3 Multiply(this Vector3 v, float x, float y, float z) { return v.Multiply(new Vector3(x, y, z)); }

        public static Vector3 Multiply(this Vector3 v, Vector3 other) { return Vector3.Scale(v, other); }

        public static Vector3 Divide(this Vector3 v, Vector3 other) { return Vector3.Scale(v, new Vector3(1 / other.x, 1 / other.y, 1 / other.z)); }

        public static Vector3Int Rounded(this Vector3 v) { return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z)); }

        public static Vector3Int Floored(this Vector3 v) { return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z)); }

        public static Vector3Int Ceiled(this Vector3 v) { return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z)); }

        #endregion


        #region Vector2

        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max) { return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y)); }

        public static Vector2 Multiply(this Vector2 v, float x, float y) { return v.Multiply(new Vector2(x, y)); }

        public static Vector2 Multiply(this Vector2 v, Vector2 other) { return Vector2.Scale(v, other); }

        public static Vector2 Divide(this Vector2 v, Vector2 other) { return Vector2.Scale(v, new Vector2(1 / other.x, 1 / other.y)); }

        public static Vector2Int Rounded(this Vector2 v) { return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); }

        public static Vector2Int Floored(this Vector2 v) { return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y)); }

        public static Vector2Int Ceiled(this Vector2 v) { return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y)); }

        #endregion


        #region Rect

        public static Vector2[] GetCorners(this Rect r)
        {
            var c1 = new Vector2(r.xMin, r.yMax);
            var c2 = new Vector2(r.xMax, r.yMin);
            return new Vector2[] {r.min, c1, r.max, c2};
        }

        public static Rect AsRect(this Bounds bound) { return new Rect(bound.min, bound.size); }

        public static bool Contains(this Rect r, Rect smaller) { return r.Contains(smaller.min) && r.Contains(smaller.max); }

        public static Vector2 Clamp(this Rect r, Vector2 v)
        {
            v.x = Mathf.Clamp(v.x, r.xMin, r.xMax);
            v.y = Mathf.Clamp(v.y, r.yMin, r.yMax);
            return v;
        }

        public static Vector2 InnerLerp(this Rect r, float x, float y) { return new Vector2(Mathf.Lerp(r.xMin, r.xMax, x), Mathf.Lerp(r.yMin, r.yMax, y)); }

        public static Rect InnerLerp(this Rect r, float xMin, float yMin, float xMax, float yMax)
        {
            var min = r.InnerLerp(xMin, yMin);
            var max = r.InnerLerp(xMax, yMax);
            return new Rect(min, max - min);
        }

        public static Vector2 RandomPosition(this Rect r) { return r.InnerLerp(UnityEngine.Random.value, UnityEngine.Random.value); }

        public static Rect FromCenter(this Rect r, Vector2 center, Vector2 size) { return new Rect(center - size / 2f, size); }

        public static Rect Scale(this Rect r, float f) { return r.Scale(f, f); }

        public static Rect Scale(this Rect r, float fx, float fy) { return r.FromCenter(r.center, Vector2.Scale(r.size, new Vector2(fx, fy))); }

        public static Rect Normalized(this Rect r, float fx, float fy) { return new Rect(r.x / fx, r.y / fy, r.width / fx, r.height / fy); }

        public static Rect Grown(this Rect r, float f) { return r.Grown(Vector2.one * f); }

        public static Rect Grown(this Rect r, Vector2 half) { return new Rect(r.position - half, r.size + half * 2); }

        // left to right, bottom to top
        public static Rect[] Split(this Rect rect, int cols = 1, int rows = 1)
        {
            rows = Mathf.Max(1, rows);
            cols = Mathf.Max(1, cols);

            Vector2 size = new Vector2(rect.width / cols, rect.height / rows);
            Rect[] rs = new Rect[rows * cols];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    float cx = rect.position.x + (x + 0.5f) * size.x;
                    float cy = rect.position.y + (y + 0.5f) * size.y;

                    int index = y * cols + x;
                    rs[index] = rect.FromCenter(new Vector2(cx, cy), size);
                }
            }

            return rs;
        }

        // left to right, bottom to top
        public static Rect[,] Split2D(this Rect rect, int cols = 1, int rows = 1)
        {
            rows = Mathf.Max(1, rows);
            cols = Mathf.Max(1, cols);

            Vector2 size = new Vector2(rect.width / cols, rect.height / rows);
            Rect[,] rs = new Rect[cols, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    float cx = rect.position.x + (x + 0.5f) * size.x;
                    float cy = rect.position.y + (y + 0.5f) * size.y;

                    int index = y * cols + x;
                    rs[x, y] = rect.FromCenter(new Vector2(cx, cy), size);
                }
            }

            return rs;
        }

        #endregion


        #region Bounds

        /// <summary>
        /// Interpolate a position inside a bounds from 3d normalized coordinates. (x,y,z) 0->1
        /// </summary>
        public static Vector3 InnerLerp(this Bounds r, float x, float y, float z)
        {
            Vector3 min = r.min;
            Vector3 max = r.max;
            return new Vector3(Mathf.Lerp(min.x, max.x, x), Mathf.Lerp(min.y, max.y, y), Mathf.Lerp(min.z, max.z, z));
        }

        /// <summary>
        /// Return the normalized & localized position of "pos" inside bounds
        /// </summary>
        public static Vector3 InnerInverseLerp(this Bounds r, Vector3 pos, bool clamped = false)
        {
            Vector3 outvec = Vector3.zero;
            Vector3 min = r.min;
            Vector3 max = r.max;
            outvec.x = clamped ? Mathf.InverseLerp(min.x, max.x, pos.x) : InverseLerpUnclamped(min.x, max.x, pos.x);
            outvec.y = clamped ? Mathf.InverseLerp(min.y, max.y, pos.y) : InverseLerpUnclamped(min.y, max.y, pos.y);
            outvec.z = clamped ? Mathf.InverseLerp(min.z, max.z, pos.z) : InverseLerpUnclamped(min.z, max.z, pos.z);
            return outvec;
        }

        /// <summary>
        /// Get a random position inside a bounds
        /// </summary>
        public static Vector3 RandomPosition(this Bounds r) { return r.InnerLerp(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value); }

        /// <summary>
        /// Calculate the 6 planes making up the bounds. With normals pointing out or in.
        /// </summary>
        public static Plane[] GetPlanes(this Bounds b, bool normalsInside = false)
        {
            float f = normalsInside ? -1 : 1;

            Plane[] pls = new Plane[6];
            pls[0] = new Plane(Vector3.left * f, b.min);
            pls[1] = new Plane(Vector3.back * f, b.min);
            pls[2] = new Plane(Vector3.down * f, b.min);
            pls[3] = new Plane(Vector3.right * f, b.max);
            pls[4] = new Plane(Vector3.forward * f, b.max);
            pls[5] = new Plane(Vector3.up * f, b.max);
            return pls;
        }

        /// <summary>
        /// Calculate the intersection of a ray from inside the bounds, if possible.
        /// </summary>
        /// <returns>The multiplier or the ray direction</returns>
        public static bool IntersectRayFromInside(this Bounds b, Ray ray, out float dist)
        {
            // use planes.raycast
            dist = 0;
            if (!b.Contains(ray.origin)) return false;

            var planes = b.GetPlanes(true);
            float d = -1;
            foreach (var plane in planes)
            {
                if (plane.Raycast(ray, out float dp))
                {
                    if (d < 0 || d > dp) d = dp;
                }
            }

            if (d >= 0) dist = d;

            return true;
        }

        /// <summary>
        /// Expand bounds so that it contains vector
        /// </summary>
        public static void Encapsulate(this ref BoundsInt b, Vector3Int v)
        {
            var min = Vector3Int.Min(v, b.min);
            var max = Vector3Int.Min(v, b.max);
            b.SetMinMax(min, max);
        }

        /// <summary>
        /// Expand the bounds equally on 3 sides
        /// </summary>
        public static void Expand(this ref BoundsInt b, int i)
        {
            b.max += Vector3Int.one * i;
            b.min -= Vector3Int.one * i;
        }

        /// <summary>
        /// Expand the bounds each min and max by vector
        /// </summary>
        public static void Expand(this ref BoundsInt b, Vector3Int i)
        {
            b.max += i;
            b.min -= i;
        }

        /// <summary>
        /// Get the corners of the bounds
        /// </summary>
        public static List<Vector3> GetCorners(this Bounds b)
        {
            var min = b.min;
            var max = b.max;
            var ps = new[] {min.x, max.x, min.y, max.y, min.z, max.z};
            List<Vector3> l = new List<Vector3>();
            for (int ix = 0; ix <= 1; ix++)
            for (int iy = 2; iy <= 3; iy++)
            for (int iz = 4; iz <= 5; iz++)
            {
                l.Add(new Vector3(ps[ix], ps[iy], ps[iz]));
            }

            return l;
        }

        /// <summary>
        /// Get the corners of the bounds
        /// </summary>
        public static List<Vector3Int> GetCorners(this BoundsInt b)
        {
            var min = b.min;
            var max = b.max;
            var ps = new[] {min.x, max.x, min.y, max.y, min.z, max.z};
            List<Vector3Int> l = new List<Vector3Int>();
            for (int ix = 0; ix <= 1; ix++)
            for (int iy = 2; iy <= 3; iy++)
            for (int iz = 4; iz <= 5; iz++)
            {
                l.Add(new Vector3Int(ps[ix], ps[iy], ps[iz]));
            }

            return l;
        }


        /// <summary>
        /// Check if bounds B fully contains Another
        /// </summary>
        public static bool Contains(this Bounds b, Bounds another) { return b.Contains(another.min) && b.Contains(another.max); }

        #endregion
    }
}