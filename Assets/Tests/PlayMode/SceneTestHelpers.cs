using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NUnit.Framework;
using KW.Core;
using KW.UI;

namespace KW.Tests
{
    /// <summary>
    /// Helper utilities for scene-based PlayMode tests
    /// Provides reusable methods for scene loading, button discovery, and common test operations
    /// </summary>
    public static class SceneTestHelpers
    {
        #region Scene Loading

        /// <summary>
        /// Load a scene and wait for it to complete with timeout
        /// </summary>
        /// <param name="sceneName">Name of scene to load</param>
        /// <param name="timeout">Maximum wait time in seconds</param>
        /// <returns>Coroutine enumerator</returns>
        public static IEnumerator LoadSceneAndWait(string sceneName, float timeout = TestConstants.SceneLoadTimeout)
        {
            float startTime = Time.realtimeSinceStartup;

            // Load scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            Assert.IsNotNull(asyncLoad, $"Failed to initiate loading of scene: {sceneName}");

            // Wait for scene to load with timeout
            while (!asyncLoad.isDone)
            {
                if (Time.realtimeSinceStartup - startTime > timeout)
                {
                    Assert.Fail($"Scene '{sceneName}' loading timed out after {timeout} seconds");
                }
                yield return null;
            }

            // Wait additional frames for scene initialization
            for (int i = 0; i < TestConstants.FramesAfterSceneLoad; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Wait for a specific scene to be loaded with polling and timeout
        /// </summary>
        /// <param name="expectedScene">Name of expected scene</param>
        /// <param name="timeout">Maximum wait time in seconds</param>
        /// <returns>Coroutine enumerator</returns>
        public static IEnumerator WaitForSceneLoad(string expectedScene, float timeout = TestConstants.SceneLoadTimeout)
        {
            float startTime = Time.realtimeSinceStartup;
            float pollInterval = TestConstants.PollingInterval;

            // Poll for scene change
            while (SceneManager.GetActiveScene().name != expectedScene)
            {
                if (Time.realtimeSinceStartup - startTime > timeout)
                {
                    string currentScene = SceneManager.GetActiveScene().name;
                    Assert.Fail($"Scene transition timed out after {timeout} seconds. Expected: '{expectedScene}', Current: '{currentScene}'");
                }
                yield return new WaitForSeconds(pollInterval);
            }

            // Wait additional frames for scene initialization
            for (int i = 0; i < TestConstants.FramesAfterSceneLoad; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Assert that the expected scene is currently loaded
        /// </summary>
        /// <param name="expectedScene">Name of expected scene</param>
        public static void AssertSceneLoaded(string expectedScene)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            Assert.AreEqual(expectedScene, currentScene,
                $"Scene mismatch. Expected: '{expectedScene}', Actual: '{currentScene}'");
        }

        #endregion

        #region Button Discovery

        /// <summary>
        /// Find a LoadSceneButton component configured to load the target scene
        /// </summary>
        /// <param name="targetScene">Scene name the button should load</param>
        /// <returns>LoadSceneButton component or null</returns>
        public static LoadSceneButton FindLoadSceneButton(string targetScene)
        {
            var allButtons = Object.FindObjectsByType<LoadSceneButton>(FindObjectsInactive.Include, FindObjectsSortMode.None); // Include inactive
            return allButtons.FirstOrDefault(btn => btn.SceneName == targetScene);
        }

        /// <summary>
        /// Find a button by component type in the current scene
        /// </summary>
        /// <typeparam name="T">Component type (LoadSceneButton, etc.)</typeparam>
        /// <returns>Component or null</returns>
        public static T FindButtonByComponent<T>() where T : MonoBehaviour
        {
            return Object.FindFirstObjectByType<T>(FindObjectsInactive.Include); // Include inactive
        }

        /// <summary>
        /// Get the Button component from a MonoBehaviour
        /// </summary>
        /// <param name="component">Component with Button</param>
        /// <returns>Button component</returns>
        public static Button GetButton(MonoBehaviour component)
        {
            var button = component.GetComponent<Button>();
            Assert.IsNotNull(button, $"Button component not found on {component.gameObject.name}");
            return button;
        }

        #endregion

        #region GameManager Setup

        /// <summary>
        /// Setup GameManager for tests that require it (stage progression tests)
        /// Creates a test GameManager if one doesn't exist
        /// </summary>
        /// <returns>GameManager instance</returns>
        public static GameManager SetupGameManager()
        {
            var existing = GameManager.Instance;
            if (existing != null)
            {
                return existing;
            }

            // Create test GameManager
            var gameManagerObj = new GameObject(TestConstants.TestGameManagerName);
            var gameManager = gameManagerObj.AddComponent<GameManager>();
            Object.DontDestroyOnLoad(gameManagerObj);

            // Initialize with default data if needed
            if (gameManager.Data == null)
            {
                gameManager.Data = new GameData_V2();
            }

            return gameManager;
        }

        /// <summary>
        /// Wait for GameManager to be fully initialized (word lists loaded)
        /// </summary>
        /// <param name="timeout">Maximum wait time in seconds</param>
        /// <returns>Coroutine enumerator</returns>
        public static IEnumerator WaitForGameManagerReady(float timeout = TestConstants.GameManagerLoadTimeout)
        {
            float startTime = Time.realtimeSinceStartup;

            // Wait for GameManager instance to exist
            while (!GameManager.Exists)
            {
                if (Time.realtimeSinceStartup - startTime > timeout)
                {
                    Assert.Fail($"GameManager instance not found after {timeout} seconds");
                }
                yield return null;
            }

            // Wait for word lists to be loaded
            while (!GameManager.Instance.IsWordListsLoaded)
            {
                if (Time.realtimeSinceStartup - startTime > timeout)
                {
                    Assert.Fail($"GameManager word lists not loaded after {timeout} seconds");
                }
                yield return null;
            }

            // Additional frame for safety
            yield return null;
        }

        /// <summary>
        /// Cleanup test GameManager after tests
        /// </summary>
        public static void CleanupGameManager()
        {
            var testGameManager = GameObject.Find(TestConstants.TestGameManagerName);
            if (testGameManager != null)
            {
                Object.Destroy(testGameManager);
            }
        }

        #endregion

        #region UI Interaction

        /// <summary>
        /// Wait for UI to settle after interaction
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        public static IEnumerator WaitForUIInteraction()
        {
            for (int i = 0; i < TestConstants.FramesAfterUIInteraction; i++)
            {
                yield return null;
            }
        }

        #endregion
    }
}
