using UnityEngine;

namespace KW.Core.Settings
{
    /// <summary>
    /// Centralized settings manager providing convenient access to ScriptableObject-based settings.
    /// UILayoutSettings and GameplaySettings are the only ScriptableObject-based settings;
    /// values that don't need per-environment tuning (Korean input tables, resource paths,
    /// scene names) live in static classes under KW.Core.Constants instead.
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// UI layout and positioning settings
        /// </summary>
        public static UILayoutSettings UI => UILayoutSettings.Instance;

        /// <summary>
        /// Gameplay mechanics and rules settings
        /// </summary>
        public static GameplaySettings Gameplay => GameplaySettings.Instance;

        /// <summary>
        /// Force reload all ScriptableObject settings from Resources
        /// Useful for testing or runtime configuration changes
        /// </summary>
        public static void ReloadAll()
        {
            UILayoutSettings.Reload();
            GameplaySettings.Reload();
        }
    }
}
