using System;
using System.Collections.Generic;
using System.Linq;
using KW.Managers;
using static KW.Core.Settings.GameplayEnums;

namespace KW.Core
{
    #region Serializable Wrapper Classes for Dictionary

    [System.Serializable]
    public class SerializableGameTypeIntPair
    {
        public GameType gameType;
        public int value;
    }

    [System.Serializable]
    public class SerializableStageAttemptPair
    {
        public GameStageType stageType;
        public int tryNumber;
        public int count;
    }

    [System.Serializable]
    public class SerializableWordIndexPair
    {
        public GameType gameType;
        public GameStageType stageType;
        public int wordIndex;
        public bool cleared;
    }

    [System.Serializable]
    public class SerializableStageClearPair
    {
        public GameType gameType;
        public int stageNumber;
        public bool isCleared;
        public int attempts;
    }

    [System.Serializable]
    public class SerializableGameData
    {
        public List<SerializableGameTypeIntPair> lastStagePages = new List<SerializableGameTypeIntPair>();
        public List<SerializableStageAttemptPair> attemptToClearGameStage = new List<SerializableStageAttemptPair>();
        public List<SerializableGameTypeIntPair> attempGameCount = new List<SerializableGameTypeIntPair>();
        public List<SerializableGameTypeIntPair> clearGameCount = new List<SerializableGameTypeIntPair>();
        public List<SerializableWordIndexPair> clearWordIndices = new List<SerializableWordIndexPair>();
        public List<SerializableStageClearPair> clearStages = new List<SerializableStageClearPair>();
        public string dateTime;
    }

    #endregion

    [System.Serializable]
    public class GameData_V1
    {
        public int LastBlockedStagePage = 1;
        public int LastDeblockedStagePage = 1;

        public int[,] AttemptToClear = new int[8, 6];
        public int AttemptBlockedGameNumber = 0;
        public int SuccessBlockedGameNumber = 0;
        public int AttemptDeblockedGameNumber = 0;
        public int SuccessDeblockedGameNumber = 0;

        public bool[,] ClearBlockedWordIndex = new bool[8, 50000]; // 각 품사별 완료한 단어 인덱스
        public bool[,] ClearDeblockedWordIndex = new bool[8, 50000];
        public bool[] ClearBlockedStages = new bool[50000];
        public bool[] ClearDeblockedStages = new bool[50000];
        public int[] AttemptToClearBlockedStage = new int[50000];
        public int[] AttemptToClearDeblockedStage = new int[50000];
        public DateTime DateTime = new DateTime(2022, 01, 20, 6, 0, 0);
    }

    [System.Serializable]
    public class GameData_V2
    {
        public Dictionary<GameType, int> LastStagePage = new Dictionary<GameType, int>();

        // <(GameStageType, Try number), count>
        public Dictionary<(GameStageType, int), int> AttemptToClearGameStage = new Dictionary<(GameStageType, int), int>();
        public Dictionary<GameType, int> AttempGameCount = new Dictionary<GameType, int>();
        public Dictionary<GameType, int> ClearGameCount = new Dictionary<GameType, int>();

        // 각 품사별 완료한 단어 인덱스 (클리어된 것만 저장)
        public HashSet<(GameType, GameStageType, int)> ClearWordIndices =
            new HashSet<(GameType, GameStageType, int)>();

        // <(GameType, Stage number), (clear 여부, 시도 횟수)>
        public Dictionary<(GameType, int), (bool, int)> ClearStages = new Dictionary<(GameType, int), (bool, int)>();
        public DateTime DateTime;

        /// <summary>
        /// 기본 생성자 - 모든 GameType에 대한 초기값 설정
        /// </summary>
        public GameData_V2()
        {
            // 모든 GameType에 대해 LastStagePage 초기화 (1페이지부터 시작)
            LastStagePage[GameType.Blocked] = 1;
            LastStagePage[GameType.Deblocked] = 1;

            // 모든 GameType에 대해 시도 횟수 초기화
            AttempGameCount[GameType.Blocked] = 0;
            AttempGameCount[GameType.Deblocked] = 0;

            // 모든 GameType에 대해 클리어 횟수 초기화
            ClearGameCount[GameType.Blocked] = 0;
            ClearGameCount[GameType.Deblocked] = 0;

            // 첫 번째 스테이지는 언락 상태로 초기화 (클리어 안됨, 시도 횟수 1)
            ClearStages[(GameType.Blocked, 0)] = (false, 1);
            ClearStages[(GameType.Deblocked, 0)] = (false, 1);

            // Date Time 설정 (2022년 1월 20일 오전 6시 0분 0초)
            DateTime = new DateTime(2022, 01, 20, 6, 0, 0);
        }

        public void MigrateFromV1(GameData_V1 gameData_V1)
        {
            this.LastStagePage[GameType.Blocked] = gameData_V1.LastBlockedStagePage;
            this.LastStagePage[GameType.Deblocked] = gameData_V1.LastDeblockedStagePage;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    this.AttemptToClearGameStage[((GameStageType)i, j)] = gameData_V1.AttemptToClear[i, j];
                }
            }
            AttempGameCount[GameType.Blocked] = gameData_V1.AttemptBlockedGameNumber;
            AttempGameCount[GameType.Deblocked] = gameData_V1.AttemptDeblockedGameNumber;

