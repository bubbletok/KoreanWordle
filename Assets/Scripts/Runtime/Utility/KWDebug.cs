using System.Diagnostics;
using UnityEngine;

namespace KW.Utility
{
    /// <summary>
    /// Korean Wordle Debug utility class.
    /// Wraps Unity Debug with namespace prefixes and conditional compilation.
    /// Automatically disabled in release builds for performance.
    /// </summary>
    public static class KWDebug
    {
        private const string Prefix = "[KW]";

        // Color codes for Unity rich text
        private const string ColorInfo = "#00BFFF";      // Deep Sky Blue
        private const string ColorWarning = "#FFA500";   // Orange
        private const string ColorError = "#FF4500";     // Orange Red
        private const string ColorSuccess = "#32CD32";   // Lime Green

        #region Log

        /// <summary>
        /// Logs a message to the Unity Console.
        /// Only active in Development builds.
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log($"{Prefix} {message}");
        }

        /// <summary>
        /// Logs a message with a specific namespace tag.
        /// Example: [KW.Input] Message here
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message, string namespaceTag)
        {
            UnityEngine.Debug.Log($"{Prefix}.{namespaceTag} {message}");
        }

        /// <summary>
        /// Logs a message with context object.
        /// Clicking the log in Unity Console will highlight the context object.
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context)
        {
            UnityEngine.Debug.Log($"{Prefix} {message}", context);
        }

        /// <summary>
        /// Logs a colored message for better visibility.
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogColored(object message, string color)
        {
            UnityEngine.Debug.Log($"{Prefix} <color={color}>{message}</color>");
        }

        /// <summary>
        /// Logs a success message in green.
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogSuccess(object message)
        {
            UnityEngine.Debug.Log($"{Prefix} <color={ColorSuccess}>{message}</color>");
        }

        #endregion

        #region Warning

        /// <summary>
        /// Logs a warning message to the Unity Console.
        /// Active in both Development and Release builds.
        /// </summary>
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning($"{Prefix} {message}");
        }

        /// <summary>
        /// Logs a warning message with a specific namespace tag.
        /// </summary>
        public static void LogWarning(object message, string namespaceTag)
        {
            UnityEngine.Debug.LogWarning($"{Prefix}.{namespaceTag} {message}");
        }

        /// <summary>
        /// Logs a warning message with context object.
        /// </summary>
        public static void LogWarning(object message, Object context)
        {
            UnityEngine.Debug.LogWarning($"{Prefix} {message}", context);
        }

        #endregion

        #region Error

        /// <summary>
        /// Logs an error message to the Unity Console.
        /// Always active regardless of build type.
        /// </summary>
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError($"{Prefix} {message}");
        }

        /// <summary>
        /// Logs an error message with a specific namespace tag.
        /// </summary>
        public static void LogError(object message, string namespaceTag)
        {
            UnityEngine.Debug.LogError($"{Prefix}.{namespaceTag} {message}");
        }

        /// <summary>
        /// Logs an error message with context object.
        /// </summary>
        public static void LogError(object message, Object context)
        {
            UnityEngine.Debug.LogError($"{Prefix} {message}", context);
        }

        /// <summary>
        /// Logs an exception to the Unity Console.
        /// Always active regardless of build type.
        /// </summary>
        public static void LogException(System.Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        /// <summary>
        /// Logs an exception with context object.
        /// </summary>
        public static void LogException(System.Exception exception, Object context)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        #endregion

        #region Assertion

        /// <summary>
        /// Assert a condition and logs an error message if the condition is false.
        /// Only active in Development builds.
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError($"{Prefix} <color={ColorError}>Assertion Failed: {message}</color>");
            }
        }

        /// <summary>
        /// Assert a condition and logs an error message with context if the condition is false.
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message, Object context)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError($"{Prefix} <color={ColorError}>Assertion Failed: {message}</color>", context);
            }
        }

        #endregion

        #region Formatted Logging

        /// <summary>
        /// Logs a formatted message using string interpolation.
        /// Example: LogFormat("Player HP: {0}/{1}", currentHP, maxHP)
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat($"{Prefix} {format}", args);
        }

        /// <summary>
        /// Logs a formatted warning message.
        /// </summary>
        public static void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat($"{Prefix} {format}", args);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        public static void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat($"{Prefix} {format}", args);
        }

        #endregion

        #region Namespace-Specific Loggers

        /// <summary>
        /// Creates a namespace-specific logger with automatic tag prefixing.
        /// Example: var logger = KWDebug.GetLogger("Input");
        ///          logger.Log("Key pressed"); // Output: [KW.Input] Key pressed
        /// </summary>
        public static NamespaceLogger GetLogger(string namespaceTag)
        {
            return new NamespaceLogger(namespaceTag);
        }

        /// <summary>
        /// Namespace-specific logger that automatically adds namespace tags.
        /// </summary>
        public class NamespaceLogger
        {
            private readonly string namespaceTag;
            private readonly string fullPrefix;

            public NamespaceLogger(string namespaceTag)
            {
                this.namespaceTag = namespaceTag;
                fullPrefix = $"{Prefix}.{namespaceTag}";
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
            public void Log(object message)
            {
                UnityEngine.Debug.Log($"{fullPrefix} {message}");
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
            public void Log(object message, Object context)
            {
                UnityEngine.Debug.Log($"{fullPrefix} {message}", context);
            }

            public void LogWarning(object message)
            {
                UnityEngine.Debug.LogWarning($"{fullPrefix} {message}");
            }

            public void LogWarning(object message, Object context)
            {
                UnityEngine.Debug.LogWarning($"{fullPrefix} {message}", context);
            }

            public void LogError(object message)
            {
                UnityEngine.Debug.LogError($"{fullPrefix} {message}");
            }

            public void LogError(object message, Object context)
            {
                UnityEngine.Debug.LogError($"{fullPrefix} {message}", context);
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
            public void LogFormat(string format, params object[] args)
            {
                UnityEngine.Debug.LogFormat($"{fullPrefix} {format}", args);
            }

            public void LogWarningFormat(string format, params object[] args)
            {
                UnityEngine.Debug.LogWarningFormat($"{fullPrefix} {format}", args);
            }

            public void LogErrorFormat(string format, params object[] args)
            {
                UnityEngine.Debug.LogErrorFormat($"{fullPrefix} {format}", args);
            }
        }

        #endregion

        #region Performance Logging

        /// <summary>
        /// Logs a performance-related message (cyan color for visibility).
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogPerformance(object message)
        {
            UnityEngine.Debug.Log($"{Prefix} <color=#00FFFF>[PERF]</color> {message}");
        }

        /// <summary>
        /// Logs the execution time of an operation.
        /// Usage:
        /// var sw = System.Diagnostics.Stopwatch.StartNew();
        /// // ... operation ...
        /// KWDebug.LogExecutionTime("MyOperation", sw);
        /// </summary>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogExecutionTime(string operationName, System.Diagnostics.Stopwatch stopwatch)
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log($"{Prefix} <color=#00FFFF>[PERF]</color> {operationName} took {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion

        #region Draw (Editor Gizmos)

        /// <summary>
        /// Draws a debug ray in the Scene view.
        /// Only visible in Unity Editor.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0f)
        {
            UnityEngine.Debug.DrawRay(start, dir, color, duration);
        }

        /// <summary>
        /// Draws a debug line in the Scene view.
        /// Only visible in Unity Editor.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }

        #endregion
    }
}
