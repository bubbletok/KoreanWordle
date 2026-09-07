using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KW.Core;
using KW.Core.Constants;
using KW.UI;
using KW.UI.Popup;
using KW.Utility;
using KW.Gameplay;
using KW.Input;
using static KW.Core.Settings.GameplayEnums;

namespace KW.Managers
{
    /// <summary>
    /// Game type/mode.
    /// </summary>
    public enum GameType
    {
        Blocked,   // 모아쓰기 - Composed syllables
        Deblocked, // 풀어쓰기 - Decomposed components
        TodayWord  // 오늘의 단어 - Daily word challenge
    }

    /// <summary>
    /// Game stage type based on Korean parts of speech (품사).
    /// Used to categorize word lists and track progress by word type.
    /// </summary>
    public enum GameStageType
    {
        Noun = 1,      // 명사 (Noun)
        Pronoun = 2,   // 대명사 (Pronoun)
        Numeral = 3,   // 수사 (Numeral)
        Verb = 4,      // 동사 (Verb)
        Adjective = 5, // 형용사 (Adjective)
        Adverb = 6     // 부사 (Adverb)
    }

    public sealed class GameplayManager : KWLocalSingleton<GameplayManager>
    {
        [Header("Game State")]
        private GameType gameType;
        public GameType GameType => gameType;

        private GameStageType gameStageType;
        public GameStageType GameStageType => gameStageType;

        private bool isGameEnded = false;
        public bool IsGameEnd => isGameEnded;

        private GameWordSelector wordSelector;
        public GameWordSelector WordSelector => wordSelector;
        private GameplayCellManager gameplayCellManager;
        public GameplayCellManager GameplayCellManager => gameplayCellManager;
        private GameplayUIManager gameplayUIManager;
        public GameplayUIManager GameplayUIManager => gameplayUIManager;
        private KoreanInputHandler koreanInputHandler;
        public KoreanInputHandler KoreanInputHandler => koreanInputHandler;
        private HintManager hintManager;
        public HintManager HintManager => hintManager;

        protected override void OnAwake()
        {
            // Get component references only
            wordSelector = GetComponent<GameWordSelector>();

            // Determine game type based on scene name
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == SceneNames.BLOCKED_STAGE)
            {
                gameType = GameType.Blocked;
            }
            else if (sceneName == SceneNames.DEBLOCKED_STAGE)
            {
                gameType = GameType.Deblocked;
            }
            // TodayWord scene not yet implemented

            // Randomly select a game stage type (Noun, Verb, etc.)
            int stageTypeCount = System.Enum.GetNames(typeof(GameStageType)).Length;
            gameStageType = (GameStageType)UnityEngine.Random.Range(1, stageTypeCount + 1);
        }

        /// <summary>
        /// Sets the game type and stage type. Called by GameManager or scene setup.
        /// </summary>
        public void SetGameState(GameType gameType, GameStageType gameStageType)
        {
            this.gameType = gameType;
            this.gameStageType = gameStageType;
        }

        public override void Init()
        {
            base.Init();

            gameplayCellManager = GameplayCellManager.Instance;
            gameplayUIManager = GameplayUIManager.Instance;
            koreanInputHandler = KoreanInputHandler.Instance;

            hintManager = HintManager.Instance;

            // Register and execute initialization sequence
            if (GameplaySetupManager.Exists)
            {
                // Register all component initializations
                GameplaySetupManager.Instance.AddInit(1.0f, () => wordSelector.Init(), "GameWordSelector");
                GameplaySetupManager.Instance.AddInit(2.0f, () => gameplayCellManager.Init(), "GameplayCellManager");
                GameplaySetupManager.Instance.AddInit(3.0f, () => gameplayUIManager.Init(), "GameplayUIManager");
                GameplaySetupManager.Instance.AddInit(4.0f, () => koreanInputHandler.Init(), "KoreanInputHandler");

                // HintManager 초기화 (있는 경우에만)
                if (hintManager != null)
                {
                    GameplaySetupManager.Instance.AddInit(5.0f, () => InitializeHints(), "HintManager");
                }

                // Execute initialization sequence
                GameplaySetupManager.Instance.Initialize();
            }
            else
            {
                KWDebug.LogError("[GameplayManager] GameplaySetupManager instance not found! Initialization order may be incorrect.");
            }
        }

        /// <summary>
        /// 힌트 시스템을 초기화합니다
        /// </summary>
        private void InitializeHints()
        {
            if (hintManager != null && wordSelector != null)
            {
                hintManager.Init();
                string answer = wordSelector.GetAnswer();
                hintManager.SetAnswer(answer, gameStageType);
                KWDebug.Log($"[GameplayManager] HintManager initialized with answer: {answer}, stage: {gameStageType}");
            }
        }

        private void Start()
        {
            // Start initialization sequence after all Awake calls complete
            Init();
        }

        /// <summary>
        /// Display a temporary status message using popup system
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="duration">Duration in seconds (default: 1.5s)</param>
        public void ShowStatusText(string message, float duration = 1.5f)
        {
            PopupManager.Instance.ShowMessage(message, duration);
        }

        /// <summary>
        /// Handle game win - show win popup and save game data
        /// </summary>
        public IEnumerator WinGame(string answer)
        {
            yield return null;
            isGameEnded = true;

            // Save game completion data
            if (GameManager.Exists)
            {
                // Increment total attempt count (승리 시에도 시도 횟수 카운트)
                GameManager.Instance.Data.IncrementAttempGameCount(gameType);

                // Record attempt count for this stage type and try number
                GameManager.Instance.Data.IncrementAttemptToClearGameStage(
                    gameStageType,
                    gameplayCellManager.NumberOfTry - 1);

                // Increment total clear count
                GameManager.Instance.Data.IncrementClearGameCount(gameType);

                // Mark current stage as cleared
                int currentStage = GameManager.Instance.CurrentStage;
                GameManager.Instance.Data.SetClearStages(
                    gameType,
                    currentStage - 1,
                    true,
                    gameplayCellManager.NumberOfTry);

                // Mark word as cleared
                GameManager.Instance.Data.SetClearWordIndices(
                    gameType,
                    gameStageType,
                    wordSelector.WordIndex,
                    true);

                // Save data to disk asynchronously (Fire-and-Forget)
                // 저장은 백그라운드에서 처리되며, 팝업 표시를 블로킹하지 않습니다
                _ = GameManager.Instance.SaveDataAsync(GameManager.Instance.Data);
            }

            // Show win popup through PopupManager (저장과 병렬로 즉시 표시)
            PopupManager.Instance.ShowGameResult(true, answer);
            // Future: Add explanation parameter from hint system
        }

        /// <summary>
        /// Handle game loss - show lose popup and save attempt data
        /// </summary>
        public IEnumerator LoseGame(string answer)
        {
            yield return null;
            isGameEnded = true;

            // Save game attempt data (even though player lost)
            if (GameManager.Exists)
            {
                // Increment attempt count for this game type
                GameManager.Instance.Data.IncrementAttempGameCount(gameType);

                // Save data to disk asynchronously (Fire-and-Forget)
                // 저장은 백그라운드에서 처리되며, 팝업 표시를 블로킹하지 않습니다
                _ = GameManager.Instance.SaveDataAsync(GameManager.Instance.Data);
            }

            // Show lose popup through PopupManager (저장과 병렬로 즉시 표시)
            PopupManager.Instance.ShowGameResult(false, answer);
            // Future: Add explanation parameter from hint system
        }
    }

}