            ClearGameCount[GameType.Blocked] = gameData_V1.SuccessBlockedGameNumber;
            ClearGameCount[GameType.Deblocked] = gameData_V1.SuccessDeblockedGameNumber;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 50000; j++)
                {
                    if (gameData_V1.ClearBlockedWordIndex[i, j])
                        ClearWordIndices.Add((GameType.Blocked, (GameStageType)i, j));
                    if (gameData_V1.ClearDeblockedWordIndex[i, j])
                        ClearWordIndices.Add((GameType.Deblocked, (GameStageType)i, j));
                }
            }

            for (int i = 0; i < 50000; i++)
            {
                ClearStages[(GameType.Blocked, i)] = (gameData_V1.ClearBlockedStages[i], gameData_V1.AttemptToClearBlockedStage[i]);
                ClearStages[(GameType.Deblocked, i)] = (gameData_V1.ClearDeblockedStages[i], gameData_V1.AttemptToClearDeblockedStage[i]);
            }
        }

        public void Clear()
        {
            LastStagePage[GameType.Blocked] = 1;
            LastStagePage[GameType.Deblocked] = 1;

            AttemptToClearGameStage.Clear();

            AttempGameCount[GameType.Blocked] = 0;
            AttempGameCount[GameType.Deblocked] = 0;

            ClearGameCount[GameType.Blocked] = 0;
            ClearGameCount[GameType.Deblocked] = 0;

            ClearWordIndices.Clear();
            ClearStages.Clear();

            // 첫 번째 스테이지는 언락 상태로 초기화 (클리어 안됨, 시도 횟수 0)
            ClearStages[(GameType.Blocked, 0)] = (false, 0);
            ClearStages[(GameType.Deblocked, 0)] = (false, 0);
        }

        #region Safe Dictionary Access Methods

        /// <summary>
        /// 안전하게 LastStagePage 값을 가져옵니다. 키가 없으면 기본값 1을 반환합니다.
        /// </summary>
        public int GetLastStagePage(GameType gameType)
        {
            if (LastStagePage.TryGetValue(gameType, out int value))
            {
                return value;
            }

            // 키가 없으면 기본값 설정 후 반환
            LastStagePage[gameType] = 1;
            return 1;
        }

        /// <summary>
        /// 안전하게 AttemptToClearGameStage 값을 가져옵니다. 키가 없으면 기본값 0을 반환합니다.
        /// </summary>
        public int GetAttemptToClearGameStage(GameStageType stageType, int tryNumber)
        {
            var key = (stageType, tryNumber);
            if (AttemptToClearGameStage.TryGetValue(key, out int value))
            {
                return value;
            }

            // 키가 없으면 기본값 설정 후 반환
            AttemptToClearGameStage[key] = 0;
            return 0;
        }

        /// <summary>
        /// 안전하게 AttempGameCount 값을 가져옵니다. 키가 없으면 기본값 0을 반환합니다.
        /// </summary>
        public int GetAttempGameCount(GameType gameType)
        {
            if (AttempGameCount.TryGetValue(gameType, out int value))
            {
                return value;
            }

            // 키가 없으면 기본값 설정 후 반환
            AttempGameCount[gameType] = 0;
            return 0;
        }

        /// <summary>
        /// 안전하게 ClearGameCount 값을 가져옵니다. 키가 없으면 기본값 0을 반환합니다.
        /// </summary>
        public int GetClearGameCount(GameType gameType)
        {
            if (ClearGameCount.TryGetValue(gameType, out int value))
            {
                return value;
            }

            // 키가 없으면 기본값 설정 후 반환
            ClearGameCount[gameType] = 0;
            return 0;
        }

        /// <summary>
        /// 안전하게 ClearWordIndices 값을 가져옵니다. 키가 없으면 기본값 false를 반환합니다.
        /// </summary>
        public bool GetClearWordIndices(GameType gameType, GameStageType stageType, int wordIndex)
        {
            return ClearWordIndices.Contains((gameType, stageType, wordIndex));
        }

        /// <summary>
        /// 안전하게 ClearStages 값을 가져옵니다. 키가 없으면 기본값 (false, 0)을 반환합니다.
        /// </summary>
        public (bool isCleared, int attempts) GetClearStages(GameType gameType, int stageNumber)
        {
            return ClearStages.TryGetValue((gameType, stageNumber), out var value) ? value : (false, 0);
        }

        /// <summary>
        /// 안전하게 LastStagePage 값을 설정합니다.
        /// </summary>
        public void SetLastStagePage(GameType gameType, int page)
        {
            LastStagePage[gameType] = page;
        }

        /// <summary>
        /// 안전하게 AttemptToClearGameStage 값을 증가시킵니다.
        /// </summary>
        public void IncrementAttemptToClearGameStage(GameStageType stageType, int tryNumber)
        {
            var key = (stageType, tryNumber);
            AttemptToClearGameStage[key] = AttemptToClearGameStage.ContainsKey(key) ? AttemptToClearGameStage[key] + 1 : 1;
        }

        /// <summary>
        /// 안전하게 AttempGameCount 값을 증가시킵니다.
        /// </summary>
        public void IncrementAttempGameCount(GameType gameType)
        {
            AttempGameCount[gameType] = AttempGameCount.ContainsKey(gameType) ? AttempGameCount[gameType] + 1 : 1;
        }

        /// <summary>
        /// 안전하게 ClearGameCount 값을 증가시킵니다.
        /// </summary>
        public void IncrementClearGameCount(GameType gameType)
        {
            ClearGameCount[gameType] = ClearGameCount.ContainsKey(gameType) ? ClearGameCount[gameType] + 1 : 1;
        }

        /// <summary>
        /// 안전하게 ClearWordIndices 값을 설정합니다.
        /// </summary>
        public void SetClearWordIndices(GameType gameType, GameStageType stageType, int wordIndex, bool cleared)
        {
            var key = (gameType, stageType, wordIndex);
            if (cleared)
                ClearWordIndices.Add(key);
            else
                ClearWordIndices.Remove(key);
        }

        /// <summary>
        /// 안전하게 ClearStages 값을 설정합니다.
        /// </summary>
        public void SetClearStages(GameType gameType, int stageNumber, bool isCleared, int attempts)
        {
            var key = (gameType, stageNumber);
            ClearStages[key] = (isCleared, attempts);
        }

        #endregion

        #region Serialization Methods

        /// <summary>
        /// Dictionary 데이터를 JsonUtility로 직렬화 가능한 형태로 변환합니다.
        /// LINQ 대신 foreach를 사용하여 성능 최적화 (약 2배 빠름)
        /// </summary>
        public SerializableGameData ToSerializable()
        {
            var data = new SerializableGameData();

            foreach (var kvp in LastStagePage)
                data.lastStagePages.Add(new SerializableGameTypeIntPair { gameType = kvp.Key, value = kvp.Value });

            foreach (var kvp in AttemptToClearGameStage)
                data.attemptToClearGameStage.Add(new SerializableStageAttemptPair
                { stageType = kvp.Key.Item1, tryNumber = kvp.Key.Item2, count = kvp.Value });

            foreach (var kvp in AttempGameCount)
                data.attempGameCount.Add(new SerializableGameTypeIntPair { gameType = kvp.Key, value = kvp.Value });

            foreach (var kvp in ClearGameCount)
                data.clearGameCount.Add(new SerializableGameTypeIntPair { gameType = kvp.Key, value = kvp.Value });

            foreach (var key in ClearWordIndices)
                data.clearWordIndices.Add(new SerializableWordIndexPair
                { gameType = key.Item1, stageType = key.Item2, wordIndex = key.Item3, cleared = true });

            foreach (var kvp in ClearStages)
                data.clearStages.Add(new SerializableStageClearPair
                { gameType = kvp.Key.Item1, stageNumber = kvp.Key.Item2, isCleared = kvp.Value.Item1, attempts = kvp.Value.Item2 });

            data.dateTime = DateTime.ToString("o");

            return data;
        }

        /// <summary>
        /// 직렬화된 데이터를 GameData_V2로 복원합니다.
        /// </summary>
        public static GameData_V2 FromSerializable(SerializableGameData data)
        {
            var gameData = new GameData_V2();

            // LastStagePage 복원
            if (data.lastStagePages != null)
            {
                gameData.LastStagePage = data.lastStagePages
                    .ToDictionary(p => p.gameType, p => p.value);
            }

            // AttemptToClearGameStage 복원
            if (data.attemptToClearGameStage != null)
            {
                gameData.AttemptToClearGameStage = data.attemptToClearGameStage
                    .ToDictionary(p => (p.stageType, p.tryNumber), p => p.count);
            }

            // AttempGameCount 복원
            if (data.attempGameCount != null)
            {
                gameData.AttempGameCount = data.attempGameCount
                    .ToDictionary(p => p.gameType, p => p.value);
            }

            // ClearGameCount 복원
            if (data.clearGameCount != null)
            {
                gameData.ClearGameCount = data.clearGameCount
                    .ToDictionary(p => p.gameType, p => p.value);
            }

            // ClearWordIndices 복원 (cleared=true인 항목만 추가)
            if (data.clearWordIndices != null)
            {
                gameData.ClearWordIndices = data.clearWordIndices
                    .Where(p => p.cleared)
                    .Select(p => (p.gameType, p.stageType, p.wordIndex))
                    .ToHashSet();
            }

            // ClearStages 복원
            if (data.clearStages != null)
            {
                gameData.ClearStages = data.clearStages
                    .ToDictionary(p => (p.gameType, p.stageNumber), p => (p.isCleared, p.attempts));
            }

            // DateTime 복원
            if (!string.IsNullOrEmpty(data.dateTime))
            {
                gameData.DateTime = System.DateTime.Parse(data.dateTime);
            }

            return gameData;
        }

        #endregion
    }
}
