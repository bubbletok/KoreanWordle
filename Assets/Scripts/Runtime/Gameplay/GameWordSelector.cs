using System;
using System.Collections.Generic;
using UnityEngine;
using KW.Core;
using KW.Core.Settings;
using KW.Core.Constants;
using static KW.Core.Settings.GameplayEnums;
using KW.Input;
using KW.Managers;
using KW.Utility;
using UnityEngine.SceneManagement;
using KW.UI.Popup;

namespace KW.Gameplay
{
    /// <summary>
    /// Unified word selection component for both Blocked and Deblocked game modes
    /// Replaces CharBlockedStage and CharDeblockedStage with a single, configurable component
    /// </summary>
    public class GameWordSelector : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Game Mode Configuration")]
        [SerializeField] private GameType gameType = GameType.Blocked;

        [Header("Debug (Read-Only)")]
        [SerializeField] private string debugSelectedWord;
        [SerializeField] private GameStageType debugStageType;
        [SerializeField] private int debugWordIndex;
        #endregion

        #region Private Fields
        private string selectedWord = " ";
        private GameStageType currentStageType;
        private int wordIndex;

        // Blocked mode specific: encrypted answer
        private List<int> encryptedAnswer = new List<int>();
        private byte[] decryptBuffer;

        // Deblocked mode specific: entry tracking
        private bool[] entryFlags;

        // Blocked mode specific: Korean character composition state
        private bool[] isFirstConsonant, isMiddleVowel, isLastConsonant;
        private int[] firstConsonantIndex, middleVowelIndex, lastConsonantIndex;

        // Korean character index mapping (for Blocked mode)
        private readonly int[] firstLetterDiff = new int[]
            { 0, 1, 0, 2, 0, 0, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 6, 7, 8, 0, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        private readonly int[] lastLetterDiff = new int[]
            { 0, 1, 2, 3, 4, 5, 6, 7, 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 0, 18, 19, 20, 21, 22, 0, 23, 24, 25, 26, 27 };
        #endregion

        #region Public Properties
        /// <summary>
        /// Current word length (number of Korean characters)
        /// </summary>
        public int WordLength { get; private set; } = 3;

        /// <summary>
        /// Current word length of decomposed (number of Korean characters)
        /// </summary> 
        public int DecomposedWordLength { get; private set; }

        /// <summary>
        /// Maximum word length from settings
        /// </summary>
        public int MaxWordLength => SettingsManager.Gameplay.MaxWordLength;

        /// <summary>
        /// Index of the selected word in the word list
        /// </summary>
        public int WordIndex => wordIndex;

        /// <summary>
        /// Current game stage type (Noun, Verb, etc.)
        /// </summary>
        public GameStageType CurrentStageType => currentStageType;

        /// <summary>
        /// Current game type (Blocked or Deblocked)
        /// </summary>
        public GameType GameType => gameType;

        // Blocked mode accessors
        public bool[] IsFirstConsonant => isFirstConsonant;
        public bool[] IsMiddleVowel => isMiddleVowel;
        public bool[] IsLastConsonant => isLastConsonant;
        public int[] FirstConsonantIndex => firstConsonantIndex;
        public int[] MiddleVowelIndex => middleVowelIndex;
        public int[] LastConsonantIndex => lastConsonantIndex;
        public int[] FirstLetterDiffMapping => firstLetterDiff;
        public int[] LastLetterDiffMapping => lastLetterDiff;

        #endregion

        #region Lifecycle
        /// <summary>
        /// Initialize word selection and game mode specific data.
        /// Called by GameplaySetupManager in controlled order.
        /// </summary>
        public void Init()
        {
            // Get game type and stage type from GameplayManager
            if (GameplayManager.Exists)
            {
                gameType = GameplayManager.Instance.GameType;
                currentStageType = GameplayManager.Instance.GameStageType;
            }

            InitializeWordSelection();
            InitializeGameModeSpecificData();
        }
        #endregion

        #region Initialization
        private void InitializeWordSelection()
        {
            if (GameManager.Instance == null)
            {
                KWDebug.LogError("[StageWordSelector] GameManager.Instance is null!");
                return;
            }

            try
            {
                if (!TrySelectWord(out selectedWord))
                {
                    KWDebug.LogWarning("[StageWordSelector] 유효한 단어를 찾지 못했습니다.");
                    SceneManager.LoadScene(gameType==GameType.Blocked ? SceneNames.BLOCKED_MAIN : SceneNames.DEBLOCKED_MAIN);
                    PopupManager.Instance.ShowMessage("플레이할 단어가 없습니다.");
                }
            }
            catch (Exception e)
            {
                KWDebug.LogError($"An error occurred with :{e}");
            }

            debugSelectedWord = selectedWord;
            debugStageType = currentStageType;
            debugWordIndex = wordIndex;
        }

        /// <summary>
        /// Initialize mode-specific data structures
        /// </summary>
        private void InitializeGameModeSpecificData()
        {
            int maxLen = MaxWordLength;
            EncryptAnswer();
            if (gameType == GameType.Blocked)
            {
                // Initialize Korean character composition arrays
                isFirstConsonant = new bool[maxLen];
                isMiddleVowel = new bool[maxLen];
                isLastConsonant = new bool[maxLen];

                firstConsonantIndex = new int[maxLen];
                middleVowelIndex = new int[maxLen];
                lastConsonantIndex = new int[maxLen];

                // Initialize all to false/zero
                for (int i = 0; i < maxLen; i++)
                {
                    isFirstConsonant[i] = false;
                    isMiddleVowel[i] = false;
                    isLastConsonant[i] = false;

                    firstConsonantIndex[i] = 0;
                    middleVowelIndex[i] = 0;
                    lastConsonantIndex[i] = 0;
                }
            }
            else if (gameType == GameType.Deblocked)
            {

            }
        }
        #endregion

        #region Word Selection
        private bool TrySelectWord(out string result)
        {
            result = string.Empty;

            List<string> wordList = GetWordListForStageType(currentStageType);
            if (wordList == null || wordList.Count == 0)
                throw new Exception($"[StageWordSelector] Word list is null or empty for stage type: {currentStageType}");

            int maxIndex = Math.Min(GameManager.Instance.CurrentStage, wordList.Count - 1);
            if (maxIndex <= 0) maxIndex = 1;

            var candidates = new List<int>(maxIndex);
            for (int i = 0; i < maxIndex; i++)
            {
                if (!IsWordAlreadyCleared(i) && IsValidWord(wordList[i]))
                    candidates.Add(i);
            }

            if (candidates.Count == 0) return false;

            int idx = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            string word = wordList[idx];

            int decomposedLen = 0;
            foreach (char c in word)
            {
                HangulComposer.DecomposeHangul(c, out _, out _, out int jongsung);
                decomposedLen += jongsung > 0 ? 3 : 2;
            }

            WordLength = word.Length;
            DecomposedWordLength = decomposedLen;
            wordIndex = idx;
            result = word;
            return true;
        }

        private bool IsValidWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;

            int decomposedLength = 0;
            foreach (char c in word)
            {
                HangulComposer.DecomposeHangul(c, out _, out _, out int jongsung);
                if (gameType == GameType.Deblocked && HangulComposer.IsDoubleConsonantUnicode(jongsung))
                    return false;
                decomposedLength += jongsung > 0 ? 3 : 2;
            }

            return gameType != GameType.Deblocked || decomposedLength < 6;
        }

