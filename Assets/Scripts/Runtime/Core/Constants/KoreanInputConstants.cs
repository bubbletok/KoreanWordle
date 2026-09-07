using System.Linq;

namespace KW.Core.Constants
{
    /// <summary>
    /// Korean input system constants
    /// Contains all Korean character arrays and keyboard layouts
    /// </summary>
    public static class KoreanInputConstants
    {
        #region Korean Character Arrays

        /// <summary>
        /// 초성 (Initial consonants) - 19 characters
        /// </summary>
        public static readonly string[] ChosungLetters = {
            "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ",
            "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
        };

        /// <summary>
        /// 중성 (Vowels) - 21 characters
        /// </summary>
        public static readonly string[] JungsungLetters = {
            "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ",
            "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"
        };

        /// <summary>
        /// 종성 (Final consonants) - 30 entries (including empty), internal index space.
        /// Includes ㄸ/ㅃ which are not valid standalone final consonants in standard
        /// Hangul; HangulComposer.lastLetterDiff maps this internal index to the
        /// 28-value Unicode jongsung range used for syllable composition.
        /// </summary>
        public static readonly string[] JongsungLetters = {
            " ", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄸ", "ㄹ",
            "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅃ",
            "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
        };

        /// <summary>
        /// 모든 자모 (초성 19 + 중성 21 + 겹받침 11 + 빈종성 1 = 52)
        /// </summary>
        public static readonly string[] AllLetters = {
            // 초성 (19)
            "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ",
            "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ",
            // 중성 (21)
            "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ",
            "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ",
            // 겹받침 - 종성에만 등장하는 것 (11)
            "ㄳ", "ㄵ", "ㄶ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅄ",
            // 빈 종성
            " "
        };

        #endregion

        #region Consonant Arrays

        /// <summary>
        /// 모든 자음 (All consonants) - 30 characters
        /// 기본 자음 14 + 쌍자음 5 + 겹받침 11
        /// </summary>
        public static readonly string[] ConsonantLetters = {
            "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄸ", "ㄹ",
            "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅃ",
            "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
        };

        #endregion

        #region Keyboard Layout - CAPS Mode

        /// <summary>
        /// Top row keys when CAPS is enabled (double consonants)
        /// </summary>
        public static readonly string[] CapsTopKeys = {
            "ㅃ", "ㅉ", "ㄸ", "ㄲ", "ㅆ", "ㅛ", "ㅕ", "ㅑ", "ㅒ", "ㅖ"
        };

        #endregion

        #region Keyboard Layout - Normal Mode

        /// <summary>
        /// Top row keys in normal mode
        /// </summary>
        public static readonly string[] TopKeys = {
            "ㅂ", "ㅈ", "ㄷ", "ㄱ", "ㅅ", "ㅛ", "ㅕ", "ㅑ", "ㅐ", "ㅔ"
        };

        /// <summary>
        /// Middle row keys
        /// </summary>
        public static readonly string[] MiddleKeys = {
            "ㅁ", "ㄴ", "ㅇ", "ㄹ", "ㅎ", "ㅗ", "ㅓ", "ㅏ", "ㅣ"
        };

        /// <summary>
        /// Bottom row keys
        /// </summary>
        public static readonly string[] BottomKeys = {
            "ㅋ", "ㅌ", "ㅊ", "ㅍ", "ㅠ", "ㅜ", "ㅡ"
        };

        #endregion

    }
}
