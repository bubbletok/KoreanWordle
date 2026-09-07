using System.Collections.Generic;
using UnityEngine;
using KW.Core;
using KW.Managers;
using static KW.Core.Settings.GameplayEnums;
using KW.UI;
using KW.Utility;

namespace KW.Managers
{
    /// <summary>
    /// 게임 통계 데이터를 계산하고 제공하는 매니저
    /// GameData를 분석하여 다양한 통계 정보를 생성합니다
    /// </summary>
    public class StatisticsManager : KWLocalSingleton<StatisticsManager>
    {
        #region Statistics Data Structures

        /// <summary>
        /// 전체 통계 데이터
        /// </summary>
        public struct OverallStats
        {
            public int totalAttempts;      // 총 시도 횟수
            public int totalClears;        // 총 클리어 횟수
            public float winRate;          // 승률 (%)
            public float averageAttempts;  // 평균 시도 횟수

            public OverallStats(int attempts, int clears, float winRate, float avgAttempts)
            {
                this.totalAttempts = attempts;
                this.totalClears = clears;
                this.winRate = winRate;
                this.averageAttempts = avgAttempts;
            }
        }

        /// <summary>
        /// 시도 횟수별 분포 데이터
        /// </summary>
        public struct AttemptDistribution
        {
            public int[] attemptCounts;    // 각 시도 횟수별 클리어 개수 (1~6회 + 실패)
            public int maxCount;           // 최대값 (그래프 스케일링용)

            public AttemptDistribution(int[] counts, int max)
            {
                this.attemptCounts = counts;
                this.maxCount = max;
            }
        }

        /// <summary>
        /// 품사별 통계 데이터
        /// </summary>
        public struct StageTypeStats
        {
            public GameStageType stageType;
            public int clearedCount;       // 클리어한 단어 수
            public int totalWords;         // 총 단어 수
            public float clearRate;        // 클리어율 (%)

            public StageTypeStats(GameStageType type, int cleared, int total, float rate)
            {
                this.stageType = type;
                this.clearedCount = cleared;
                this.totalWords = total;
                this.clearRate = rate;
            }
        }

        #endregion

        #region Private Fields

        private GameData_V2 gameData;
        private GameManager gameManager;

        #endregion

        #region Initialization

        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();

            if (GameManager.Exists)
            {
                gameManager = GameManager.Instance;
                gameData = gameManager.Data;
            }
            else
            {
                KWDebug.LogError("[StatisticsManager] GameManager not found!");
            }

            StatisticsUIManager.Instance.Init();
        }

        #endregion

        #region Public API - Get Statistics

        /// <summary>
        /// 전체 통계 데이터를 가져옵니다
        /// </summary>
        public OverallStats GetOverallStats(GameType gameType)
        {
            if (gameData == null)
            {
                KWDebug.LogWarning("[StatisticsManager] GameData is null");
                return new OverallStats(0, 0, 0f, 0f);
            }

            int attempts = gameData.GetAttempGameCount(gameType);
            int clears = gameData.GetClearGameCount(gameType);
            float winRate = attempts > 0 ? (clears / (float)attempts) * 100f : 0f;

            // 평균 시도 횟수 계산 (1~6회 시도한 게임들의 평균)
            float avgAttempts = CalculateAverageAttempts(gameType);

            return new OverallStats(attempts, clears, winRate, avgAttempts);
        }

