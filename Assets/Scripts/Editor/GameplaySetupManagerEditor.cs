using UnityEditor;
using UnityEngine;
using KW.Gameplay;

namespace KW.Editor
{
    [CustomEditor(typeof(GameplaySetupManager))]
    public class GameplaySetupManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GameplaySetupManager manager = (GameplaySetupManager)target;

            // Title
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Gameplay Setup Manager", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Manages initialization order for gameplay components. Components register callbacks with priority values (lower = earlier execution).", MessageType.Info);
            EditorGUILayout.Space(10);

            // Initialization State Section
            DrawSectionHeader("Initialization State");
            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("Has Initialized", manager.HasInitialized);
                EditorGUILayout.IntField("Callback Count", manager.CallbackCount);
                EditorGUI.EndDisabledGroup();

                if (manager.HasInitialized)
                {
                    EditorGUILayout.HelpBox("Initialization sequence has been executed.", MessageType.None);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Waiting for initialization. {manager.CallbackCount} callback(s) registered.", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Initialization state is tracked at runtime.", MessageType.None);
            }

            EditorGUILayout.Space(10);

            // Standard Initialization Order Section
            DrawSectionHeader("Standard Initialization Order");
            EditorGUILayout.HelpBox(
                "1.0 - GameWordSelector (Word selection)\n" +
                "2.0 - GameplayCellManager (Grid creation)\n" +
                "3.0 - GameplayUIManager (UI setup)\n" +
                "4.0 - KoreanInputHandler (Input system)\n" +
                "5.0 - HintManager (Hint initialization)",
                MessageType.None
            );

            EditorGUILayout.Space(10);

            // Usage Information Section
            DrawSectionHeader("Usage");
            EditorGUILayout.HelpBox(
                "Components register init callbacks:\n" +
                "  GameplaySetupManager.Instance.AddInit(time, callback, name)\n\n" +
                "GameplayManager calls:\n" +
                "  GameplaySetupManager.Instance.Initialize()\n\n" +
                "All callbacks execute in priority order.",
                MessageType.None
            );

            EditorGUILayout.Space(10);

            // Debug Actions Section (Runtime Only)
            if (Application.isPlaying)
            {
                DrawSectionHeader("Debug Actions");

                if (GUILayout.Button("Reset Initialization State"))
                {
                    manager.Reset();
                    Debug.Log("[GameplaySetupManager] Initialization state reset.");
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("Reset clears all callbacks and allows re-registration. Use for testing only.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSectionHeader(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(2);
        }
    }
}
