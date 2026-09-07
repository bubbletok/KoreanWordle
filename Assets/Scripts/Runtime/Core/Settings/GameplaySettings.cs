using UnityEngine;
using KW.Managers;

namespace KW.Core.Settings
{
    /// <summary>
    /// Gameplay configuration settings
    /// Contains core game mechanics, rules, and progression settings
    /// </summary>
    [CreateAssetMenu(fileName = "GameplaySettings", menuName = "KW/Settings/Gameplay Settings")]
    public class GameplaySettings : KWSingletonScriptable<GameplaySettings>
    {
        [Header("Grid Configuration")]
        [Tooltip("Maximum number of attempts per game")]
        public int MaxAttempts = 6;

        [Tooltip("Maximum word length")]
        public int MaxWordLength = 5;

        [Tooltip("Maximum number of game types")]
        public int MaxGameTypes = 8;

        [Header("Stage Configuration")]
        [Tooltip("Number of stages per page in stage selection")]
        public int StagesPerPage = 20;

        [Tooltip("Maximum number of stage pages")]
        public int MaxStagePages = 20;

        [Tooltip("Number of columns in stage grid")]
        public int StageGridColumns = 4;

        [Tooltip("Number of rows in stage grid")]
        public int StageGridRows = 5;

        [Tooltip("Maximum total stages")]
        public int MaxStages = 50000;

        [Tooltip("Maximum word index")]
        public int MaxWordIndex = 50000;

        [Header("Daily Word Configuration")]
        [Tooltip("Hour of day when daily word resets (24-hour format)")]
        public int DailyResetHour = 6; // 6 AM

        [Header("Security")]
        [Tooltip("Encryption offset for answer obfuscation")]
        public int EncryptionOffset = 214743673;

        [Header("Save/Load Configuration")]
        [Tooltip("Save file name")]
        public string SaveFileName = "savefile.json";

        [Header("Cell State Colors")]
        [Tooltip("Color for empty/unused Jongsung (light gray)")]
        public Color EmptyColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Light Gray

        [Tooltip("Color for correct position (default: green)")]
        public Color CorrectColor = new Color(0.4f, 0.8f, 0.4f, 1f); // Green

        [Tooltip("Color for wrong position (default: yellow)")]
        public Color WrongPositionColor = new Color(0.8f, 0.8f, 0.4f, 1f); // Yellow

        [Tooltip("Color for not in word (default: gray)")]
        public Color NotInWordColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray

        [Tooltip("Default cell color (no state)")]
        public Color DefaultCellColor = Color.white;
    }
}
