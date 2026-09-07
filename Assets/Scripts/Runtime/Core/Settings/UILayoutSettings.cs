using UnityEngine;
using TMPro;
using KW.Managers;

namespace KW.Core.Settings
{
    /// <summary>
    /// UI Layout configuration settings
    /// Contains all positioning, spacing, and scaling values for UI elements
    /// </summary>
    [CreateAssetMenu(fileName = "UILayoutSettings", menuName = "KW/Settings/UI Layout Settings")]
    public class UILayoutSettings : KWSingletonScriptable<UILayoutSettings>
    {
        [Header("Font Settings")]
        [Tooltip("Default TMP font for UI")]
        public TMP_FontAsset defaultFont;

        [Tooltip("Default font size for normal text")]
        public float defaultFontSize = 36f;

        [Tooltip("Font size for stage numbers")]
        public float stageNumberFontSize = 48f;

        [Tooltip("Font size for cell text")]
        public float cellFontSize = 60f;

        [Tooltip("Font size for hint text")]
        public float hintFontSize = 28f;

        [Header("Resolution Settings")]
        public int targetWidth = 1080;
        public int targetHeight = 2408;

        [Header("Cell Grid Layout")]
        public Vector2 cellDefaultSize = new Vector2(180, 180);
        public Vector2 cellSize5Letter = new Vector2(145, 145);
        public Vector2 cellGridSpacing = new Vector2(10, 10);

        [Header("Stage Cell Colors")]
        [Tooltip("Color for uncompleted stage cells")]
        public Color uncompleteCellColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        [Tooltip("Color for completed stage cells")]
        public Color completeCellColor = new Color(0.6f, 0.9f, 0.6f, 1f);

        [Tooltip("Color for fully completed stage cells (cleared in 1 attempt)")]
        public Color fullCompleteCellColor = new Color(1f, 0.84f, 0f, 1f);

        [Tooltip("Color for attempt stars on completed stages")]
        public Color completeTryColor = new Color(1f, 0.8f, 0f, 1f);

        [Tooltip("Color for attempt stars on fully completed stages")]
        public Color fullCompleteTryColor = new Color(1f, 1f, 0f, 1f);
    }
}
