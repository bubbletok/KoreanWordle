using UnityEditor;
using UnityEngine;
using KW.Gameplay;
using KW.Managers;

namespace KW.Editor
{
    [CustomEditor(typeof(GameWordSelector))]
    public class GameWordSelectorEditor : UnityEditor.Editor
    {
        private SerializedProperty gameTypeProp;
        private SerializedProperty debugSelectedWordProp;
        private SerializedProperty debugStageTypeProp;
        private SerializedProperty debugWordIndexProp;

        private void OnEnable()
        {
            gameTypeProp = serializedObject.FindProperty("gameType");
            debugSelectedWordProp = serializedObject.FindProperty("debugSelectedWord");
            debugStageTypeProp = serializedObject.FindProperty("debugStageType");
            debugWordIndexProp = serializedObject.FindProperty("debugWordIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GameWordSelector selector = (GameWordSelector)target;

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Game Word Selector", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Unified word selection for both Blocked and Deblocked modes. Replaces CharBlockedStage/CharDeblockedStage.", MessageType.Info);
            EditorGUILayout.Space(10);

            DrawSectionHeader("Configuration");
            EditorGUILayout.PropertyField(gameTypeProp, new GUIContent("Game Type", "Game mode (Blocked/Deblocked)"));
            EditorGUILayout.Space(10);

            DrawSectionHeader("Debug Information (Read-Only)");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(debugSelectedWordProp, new GUIContent("Selected Word", "Currently selected word"));
            EditorGUILayout.PropertyField(debugStageTypeProp, new GUIContent("Stage Type", "Word category (Noun/Verb/etc.)"));
            EditorGUILayout.PropertyField(debugWordIndexProp, new GUIContent("Word Index", "Index in word list"));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);

            if (Application.isPlaying)
            {
                DrawSectionHeader("Runtime Properties");
                EditorGUILayout.LabelField("Word Length", selector.WordLength.ToString());
                EditorGUILayout.LabelField("Max Word Length", selector.MaxWordLength.ToString());
                EditorGUILayout.LabelField("Current Stage Type", selector.CurrentStageType.ToString());
                EditorGUILayout.LabelField("Game Type", selector.GameType.ToString());

                EditorGUILayout.Space(5);
                if (GUILayout.Button("Show Answer (Console)"))
                {
                    Debug.Log($"[GameWordSelector] Current Answer: {selector.GetAnswer()}");
                }
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.Space(10);
                DrawSectionHeader("Mode-Specific Data");

                if (selector.GameType == GameType.Blocked)
                {
                    EditorGUILayout.LabelField("Mode", "Blocked (모아쓰기)");
                    EditorGUILayout.LabelField("Encrypted Answer Length", selector.GetEncryptedAnswer()?.Count.ToString() ?? "0");

                    if (selector.IsFirstConsonant != null)
                    {
                        EditorGUILayout.LabelField("Composition Arrays", "Initialized");
                    }
                }
                else if (selector.GameType == GameType.Deblocked)
                {
                    EditorGUILayout.LabelField("Mode", "Deblocked (풀어쓰기)");
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("Mode-specific data structures are initialized at runtime.", MessageType.None);
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
