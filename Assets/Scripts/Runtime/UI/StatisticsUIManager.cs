using UnityEngine;
using UnityEngine.UIElements;
using KW.Managers;
using KW.Core.Constants;
using static KW.Core.Settings.GameplayEnums;
using KW.UI.Popup;
using KW.Utility;

namespace KW.UI
{
    public class StatisticsUIManager : KWLocalSingleton<StatisticsUIManager>
    {
        private const string ACTIVE_TAB_CLASS = "stats-tab-button--active";

        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        private Button blockedTabButton;
        private Button deblockedTabButton;
        private Button backButton;
        private Button resetDataButton;

        private Label totalAttemptsLabel;
        private Label totalClearsLabel;
        private Label winRateLabel;
        private Label averageAttemptsLabel;

        private readonly VisualElement[] barFills = new VisualElement[7];
        private readonly Label[] barCounts = new Label[7];

        private readonly VisualElement[] stageStatFills = new VisualElement[6];
        private readonly Label[] stageStatProgress = new Label[6];
        private readonly Label[] stageStatRates = new Label[6];

        private GameType currentGameType = GameType.Blocked;
        private StatisticsManager statisticsManager;

        #region Unity Lifecycle

        public override void Init()
        {
            base.Init();

            if (document == null)
            {
                KWDebug.LogError("[StatisticsUIManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            statisticsManager = StatisticsManager.Instance;

            VisualElement root = document.rootVisualElement;
            BindElements(root);

            blockedTabButton?.RegisterCallback<ClickEvent>(_ => SwitchTab(GameType.Blocked));
            deblockedTabButton?.RegisterCallback<ClickEvent>(_ => SwitchTab(GameType.Deblocked));
            backButton?.RegisterCallback<ClickEvent>(_ => OnBackButtonClicked());
            resetDataButton?.RegisterCallback<ClickEvent>(_ => OnResetDataButtonClicked());

            SwitchTab(GameType.Blocked);
        }

        private void BindElements(VisualElement root)
        {
            blockedTabButton = root.Q<Button>("blocked-tab-button");
            deblockedTabButton = root.Q<Button>("deblocked-tab-button");
            backButton = root.Q<Button>("back-button");
            resetDataButton = root.Q<Button>("reset-data-button");

            totalAttemptsLabel = root.Q<Label>("total-attempts-label");
            totalClearsLabel = root.Q<Label>("total-clears-label");
            winRateLabel = root.Q<Label>("win-rate-label");
            averageAttemptsLabel = root.Q<Label>("average-attempts-label");

            for (int i = 0; i < barFills.Length; i++)
            {
                barFills[i] = root.Q<VisualElement>($"bar-fill-{i}");
                barCounts[i] = root.Q<Label>($"bar-count-{i}");
            }

            for (int i = 0; i < stageStatFills.Length; i++)
            {
                stageStatFills[i] = root.Q<VisualElement>($"stage-stat-fill-{i}");
                stageStatProgress[i] = root.Q<Label>($"stage-stat-progress-{i}");
                stageStatRates[i] = root.Q<Label>($"stage-stat-rate-{i}");
            }
        }

        #endregion

        #region Tab Management

        private void SwitchTab(GameType gameType)
        {
            currentGameType = gameType;

            blockedTabButton?.EnableInClassList(ACTIVE_TAB_CLASS, currentGameType == GameType.Blocked);
            deblockedTabButton?.EnableInClassList(ACTIVE_TAB_CLASS, currentGameType == GameType.Deblocked);

            UpdateAllStatistics();
        }

        #endregion

        #region Statistics Update

        private void UpdateAllStatistics()
        {
            if (statisticsManager == null)
            {
                KWDebug.LogError("[StatisticsUIManager] StatisticsManager not found!");
                return;
            }

            UpdateOverallStats();
            UpdateAttemptDistribution();
            UpdateStageTypeStats();
        }

        private void UpdateOverallStats()
        {
            var stats = statisticsManager.GetOverallStats(currentGameType);

            if (totalAttemptsLabel != null) totalAttemptsLabel.text = $"{stats.totalAttempts}회";
            if (totalClearsLabel != null) totalClearsLabel.text = $"{stats.totalClears}회";
            if (winRateLabel != null) winRateLabel.text = $"{stats.winRate:F1}%";
            if (averageAttemptsLabel != null)
                averageAttemptsLabel.text = stats.averageAttempts > 0 ? $"{stats.averageAttempts:F1}회" : "-";
        }

        private void UpdateAttemptDistribution()
        {
            var distribution = statisticsManager.GetAttemptDistribution(currentGameType);
            int[] counts = distribution.attemptCounts;
            int maxCount = distribution.maxCount;

            for (int i = 0; i < barFills.Length && i < counts.Length; i++)
            {
                float normalized = maxCount > 0 ? counts[i] / (float)maxCount : 0f;
                barFills[i]?.SetStyleWidthPercent(normalized);
                if (barCounts[i] != null) barCounts[i].text = counts[i].ToString();
            }
        }

        private void UpdateStageTypeStats()
        {
            var statsList = statisticsManager.GetStageTypeStats(currentGameType);

            for (int i = 0; i < stageStatFills.Length && i < statsList.Count; i++)
            {
                var stats = statsList[i];
                float normalized = stats.totalWords > 0 ? stats.clearedCount / (float)stats.totalWords : 0f;

                stageStatFills[i]?.SetStyleWidthPercent(normalized);
                if (stageStatProgress[i] != null) stageStatProgress[i].text = $"{stats.clearedCount} / {stats.totalWords}";
                if (stageStatRates[i] != null) stageStatRates[i].text = $"{stats.clearRate:F1}%";
            }
        }

        #endregion

        #region Button Callbacks

        private void OnBackButtonClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.MAIN);
        }

        private void OnResetDataButtonClicked()
        {
            var popup = PopupManager.Instance.GetPopup<ConfirmPopup>();
            if (popup != null)
            {
                popup.ShowConfirm(
                    "모든 게임 데이터를 초기화하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
                    OnConfirmReset,
                    OnCancelReset
                );
            }
            else
            {
                KWDebug.LogWarning("[StatisticsUIManager] ConfirmPopup not found. Resetting without confirmation.");
                OnCancelReset();
            }
        }

        private void OnConfirmReset()
        {
            if (statisticsManager != null)
            {
                statisticsManager.ResetStatistics();
                UpdateAllStatistics();
                KWDebug.Log("[StatisticsUIManager] Statistics reset confirmed");
            }
        }

        private void OnCancelReset()
        {
            KWDebug.Log("[StatisticsUIManager] Statistics reset cancelled");
        }

        #endregion
    }

    internal static class VisualElementExtensions
    {
        public static void SetStyleWidthPercent(this VisualElement element, float normalized)
        {
            element.style.width = new Length(Mathf.Clamp01(normalized) * 100f, LengthUnit.Percent);
        }
    }
}
