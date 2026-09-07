using System.Collections.Generic;
using UnityEngine;

namespace KW.Utility
{
    /// <summary>
    /// Utility class for optimized coroutine yield instructions.
    /// Pre-caches WaitForSeconds instances to avoid repeated allocations.
    /// </summary>
    public static class CoroutineUtility
    {
        // Common fixed delays
        private static readonly WaitForSeconds waitForFixedUpdate = new(0.02f);
        private static readonly WaitForSeconds waitHalfSecond = new(0.5f);
        private static readonly WaitForSeconds waitOneSecond = new(1.0f);
        private static readonly WaitForSeconds waitTwoSeconds = new(2.0f);
        private static readonly WaitForSeconds waitThreeSeconds = new(3.0f);

        // Cache dictionary for dynamic delays
        private static readonly Dictionary<float, WaitForSeconds> waitForSecondsCache = new();

        // Other common yield instructions
        private static readonly WaitForEndOfFrame waitForEndOfFrame = new();
        private static readonly WaitForFixedUpdate waitForFixedUpdateYield = new();

        /// <summary>
        /// Gets a cached WaitForSeconds instance for the specified duration.
        /// Creates and caches new instances if not already cached.
        /// </summary>
        /// <param name="seconds">The duration in seconds</param>
        /// <returns>Cached WaitForSeconds instance</returns>
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            // Return pre-defined instances for common durations
            if (Mathf.Approximately(seconds, 0.02f)) return waitForFixedUpdate;
            if (Mathf.Approximately(seconds, 0.5f)) return waitHalfSecond;
            if (Mathf.Approximately(seconds, 1.0f)) return waitOneSecond;
            if (Mathf.Approximately(seconds, 2.0f)) return waitTwoSeconds;
            if (Mathf.Approximately(seconds, 3.0f)) return waitThreeSeconds;

            // Check cache for dynamic durations
            if (!waitForSecondsCache.TryGetValue(seconds, out WaitForSeconds wait))
            {
                wait = new WaitForSeconds(seconds);
                waitForSecondsCache[seconds] = wait;
            }

            return wait;
        }

        /// <summary>
        /// Gets a cached WaitForEndOfFrame instance.
        /// </summary>
        public static WaitForEndOfFrame WaitForEndOfFrame => waitForEndOfFrame;

        /// <summary>
        /// Gets a cached WaitForFixedUpdate instance.
        /// </summary>
        public static WaitForFixedUpdate WaitForFixedUpdate => waitForFixedUpdateYield;

        /// <summary>
        /// Pre-defined wait for one physics frame (0.02s @ 50 FPS).
        /// </summary>
        public static WaitForSeconds WaitForPhysicsFrame => waitForFixedUpdate;

        /// <summary>
        /// Pre-defined wait for half a second.
        /// </summary>
        public static WaitForSeconds WaitHalfSecond => waitHalfSecond;

        /// <summary>
        /// Pre-defined wait for one second.
        /// </summary>
        public static WaitForSeconds WaitOneSecond => waitOneSecond;

        /// <summary>
        /// Pre-defined wait for two seconds.
        /// </summary>
        public static WaitForSeconds WaitTwoSeconds => waitTwoSeconds;

        /// <summary>
        /// Pre-defined wait for three seconds.
        /// </summary>
        public static WaitForSeconds WaitThreeSeconds => waitThreeSeconds;

        /// <summary>
        /// Clears the dynamic WaitForSeconds cache.
        /// Call this if memory usage becomes a concern.
        /// </summary>
        public static void ClearCache()
        {
            waitForSecondsCache.Clear();
        }

        /// <summary>
        /// Gets the current cache size (number of cached WaitForSeconds instances).
        /// </summary>
        public static int CacheSize => waitForSecondsCache.Count;
    }
}