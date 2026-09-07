using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using KW.Core;
using KW.Core.Constants;
using KW.Managers;

namespace KW.UI.Popup
{
    public class GameResultPopup : BasePopup
    {
        private Label titleLabel;
        private Label answerLabel;
        private Label explanationLabel;
        private VisualElement resultIcon;
        private Button returnToStageButton;
        private Button nextStageButton;

        private bool isWin;
        private GameType currentGameType;

        protected override void OnAttach()
        {
            titleLabel = Root.Q<Label>("title-label");
            answerLabel = Root.Q<Label>("answer-label");
            explanationLabel = Root.Q<Label>("explanation-label");
            resultIcon = Root.Q<VisualElement>("result-icon");
            returnToStageButton = Root.Q<Button>("return-button");
            nextStageButton = Root.Q<Button>("next-stage-button");

            returnToStageButton?.RegisterCallback<ClickEvent>(_ => ReturnToStageSelection());
            nextStageButton?.RegisterCallback<ClickEvent>(_ => LoadNextStage());
        }

        public void ShowResult(bool isWin, string answer, string explanation = null)
        {
            this.isWin = isWin;

            if (GameplayManager.Exists)
            {
                currentGameType = GameplayManager.Instance.GameType;
            }

            if (titleLabel != null) titleLabel.text = isWin ? "성공!" : "실패";
            if (answerLabel != null) answerLabel.text = $"답: {answer}";

            if (explanationLabel != null)
            {
                bool hasExplanation = !string.IsNullOrEmpty(explanation);
                explanationLabel.text = hasExplanation ? explanation : string.Empty;
                explanationLabel.style.display = hasExplanation ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (resultIcon != null)
            {
                resultIcon.EnableInClassList("result-icon--win", isWin);
                resultIcon.EnableInClassList("result-icon--lose", !isWin);
            }

            if (nextStageButton != null)
            {
                nextStageButton.style.display = isWin ? DisplayStyle.Flex : DisplayStyle.None;
            }

            Show();
        }

        private void ReturnToStageSelection()
        {
            Close();

            string targetScene = GetStageSelectionScene();
            if (!string.IsNullOrEmpty(targetScene))
            {
                SceneManager.LoadScene(targetScene);
            }
        }

        private void LoadNextStage()
        {
            Close();

            if (GameManager.Exists)
            {
                GameManager.Instance.CurrentStage++;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private string GetStageSelectionScene()
        {
            return currentGameType switch
            {
                GameType.Blocked => SceneNames.BLOCKED_MAIN,
                GameType.Deblocked => SceneNames.DEBLOCKED_MAIN,
                GameType.TodayWord => SceneNames.MAIN,
                _ => SceneNames.MAIN
            };
        }

        public override void ResetState()
        {
            base.ResetState();
            isWin = false;

            if (titleLabel != null) titleLabel.text = string.Empty;
            if (answerLabel != null) answerLabel.text = string.Empty;
            if (explanationLabel != null)
            {
                explanationLabel.text = string.Empty;
                explanationLabel.style.display = DisplayStyle.None;
            }
            if (resultIcon != null)
            {
                resultIcon.RemoveFromClassList("result-icon--win");
                resultIcon.RemoveFromClassList("result-icon--lose");
            }
        }
    }
}
