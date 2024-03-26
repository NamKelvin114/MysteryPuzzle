namespace Lance.UI
{
    public interface IScrollController
    {
        /// <summary>
        /// returns the number of cells available
        /// </summary>
        uint Count { get; }

        /// <summary>
        /// return cell size
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        float GetCellSize(Scroller scroller, int dataIndex);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        void SetCellView(ICellView cell, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        ICellView GetCellView(Scroller scroller, int dataIndex, int cellIndex);
    }
}