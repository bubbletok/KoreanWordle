using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using KW.Managers;
using KW.Core.Constants;
using KW.UI.Popup;
using KW.Utility;

namespace KW.UI
{
    public class GameplayUIManager : KWLocalSingleton<GameplayUIManager>
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        [SerializeField] private KeyboardView keyboardView;

        public bool IsCaps => keyboardView != null && keyboardView.IsCaps;

        public override void Init()
        {
            base.Init();

            if (document == null)
            {
                KWDebug.LogError("[GameplayUIManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            VisualElement root = document.rootVisualElement;

            root.Q<Button>("back-button")?.RegisterCallback<ClickEvent>(_ => OnBackButtonClicked());
            root.Q<Button>("help-button")?.RegisterCallback<ClickEvent>(_ => OnHelpButtonClicked());
        }

        public void ToggleCapsMode()
        {
            keyboardView?.ToggleCaps();
        }

        private void OnBackButtonClicked()
        {
            string targetScene = GameplayManager.Instance.GameType == GameType.Deblocked
                ? SceneNames.DEBLOCKED_MAIN
                : SceneNames.BLOCKED_MAIN;
            SceneManager.LoadScene(targetScene);
        }

        private void OnHelpButtonClicked()
        {
            bool isBlocked = GameplayManager.Instance.GameType != GameType.Deblocked;
            PopupManager.Instance.ShowTutorial(isBlocked);
        }
    }
}