        /// <summary>
        /// 시도 횟수별 분포 데이터를 가져옵니다
        /// </summary>
        public AttemptDistribution GetAttemptDistribution(GameType gameType)
        {
            if (gameData == null)
            {
                KWDebug.LogWarning("[StatisticsManager] GameData is null");
                return new AttemptDistribution(new int[7], 0);
            }

            // 각 시도 횟수별 클리어 개수를 저장 (인덱스 0~5: 1~6회, 인덱스 6: 실패)
            int[] counts = new int[7];

            // 모든 스테이지를 순회하며 시도 횟수 집계
            foreach (var kvp in gameData.ClearStages)
            {
                var (type, stageNumber) = kvp.Key;
                var (isCleared, attempts) = kvp.Value;

                // 해당 게임 타입만 필터링
                if (type != gameType) continue;

                if (isCleared && attempts >= 1 && attempts <= 6)
                {
                    counts[attempts - 1]++;
                }
                else if (!isCleared && attempts > 0)
                {
                    counts[6]++; // 실패
                }
            }

            int maxCount = 0;
            for (int i = 0; i < counts.Length; i++)
                if (counts[i] > maxCount) maxCount = counts[i];

            return new AttemptDistribution(counts, maxCount);
        }

        /// <summary>
        /// 품사별 통계 데이터를 가져옵니다
        /// </summary>
        public List<StageTypeStats> GetStageTypeStats(GameType gameType)
        {
            if (gameData == null)
            {
                KWDebug.LogWarning("[StatisticsManager] GameData is null");
                return new List<StageTypeStats>();
            }

            var statsList = new List<StageTypeStats>();

            // 각 품사별 통계 계산
            for (int i = 1; i <= 6; i++) // GameStageType: 1~6
            {
                GameStageType stageType = (GameStageType)i;
                int clearedCount = GetClearedWordCount(gameType, stageType);
                int totalWords = GetTotalWordCount(stageType);
                float clearRate = totalWords > 0 ? (clearedCount / (float)totalWords) * 100f : 0f;

                statsList.Add(new StageTypeStats(stageType, clearedCount, totalWords, clearRate));
            }

            return statsList;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// 평균 시도 횟수를 계산합니다
        /// </summary>
        private float CalculateAverageAttempts(GameType gameType)
        {
            int totalAttempts = 0;
            int totalGames = 0;

            foreach (var kvp in gameData.ClearStages)
            {
                var (type, stageNumber) = kvp.Key;
                var (isCleared, attempts) = kvp.Value;

                if (type != gameType || !isCleared) continue;

                totalAttempts += attempts;
                totalGames++;
            }

            return totalGames > 0 ? totalAttempts / (float)totalGames : 0f;
        }

        /// <summary>
        /// 특정 품사에서 클리어한 단어 수를 가져옵니다
        /// </summary>
        private int GetClearedWordCount(GameType gameType, GameStageType stageType)
        {
            int count = 0;

            foreach (var (type, stage, wordIndex) in gameData.ClearWordIndices)
            {
                if (type == gameType && stage == stageType)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 특정 품사의 총 단어 수를 가져옵니다
        /// GameManager의 단어 리스트에서 가져옵니다
        /// </summary>
        private int GetTotalWordCount(GameStageType stageType)
        {
            if (gameManager == null) return 0;

            // 품사별 단어 리스트를 GameManager의 캐시된 리스트에서 가져오기
            List<string> wordList = stageType switch
            {
                GameStageType.Noun => gameManager.NounList,
                GameStageType.Pronoun => gameManager.PronounList,
                GameStageType.Numeral => gameManager.NumerList,
                GameStageType.Verb => gameManager.VerbList,
                GameStageType.Adjective => gameManager.AdjList,
                GameStageType.Adverb => gameManager.AdvList,
                _ => gameManager.AllList
            };

            return wordList?.Count ?? 0;
        }

        #endregion

        #region Public API - Data Reset

        /// <summary>
        /// 게임 데이터를 초기화합니다 (통계 리셋)
        /// </summary>
        public void ResetStatistics()
        {
            if (gameData == null)
            {
                KWDebug.LogWarning("[StatisticsManager] GameData is null");
                return;
            }

            // GameData 초기화 (Clear 메서드에서 첫 번째 스테이지 언락 포함)
            gameData.Clear();

            // 저장 (비동기)
            if (gameManager != null)
            {
                _ = gameManager.SaveDataAsync(gameData);
                KWDebug.Log("[StatisticsManager] Statistics reset complete");
            }
        }

        #endregion
    }
}
