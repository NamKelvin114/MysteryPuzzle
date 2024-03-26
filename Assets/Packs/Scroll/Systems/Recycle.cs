/* Original license and copyright from file copied from https://github.com/MdIqubal/Recyclable-Scroll-Rect
 * Copyright (c) MdIqubal. All rights reserved.
 * Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/

using System.Collections;
using UnityEngine;

namespace Lance.UI
{
    public abstract class Recycle
    {
        public IScrollController dataSource;
        protected RectTransform viewport;
        protected RectTransform content;
        protected RectTransform prototypeCell;
        protected bool isGrid;
        protected float minPoolCoverage;
        protected uint minPoolSize;
        protected float recyclingThreshold;

        public abstract IEnumerator InitCoroutine(System.Action onInitialized = null);

        public abstract Vector2 OnValueChangedListener(Vector2 direction);
    }
}