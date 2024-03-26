using System.Runtime.CompilerServices;

namespace Lance.Common
{
    public readonly struct FasterReadOnlyList<T>
    {
        public static FasterReadOnlyList<T> defaultEmptyList = new FasterReadOnlyList<T>(FasterList<T>.DefaultEmptyList);

        public uint Count => _list.Count;
        public uint Capacity => _list.Capacity;

        public FasterReadOnlyList(FasterList<T> list) { _list = list; }

        public static implicit operator FasterReadOnlyList<T>(FasterList<T> list) { return new FasterReadOnlyList<T>(list); }

        public static implicit operator LocalFasterReadOnlyList<T>(FasterReadOnlyList<T> list) { return new LocalFasterReadOnlyList<T>(list._list); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FasterListEnumerator<T> GetEnumerator() { return _list.GetEnumerator(); }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _list[index];
        }

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _list[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArrayFast(out uint count) { return _list.ToArrayFast(out count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }

        internal readonly FasterList<T> _list;
    }

    public readonly ref struct LocalFasterReadOnlyList<T>
    {
        public uint count => _list.Count;
        public uint capacity => _list.Capacity;

        public LocalFasterReadOnlyList(FasterList<T> list) { _list = list; }

        public static implicit operator LocalFasterReadOnlyList<T>(FasterList<T> list) { return new LocalFasterReadOnlyList<T>(list); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FasterListEnumerator<T> GetEnumerator() { return _list.GetEnumerator(); }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _list[index];
        }

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _list[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArrayFast(out uint count) { return _list.ToArrayFast(out count); }

        readonly FasterList<T> _list;
    }
}