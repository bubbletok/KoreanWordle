using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using KW.Core.Constants;
using KW.UI.Popup;
using KW.Utility;

namespace KW.UI
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        private void Start()
        {
            if (document == null)
            {
                KWDebug.LogError("[MainMenuUIManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            VisualElement root = document.rootVisualElement;

            root.Q<Button>("blocked-button")?.RegisterCallback<ClickEvent>(_ => SceneManager.LoadScene(SceneNames.BLOCKED_MAIN));
            root.Q<Button>("deblocked-button")?.RegisterCallback<ClickEvent>(_ => SceneManager.LoadScene(SceneNames.DEBLOCKED_MAIN));
            root.Q<Button>("statistics-button")?.RegisterCallback<ClickEvent>(_ => SceneManager.LoadScene(SceneNames.STATISTIC));

            root.Q<Button>("blocked-tutorial-button")?.RegisterCallback<ClickEvent>(_ => PopupManager.Instance.ShowTutorial(true));
            root.Q<Button>("deblocked-tutorial-button")?.RegisterCallback<ClickEvent>(_ => PopupManager.Instance.ShowTutorial(false));
        }
    }
}
