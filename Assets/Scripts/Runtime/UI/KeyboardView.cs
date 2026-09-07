using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using KW.Input;
using KW.Core.Constants;
using KW.Core.Settings;
using KW.Utility;

namespace KW.UI
{
    public class KeyboardView : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        private VisualElement[] capsTopKeyElements;
        private VisualElement[] topKeyElements;

        private readonly Dictionary<string, KeyboardCell> keyDictionary = new();

        public bool IsCaps { get; private set; }

        private void Start()
        {
            if (document == null)
            {
                KWDebug.LogError("[KeyboardView] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            VisualElement root = document.rootVisualElement;

            capsTopKeyElements = BindRow(root, "key-capstop", KoreanInputConstants.CapsTopKeys);
            topKeyElements = BindRow(root, "key-top", KoreanInputConstants.TopKeys);
            BindRow(root, "key-mid", KoreanInputConstants.MiddleKeys);
            BindRow(root, "key-bottom", KoreanInputConstants.BottomKeys);

            SetCapsRowActive(false);

            root.Q<Button>("key-caps")?.RegisterCallback<ClickEvent>(_ => KoreanInputHandler.Instance.CapsWord());
            root.Q<Button>("key-backspace")?.RegisterCallback<ClickEvent>(_ => KoreanInputHandler.Instance.Backspace());
            root.Q<Button>("key-enter")?.RegisterCallback<ClickEvent>(_ => KoreanInputHandler.Instance.EnterWord());
        }

        private VisualElement[] BindRow(VisualElement root, string namePrefix, string[] letters)
        {
            var elements = new VisualElement[letters.Length];

            for (int i = 0; i < letters.Length; i++)
            {
                Button button = root.Q<Button>($"{namePrefix}-{i}");
                elements[i] = button;

                if (button == null)
                {
                    KWDebug.LogWarning($"[KeyboardView] Button '{namePrefix}-{i}' not found in UXML.");
                    continue;
                }

                var cell = new KeyboardCell();
                cell.Attach(button, letters[i]);
                keyDictionary[letters[i]] = cell;
            }

            return elements;
        }

        public void ToggleCaps()
        {
            IsCaps = !IsCaps;
            SetCapsRowActive(IsCaps);
        }

        private void SetCapsRowActive(bool capsActive)
        {
            SetRowDisplay(capsTopKeyElements, capsActive);
            SetRowDisplay(topKeyElements, !capsActive);
        }

        private static void SetRowDisplay(VisualElement[] elements, bool visible)
        {
            if (elements == null) return;

            foreach (VisualElement element in elements)
            {
                if (element != null)
                    element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public void SetKeyCellState(string key, GameplayEnums.CellState cellState)
        {
            if (!keyDictionary.ContainsKey(key)) return;

            keyDictionary[key].SetKeyCellState(key, cellState);
        }
    }
}
