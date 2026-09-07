using System.Collections;
using KW.Core.Constants;
using KW.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace KW.Tests
{
    /// <summary>
    /// Tests for LoadSceneButton component
    /// Validates scene transitions triggered by button clicks
    /// </summary>
    [TestFixture]
    public class SceneLoadButtonTest
    {
        #region Test Case Data

        private static object[] SceneTransitionCases =
        {
            // new object[] { SceneNames.Start, SceneNames.Main },
            new object[] { SceneNames.MAIN, SceneNames.BLOCKED_MAIN },
            new object[] {SceneNames.BLOCKED_MAIN, SceneNames.MAIN},
        };

        #endregion

        #region Setup and Teardown

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        #region Scene Transition Tests

        /// <summary>
        /// Test scene transitions via LoadSceneButton clicks
        /// Uses parameterized tests to validate multiple transitions
        /// </summary>
        [UnityTest]
        public IEnumerator LoadSceneButton_Click_TransitionsToTargetScene(
            [ValueSource(nameof(SceneTransitionCases))] object[] testCase)
        {
            string sourceScene = (string)testCase[0];
            string targetScene = (string)testCase[1];

            // Arrange: Load source scene
            yield return SceneTestHelpers.LoadSceneAndWait(sourceScene);
            SceneTestHelpers.AssertSceneLoaded(sourceScene);

            // Wait for GameManager to be fully initialized (word lists loaded)
            yield return SceneTestHelpers.WaitForGameManagerReady();

            // Find button configured to load target scene
            var loadSceneButton = SceneTestHelpers.FindLoadSceneButton(targetScene);
            Assert.IsNotNull(loadSceneButton,
                $"No LoadSceneButton found in '{sourceScene}' scene configured to load '{targetScene}'");

            var button = SceneTestHelpers.GetButton(loadSceneButton);
            Assert.IsNotNull(button.onClick,
                $"Button onClick is null on LoadSceneButton for '{targetScene}'");

            // Act: Click button
            button.onClick.Invoke();

            // Wait for UI interaction to process
            yield return SceneTestHelpers.WaitForUIInteraction();

            // Wait for scene transition with polling
            yield return SceneTestHelpers.WaitForSceneLoad(targetScene);

            // Assert: Verify target scene is loaded
            SceneTestHelpers.AssertSceneLoaded(targetScene);
        }

        #endregion
    }
}