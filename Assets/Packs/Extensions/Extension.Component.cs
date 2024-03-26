using UnityEngine;
using UnityEngine.UI;

namespace Lance.Common
{
    public static partial class Util
    {
        /// <summary>
        /// add blank button
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Button AddBlankButtonComponent(this GameObject target)
        {
            var button = target.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            return button;
        }
    }
}