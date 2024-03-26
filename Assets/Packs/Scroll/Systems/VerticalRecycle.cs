/* Original license and copyright from file copied from https://github.com/MdIqubal/Recyclable-Scroll-Rect
 * Copyright (c) MdIqubal. All rights reserved.
 * Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lance.Common;

namespace Lance.UI
{
    public class VerticalRecycle : Recycle
    {
        //Assigned by constructor
        private readonly int _coloumns;

        //Cell dimensions
        private float _cellWidth, _cellHeight;

        //Pool Generation
        private List<RectTransform> _cellPool;
        private List<ICellView> _cachedCells;
        private Bounds _recyclableViewBounds;

        //Temps, Flags 
        private readonly Vector3[] _corners = new Vector3[4];
        private bool _recycling;

        //Trackers
        private int _currentItemCount; //item count corresponding to the datasource.
        private int _topMostCellIndex; //Topmost and bottommost cell in the heirarchy
        private int _bottomMostCellIndex; //Topmost and bottommost cell in the heirarchy
        private int _topMostCellColoumn; // used for recyling in Grid layout. top-most and bottom-most coloumn
        private int _bottomMostCellColoumn; // used for recyling in Grid layout. top-most and bottom-most coloumn

        //Cached zero vector 
        private readonly Vector2 _zeroVector = Vector2.zero;

        #region initialize

        public VerticalRecycle(
            RectTransform prototypeCell,
            RectTransform viewport,
            RectTransform content,
            IScrollController dataSource,
            bool isGrid,
            int coloumns,
            float minPoolCoverage,
            uint minPoolSize,
            float recyclingThreshold)
        {
            this.prototypeCell = prototypeCell;
            this.viewport = viewport;
            this.content = content;
            this.isGrid = isGrid;
            this.minPoolCoverage = minPoolCoverage;
            this.minPoolSize = minPoolSize;
            this.recyclingThreshold = recyclingThreshold;
            this.dataSource = dataSource;
            _coloumns = isGrid ? coloumns : 1;
            _recyclableViewBounds = new Bounds();
        }

        /// <summary>
        /// Corotuine for initiazation.
        /// Using coroutine for init because few UI stuff requires a frame to update
        /// </summary>
        /// <param name="onInitialized">callback when init done</param>
        /// <returns></returns>>
        public override IEnumerator InitCoroutine(Action onInitialized)
        {
            SetTopAnchor(content);
            content.anchoredPosition = Vector3.zero;
            yield return null;
            SetRecyclingBounds();

            //Cell Poool
            CreateCellPool();
            _currentItemCount = _cellPool.Count;
            _topMostCellIndex = 0;
            _bottomMostCellIndex = _cellPool.Count - 1;

            //Set content height according to no of rows
            var noOfRows = (int) Mathf.Ceil(_cellPool.Count / (float) _coloumns);
            var contentYSize = noOfRows * _cellHeight;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentYSize);
            SetTopAnchor(content);

            onInitialized?.Invoke();
        }

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        private void SetRecyclingBounds()
        {
            viewport.GetWorldCorners(_corners);
            var threshHold = recyclingThreshold * (_corners[2].y - _corners[0].y);
            _recyclableViewBounds.min = new Vector3(_corners[0].x, _corners[0].y - threshHold);
            _recyclableViewBounds.max = new Vector3(_corners[2].x, _corners[2].y + threshHold);
        }

        /// <summary>
        /// Creates cell Pool for recycling, Caches ICells
        /// </summary>
        private void CreateCellPool()
        {
            //Reseting Pool
            if (_cellPool != null)
            {
                _cellPool.ForEach(item => UnityEngine.Object.Destroy(item.gameObject));
                _cellPool.Clear();
                _cachedCells.Clear();
            }
            else
            {
                _cachedCells = new List<ICellView>();
                _cellPool = new List<RectTransform>();
            }

            //Set the prototype cell active and set cell anchor as top 
            prototypeCell.gameObject.SetActive(true);
            if (isGrid)
            {
                SetTopLeftAnchor(prototypeCell);
            }
            else
            {
                SetTopAnchor(prototypeCell);
            }
            
            //Reset
            _topMostCellColoumn = _bottomMostCellColoumn = 0;

            //Temps
            float currentPoolCoverage = 0;
            var poolSize = 0;
            float posY = 0;

            //set new cell size according to its aspect ratio
            _cellWidth = content.rect.width / _coloumns;
            _cellHeight = prototypeCell.sizeDelta.y / prototypeCell.sizeDelta.x * _cellWidth;

            //Get the required pool coverage and mininum size for the Cell pool
            var requriedCoverage = minPoolCoverage * viewport.rect.height;
            var minSize = Math.Min(minPoolSize, dataSource.Count);

            //create cells untill the Pool area is covered and pool size is the minimum required
            while ((poolSize < minSize || currentPoolCoverage < requriedCoverage) && poolSize < dataSource.Count)
            {
                //Instantiate and add to Pool
                var item = (UnityEngine.Object.Instantiate(prototypeCell.gameObject)).GetComponent<RectTransform>();
                item.name = "Cell";
                item.sizeDelta = new Vector2(_cellWidth, _cellHeight);
                _cellPool.Add(item);
                item.SetParent(content, false);

                if (isGrid)
                {
                    var posX = _bottomMostCellColoumn * _cellWidth;
                    item.anchoredPosition = new Vector2(posX, posY);
                    if (++_bottomMostCellColoumn >= _coloumns)
                    {
                        _bottomMostCellColoumn = 0;
                        posY -= _cellHeight;
                        currentPoolCoverage += item.rect.height;
                    }
                }
                else
                {
                    item.anchoredPosition = new Vector2(0, posY);
                    posY = item.anchoredPosition.y - item.rect.height;
                    currentPoolCoverage += item.rect.height;
                }

                //Setting data for Cell
                _cachedCells.Add(item.GetComponent<ICellView>());
                dataSource.SetCellView(_cachedCells[_cachedCells.Count - 1], poolSize);

                //Update the Pool size
                poolSize++;
            }

            //TODO : you alrady have a _currentColoumn varaiable. Why this calculation?????
            if (isGrid)
            {
                _bottomMostCellColoumn = (_bottomMostCellColoumn - 1 + _coloumns) % _coloumns;
            }

            //Deactivate prototype cell if it is not a prefab(i.e it's present in scene)
            if (prototypeCell.gameObject.scene.IsValid())
            {
                prototypeCell.gameObject.SetActive(false);
            }
        }

        #endregion

        #region recycling

        /// <summary>
        /// Recyling entry point
        /// </summary>
        /// <param name="direction">scroll direction </param>
        /// <returns></returns>
        public override Vector2 OnValueChangedListener(Vector2 direction)
        {
            if (_recycling || _cellPool == null || _cellPool.Count == 0) return _zeroVector;

            //Updating Recyclable view bounds since it can change with resolution changes.
            SetRecyclingBounds();

            if (direction.y > 0 && _cellPool[_bottomMostCellIndex].MaxCornerY() > _recyclableViewBounds.min.y)
            {
                return RecycleTopToBottom();
            }
            else if (direction.y < 0 && _cellPool[_topMostCellIndex].MinCornerY() < _recyclableViewBounds.max.y)
            {
                return RecycleBottomToTop();
            }

            return _zeroVector;
        }

        /// <summary>
        /// Recycles cells from top to bottom in the List heirarchy
        /// </summary>
        private Vector2 RecycleTopToBottom()
        {
            _recycling = true;

            var n = 0;
            var posY = isGrid ? _cellPool[_bottomMostCellIndex].anchoredPosition.y : 0;

            //to determine if content size needs to be updated
            var additionalRows = 0;
            //Recycle until cell at Top is avaiable and current item count smaller than datasource
            while (_cellPool[_topMostCellIndex].MinCornerY() > _recyclableViewBounds.max.y && _currentItemCount < dataSource.Count)
            {
                if (isGrid)
                {
                    if (++_bottomMostCellColoumn >= _coloumns)
                    {
                        n++;
                        _bottomMostCellColoumn = 0;
                        posY = _cellPool[_bottomMostCellIndex].anchoredPosition.y - _cellHeight;
                        additionalRows++;
                    }

                    //Move top cell to bottom
                    var posX = _bottomMostCellColoumn * _cellWidth;
                    _cellPool[_topMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (++_topMostCellColoumn >= _coloumns)
                    {
                        _topMostCellColoumn = 0;
                        additionalRows--;
                    }
                }
                else
                {
                    //Move top cell to bottom
                    posY = _cellPool[_bottomMostCellIndex].anchoredPosition.y - _cellPool[_bottomMostCellIndex].sizeDelta.y;
                    _cellPool[_topMostCellIndex].anchoredPosition = new Vector2(_cellPool[_topMostCellIndex].anchoredPosition.x, posY);
                }

                //Cell for row at
                dataSource.SetCellView(_cachedCells[_topMostCellIndex], _currentItemCount);

                //set new indices
                _bottomMostCellIndex = _topMostCellIndex;
                _topMostCellIndex = (_topMostCellIndex + 1) % _cellPool.Count;

                _currentItemCount++;
                if (!isGrid) n++;
            }

            //Content size adjustment 
            if (isGrid)
            {
                content.sizeDelta += additionalRows * Vector2.up * _cellHeight;
                //TODO : check if it is supposed to be done only when > 0
                if (additionalRows > 0)
                {
                    n -= additionalRows;
                }
            }

            //Content anchor position adjustment.
            _cellPool.ForEach(cell => cell.anchoredPosition += n * Vector2.up * _cellPool[_topMostCellIndex].sizeDelta.y);
            content.anchoredPosition -= n * Vector2.up * _cellPool[_topMostCellIndex].sizeDelta.y;
            _recycling = false;
            return -new Vector2(0, n * _cellPool[_topMostCellIndex].sizeDelta.y);
        }

        /// <summary>
        /// Recycles cells from bottom to top in the List heirarchy
        /// </summary>
        private Vector2 RecycleBottomToTop()
        {
            _recycling = true;

            var n = 0;
            var posY = isGrid ? _cellPool[_topMostCellIndex].anchoredPosition.y : 0;

            //to determine if content size needs to be updated
            var additionalRows = 0;
            //Recycle until cell at bottom is avaiable and current item count is greater than cellpool size
            while (_cellPool[_bottomMostCellIndex].MaxCornerY() < _recyclableViewBounds.min.y && _currentItemCount > _cellPool.Count)
            {
                if (isGrid)
                {
                    if (--_topMostCellColoumn < 0)
                    {
                        n++;
                        _topMostCellColoumn = _coloumns - 1;
                        posY = _cellPool[_topMostCellIndex].anchoredPosition.y + _cellHeight;
                        additionalRows++;
                    }

                    //Move bottom cell to top
                    var posX = _topMostCellColoumn * _cellWidth;
                    _cellPool[_bottomMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (--_bottomMostCellColoumn < 0)
                    {
                        _bottomMostCellColoumn = _coloumns - 1;
                        additionalRows--;
                    }
                }
                else
                {
                    //Move bottom cell to top
                    posY = _cellPool[_topMostCellIndex].anchoredPosition.y + _cellPool[_topMostCellIndex].sizeDelta.y;
                    _cellPool[_bottomMostCellIndex].anchoredPosition = new Vector2(_cellPool[_bottomMostCellIndex].anchoredPosition.x, posY);
                    n++;
                }

                _currentItemCount--;

                //Cell for row at
                dataSource.SetCellView(_cachedCells[_bottomMostCellIndex], _currentItemCount - _cellPool.Count);

                //set new indices
                _topMostCellIndex = _bottomMostCellIndex;
                _bottomMostCellIndex = (_bottomMostCellIndex - 1 + _cellPool.Count) % _cellPool.Count;
            }

            if (isGrid)
            {
                content.sizeDelta += additionalRows * Vector2.up * _cellHeight;
                //TODOL : check if it is supposed to be done only when > 0
                if (additionalRows > 0)
                {
                    n -= additionalRows;
                }
            }

            _cellPool.ForEach(cell => cell.anchoredPosition -= n * Vector2.up * _cellPool[_topMostCellIndex].sizeDelta.y);
            content.anchoredPosition += n * Vector2.up * _cellPool[_topMostCellIndex].sizeDelta.y;
            _recycling = false;
            return new Vector2(0, n * _cellPool[_topMostCellIndex].sizeDelta.y);
        }

        #endregion

        #region helpers

        /// <summary>
        /// Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
        /// </summary>
        /// <param name="rectTransform"></param>
        private void SetTopAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;

            //Setting top anchor 
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        private void SetTopLeftAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;

            //Setting top anchor 
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        #endregion
    }
}