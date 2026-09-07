using UnityEngine;
using KW.Utility;

namespace KW.Input
{
    /// <summary>
    /// Static utility class for Korean Hangul character composition and decomposition
    /// Handles Unicode calculations and character validation
    /// </summary>
    public static class HangulComposer
    {
        // Unicode ranges for Korean characters
        public static readonly int HANGUL_BASE = 0xAC00;          // '가'
        public static readonly int CHOSUNG_BASE = 0x3131;         // 'ㄱ'
        public static readonly int JUNGSUNG_BASE = 0x314F;        // 'ㅏ'

        public static readonly int CHOSUNG_COUNT = 19;
        public static readonly int JUNGSUNG_COUNT = 21;
        public static readonly int JONGSUNG_COUNT = 28;

        // Special consonants that cannot be final consonants
        public static readonly int SSANGDIGEUT = 0x3138;  // ㄸ
        public static readonly int SSANGBIEUP = 0x3143;   // ㅃ
        public static readonly int SSANGJIEUT = 0x3149;   // ㅉ

        private static readonly int[] firstLetterDiff = {
            0, 1, 0, 2, 0, 0, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 6, 7, 8, 0, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18
        };

        private static readonly int[] lastLetterDiff = {
            0, 1, 2, 3, 4, 5, 6, 7, 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 0, 18, 19, 20, 21, 22, 0, 23, 24, 25, 26, 27
        };

        private static readonly char[] chosungChars = {
            'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ',
            'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
        };

        // index 0 = empty, 1-29 = consonants (matches ConsonantCodeToJongsungIndex output)
        private static readonly char[] jongsungChars = {
            ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄸ', 'ㄹ',
            'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅃ',
            'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
        };

        /// <summary>
        /// Get the jamo char for a chosung index (0–18)
        /// </summary>
        public static char GetChosungChar(int chosungIndex) => chosungChars[chosungIndex];

        /// <summary>
        /// Get the jamo char for a jungsung index (0–20)
        /// </summary>
        public static char GetJungsungChar(int jungsungIndex) => (char)(JUNGSUNG_BASE + jungsungIndex);

        /// <summary>
        /// Get the jamo char for a jongsung internal index (0 = empty, 1–29 = consonants)
        /// </summary>
        public static char GetJongsungChar(int jongsungIndex) => jongsungChars[jongsungIndex];

        /// <summary>
        /// Compose a complete Korean syllable from 초성, 중성, 종성 indices
        /// </summary>
        public static char ComposeHangul(int chosungIndex, int jungsungIndex, int jongsungIndex)
        {
            int jongsungAdjusted = lastLetterDiff[jongsungIndex];
            int unicode = HANGUL_BASE + (chosungIndex * JUNGSUNG_COUNT * JONGSUNG_COUNT) + (jungsungIndex * JONGSUNG_COUNT) + jongsungAdjusted;
            return System.Convert.ToChar(unicode);
        }

        /// <summary>
        /// Returns the adjusted jongsung index used in Unicode composition
        /// </summary>
        public static int GetJongsungMappedIndex(int jongsungIndex)
        {
            return lastLetterDiff[jongsungIndex];
        }

        /// <summary>
        /// Compose a Korean syllable with only 초성 and 중성 (no final consonant)
        /// </summary>
        public static char ComposeHangulWithoutJongsung(int chosungIndex, int jungsungIndex)
        {
            int unicode = HANGUL_BASE + (chosungIndex * JUNGSUNG_COUNT * JONGSUNG_COUNT) + (jungsungIndex * JONGSUNG_COUNT);
            return System.Convert.ToChar(unicode);
        }

        /// <summary>
        /// Check if a character is a Korean consonant (초성/종성)
        /// </summary>
        public static bool IsConsonant(int charCode)
        {
            return charCode >= 0x3131 && charCode <= 0x314E;
        }

        /// <summary>
        /// Check if a character is a Korean vowel (중성)
        /// </summary>
        public static bool IsVowel(int charCode)
        {
            return charCode >= 0x314F && charCode <= 0x3163;
        }

        /// <summary>
        /// Check if a character is a Korean character
        /// </summary>
        /// <returns></returns>
        public static bool IsKoreanChar(int charCode)
        {
            return charCode >= 0x3131 && charCode <= 0x3163;
        }

        /// <summary>
        /// Check if a consonant can be used as a final consonant (종성)
        /// ㄸ, ㅃ, ㅉ cannot be final consonants
        /// </summary>
        public static bool CanBeFinalConsonant(int charCode)
        {
            if (!IsConsonant(charCode))
                return false;

            return charCode != SSANGDIGEUT && charCode != SSANGBIEUP && charCode != SSANGJIEUT;
        }

