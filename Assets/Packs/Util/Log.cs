using System;
using System.Diagnostics;

namespace Worldreaver.Utility
{
    /// <summary>
    /// log everything
    /// </summary>
    public static class Log
    {
        private const string ENABLE_LOG = "ENABLE_LOG"; // flag on of log

#if UNITY_EDITOR
#if ENABLE_LOG
        [UnityEditor.MenuItem("Window/Worldreaver/Utility/Disable Log")]
        private static void DisableLog()
        {
            EditorUtility.EditorUtil.RemoveDefineSymbols(ENABLE_LOG);
        }
#else
         [UnityEditor.MenuItem("Window/Worldreaver/Utility/Enable Log")]
         private static void EnableLog()
         {
             EditorUtility.EditorUtil.AddDefineSymbols(ENABLE_LOG);
         }
#endif
#endif

        #region log

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional(ENABLE_LOG)]
        public static void Info(string header, string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(header + format, args);
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional(ENABLE_LOG)]
        public static void Info(string header, object message)
        {
            UnityEngine.Debug.Log(header + message);
        }

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional(ENABLE_LOG)]
        public static void Warning(string header, string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(header + format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional(ENABLE_LOG)]
        public static void Warning(string header, object message)
        {
            UnityEngine.Debug.LogWarning(header + message);
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional(ENABLE_LOG)]
        public static void Error(string header, string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(header + format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [Conditional(ENABLE_LOG)]
        public static void Error(string header, object message)
        {
            UnityEngine.Debug.LogError(header + message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="header">A header</param>
        /// <param name="e">Runtime Exception.</param>
        [Conditional(ENABLE_LOG)]
        public static void Exception(string header, Exception e)
        {
            UnityEngine.Debug.LogException(new Exception(header + e.Message, e.InnerException));
        }

        #endregion

        #region draw line

        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end)
        {
            UnityEngine.Debug.DrawLine(start, end);
        }

        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        /// <param name="color">Color of the line.</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color)
        {
            UnityEngine.Debug.DrawLine(start, end, color);
        }

        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="duration">How long the line should be visible for.</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color, float duration)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }

        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="duration">How long the line should be visible for.</param>
        /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color, float duration, bool depthTest)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
        }

        #endregion

        #region draw ray

        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="direction">Direction and length of the ray.</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawRay(UnityEngine.Vector3 start, UnityEngine.Vector3 direction)
        {
            UnityEngine.Debug.DrawRay(start, direction);
        }

        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="direction">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawRay(UnityEngine.Vector3 start, UnityEngine.Vector3 direction, UnityEngine.Color color)
        {
            UnityEngine.Debug.DrawRay(start, direction, color);
        }

        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="direction">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        /// <param name="duration">How long the line will be visible for (in seconds).</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawRay(UnityEngine.Vector3 start, UnityEngine.Vector3 direction, UnityEngine.Color color, float duration)
        {
            UnityEngine.Debug.DrawRay(start, direction, color, duration);
        }

        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="direction">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        /// <param name="duration">How long the line will be visible for (in seconds).</param>
        /// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
        [Conditional(ENABLE_LOG)]
        public static void DrawRay(UnityEngine.Vector3 start, UnityEngine.Vector3 direction, UnityEngine.Color color, float duration, bool depthTest)
        {
            UnityEngine.Debug.DrawRay(start, direction, color, duration, depthTest);
        }

        #endregion
    }
}