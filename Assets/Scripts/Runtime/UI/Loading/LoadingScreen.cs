using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using DG.Tweening;
using KW.Managers;
using KW.Utility;

namespace KW.UI.Loading
{
    public class LoadingScreen : KWLocalSingleton<LoadingScreen>
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        [Header("Loading Settings")]
        [SerializeField] private float minimumLoadingTime = 1f;
        [SerializeField] private bool autoActivateScene = true;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private float progressTweenDuration = 0.3f;
        [SerializeField] private Ease progressTweenEase = Ease.OutCubic;

        private VisualElement root;
        private ProgressBar progressBar;
        private Label statusLabel;

        private AsyncOperation currentLoadOperation;
        private float loadingStartTime;
        private bool isLoading;
        private Tween progressTween;

        protected override void OnAwake()
        {
            base.OnAwake();

            if (document == null)
            {
                KWDebug.LogError("[LoadingScreen] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            root = document.rootVisualElement.Q("loading-root");
            progressBar = root?.Q<ProgressBar>("progress-bar");
            statusLabel = root?.Q<Label>("status-label");
        }

        public void LoadSceneAsync(string sceneName, Action onComplete = null)
        {
            if (isLoading)
            {
                KWDebug.LogWarning("[LoadingScreen] Already loading a scene!");
                return;
            }

            Show();

            StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
        }

        public void LoadSceneAsync(int sceneIndex, Action onComplete = null)
        {
            if (isLoading)
            {
                KWDebug.LogWarning("[LoadingScreen] Already loading a scene!");
                return;
            }

            StartCoroutine(LoadSceneByIndexCoroutine(sceneIndex, onComplete));
        }

        public void StartCustomLoading(Func<IEnumerator> loadingTask, Action onComplete = null)
        {
            if (isLoading)
            {
                KWDebug.LogWarning("[LoadingScreen] Already loading!");
                return;
            }

            StartCoroutine(CustomLoadingCoroutine(loadingTask, onComplete));
        }

        #region Coroutines

        private IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete)
        {
            isLoading = true;
            loadingStartTime = Time.realtimeSinceStartup;

            currentLoadOperation = SceneManager.LoadSceneAsync(sceneName);
            currentLoadOperation.allowSceneActivation = !autoActivateScene;

            yield return StartCoroutine(UpdateLoadingProgress());

            float elapsedTime = Time.realtimeSinceStartup - loadingStartTime;
            if (elapsedTime < minimumLoadingTime)
            {
                SetStatusText("완료...");
                yield return new WaitForSeconds(minimumLoadingTime - elapsedTime);
            }

            CompleteProgress();
            yield return new WaitForSeconds(0.3f);

            yield return StartCoroutine(FadeOut());

            if (!autoActivateScene && currentLoadOperation != null)
            {
                currentLoadOperation.allowSceneActivation = true;
            }

            yield return new WaitUntil(() => currentLoadOperation.isDone);

            onComplete?.Invoke();

            Hide(true);

            isLoading = false;
            currentLoadOperation = null;
        }

        private IEnumerator LoadSceneByIndexCoroutine(int sceneIndex, Action onComplete)
        {
            string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = $"Scene {sceneIndex}";
            }

            yield return StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
        }

        private IEnumerator CustomLoadingCoroutine(Func<IEnumerator> loadingTask, Action onComplete)
        {
            isLoading = true;
            loadingStartTime = Time.realtimeSinceStartup;

            yield return StartCoroutine(FadeIn());

            if (loadingTask != null)
            {
                yield return StartCoroutine(loadingTask());
            }

            float elapsedTime = Time.realtimeSinceStartup - loadingStartTime;
            if (elapsedTime < minimumLoadingTime)
            {
                yield return new WaitForSeconds(minimumLoadingTime - elapsedTime);
            }

            CompleteProgress();

            onComplete?.Invoke();

            yield return StartCoroutine(FadeOut());

            isLoading = false;
        }

        private IEnumerator UpdateLoadingProgress()
        {
            if (progressBar == null || currentLoadOperation == null)
            {
                yield break;
            }

            while (!currentLoadOperation.isDone)
            {
                float progress = Mathf.Clamp01(currentLoadOperation.progress / 0.9f);
                SetProgress(progress);

                if (progress < 0.3f)
                {
                    SetStatusText("로딩 중...");
                }
                else if (progress < 0.7f)
                {
                    SetStatusText("준비 중...");
                }
                else
                {
                    SetStatusText("거의 완료...");
                }

                yield return null;
            }

            SetProgress(1f);
        }

        private IEnumerator FadeIn()
        {
            Show(true);
            yield return Fade(0f, 1f, fadeInDuration);
        }

        private IEnumerator FadeOut()
        {
            yield return Fade(1f, 0f, fadeOutDuration);
            Hide(true);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (root == null) yield break;

            root.style.opacity = from;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                root.style.opacity = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            root.style.opacity = to;
        }

        #endregion

        #region Show/Hide

        private void Show(bool immediate = false)
        {
            gameObject.SetActive(true);

            if (immediate && root != null)
            {
                root.style.opacity = 1f;
            }

            ResetProgress();
        }

        private void Hide(bool immediate = false)
        {
            if (immediate && root != null)
            {
                root.style.opacity = 0f;
            }

            gameObject.SetActive(false);
        }

        #endregion

        #region Public Utility

        public bool IsLoading => isLoading;

        public void SetProgress(float progress)
        {
            if (progressBar == null) return;

            float target = Mathf.Clamp01(progress);
            progressTween?.Kill();
            progressTween = DOTween.To(() => progressBar.value, ApplyProgress, target, progressTweenDuration)
                .SetEase(progressTweenEase);
        }

        public void SetStatusText(string text)
        {
            if (statusLabel != null) statusLabel.text = text;
        }

        #endregion

        private void ApplyProgress(float value)
        {
            progressBar.value = value;
            progressBar.title = $"{value * 100f:0}%";
        }

        private void ResetProgress()
        {
            progressTween?.Kill();
            if (progressBar != null) ApplyProgress(0f);
            SetStatusText(string.Empty);
        }

        private void CompleteProgress()
        {
            progressTween?.Kill();
            if (progressBar != null) ApplyProgress(1f);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            progressTween?.Kill();
        }
    }
}
