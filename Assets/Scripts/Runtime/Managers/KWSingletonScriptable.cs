using UnityEngine;
using KW.Utility;

namespace KW.Managers
{
    public abstract class KWSingletonScriptable<T> : ScriptableObject where T : KWSingletonScriptable<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    string resourcePath = GetResourcePath();
                    instance = Resources.Load<T>(resourcePath);

                    if (instance == null)
                    {
                        KWDebug.LogError($"[KWSingletonScriptable] {typeof(T).Name} asset not found at 'Resources/{resourcePath}'! " +
                                         $"Please create the asset via CreateAssetMenu or ensure it's in the correct location.");
                    }
                    else
                    {
                        KWDebug.Log($"[KWSingletonScriptable] Loaded {typeof(T).Name} from Resources/{resourcePath}");
                    }
                }

                return instance;
            }
        }

        public static bool Exists => instance != null;

        protected static string GetResourcePath()
        {
            return typeof(T).Name;
        }

        public static void Reload()
        {
            instance = null;
            _ = Instance;
        }

        protected virtual void OnEnable()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                KWDebug.LogWarning($"[KWSingletonScriptable] Multiple instances of {typeof(T).Name} found. " +
                                   "Only one should exist in Resources folder.");
            }
        }

        protected virtual void OnValidate() { }
    }
}
