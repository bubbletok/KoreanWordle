using UnityEditor;
using System.Collections.Generic;

using KW.UI;
using KW.Core.Constants;
using UnityEngine;
using System.Reflection;

namespace KW.Editor
{
    [CustomEditor(typeof(LoadSceneButton))]
    public class LoadSceneButtonEditor : KWEditor<LoadSceneButton>
    {
        private SerializedProperty sceneNameProp;

        private List<string> sceneFieldNames;
        private List<string> sceneFieldValues;

        private int selectedIndex = 0;

        private void OnEnable()
        {
            sceneNameProp = serializedObject.FindProperty("SceneName");

            PopulateSceneLists();

            UpdateCurrentIndex();
        }

        // SceneNames 기반으로 필드 이름/값 목록을 채웁니다.
        private void PopulateSceneLists()
        {
            sceneFieldNames = new List<string>();
            sceneFieldValues = new List<string>();

            FieldInfo[] fields = typeof(SceneNames).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string) && field.IsLiteral)
                {
                    sceneFieldNames.Add(field.Name);
                    sceneFieldValues.Add((string)field.GetValue(null));
                }
            }
        }

        private void UpdateCurrentIndex()
        {
            if (sceneFieldValues == null || sceneFieldValues.Count == 0)
            {
                selectedIndex = 0;
                return;
            }

            int currentIndex = sceneFieldValues.IndexOf(sceneNameProp.stringValue);

            selectedIndex = Mathf.Max(0, currentIndex);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultInspector();
            EditorGUILayout.Space(10);

            if (sceneFieldNames == null || sceneFieldNames.Count == 0)
            {
                EditorGUILayout.HelpBox($"{typeof(SceneNames).Name} 클래스에서 public const string 필드를 찾을 수 없습니다.", MessageType.Warning);
                return;
            }

            // GUI 변경 사항을 감지
            EditorGUI.BeginChangeCheck();

            int newIndex = EditorGUILayout.Popup("Scene List", selectedIndex, sceneFieldNames.ToArray());

            if (EditorGUI.EndChangeCheck() || newIndex != selectedIndex)
            {
                selectedIndex = newIndex;

                // 'sceneName' 프로퍼티의 값을 선택된 필드의 값으로 변경합니다.
                sceneNameProp.stringValue = sceneFieldValues[selectedIndex];

                // 변경 사항을 타겟 오브젝트에 적용합니다
                serializedObject.ApplyModifiedProperties();

                // 씬 뷰에도 즉시 반영
                EditorUtility.SetDirty(target);
            }
        }

    }
}