using System;
using UnityEngine;

namespace Lance.UI
{
    public abstract class CellView<T> : MonoBehaviour, ICellView
    {
        public int Index { get; private set; } // index cell
        public int DataIndex { get; private set; } // index of cell data
        protected Func<int, T> FuncDataByIndex { get; private set; }

        /// <summary>
        /// initialize cell
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="dataIndex"></param>
        public virtual void Initialized(int index, int dataIndex)
        {
            Index = index;
            DataIndex = dataIndex;
        }

        /// <summary>
        /// initialize cell
        /// </summary>
        /// <param name="funcDataByIndex"></param>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="dataIndex"></param>
        public virtual void Initialized(Func<int, T> funcDataByIndex, int index, int dataIndex)
        {
            Index = index;
            DataIndex = dataIndex;
            FuncDataByIndex = funcDataByIndex;
        }

        /// <summary>
        /// refresh cell view
        /// </summary>
        public abstract void Refresh();
    }
}