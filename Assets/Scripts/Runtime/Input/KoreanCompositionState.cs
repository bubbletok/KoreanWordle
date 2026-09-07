using UnityEngine;

namespace KW.Input
{
    /// <summary>
    /// Manages Korean character composition state for Blocked mode
    /// Tracks 초성(first), 중성(middle), 종성(last) composition for each cell
    /// </summary>
    public class KoreanCompositionState
    {
        // Composition flags for each cell
        public bool[] IsFirstConsonant;   // 초성 입력 여부
        public bool[] IsMiddleVowel;      // 중성 입력 여부
        public bool[] IsLastConsonant;    // 종성 입력 여부

        // Character indices for each cell
        public int[] FirstConsonantIndex;  // 초성 인덱스 (0-18)
        public int[] MiddleVowelIndex;     // 중성 인덱스 (0-20)
        public int[] LastConsonantIndex;   // 종성 인덱스 (0-27)

        private readonly int maxWordLength;

        /// <summary>
        /// Initialize composition state arrays
        /// </summary>
        /// <param name="maxLength">Maximum word length</param>
        public KoreanCompositionState(int maxLength)
        {
            maxWordLength = maxLength;

            // Initialize flags
            IsFirstConsonant = new bool[maxLength];
            IsMiddleVowel = new bool[maxLength];
            IsLastConsonant = new bool[maxLength];

            // Initialize indices
            FirstConsonantIndex = new int[maxLength];
            MiddleVowelIndex = new int[maxLength];
            LastConsonantIndex = new int[maxLength];

            Reset();
        }

        /// <summary>
        /// Reset all composition state to default values
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < maxWordLength; i++)
            {
                IsFirstConsonant[i] = false;
                IsMiddleVowel[i] = false;
                IsLastConsonant[i] = false;

                FirstConsonantIndex[i] = 0;
                MiddleVowelIndex[i] = 0;
                LastConsonantIndex[i] = 0;
            }
        }

        /// <summary>
        /// Reset composition state for a specific cell
        /// </summary>
        public void ResetCell(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            IsFirstConsonant[cellIndex] = false;
            IsMiddleVowel[cellIndex] = false;
            IsLastConsonant[cellIndex] = false;

            FirstConsonantIndex[cellIndex] = 0;
            MiddleVowelIndex[cellIndex] = 0;
            LastConsonantIndex[cellIndex] = 0;
        }

        /// <summary>
        /// Check if a cell has any composition
        /// </summary>
        public bool HasAnyComposition(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return false;

            return IsFirstConsonant[cellIndex] || IsMiddleVowel[cellIndex] || IsLastConsonant[cellIndex];
        }

        /// <summary>
        /// Check if a cell has complete syllable (초성 + 중성)
        /// </summary>
        public bool HasCompleteSyllable(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return false;

            return IsFirstConsonant[cellIndex] && IsMiddleVowel[cellIndex];
        }

        /// <summary>
        /// Set first consonant for a cell
        /// </summary>
        public void SetFirstConsonant(int cellIndex, int consonantIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            FirstConsonantIndex[cellIndex] = consonantIndex;
            IsFirstConsonant[cellIndex] = true;
        }

        /// <summary>
        /// Set middle vowel for a cell
        /// </summary>
        public void SetMiddleVowel(int cellIndex, int vowelIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            MiddleVowelIndex[cellIndex] = vowelIndex;
            IsMiddleVowel[cellIndex] = true;
        }

        /// <summary>
        /// Set last consonant for a cell
        /// </summary>
        public void SetLastConsonant(int cellIndex, int consonantIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            LastConsonantIndex[cellIndex] = consonantIndex;
            IsLastConsonant[cellIndex] = true;
        }

        /// <summary>
        /// Clear last consonant for a cell
        /// </summary>
        public void ClearLastConsonant(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            LastConsonantIndex[cellIndex] = 0;
            IsLastConsonant[cellIndex] = false;
        }

        /// <summary>
        /// Clear middle vowel for a cell
        /// </summary>
        public void ClearMiddleVowel(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            MiddleVowelIndex[cellIndex] = 0;
            IsMiddleVowel[cellIndex] = false;
        }

        /// <summary>
        /// Clear first consonant for a cell
        /// </summary>
        public void ClearFirstConsonant(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return;

            FirstConsonantIndex[cellIndex] = 0;
            IsFirstConsonant[cellIndex] = false;
        }

        /// <summary>
        /// Get composition summary for debugging
        /// </summary>
        public string GetCompositionSummary(int cellIndex)
        {
            if (cellIndex < 0 || cellIndex >= maxWordLength)
                return "Invalid cell index";

            return $"Cell {cellIndex}: First={IsFirstConsonant[cellIndex]}({FirstConsonantIndex[cellIndex]}), " +
                   $"Middle={IsMiddleVowel[cellIndex]}({MiddleVowelIndex[cellIndex]}), " +
                   $"Last={IsLastConsonant[cellIndex]}({LastConsonantIndex[cellIndex]})";
        }
    }
}
