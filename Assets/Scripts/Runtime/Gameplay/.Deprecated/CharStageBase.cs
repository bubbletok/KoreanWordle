using System;
using System.Collections.Generic;
using UnityEngine;

using KW.Core;
using KW.Core.Settings;
using KW.Core.Constants;
using static KW.Core.Settings.GameplayEnums;

namespace KW.Gameplay
{
    /// <summary>
    /// Abstract base class for character stage management (answer selection and initialization)
    /// Eliminates duplication between CharBlockedStage and CharDeblockedStage
    /// </summary>
    [System.Obsolete("This class is not used anymore. Use GameWordSelector instead.", true)]
    public abstract class CharStageBase : MonoBehaviour
    {
        // Korean character arrays (shared by both modes)
        public string[] firstLetters;

        [Header("Word Configuration")]
        public int wordLen = 3;
        public int maxWordLen;
        public int curIndex;

        [SerializeField] protected GameStageType gameStageType;
        protected string originalAnswer = " ";

        /// <summary>
        /// Initialize the stage and select answer word
        /// </summary>
        protected virtual void Awake()
        {
            // Initialize from Constants
            firstLetters = KoreanInputConstants.ChosungLetters;
            maxWordLen = SettingsManager.Gameplay.MaxWordLength;

            if (GameManager.Instance != null)
            {
                while (originalAnswer == " ")
                {
                    GameManager.Instance.GameStageType = (GameStageType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(GameStageType)).Length);
                    originalAnswer = GetWord();
                }
            }

            InitializeWordData();
        }

        /// <summary>
        /// Initialize word-specific data structures (override in derived classes)
        /// </summary>
        protected abstract void InitializeWordData();

        /// <summary>
        /// Selects a word from the appropriate word list based on game type and stage
        /// This logic is identical between Blocked and Deblocked modes
        /// </summary>
        /// <returns>Selected word or " " if no valid word found</returns>
        public string GetWord()
        {
            gameStageType = GameManager.Instance.GameStageType;
            List<string> wordlist = GetWordListForGameType(gameStageType);

            // Calculate max available words based on current stage
            int maxNumber = GameManager.Instance.CurrentStage < wordlist.Count - 1
                ? GameManager.Instance.CurrentStage
                : wordlist.Count - 1;

            string word = " ";
            string emptyWord = " ";
            int idx = UnityEngine.Random.Range(0, maxNumber);

            // Validate word exists and hasn't been cleared
            if (wordlist[idx] == null)
                return emptyWord;

            if (GameManager.Instance.Data.ClearWordIndices[(GameManager.Instance.GameType, gameStageType - 1, idx)])
                return emptyWord;

            word = wordlist[idx];
            wordLen = word.Length;
            curIndex = idx;

            return word;
        }

        /// <summary>
        /// Gets the appropriate word list based on game type
        /// Centralized logic to avoid duplication
        /// </summary>
        protected List<string> GetWordListForGameType(GameStageType type)
        {
            switch (type)
            {
                case GameStageType.Noun: return GameManager.Instance.NounList;
                case GameStageType.Pronoun: return GameManager.Instance.PronounList;
                case GameStageType.Numeral: return GameManager.Instance.NumerList;
                case GameStageType.Verb: return GameManager.Instance.VerbList;
                case GameStageType.Adjective: return GameManager.Instance.AdjList;
                case GameStageType.Adverb: return GameManager.Instance.AdvList;
                default: return GameManager.Instance.NounList;
            }
        }

        /// <summary>
        /// Returns the decrypted answer (for validation and end game display)
        /// </summary>
        public string GetAnswer()
        {
            return originalAnswer;
        }

        /// <summary>
        /// Encrypts the answer word using the encryption offset
        /// Only needed for Blocked mode (to prevent memory inspection)
        /// </summary>
        protected List<int> EncryptAnswer(string answer)
        {
            List<int> encrypted = new List<int>();
            byte[] answerBytes = System.Text.Encoding.UTF8.GetBytes(answer);

            for (int i = 0; i < answerBytes.Length; i++)
            {
                encrypted.Add(System.Convert.ToInt32(answerBytes[i]) + SettingsManager.Gameplay.EncryptionOffset);
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypts an encrypted answer word
        /// </summary>
        protected string DecryptAnswer(List<int> encryptedAnswer)
        {
            List<byte> codedWord = new List<byte>();

            for (int i = 0; i < encryptedAnswer.Count; i++)
            {
                codedWord.Add((byte)(encryptedAnswer[i] - SettingsManager.Gameplay.EncryptionOffset));
            }

            return System.Text.Encoding.UTF8.GetString(codedWord.ToArray());
        }
    }
}