        /// <summary>
        /// Get consonant index from character code
        /// </summary>
        public static int GetConsonantIndex(int charCode)
        {
            if (!IsConsonant(charCode))
                return -1;

            int offset = charCode - CHOSUNG_BASE;
            if (offset < 0 || offset >= firstLetterDiff.Length)
                return -1;

            return firstLetterDiff[offset];
        }

        /// <summary>
        /// Get vowel index from character code
        /// </summary>
        public static int GetVowelIndex(int charCode)
        {
            if (!IsVowel(charCode))
                return -1;

            return charCode - JUNGSUNG_BASE;
        }

        /// <summary>
        /// Check if a final consonant is a double consonant (곁받침)
        /// jongsungIndex: internal representation from ConsonantCodeToJongsungIndex
        /// </summary>
        public static bool IsDoubleConsonant(int jongsungIndex)
        {
            // 곁받침: ㄳ(3), ㄵ(5), ㄶ(6), ㄺ-ㅀ(10-16), ㅄ(20)
            return jongsungIndex == 3 || jongsungIndex == 5 || jongsungIndex == 6 ||
                   (jongsungIndex >= 10 && jongsungIndex <= 16) || jongsungIndex == 20;
        }

        /// <summary>
        /// Check if a final consonant is a double consonant using Unicode jongsung index (from DecomposeHangul)
        /// unicodeJongsungIndex: 0-27 range returned by DecomposeHangul
        /// </summary>
        public static bool IsDoubleConsonantUnicode(int unicodeJongsungIndex)
        {
            // 곁받침: ㄳ(3), ㄵ(5), ㄶ(6), ㄺ-ㅀ(9-15), ㅄ(18)
            return unicodeJongsungIndex == 3 || unicodeJongsungIndex == 5 || unicodeJongsungIndex == 6 ||
                   (unicodeJongsungIndex >= 9 && unicodeJongsungIndex <= 15) || unicodeJongsungIndex == 18;
        }

        /// <summary>
        /// Split a double consonant into two separate consonants
        /// Returns the first consonant index (to keep) and second consonant index (to move to next cell)
        /// </summary>
        public static (int firstPart, int secondPart) SplitDoubleConsonant(int jongsungIndex)
        {
            switch (jongsungIndex)
            {
                case 3:  // ㄳ -> ㄱ + ㅅ
                    return (1, 9);
                case 5:  // ㄵ -> ㄴ + ㅈ
                    return (4, 12);
                case 6:  // ㄶ -> ㄴ + ㅎ
                    return (4, 18);
                case 10: // ㄺ -> ㄹ + ㄱ
                    return (9, 0);
                case 11: // ㄻ -> ㄹ + ㅁ
                    return (9, 6);
                case 12: // ㄼ -> ㄹ + ㅂ
                    return (9, 7);
                case 13: // ㄽ -> ㄹ + ㅅ
                    return (9, 9);
                case 14: // ㄾ -> ㄹ + ㅌ
                    return (9, 16);
                case 15: // ㄿ -> ㄹ + ㅍ
                    return (9, 17);
                case 16: // ㅀ -> ㄹ + ㅎ
                    return (9, 18);
                case 20: // ㅄ -> ㅂ + ㅅ
                    return (18, 9);
                default:
                    KWDebug.LogWarning($"[HangulComposer] Invalid double consonant index: {jongsungIndex}");
                    return (jongsungIndex, -1);
            }
        }

        /// <summary>
        /// Simplify a double consonant to single consonant (for backspace)
        /// </summary>
        public static int SimplifyDoubleConsonant(int jongsungIndex)
        {
            switch (jongsungIndex)
            {
                case 3:  // ㄳ -> ㄱ
                    return 1;
                case 5:  // ㄵ -> ㄴ
                case 6:  // ㄶ -> ㄴ
                    return 4;
                case 10: // ㄺ -> ㄹ
                case 11: // ㄻ -> ㄹ
                case 12: // ㄼ -> ㄹ
                case 13: // ㄽ -> ㄹ
                case 14: // ㄾ -> ㄹ
                case 15: // ㄿ -> ㄹ
                case 16: // ㅀ -> ㄹ
                    return 9;
                case 20: // ㅄ -> ㅂ
                    return 18;
                default:
                    return jongsungIndex; // Not a double consonant
            }
        }

        /// <summary>
        /// Check if a double vowel can be formed
        /// </summary>
        public static bool CanFormDoubleVowel(int currentVowelIndex, string inputChar)
        {
            // ㅗ + ㅏ/ㅐ/ㅣ -> ㅘ/ㅙ/ㅚ
            if (currentVowelIndex == 8)
                return inputChar == "ㅏ" || inputChar == "ㅐ" || inputChar == "ㅣ";

            // ㅜ + ㅓ/ㅔ/ㅣ -> ㅝ/ㅞ/ㅟ
            if (currentVowelIndex == 13)
                return inputChar == "ㅓ" || inputChar == "ㅔ" || inputChar == "ㅣ";

            // ㅡ + ㅣ -> ㅢ
            if (currentVowelIndex == 18)
                return inputChar == "ㅣ";

            return false;
        }

