using System;

namespace KW.Managers
{
    /// <summary>
    /// Generic singleton pattern for pure C# classes (non-MonoBehaviour)
    /// Provides thread-safe, lazy initialization without DontDestroyOnLoad
    /// Ideal for data managers, service classes, and non-Unity dependent systems
    /// </summary>
    /// <typeparam name="T">The type of the singleton (must inherit from KWPureSingleton and have a parameterless constructor)</typeparam>
    /// <example>
    /// Usage:
    /// public class DataManager : KWPureSingleton<DataManager>
    /// {
    ///     protected override void OnInitialize()
    ///     {
    ///         // Initialize your manager here
    ///     }
    /// }
    ///
    /// // Access anywhere:
    /// DataManager.Instance.DoSomething();
    /// </example>
    public abstract class KWPureSingleton<T> where T : KWPureSingleton<T>, new()
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() =>
        {
            T instance = new T();
            instance.OnInitialize();
            return instance;
        });

        /// <summary>
        /// Gets the singleton instance. Creates one if it doesn't exist.
        /// Thread-safe implementation using Lazy<T>.
        /// </summary>
        public static T Instance => lazyInstance.Value;

        /// <summary>
        /// Checks if the singleton instance has been created
        /// </summary>
        public static bool IsInitialized => lazyInstance.IsValueCreated;

        /// <summary>
        /// Protected constructor to prevent direct instantiation
        /// </summary>
        protected KWPureSingleton()
        {
            // Ensure only one instance can be created
            if (lazyInstance.IsValueCreated)
            {
                throw new InvalidOperationException(
                    $"Cannot create multiple instances of singleton type {typeof(T).Name}. " +
                    $"Use {typeof(T).Name}.Instance instead.");
            }
        }

        /// <summary>
        /// Override this method for initialization logic
        /// Called once when the singleton is first accessed
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Optional cleanup method that can be called manually
        /// Not automatically called - implement IDisposable if needed
        /// </summary>
        protected virtual void OnCleanup() { }
    }
}
