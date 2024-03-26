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
    public class HorizontalRecycle : Recycle
    {
        //Assigned by constructor
        private readonly int _rows;

        //Cell dimensions
        private float _cellWidth;
        private float _cellHeight;

        //Pool Generation
        private List<RectTransform> _cellPool;
        private List<ICellView> _cachedCells;
        private Bounds _recyclableViewBounds;

        //Temps, Flags
        private readonly Vector3[] _corners = new Vector3[4];
        private bool _recycling;

        //Trackers
        private int _currentItemCount; //item count corresponding to the datasource.
        private int _leftMostCellIndex; //Topmost and bottommost cell in the List
        private int _rightMostCellIndex; //Topmost and bottommost cell in the List
        private int _leftMostCellRow; // used for recyling in Grid layout. leftmost and rightmost row
        private int _rightMostCellRow; // used for recyling in Grid layout. leftmost and rightmost row

        //Cached zero vector 
        private readonly Vector2 _zeroVector = Vector2.zero;

        #region initialize

        public HorizontalRecycle(
            RectTransform prototypeCell,
            RectTransform viewport,
            RectTransform content,
            IScrollController dataSource,
            bool isGrid,
            int rows,
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
            _rows = isGrid ? rows : 1;
            _recyclableViewBounds = new Bounds();
        }

        /// <summary>
        /// Corotuine for initiazation.
        /// Using coroutine for init because few UI stuff requires a frame to update
        /// </summary>
        /// <param name="onInitialized">callback when init done</param>
        /// <returns></returns>
        public override IEnumerator InitCoroutine(Action onInitialized)
        {
            //Setting up container and bounds
            SetLeftAnchor(content);
            content.anchoredPosition = Vector3.zero;
            yield return null;
            SetRecyclingBounds();

            //Cell Poool
            CreateCellPool();
            _currentItemCount = _cellPool.Count;
            _leftMostCellIndex = 0;
            _rightMostCellIndex = _cellPool.Count - 1;

            //Set content width according to no of coloums
            var coloums = Mathf.CeilToInt((float) _cellPool.Count / _rows);
            var contentXSize = coloums * _cellWidth;
            content.sizeDelta = new Vector2(contentXSize, content.sizeDelta.y);
            SetLeftAnchor(content);

            onInitialized?.Invoke();
        }

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        private void SetRecyclingBounds()
        {
            viewport.GetWorldCorners(_corners);
            float threshHold = recyclingThreshold * (_corners[2].x - _corners[0].x);
            _recyclableViewBounds.min = new Vector3(_corners[0].x - threshHold, _corners[0].y);
            _recyclableViewBounds.max = new Vector3(_corners[2].x + threshHold, _corners[2].y);
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
            SetLeftAnchor(prototypeCell);

            //set new cell size according to its aspect ratio
            _cellHeight = content.rect.height / _rows;
            _cellWidth = prototypeCell.sizeDelta.x / prototypeCell.sizeDelta.y * _cellHeight;

            //Reset
            _leftMostCellRow = _rightMostCellRow = 0;
            
            //Temps
            float currentPoolCoverage = 0;
            int poolSize = 0;
            float posX = 0;
            float posY = 0;

            //Get the required pool coverage and mininum size for the Cell pool
            float requriedCoverage = minPoolCoverage * viewport.rect.width;
            uint minSize = Math.Min(minPoolSize, dataSource.Count);

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
                    posY = -_rightMostCellRow * _cellHeight;
                    item.anchoredPosition = new Vector2(posX, posY);
                    if (++_rightMostCellRow >= _rows)
                    {
                        _rightMostCellRow = 0;
                        posX += _cellWidth;
                        currentPoolCoverage += item.rect.width;
                    }
                }
                else
                {
                    item.anchoredPosition = new Vector2(posX, 0);
                    posX = item.anchoredPosition.x + item.rect.width;
                    currentPoolCoverage += item.rect.width;
                }

                //Setting data for Cell
                _cachedCells.Add(item.GetComponent<ICellView>());
                dataSource.SetCellView(_cachedCells[_cachedCells.Count - 1], poolSize);

                //Update the Pool size
                poolSize++;
            }

            if (isGrid)
            {
                _rightMostCellRow = (_rightMostCellRow - 1 + _rows) % _rows;
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

            if (direction.x < 0 && _cellPool[_rightMostCellIndex].MinCornerX() < _recyclableViewBounds.max.x)
            {
                return RecycleLeftToRight();
            }
            else if (direction.x > 0 && _cellPool[_leftMostCellIndex].MaxCornerX() > _recyclableViewBounds.min.x)
            {
                return RecycleRightToleft();
            }

            return _zeroVector;
        }

        /// <summary>
        /// Recycles cells from Left to Right in the List heirarchy
        /// </summary>
        private Vector2 RecycleLeftToRight()
        {
            _recycling = true;

            var n = 0;
            var posX = isGrid ? _cellPool[_rightMostCellIndex].anchoredPosition.x : 0;

            //to determine if content size needs to be updated
            var additionalColoums = 0;

            //Recycle until cell at left is avaiable and current item count smaller than datasource
            while (_cellPool[_leftMostCellIndex].MaxCornerX() < _recyclableViewBounds.min.x && _currentItemCount < dataSource.Count)
            {
                if (isGrid)
                {
                    if (++_rightMostCellRow >= _rows)
                    {
                        n++;
                        _rightMostCellRow = 0;
                        posX = _cellPool[_rightMostCellIndex].anchoredPosition.x + _cellWidth;
                        additionalColoums++;
                    }

                    //Move Left most cell to right
                    var posY = -_rightMostCellRow * _cellHeight;
                    _cellPool[_leftMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (++_leftMostCellRow >= _rows)
                    {
                        _leftMostCellRow = 0;
                        additionalColoums--;
                    }
                }
                else
                {
                    //Move Left most cell to right
                    posX = _cellPool[_rightMostCellIndex].anchoredPosition.x + _cellPool[_rightMostCellIndex].sizeDelta.x;
                    _cellPool[_leftMostCellIndex].anchoredPosition = new Vector2(posX, _cellPool[_leftMostCellIndex].anchoredPosition.y);
                }

                //Cell for row at
                dataSource.SetCellView(_cachedCells[_leftMostCellIndex], _currentItemCount);

                //set new indices
                _rightMostCellIndex = _leftMostCellIndex;
                _leftMostCellIndex = (_leftMostCellIndex + 1) % _cellPool.Count;

                _currentItemCount++;
                if (!isGrid) n++;
            }

            //Content size adjustment 
            if (isGrid)
            {
                content.sizeDelta += additionalColoums * Vector2.right * _cellWidth;
                if (additionalColoums > 0)
                {
                    n -= additionalColoums;
                }
            }

            //Content anchor position adjustment.
            _cellPool.ForEach(cell => cell.anchoredPosition -= n * Vector2.right * _cellPool[_leftMostCellIndex].sizeDelta.x);
            content.anchoredPosition += n * Vector2.right * _cellPool[_leftMostCellIndex].sizeDelta.x;
            _recycling = false;
            return n * Vector2.right * _cellPool[_leftMostCellIndex].sizeDelta.x;
        }

        /// <summary>
        /// Recycles cells from Right to Left in the List heirarchy
        /// </summary>
        private Vector2 RecycleRightToleft()
        {
            _recycling = true;

            var n = 0;
            var posX = isGrid ? _cellPool[_leftMostCellIndex].anchoredPosition.x : 0;

            //to determine if content size needs to be updated
            var additionalColoums = 0;
            //Recycle until cell at Right end is avaiable and current item count is greater than cellpool size
            while (_cellPool[_rightMostCellIndex].MinCornerX() > _recyclableViewBounds.max.x && _currentItemCount > _cellPool.Count)
            {
                if (isGrid)
                {
                    if (--_leftMostCellRow < 0)
                    {
                        n++;
                        _leftMostCellRow = _rows - 1;
                        posX = _cellPool[_leftMostCellIndex].anchoredPosition.x - _cellWidth;
                        additionalColoums++;
                    }

                    //Move Right most cell to left
                    var posY = -_leftMostCellRow * _cellHeight;
                    _cellPool[_rightMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (--_rightMostCellRow < 0)
                    {
                        _rightMostCellRow = _rows - 1;
                        additionalColoums--;
                    }
                }
                else
                {
                    //Move Right most cell to left
                    posX = _cellPool[_leftMostCellIndex].anchoredPosition.x - _cellPool[_leftMostCellIndex].sizeDelta.x;
                    _cellPool[_rightMostCellIndex].anchoredPosition = new Vector2(posX, _cellPool[_rightMostCellIndex].anchoredPosition.y);
                    n++;
                }

                _currentItemCount--;
                //Cell for row at
                dataSource.SetCellView(_cachedCells[_rightMostCellIndex], _currentItemCount - _cellPool.Count);

                //set new indices
                _leftMostCellIndex = _rightMostCellIndex;
                _rightMostCellIndex = (_rightMostCellIndex - 1 + _cellPool.Count) % _cellPool.Count;
            }

            //Content size adjustment
            if (isGrid)
            {
                content.sizeDelta += additionalColoums * Vector2.right * _cellWidth;
                if (additionalColoums > 0)
                {
                    n -= additionalColoums;
                }
            }

            //Content anchor position adjustment.
            _cellPool.ForEach(cell => cell.anchoredPosition += n * Vector2.right * _cellPool[_leftMostCellIndex].sizeDelta.x);
            content.anchoredPosition -= n * Vector2.right * _cellPool[_leftMostCellIndex].sizeDelta.x;
            _recycling = false;
            return -n * Vector2.right * _cellPool[_leftMostCellIndex].sizeDelta.x;
        }

        #endregion

        #region helpers

        /// <summary>
        /// Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
        /// </summary>
        /// <param name="rectTransform"></param>
        private void SetLeftAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;

            var pos = isGrid ? new Vector2(0, 1) : new Vector2(0, 0.5f);

            //Setting top anchor 
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        #endregion
    }
}