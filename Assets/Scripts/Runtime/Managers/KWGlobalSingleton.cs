using UnityEngine;
using KW.Utility;

namespace KW.Managers
{
    public abstract class KWGlobalSingleton<T> : MonoBehaviour where T : KWGlobalSingleton<T>
    {
        private static T instance;
        private static readonly object instanceLock = new object();
        private static bool applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    KWDebug.LogWarning($"[KWGlobalSingleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = FindFirstObjectByType<T>();

                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            instance = singletonObject.AddComponent<T>();
                            singletonObject.name = $"[Singleton] {typeof(T).Name}";

                            KWDebug.Log($"[KWGlobalSingleton] Created new instance of {typeof(T).Name}");
                        }
                        else
                        {
                            KWDebug.Log($"[KWGlobalSingleton] Using existing instance of {typeof(T).Name}");
                        }
                    }

                    return instance;
                }
            }
        }

        public static bool Exists => instance != null;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnAwake() { }

        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                applicationIsQuitting = true;
            }
        }
    }
}
