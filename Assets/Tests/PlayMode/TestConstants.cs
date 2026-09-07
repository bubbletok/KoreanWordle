namespace KW.Tests
{
    /// <summary>
    /// Constants used across all PlayMode tests
    /// </summary>
    public static class TestConstants
    {
        #region Timeouts

        /// <summary>
        /// Maximum time to wait for scene loading (seconds)
        /// </summary>
        public const float SceneLoadTimeout = 5.0f;

        /// <summary>
        /// Maximum time to wait for button discovery (seconds)
        /// </summary>
        public const float ButtonDiscoveryTimeout = 1.0f;

        /// <summary>
        /// Maximum time to wait for GameManager word lists to load (seconds)
        /// </summary>
        public const float GameManagerLoadTimeout = 10.0f;

        /// <summary>
        /// Polling interval for async operations (seconds)
        /// </summary>
        public const float PollingInterval = 0.1f;

        #endregion

        #region Frame Waits

        /// <summary>
        /// Number of frames to wait after UI interaction
        /// </summary>
        public const int FramesAfterUIInteraction = 2;

        /// <summary>
        /// Number of frames to wait after scene load
        /// </summary>
        public const int FramesAfterSceneLoad = 3;

        #endregion

        #region Test GameObject Names

        /// <summary>
        /// Name for test GameManager GameObject
        /// </summary>
        public const string TestGameManagerName = "[Test] GameManager";

        #endregion
    }
}
