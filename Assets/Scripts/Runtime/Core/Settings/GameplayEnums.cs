namespace KW.Core.Settings
{
    /// <summary>
    /// Gameplay mechanics enumerations for Korean Wordle.
    /// Contains enum types related to game mechanics (cell states, Hangul components).
    /// For game management types (GameType, GameStageType), see GameplayManager.
    /// </summary>
    public static class GameplayEnums
    {
        /// <summary>
        /// Cell state for indicating correctness in Wordle gameplay.
        /// </summary>
        public enum CellState
        {
            None = -1,
            Empty = 0,         // Black - Empty/unused (for Jongsung without final consonant)
            Correct = 1,       // Green - Correct position
            WrongPosition = 2, // Yellow - Exists but wrong position
            NotInWord = 3      // Gray - Not in word
        }

        /// <summary>
        /// Hangul component position for Blocked mode (모아쓰기: composed syllable cells).
        /// Used to identify which component of a Hangul syllable is being referenced.
        /// </summary>
        public enum HangulComponentPosition
        {
            Chosung = 0,   // 초성 (first consonant)
            Jungsung = 1,  // 중성 (middle vowel)
            Jongsung = 2   // 종성 (final consonant)
        }
    }
}
