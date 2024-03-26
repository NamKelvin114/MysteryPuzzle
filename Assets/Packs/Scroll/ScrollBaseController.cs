using UnityEngine;
using Lance.Common;

namespace Lance.UI
{
    public abstract class ScrollBaseController<T> : MonoBehaviour, IScrollController
    {
        protected FasterList<T> data;
        [SerializeField] protected Scroller scroller;
        [SerializeField] private bool selfInitialize;
        public virtual uint Count => data?.Count ?? (uint) 0;

        private void Awake()
        {
            if (selfInitialize) Initialized();
            scroller.dataSource = this;
        }

        protected virtual void Initialized() { }

        protected virtual void Initialized(FasterList<T> data) { }

        public abstract float GetCellSize(Scroller scroller, int dataIndex);

        public abstract void SetCellView(ICellView cell, int index);

        public abstract ICellView GetCellView(Scroller scroller, int dataIndex, int cellIndex);

        public T DataByIndex(int dataIndex) => data[dataIndex];
    }
}