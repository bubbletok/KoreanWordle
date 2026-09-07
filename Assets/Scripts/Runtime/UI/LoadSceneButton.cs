using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KW.UI
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        public enum LoadSceneType
        {
            Sync,
            Async,
        }

        private Button button;
        public string SceneName;
        [SerializeField, FormerlySerializedAs("_loadSceneType")] private LoadSceneType loadSceneType = LoadSceneType.Sync;

        void Awake()
        {
            button = GetComponent<Button>();
        }

        void Start()
        {
            button.onClick.AddListener(LoadScene);
        }

        void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        void LoadScene()
        {
            if (loadSceneType == LoadSceneType.Sync)
                SceneManager.LoadScene(SceneName);
            else
                SceneManager.LoadSceneAsync(SceneName);
        }
    }
}
