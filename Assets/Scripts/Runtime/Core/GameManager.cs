using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

using KW.Managers;
using KW.Core.Settings;
using KW.Core.Constants;
using KW.Utility;


namespace KW.Core
{
    public class GameManager : KWGlobalSingleton<GameManager>
    {
        public GameData_V2 Data;
        [HideInInspector] public List<string> NounList = new List<string>(); // 명사 리스트
        [HideInInspector] public List<string> PronounList = new List<string>(); // 대명사 리스트
        [HideInInspector] public List<string> NumerList = new List<string>(); // 수사 리스트
        [HideInInspector] public List<string> VerbList = new List<string>(); // 동사 리스트
        [HideInInspector] public List<string> AdjList = new List<string>(); // 형용사 리스트
        [HideInInspector] public List<string> AdvList = new List<string>(); // 부사 리스트
        [HideInInspector] public List<string> DefaultList = new List<string>();
        [HideInInspector] public List<string> AllList = new List<string>();

        public int CurrentStage;
        public bool IsTodayCompleted = false;

        private bool isWordListsLoaded = false;
        private Dictionary<string, List<string>> wordListCache = new Dictionary<string, List<string>>();
        private string cachedSceneName;

        public bool IsWordListsLoaded => isWordListsLoaded;

        protected override void OnAwake()
        {
            LoadData();
            StartCoroutine(LoadAllWordListsAsync());
        }

        /// <summary>
        /// 비동기로 모든 단어 리스트를 로드합니다. 백그라운드에서 처리되어 메인 스레드 블록킹을 방지합니다.
        /// </summary>
        private IEnumerator LoadAllWordListsAsync()
        {
            var wordListPath = ResourcePathsConstants.WORD_LIST_PATH;

            // 로딩 작업들을 Task로 시작
            var loadTasks = new List<Task<(string key, List<string> list)>>
            {
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_ALL), "All"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_DEFAULT), "Default"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_NOUN), "Noun"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_PRONOUN), "Pronoun"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_NUMERAL), "Numer"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_VERB), "Verb"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_ADJECTIVE), "Adj"),
                LoadTextAsync(Path.Join(wordListPath, ResourcePathsConstants.WORD_LIST_ADVERB), "Adv")
            };

            // 모든 작업이 완료될 때까지 대기 (프레임마다 체크)
            while (!Task.WhenAll(loadTasks).IsCompleted)
            {
                yield return null;
            }

            // 결과 할당
            foreach (var task in loadTasks)
            {
                var (key, list) = task.Result;
                wordListCache[key] = list;

                switch (key)
                {
                    case "All": AllList = list; break;
                    case "Default": DefaultList = list; break;
                    case "Noun": NounList = list; break;
                    case "Pronoun": PronounList = list; break;
                    case "Numer": NumerList = list; break;
                    case "Verb": VerbList = list; break;
                    case "Adj": AdjList = list; break;
                    case "Adv": AdvList = list; break;
                }
            }

            isWordListsLoaded = true;
            KWDebug.Log($"Word lists loaded successfully. Total lists: {wordListCache.Count}");
        }

        private static readonly char[] lineSeparators = { '\r', '\n' };

        /// <summary>
        /// 비동기로 텍스트 파일을 로드합니다. 백그라운드 스레드에서 파싱을 처리합니다.
        /// </summary>
        private async Task<(string key, List<string> list)> LoadTextAsync(string path, string key)
        {
            var list = new List<string>();

            TextAsset textAsset = Resources.Load<TextAsset>(path);

            if (textAsset == null)
            {
                KWDebug.LogError($"파일이 발견되지 않았습니다: {path}");
                return (key, list);
            }

            string textContent = textAsset.text;
            Resources.UnloadAsset(textAsset);

            await Task.Run(() =>
            {
                string[] lines = textContent.Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);
                list.Capacity = lines.Length;

                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        list.Add(line);
                }
            });

            return (key, list);
        }


        private void Start()
        {
            cachedSceneName = SceneManager.GetActiveScene().name;
            SetResolution();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            cachedSceneName = scene.name;
        }

