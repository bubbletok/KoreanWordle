using UnityEngine;
using KW.Utility;

namespace KW.Managers
{
    public abstract class KWLocalSingleton<T> : MonoBehaviour where T : KWLocalSingleton<T>
    {
        private static T instance;

        public static T Instance => instance;
        public static bool Exists => instance != null;

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                KWDebug.LogWarning($"[KWLocalSingleton] Multiple instances of {typeof(T).Name} found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            OnSceneDestroy();
        }

        protected virtual void OnAwake() { }
        public virtual void Init() { }
        protected virtual void OnSceneDestroy() { }
    }
}
