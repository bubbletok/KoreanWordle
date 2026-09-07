using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using KW.Core.Constants;
using KW.UI.Loading;
using KW.UI.Animation;
using KW.Utility;

namespace KW.UI.Scene
{
    public class StartSceneController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField, FormerlySerializedAs("_logoAnimator")] private UIElementAnimator logoAnimator;
        [SerializeField, FormerlySerializedAs("_titleAnimator")] private UIElementAnimator titleAnimator;
        [SerializeField, FormerlySerializedAs("_subtitleAnimator")] private UIElementAnimator subtitleAnimator;

        [Header("Animation Timing")]
        [SerializeField, FormerlySerializedAs("_logoDelay")] private float logoDelay = 0f;
        [SerializeField, FormerlySerializedAs("_titleDelay")] private float titleDelay = 0.5f;
        [SerializeField, FormerlySerializedAs("_subtitleDelay")] private float subtitleDelay = 0.8f;
        [SerializeField, FormerlySerializedAs("_loadSceneDelay")] private float loadSceneDelay = 2.5f;

        [Header("Scene Settings")]
        [SerializeField, FormerlySerializedAs("_targetSceneName")] private string targetSceneName = SceneNames.MAIN;

        private void Start()
        {
            StartCoroutine(StartSequence());
        }

        private IEnumerator StartSequence()
        {
            yield return StartCoroutine(PlayTitleAnimations());

            yield return CoroutineUtility.WaitForSeconds(loadSceneDelay);

            LoadMainScene();
        }

        private IEnumerator PlayTitleAnimations()
        {
            if (logoAnimator != null)
            {
                yield return CoroutineUtility.WaitForSeconds(logoDelay);
                logoAnimator.Play();
            }

            if (titleAnimator != null)
            {
                yield return CoroutineUtility.WaitForSeconds(titleDelay - logoDelay);
                titleAnimator.Play();
            }

            if (subtitleAnimator != null)
            {
                yield return CoroutineUtility.WaitForSeconds(subtitleDelay - titleDelay);
                subtitleAnimator.Play();
            }
        }

        private void LoadMainScene()
        {
            if (LoadingScreen.Instance != null)
            {
                LoadingScreen.Instance.LoadSceneAsync(targetSceneName, OnSceneLoaded);
            }
            else
            {
                KWDebug.LogError("[StartSceneController] LoadingScreen instance not found!");
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
            }
        }

        private void OnSceneLoaded()
        {
            KWDebug.Log($"[StartSceneController] Successfully loaded scene: {targetSceneName}");
        }

        #region Alternative Methods

        [ContextMenu("Load Main Scene Immediately")]
        private void LoadMainSceneImmediately()
        {
            StopAllCoroutines();
            LoadMainScene();
        }

        [ContextMenu("Skip to Main Scene")]
        private void SkipToMainScene()
        {
            StopAllCoroutines();
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (titleDelay < logoDelay)
            {
                titleDelay = logoDelay;
            }

            if (subtitleDelay < titleDelay)
            {
                subtitleDelay = titleDelay;
            }

            if (loadSceneDelay < subtitleDelay)
            {
                loadSceneDelay = subtitleDelay + 0.5f;
            }
        }
#endif
    }
}