#if UNITY_ANDROID
        void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.Escape))
            {
                if (cachedSceneName == SceneNames.MAIN)
                {
                    Application.Quit();
                }
                else if (cachedSceneName == SceneNames.BLOCKED_MAIN || cachedSceneName == SceneNames.DEBLOCKED_MAIN)
                {
                    SceneManager.LoadScene(SceneNames.MAIN);
                }
                else if (cachedSceneName == SceneNames.BLOCKED_STAGE)
                {
                    SceneManager.LoadScene(SceneNames.BLOCKED_MAIN);
                }
                else if (cachedSceneName == SceneNames.DEBLOCKED_STAGE)
                {
                    SceneManager.LoadScene(SceneNames.DEBLOCKED_MAIN);
                }
            }
        }
#endif

        protected override void OnApplicationQuit()
        {
            SaveData(Data);
            base.OnApplicationQuit();
        }

        public void SetResolution()
        {
            int deviceWidth = Screen.width;
            int deviceHeight = Screen.height;

            Screen.SetResolution(SettingsManager.UI.targetWidth,
                (int)(((float)deviceHeight / deviceWidth) * SettingsManager.UI.targetWidth), true);

            if ((float)SettingsManager.UI.targetWidth / SettingsManager.UI.targetHeight < (float)deviceWidth / deviceHeight)
            {
                // 기기의 해상도 비가 더 큰 경우 - 좌우 레터박스
                float newWidth = ((float)SettingsManager.UI.targetWidth / SettingsManager.UI.targetHeight) / ((float)deviceWidth / deviceHeight);
                Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
            }
            else
            {
                // 게임의 해상도 비가 더 큰 경우 - 상하 레터박스
                float newHeight = ((float)deviceWidth / deviceHeight) / ((float)SettingsManager.UI.targetWidth / SettingsManager.UI.targetHeight);
                Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
            }
        }

        public async Task SaveDataAsync(GameData_V2 data)
        {
            try
            {
                string path = Application.persistentDataPath + "/" + SettingsManager.Gameplay.SaveFileName;

                await Task.Run(() =>
                {
                    SerializableGameData serializableData = data.ToSerializable();
                    string json = JsonUtility.ToJson(serializableData);
                    File.WriteAllText(path, json);
                });

                KWDebug.Log("Data saved successfully (async)", "Save");
            }
            catch (Exception ex)
            {
                KWDebug.LogError($"Failed to save data async: {ex.Message}", "Save");
            }
        }

        public void SaveData(GameData_V2 data)
        {
            string path = Application.persistentDataPath + "/" + SettingsManager.Gameplay.SaveFileName;
            SerializableGameData serializableData = data.ToSerializable();
            string json = JsonUtility.ToJson(serializableData);
            File.WriteAllText(path, json);
        }

        public void LoadData()
        {
            GameData_V2 gameData_V2 = LoadData_V2();
            if (gameData_V2 != null)
            {
                Data = gameData_V2;
                return;
            }
            Data = new GameData_V2();

            GameData_V1 gameData_V1 = LoadData_V1();
            if (gameData_V1 == null)
            {
                KWDebug.LogError("Cannot load GameData_V1");
                return;
            }

            Data.MigrateFromV1(gameData_V1);
        }

        public GameData_V2 LoadData_V2()
        {
            string path = Application.persistentDataPath + "/" + SettingsManager.Gameplay.SaveFileName;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SerializableGameData serializableData = JsonUtility.FromJson<SerializableGameData>(json);

                if (serializableData != null)
                {
                    // 직렬화된 데이터를 GameData_V2로 복원
                    return GameData_V2.FromSerializable(serializableData);
                }
            }

            KWDebug.Log("Cant find save");
            return null;
        }

        /// <summary>
        /// Legacy save format loader (pre-GameData_V2). Used only for migrating
        /// older save files; new saves are written as GameData_V2.
        /// </summary>
        public GameData_V1 LoadData_V1()
        {
            GameData_V1 loadData = new GameData_V1();
            string path = Application.persistentDataPath + "/" + SettingsManager.Gameplay.SaveFileName;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                loadData = JsonUtility.FromJson<GameData_V1>(json);
            }
            if (loadData == null)
            {
                KWDebug.Log("Cant find save");
            }
            return loadData;
        }

        public void ClearData()
        {
            Data.Clear();
        }
    }
}