        /// <summary>
        /// Get the appropriate word list based on game stage type
        /// </summary>
        private List<string> GetWordListForStageType(GameStageType stageType)
        {
            switch (stageType)
            {
                case GameStageType.Noun:
                    return GameManager.Instance.NounList;
                case GameStageType.Pronoun:
                    return GameManager.Instance.PronounList;
                case GameStageType.Numeral:
                    return GameManager.Instance.NumerList;
                case GameStageType.Verb:
                    return GameManager.Instance.VerbList;
                case GameStageType.Adjective:
                    return GameManager.Instance.AdjList;
                case GameStageType.Adverb:
                    return GameManager.Instance.AdvList;
                default:
                    KWDebug.LogWarning($"[StageWordSelector] Unknown stage type: {stageType}, using default list");
                    return GameManager.Instance.DefaultList;
            }
        }

        /// <summary>
        /// Check if the word at the given index has already been cleared
        /// </summary>
        private bool IsWordAlreadyCleared(int idx)
        {
            var data = GameManager.Instance.Data;

            // Use safe access method from GameData_V2
            return data.GetClearWordIndices(gameType, currentStageType, idx);
        }
        #endregion

        #region Answer Management
        /// <summary>
        /// Get the answer word
        /// </summary>
        public string GetAnswer()
        {
            return DecryptAnswer();
        }

        /// <summary>
        /// Encrypt the answer word
        /// </summary>
        private void EncryptAnswer()
        {
            encryptedAnswer.Clear();
            byte[] answerBytes = System.Text.Encoding.UTF8.GetBytes(selectedWord);

            for (int i = 0; i < answerBytes.Length; i++)
            {
                int encrypted = System.Convert.ToInt32(answerBytes[i]) + SettingsManager.Gameplay.EncryptionOffset;
                encryptedAnswer.Add(encrypted);
            }
        }

        /// <summary>
        /// Decrypt the answer word
        /// </summary>
        private string DecryptAnswer()
        {
            if (encryptedAnswer == null || encryptedAnswer.Count == 0)
                return selectedWord;

            if (decryptBuffer == null || decryptBuffer.Length != encryptedAnswer.Count)
                decryptBuffer = new byte[encryptedAnswer.Count];

            int offset = SettingsManager.Gameplay.EncryptionOffset;
            for (int i = 0; i < encryptedAnswer.Count; i++)
                decryptBuffer[i] = (byte)(encryptedAnswer[i] - offset);

            return System.Text.Encoding.UTF8.GetString(decryptBuffer);
        }

        /// <summary>
        /// Get the encrypted answer
        /// </summary>
        public List<int> GetEncryptedAnswer()
        {
            return encryptedAnswer;
        }
        #endregion

        #region Editor Helpers
#if UNITY_EDITOR
        /// <summary>
        /// Force reselect a new word (for testing)
        /// </summary>
        [ContextMenu("Reselect Word")]
        private void ReselectWord()
        {
            selectedWord = " ";
            InitializeWordSelection();
            InitializeGameModeSpecificData();
            KWDebug.Log($"[StageWordSelector] New word selected: {debugSelectedWord} ({debugStageType})");
        }

        /// <summary>
        /// Show current answer (for debugging)
        /// </summary>
        [ContextMenu("Show Answer")]
        private void ShowAnswer()
        {
            KWDebug.Log($"[StageWordSelector] Answer: {GetAnswer()}");
        }
#endif
        #endregion
    }
}
