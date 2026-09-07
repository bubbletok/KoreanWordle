using UnityEngine;
using UnityEditor;
using KW.Core;
using KW.Managers;

namespace KW.Editor.Core
{
    /// <summary>
    /// Custom editor to display GameData_V2 debugging information in Inspector
    /// </summary>
    [CustomEditor(typeof(GameManager))]
    public class GameDataDebugger : UnityEditor.Editor
    {
        private bool showGameData = true;
        private bool showLastStagePage = true;
        private bool showAttemptStats = true;
        private bool showClearStats = true;
        private bool showWordIndices = false;
        private bool showClearStages = false;

        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();

            GameManager gameManager = (GameManager)target;
            if (gameManager.Data == null)
            {
                EditorGUILayout.HelpBox("GameData is null. Play the game to initialize.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("GameData Debugger", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            showGameData = EditorGUILayout.Foldout(showGameData, "GameData Information", true);
            if (showGameData)
            {
                EditorGUI.indentLevel++;

                // DateTime
                EditorGUILayout.LabelField("Last Played", gameManager.Data.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                EditorGUILayout.Space(5);

                // Last Stage Page
                showLastStagePage = EditorGUILayout.Foldout(showLastStagePage, "Last Stage Page", true);
                if (showLastStagePage)
                {
                    EditorGUI.indentLevel++;
                    foreach (var kvp in gameManager.Data.LastStagePage)
                    {
                        EditorGUILayout.LabelField($"{kvp.Key}", kvp.Value.ToString());
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);

                // Attempt Stats
                showAttemptStats = EditorGUILayout.Foldout(showAttemptStats, "Attempt Statistics", true);
                if (showAttemptStats)
                {
                    EditorGUI.indentLevel++;

                    // Attempt Game Count
                    EditorGUILayout.LabelField("Total Attempts:", EditorStyles.boldLabel);
                    foreach (var kvp in gameManager.Data.AttempGameCount)
                    {
                        EditorGUILayout.LabelField($"  {kvp.Key}", kvp.Value.ToString());
                    }

                    EditorGUILayout.Space(3);

                    // Attempt to Clear by Stage Type
                    EditorGUILayout.LabelField("Attempts by Stage & Try:", EditorStyles.boldLabel);
                    if (gameManager.Data.AttemptToClearGameStage.Count > 0)
                    {
                        foreach (var kvp in gameManager.Data.AttemptToClearGameStage)
                        {
                            EditorGUILayout.LabelField($"  {kvp.Key.Item1} (Try {kvp.Key.Item2})", kvp.Value.ToString());
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("  (No data)", EditorStyles.miniLabel);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);

                // Clear Stats
                showClearStats = EditorGUILayout.Foldout(showClearStats, "Clear Statistics", true);
                if (showClearStats)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("Total Clears:", EditorStyles.boldLabel);
                    foreach (var kvp in gameManager.Data.ClearGameCount)
                    {
                        EditorGUILayout.LabelField($"  {kvp.Key}", kvp.Value.ToString());
                    }

                    EditorGUILayout.Space(3);

                    // Success Rate
                    EditorGUILayout.LabelField("Success Rate:", EditorStyles.boldLabel);
                    foreach (GameType gameType in System.Enum.GetValues(typeof(GameType)))
                    {
                        int attempts = gameManager.Data.GetAttempGameCount(gameType);
                        int clears = gameManager.Data.GetClearGameCount(gameType);
                        float rate = attempts > 0 ? (float)clears / attempts * 100f : 0f;
                        EditorGUILayout.LabelField($"  {gameType}", $"{clears}/{attempts} ({rate:F1}%)");
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);

                // Word Indices (can be very large, collapsed by default)
                showWordIndices = EditorGUILayout.Foldout(showWordIndices, $"Clear Word Indices (Count: {gameManager.Data.ClearWordIndices.Count})", true);
                if (showWordIndices)
                {
                    EditorGUI.indentLevel++;
                    if (gameManager.Data.ClearWordIndices.Count > 0)
                    {
                        int displayCount = 0;
                        foreach (var key in gameManager.Data.ClearWordIndices)
                        {
                            EditorGUILayout.LabelField($"  {key.Item1} - {key.Item2} [Index: {key.Item3}]", "Cleared");
                            displayCount++;
                            if (displayCount > 50)
                            {
                                EditorGUILayout.LabelField($"  ... and {gameManager.Data.ClearWordIndices.Count - 50} more", EditorStyles.miniLabel);
                                break;
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("  (No data)", EditorStyles.miniLabel);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);

                // Clear Stages (can be very large, collapsed by default)
                showClearStages = EditorGUILayout.Foldout(showClearStages, $"Clear Stages (Count: {gameManager.Data.ClearStages.Count})", true);
                if (showClearStages)
                {
                    EditorGUI.indentLevel++;
                    if (gameManager.Data.ClearStages.Count > 0)
                    {
                        int displayCount = 0;
                        foreach (var kvp in gameManager.Data.ClearStages)
                        {
                            string status = kvp.Value.Item1 ? "Cleared" : "Unlocked";
                            EditorGUILayout.LabelField($"  {kvp.Key.Item1} Stage {kvp.Key.Item2 + 1}", $"{status} (Attempts: {kvp.Value.Item2})");
                            displayCount++;
                            if (displayCount > 50) // Limit display to 50 entries
                            {
                                EditorGUILayout.LabelField($"  ... and {gameManager.Data.ClearStages.Count - 50} more", EditorStyles.miniLabel);
                                break;
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("  (No data)", EditorStyles.miniLabel);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Utility Buttons
            EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);
            if (GUILayout.Button("Clear All Game Data"))
            {
                if (EditorUtility.DisplayDialog("Clear Game Data",
                    "Are you sure you want to clear all game data? This cannot be undone.",
                    "Clear", "Cancel"))
                {
                    gameManager.Data.Clear();
                    gameManager.SaveData(gameManager.Data);
                    EditorUtility.DisplayDialog("Success", "Game data cleared successfully.", "OK");
                }
            }

            if (GUILayout.Button("Save Data Now"))
            {
                gameManager.SaveData(gameManager.Data);
                EditorUtility.DisplayDialog("Success", "Game data saved successfully.", "OK");
            }

            if (GUILayout.Button("Reload Data"))
            {
                gameManager.LoadData();
                EditorUtility.DisplayDialog("Success", "Game data reloaded successfully.", "OK");
            }
        }
    }
}
