using UnityEditor;
using UnityEngine;
using KW.Core.Constants;
using KW.Core.Settings;

namespace KW.Editor
{
    /// <summary>
    /// Editor utility to create all game settings ScriptableObject assets
    /// Creates modular settings accessible via SettingsManager
    /// </summary>
    public static class GameSettingsCreator
    {
        [MenuItem("KW/Create Game Settings (Complete Setup)", priority = 0)]
        public static void CreateAllGameSettings()
        {
            // Create Resources folder if it doesn't exist
            string resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            // Create ScriptableObject settings (UI and Gameplay only)
            var uiLayout = CreateUILayoutSettings();
            var gameplay = CreateGameplaySettings();

            // Save all assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the UI Layout settings as the first one
            Selection.activeObject = uiLayout;
            EditorGUIUtility.PingObject(uiLayout);

            Debug.Log("✓ Game settings created successfully!");
            Debug.Log($"  - UILayoutSettings: {AssetDatabase.GetAssetPath(uiLayout)}");
            Debug.Log($"  - GameplaySettings: {AssetDatabase.GetAssetPath(gameplay)}");
            Debug.Log("\n✓ Access settings:");
            Debug.Log("  - ScriptableObject settings: SettingsManager.UI, SettingsManager.Gameplay");
            Debug.Log("  - Constant settings: KoreanInputConstants, ResourcePathsConstants, SceneNames");
        }

        private static UILayoutSettings CreateUILayoutSettings()
        {
            string path = "Assets/Resources/UILayoutSettings.asset";
            var existing = AssetDatabase.LoadAssetAtPath<UILayoutSettings>(path);
            if (existing != null) return existing;

            var settings = ScriptableObject.CreateInstance<UILayoutSettings>();

            // Set default values - Resolution Settings
            settings.targetWidth = 1080;
            settings.targetHeight = 2408;

            // Cell Grid Layout
            settings.cellDefaultSize = new UnityEngine.Vector2(180, 180);
            settings.cellSize5Letter = new UnityEngine.Vector2(145, 145);
            settings.cellGridSpacing = new UnityEngine.Vector2(10, 10);

            AssetDatabase.CreateAsset(settings, path);
            return settings;
        }

        private static GameplaySettings CreateGameplaySettings()
        {
            string path = "Assets/Resources/GameplaySettings.asset";
            var existing = AssetDatabase.LoadAssetAtPath<GameplaySettings>(path);
            if (existing != null) return existing;

            var settings = ScriptableObject.CreateInstance<GameplaySettings>();

            // Set default values
            settings.MaxAttempts = 6;
            settings.MaxWordLength = 5;
            settings.MaxGameTypes = 8;
            settings.StagesPerPage = 20;
            settings.MaxStagePages = 20;
            settings.StageGridColumns = 4;
            settings.StageGridRows = 5;
            settings.MaxStages = 50000;
            settings.MaxWordIndex = 50000;
            settings.DailyResetHour = 6;
            settings.EncryptionOffset = 214743673;
            settings.SaveFileName = "savefile.json";

            AssetDatabase.CreateAsset(settings, path);
            return settings;
        }


        #region Individual Settings Creators
        [MenuItem("KW/Settings/Create UI Layout Settings", priority = 100)]
        public static void CreateUILayoutSettingsOnly()
        {
            var settings = CreateUILayoutSettings();
            AssetDatabase.SaveAssets();
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }

        [MenuItem("KW/Settings/Create Gameplay Settings", priority = 101)]
        public static void CreateGameplaySettingsOnly()
        {
            var settings = CreateGameplaySettings();
            AssetDatabase.SaveAssets();
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }
        #endregion
    }
}
