namespace Lance.Common
{
    public class TypeCache<T>
    {
        public static readonly System.Type Type = typeof(T);
    }

    public class TypeHash<T>
    {
#if !UNITY_COLLECTIONS
        public static readonly int Hash = TypeCache<T>.Type.GetHashCode();
#else
        public static readonly int Hash = Unity.Burst.BurstRuntime.GetHashCode32<T>();
#endif
    }
}