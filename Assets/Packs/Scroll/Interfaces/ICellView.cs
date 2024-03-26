namespace Lance.UI
{
    public interface ICellView
    {
        /// <summary>
        /// cell index
        /// </summary>
        int Index { get; }

        /// <summary>
        /// data index of cell
        /// </summary>
        int DataIndex { get; }

        /// <summary>
        /// refresh cell
        /// </summary>
        void Refresh();
    }
}