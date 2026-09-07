namespace KW.Core.Constants
{
    /// <summary>
    /// Scene name constants
    /// Contains all scene names used for navigation and loading
    /// </summary>
    public static class SceneNames
    {
        #region Main Scenes

        /// <summary>
        /// Main menu scene
        /// </summary>
        public const string START = "KW_Start";

        /// <summary>
        /// Main game menu scene
        /// </summary>
        public const string MAIN = "KW_Main";

        #endregion

        #region Blocked Mode Scenes

        /// <summary>
        /// Blocked mode stage selection screen (StageCellManager pagination).
        /// </summary>
        public const string BLOCKED_MAIN = "KW_BlockedMain";

        /// <summary>
        /// Blocked mode gameplay screen (loaded when a stage is started).
        /// </summary>
        public const string BLOCKED_STAGE = "KW_BlockedGameStage";

        #endregion

        #region Deblocked Mode Scenes

        /// <summary>
        /// Deblocked mode stage selection screen (StageCellManager pagination).
        /// </summary>
        public const string DEBLOCKED_MAIN = "KW_DeblockedMain";

        /// <summary>
        /// Deblocked mode gameplay screen (loaded when a stage is started).
        /// </summary>
        public const string DEBLOCKED_STAGE = "KW_DeblockedGameStage";

        #endregion

        #region Statistics

        /// <summary>
        /// Statistics menu
        /// </summary>
        public const string STATISTIC = "KW_Stastics";

        #endregion
    }
}
