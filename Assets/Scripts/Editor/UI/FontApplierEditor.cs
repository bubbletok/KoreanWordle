using UnityEngine;
using UnityEditor;
using TMPro;
using KW.UI;

namespace KW.Editor.UI
{
    [CustomEditor(typeof(FontApplier))]
    public class FontApplierEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FontApplier applier = (FontApplier)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Font Now", GUILayout.Height(30)))
            {
                applier.ApplyFont();
                EditorUtility.SetDirty(applier.gameObject);
            }
        }
    }

    public class BulkFontApplierTool : EditorWindow
    {
        private bool addComponentIfMissing = true;
        private bool applyToActiveSceneOnly = true;

        [MenuItem("KW/Tools/Bulk Font Applier")]
        public static void ShowWindow()
        {
            GetWindow<BulkFontApplierTool>("Bulk Font Applier");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Bulk Font Applier Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "이 도구는 씬 내의 모든 TMP_Text 컴포넌트에 FontApplier를 추가하고 폰트를 일괄 적용합니다.\n" +
                "Prefab 모드에서도 사용 가능합니다.",
                MessageType.Info
            );

            EditorGUILayout.Space();

            addComponentIfMissing = EditorGUILayout.Toggle(
                "Add FontApplier if Missing",
                addComponentIfMissing
            );

            applyToActiveSceneOnly = EditorGUILayout.Toggle(
                "Apply to Active Scene Only",
                applyToActiveSceneOnly
            );

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply to All TMP_Text in Scene", GUILayout.Height(40)))
            {
                ApplyToAll();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply to Selected Objects Only", GUILayout.Height(30)))
            {
                ApplyToSelected();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);

            var allTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EditorGUILayout.LabelField($"Total TMP_Text in scene: {allTexts.Length}");

            int withApplier = 0;
            foreach (var text in allTexts)
            {
                if (text.GetComponent<FontApplier>() != null)
                    withApplier++;
            }
            EditorGUILayout.LabelField($"With FontApplier: {withApplier}");
            EditorGUILayout.LabelField($"Without FontApplier: {allTexts.Length - withApplier}");
        }

        private void ApplyToAll()
        {
            TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            if (allTexts.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "No TMP_Text Found",
                    "No TMP_Text components found in the scene.",
                    "OK"
                );
                return;
            }

            if (!EditorUtility.DisplayDialog(
                "Confirm Bulk Font Apply",
                $"Found {allTexts.Length} TMP_Text components.\n\n" +
                $"Add FontApplier: {addComponentIfMissing}\n\n" +
                "Continue?",
                "Yes",
                "Cancel"
            ))
            {
                return;
            }

            int processedCount = 0;
            int addedCount = 0;

            foreach (var text in allTexts)
            {
                FontApplier applier = text.GetComponent<FontApplier>();

                if (applier == null && addComponentIfMissing)
                {
                    applier = text.gameObject.AddComponent<FontApplier>();
                    addedCount++;
                }

                if (applier != null)
                {
                    applier.ApplyFont();
                    EditorUtility.SetDirty(text.gameObject);
                    processedCount++;
                }
            }

            EditorUtility.DisplayDialog(
                "Bulk Font Apply Complete",
                $"Processed: {processedCount}\n" +
                $"Added FontApplier: {addedCount}",
                "OK"
            );

            Debug.Log($"[BulkFontApplier] Processed {processedCount} TMP_Text components, added {addedCount} FontApplier components");
        }

        private void ApplyToSelected()
        {
            if (Selection.gameObjects.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Selection",
                    "Please select one or more GameObjects.",
                    "OK"
                );
                return;
            }

            int processedCount = 0;
            int addedCount = 0;

            foreach (var obj in Selection.gameObjects)
            {
                TMP_Text[] texts = obj.GetComponentsInChildren<TMP_Text>(true);

                foreach (var text in texts)
                {
                    FontApplier applier = text.GetComponent<FontApplier>();

                    if (applier == null && addComponentIfMissing)
                    {
                        applier = text.gameObject.AddComponent<FontApplier>();
                        addedCount++;
                    }

                    if (applier != null)
                    {
                        applier.ApplyFont();
                        EditorUtility.SetDirty(text.gameObject);
                        processedCount++;
                    }
                }
            }

            EditorUtility.DisplayDialog(
                "Bulk Font Apply Complete",
                $"Processed: {processedCount}\n" +
                $"Added FontApplier: {addedCount}",
                "OK"
            );

            Debug.Log($"[BulkFontApplier] Processed {processedCount} TMP_Text components in selected objects, added {addedCount} FontApplier components");
        }
    }
}
