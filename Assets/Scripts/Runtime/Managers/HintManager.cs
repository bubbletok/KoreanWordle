using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using KW.Core;
using KW.Core.Settings;
using static KW.Core.Settings.GameplayEnums;
using KW.Utility;

namespace KW.Managers
{
    /// <summary>
    /// 게임 플레이 중 힌트(초성, 품사) 관리를 담당하는 매니저
    /// </summary>
    public class HintManager : KWLocalSingleton<HintManager>
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        [Header("Hint Settings")]
        [SerializeField] private bool showChoSeongHintByDefault = true;
        [SerializeField] private bool showPartOfSpeechHintByDefault = true;
        [SerializeField] private int maxRevealedChoSeong = 3;

        private Label choSeongHintText;
        private Label partOfSpeechHintText;

        private static readonly string[] ChoSeongLetters = new string[]
        {
            "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ",
            "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
        };

        private static readonly Dictionary<GameStageType, string> PartOfSpeechNames = new Dictionary<GameStageType, string>
        {
            { GameStageType.Noun, "명사" },
            { GameStageType.Pronoun, "대명사" },
            { GameStageType.Numeral, "수사" },
            { GameStageType.Verb, "동사" },
            { GameStageType.Adjective, "형용사" },
            { GameStageType.Adverb, "부사" }
        };

        private string currentAnswer;
        private GameStageType currentStageType;
        private bool isChoSeongHintVisible;
        private bool isPartOfSpeechHintVisible;
        private readonly StringBuilder hintBuilder = new StringBuilder(32);

        private bool IsDeblockedMode => GameplayManager.Exists && GameplayManager.Instance.GameType == GameType.Deblocked;

        protected override void OnAwake()
        {
            base.OnAwake();
            isChoSeongHintVisible = showChoSeongHintByDefault;
            isPartOfSpeechHintVisible = showPartOfSpeechHintByDefault;
        }

        public override void Init()
        {
            base.Init();

            if (document == null)
            {
                KWDebug.LogError("[HintManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            VisualElement root = document.rootVisualElement;
            choSeongHintText = root.Q<Label>("title-label");
            partOfSpeechHintText = root.Q<Label>("subtitle-label");
        }

        public void SetAnswer(string answer, GameStageType stageType)
        {
            currentAnswer = answer;
            currentStageType = stageType;
            UpdateHints();
            KWDebug.Log($"[HintManager] Hints set for answer: '{answer}', stage type: {stageType}");
        }

        private void UpdateHints()
        {
            UpdateChoSeongHint();
            UpdatePartOfSpeechHint();
        }

        private void UpdateChoSeongHint()
        {
            if (choSeongHintText == null) return;

            if (IsDeblockedMode)
            {
                choSeongHintText.text = "풀어쓰기";
                choSeongHintText.style.display = DisplayStyle.Flex;
                return;
            }

            if (string.IsNullOrEmpty(currentAnswer))
                return;

            int wordLength = currentAnswer.Length;
            int revealCount = CalculateRevealCount(wordLength);
            HashSet<int> hiddenSet = SelectHiddenIndices(wordLength, revealCount);

            hintBuilder.Clear();
            hintBuilder.Append("초성: ");
            for (int i = 0; i < wordLength; i++)
            {
                if (hiddenSet.Contains(i))
                {
                    hintBuilder.Append('?');
                }
                else
                {
                    int choSeongIndex = (currentAnswer[i] - 0xAC00) / 588;
                    if (choSeongIndex >= 0 && choSeongIndex < ChoSeongLetters.Length)
                        hintBuilder.Append(ChoSeongLetters[choSeongIndex]);
                    else
                        hintBuilder.Append('?');
                }
            }

            choSeongHintText.text = hintBuilder.ToString();
            choSeongHintText.style.display = isChoSeongHintVisible ? DisplayStyle.Flex : DisplayStyle.None;
            KWDebug.Log($"[HintManager] 초성 힌트 업데이트: {choSeongHintText.text}");
        }

        private int CalculateRevealCount(int wordLength)
        {
            if (wordLength <= 3)
                return wordLength;

            if (wordLength <= 5)
                return Mathf.Max(wordLength - 2, maxRevealedChoSeong);

            return Mathf.Max(wordLength / 2, maxRevealedChoSeong);
        }

        private HashSet<int> SelectHiddenIndices(int wordLength, int revealCount)
        {
            int hideCount = wordLength - revealCount;
            var hiddenSet = new HashSet<int>();

            if (hideCount <= 0)
                return hiddenSet;

            int[] indices = new int[wordLength];
            for (int i = 0; i < wordLength; i++) indices[i] = i;

            for (int i = wordLength - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }

            for (int i = 0; i < hideCount; i++)
                hiddenSet.Add(indices[i]);

            return hiddenSet;
        }

        private void UpdatePartOfSpeechHint()
        {
            if (partOfSpeechHintText == null)
                return;

            string hintText = "품사: ";

            if (PartOfSpeechNames.TryGetValue(currentStageType, out string partName))
            {
                hintText += partName;
            }
            else
            {
                hintText += "알 수 없음";
            }

            partOfSpeechHintText.text = hintText;
            partOfSpeechHintText.style.display = isPartOfSpeechHintVisible ? DisplayStyle.Flex : DisplayStyle.None;
            KWDebug.Log($"[HintManager] 품사 힌트 업데이트: {hintText}");
        }

        public void ToggleChoSeongHint()
        {
            if (IsDeblockedMode) return;

            isChoSeongHintVisible = !isChoSeongHintVisible;
            if (choSeongHintText != null)
                choSeongHintText.style.display = isChoSeongHintVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void TogglePartOfSpeechHint()
        {
            isPartOfSpeechHintVisible = !isPartOfSpeechHintVisible;
            if (partOfSpeechHintText != null)
                partOfSpeechHintText.style.display = isPartOfSpeechHintVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void ShowAllHints()
        {
            isChoSeongHintVisible = true;
            isPartOfSpeechHintVisible = true;
            UpdateHints();
        }

        public void HideAllHints()
        {
            isPartOfSpeechHintVisible = false;
            if (partOfSpeechHintText != null)
                partOfSpeechHintText.style.display = DisplayStyle.None;

            if (IsDeblockedMode) return;

            isChoSeongHintVisible = false;
            if (choSeongHintText != null)
                choSeongHintText.style.display = DisplayStyle.None;
        }

        public void ResetHints()
        {
            currentAnswer = null;
            isChoSeongHintVisible = showChoSeongHintByDefault;
            isPartOfSpeechHintVisible = showPartOfSpeechHintByDefault;

            if (partOfSpeechHintText != null)
            {
                partOfSpeechHintText.text = "품사: ";
                partOfSpeechHintText.style.display = isPartOfSpeechHintVisible ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (choSeongHintText == null) return;

            if (IsDeblockedMode)
            {
                choSeongHintText.text = "풀어쓰기";
                choSeongHintText.style.display = DisplayStyle.Flex;
                return;
            }

            choSeongHintText.text = "초성: ";
            choSeongHintText.style.display = isChoSeongHintVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