        /// <summary>
        /// Get the resulting double vowel index
        /// </summary>
        public static int GetDoubleVowelIndex(int currentVowelIndex, string inputChar)
        {
            // ㅗ combinations
            if (currentVowelIndex == 8)
            {
                if (inputChar == "ㅏ") return 9;   // ㅘ
                if (inputChar == "ㅐ") return 10;  // ㅙ
                if (inputChar == "ㅣ") return 11;  // ㅚ
            }

            // ㅜ combinations
            if (currentVowelIndex == 13)
            {
                if (inputChar == "ㅓ") return 14;  // ㅝ
                if (inputChar == "ㅔ") return 15;  // ㅞ
                if (inputChar == "ㅣ") return 16;  // ㅟ
            }

            // ㅡ combination
            if (currentVowelIndex == 18)
            {
                if (inputChar == "ㅣ") return 19;  // ㅢ
            }

            return currentVowelIndex; // No change
        }

        /// <summary>
        /// Simplify a double vowel (for backspace)
        /// </summary>
        public static int SimplifyDoubleVowel(int vowelIndex)
        {
            // ㅘ, ㅙ, ㅚ -> ㅗ
            if (vowelIndex >= 9 && vowelIndex <= 11)
                return 8;

            // ㅝ, ㅞ, ㅟ -> ㅜ
            if (vowelIndex >= 14 && vowelIndex <= 16)
                return 13;

            // ㅢ -> ㅡ
            if (vowelIndex == 19)
                return 18;

            return vowelIndex; // Not a double vowel
        }

        /// <summary>
        /// Check if a vowel is a double vowel
        /// </summary>
        public static bool IsDoubleVowel(int vowelIndex)
        {
            return (vowelIndex >= 9 && vowelIndex <= 11) ||
                   (vowelIndex >= 14 && vowelIndex <= 16) ||
                   vowelIndex == 19;
        }

        /// <summary>
        /// Decompose a complete Korean syllable into 초성, 중성, 종성 indices
        /// </summary>
        public static void DecomposeHangul(char syllable, out int chosungIndex, out int jungsungIndex, out int jongsungIndex)
        {
            int order = syllable - HANGUL_BASE;
            chosungIndex = order / (JUNGSUNG_COUNT * JONGSUNG_COUNT);
            jungsungIndex = order % (JUNGSUNG_COUNT * JONGSUNG_COUNT) / JONGSUNG_COUNT;
            jongsungIndex = order % JONGSUNG_COUNT;
        }

        /// <summary>
        /// Get chosung index from a jongsung storage index (종성 인덱스 → 초성 인덱스)
        /// Used when moving a jongsung to the next cell as a new chosung
        /// </summary>
        public static int GetChosungIndexFromJongsung(int jongsungIndex)
        {
            return firstLetterDiff[jongsungIndex - 1];
        }

        /// <summary>
        /// Convert a consonant Unicode char code to a jongsung storage index
        /// </summary>
        public static int ConsonantCodeToJongsungIndex(int charCode)
        {
            return (charCode - CHOSUNG_BASE) + 1;
        }

        /// <summary>
        /// Try to form a double consonant (곁받침)
        /// Returns the new jongsung index if successful, or -1 if not possible
        /// </summary>
        public static int TryFormDoubleConsonant(int currentJongsungIndex, string inputChar)
        {
            // ㄱ + ㅅ -> ㄳ
            if (currentJongsungIndex == 1 && inputChar == "ㅅ")
                return 3;

            // ㄴ + ㅈ -> ㄵ
            if (currentJongsungIndex == 4 && inputChar == "ㅈ")
                return 5;

            // ㄴ + ㅎ -> ㄶ
            if (currentJongsungIndex == 4 && inputChar == "ㅎ")
                return 6;

            // ㄹ + 여러 자음
            if (currentJongsungIndex == 9)
            {
                if (inputChar == "ㄱ") return 10; // ㄺ
                if (inputChar == "ㅁ") return 11; // ㄻ
                if (inputChar == "ㅂ") return 12; // ㄼ
                if (inputChar == "ㅅ") return 13; // ㄽ
                if (inputChar == "ㅌ") return 14; // ㄾ
                if (inputChar == "ㅍ") return 15; // ㄿ
                if (inputChar == "ㅎ") return 16; // ㅀ
            }

            // ㅂ + ㅅ -> ㅄ
            if (currentJongsungIndex == 18 && inputChar == "ㅅ")
                return 20;

            return -1; // Cannot form double consonant
        }
    }
}
