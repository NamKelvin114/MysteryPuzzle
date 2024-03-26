/* Original license and copyright from file copied from https://github.com/MdIqubal/Recyclable-Scroll-Rect
 * Copyright (c) MdIqubal. All rights reserved.
 * Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lance.UI
{
    public class Scroller : ScrollRect
    {
        public IScrollController dataSource;
        public bool isGrid;

        /// <summary>
        /// //Prototype cell can either be a prefab or present as a child to the content(will automatically be disabled in runtime)
        /// </summary>
        [SerializeField] private RectTransform prototypeCell;
        [SerializeField] private float minPoolCoverage = 1.5f; // The recyclable pool must cover (viewPort * _poolCoverage) area.
        [SerializeField] private uint minPoolSize = 10; // Cell pool must have a min size
        [SerializeField] private float recyclingThreshold = 0.2f; //Threshold for recycling above and below viewport

        //If true the intiziation happens at Start. Controller must assign the datasource in Awake.
        //Set to false if self init is not required and use public init API.
        public bool selfInitialize = true;
        public EScrollDirection direction;

        //Segments : coloums for vertical and rows for horizontal.
        public int Segments { get => segments; set => segments = Math.Max(value, 2); }
        [SerializeField] private int segments;

        private Recycle _recyclingSystem;
        private Vector2 _prevAnchoredPos;

        protected override void Start()
        {
            //defafult(built-in) in scroll rect can have both directions enabled, Recyclable scroll rect can be scrolled in only one direction.
            //setting default as vertical, Initialize() will set this again. 
            vertical = true;
            horizontal = false;

            if (!Application.isPlaying) return;

            if (selfInitialize) Initialize();
        }

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize()
        {
            //Contruct the recycling system.
            if (direction == EScrollDirection.Vertical)
            {
                _recyclingSystem = new VerticalRecycle(prototypeCell,
                    viewport,
                    content,
                    dataSource,
                    isGrid,
                    Segments,
                    minPoolCoverage,
                    minPoolSize,
                    recyclingThreshold);
            }
            else if (direction == EScrollDirection.Horizontal)
            {
                _recyclingSystem = new HorizontalRecycle(prototypeCell,
                    viewport,
                    content,
                    dataSource,
                    isGrid,
                    Segments,
                    minPoolCoverage,
                    minPoolSize,
                    recyclingThreshold);
            }

            vertical = direction == EScrollDirection.Vertical;
            horizontal = direction == EScrollDirection.Horizontal;

            _prevAnchoredPos = content.anchoredPosition;
            onValueChanged.RemoveListener(OnValueChangedListener);
            //Adding listener after pool creation to avoid any unwanted recycling behaviour.(rare scenerio)
            StartCoroutine(_recyclingSystem.InitCoroutine(() => onValueChanged.AddListener(OnValueChangedListener)));
        }

        /// <summary>
        /// public API for Initializing when datasource is not set in controller's Awake. Make sure selfInitalize is set to false. 
        /// </summary>
        public void Initialize(IScrollController dataSource)
        {
            this.dataSource = dataSource;
            Initialize();
        }

        /// <summary>
        /// Added as a listener to the OnValueChanged event of Scroll rect.
        /// Recycling entry point for recyling systems.
        /// </summary>
        /// <param name="normalized"></param>
        public void OnValueChangedListener(Vector2 normalized)
        {
            var dir = content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(dir);
            _prevAnchoredPos = content.anchoredPosition;
        }
        
        /// <summary>
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData()
        {
            ReloadData(dataSource);
        }
        
        /// <summary>
        /// Overloaded ReloadData with dataSource param
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData(IScrollController dataSource)
        {
            if (_recyclingSystem != null)
            {
                StopMovement();
                onValueChanged.RemoveListener(OnValueChangedListener);
                _recyclingSystem.dataSource = dataSource;
                StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                    onValueChanged.AddListener(OnValueChangedListener)
                ));
                _prevAnchoredPos = content.anchoredPosition;
            }
        }
    }
}