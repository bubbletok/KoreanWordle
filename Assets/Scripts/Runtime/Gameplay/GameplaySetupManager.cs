using System;
using System.Collections.Generic;
using KW.Managers;
using KW.Utility;
using UnityEngine;

namespace KW.Gameplay
{
    public class GameplaySetupManager : KWLocalSingleton<GameplaySetupManager>
    {
        private readonly List<InitCallback> callbacks = new List<InitCallback>();
        private bool hasInitialized = false;

        private struct InitCallback : IComparable<InitCallback>
        {
            public float Time;
            public Action Callback;
            public string DebugName;

            public InitCallback(float time, Action callback, string debugName)
            {
                Time = time;
                Callback = callback;
                DebugName = debugName;
            }

            public int CompareTo(InitCallback other)
            {
                return Time.CompareTo(other.Time);
            }
        }

        public void AddInit(float time, Action callback, string debugName = null)
        {
            if (hasInitialized)
            {
                KWDebug.LogWarning($"[GameplaySetupManager] Cannot add init callback after initialization has started. Callback will not be executed.");
                return;
            }

            if (callback == null)
            {
                KWDebug.LogError($"[GameplaySetupManager] Cannot add null callback at time {time}");
                return;
            }

            string name = debugName ?? callback.Method.DeclaringType?.Name ?? "Unknown";
            callbacks.Add(new InitCallback(time, callback, name));
        }

        public void Initialize()
        {
            if (hasInitialized)
            {
                KWDebug.LogWarning("[GameplaySetupManager] Initialize() called multiple times. Ignoring.");
                return;
            }

            hasInitialized = true;

            callbacks.Sort();

            KWDebug.Log($"[GameplaySetupManager] Starting initialization sequence with {callbacks.Count} callbacks");

            for (int i = 0; i < callbacks.Count; i++)
            {
                InitCallback callback = callbacks[i];
                try
                {
                    KWDebug.Log($"[GameplaySetupManager] [{i + 1}/{callbacks.Count}] Executing {callback.DebugName} (time: {callback.Time})");
                    callback.Callback?.Invoke();
                }
                catch (Exception ex)
                {
                    KWDebug.LogError($"[GameplaySetupManager] Error executing callback {callback.DebugName} at time {callback.Time}: {ex}");
                }
            }

            KWDebug.Log("[GameplaySetupManager] Initialization sequence completed");
        }

        public void Reset()
        {
            callbacks.Clear();
            hasInitialized = false;
            KWDebug.Log("[GameplaySetupManager] Reset completed");
        }

        public int CallbackCount => callbacks.Count;
        public bool HasInitialized => hasInitialized;
    }
}
