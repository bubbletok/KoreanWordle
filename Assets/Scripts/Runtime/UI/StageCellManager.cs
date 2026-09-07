using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using DG.Tweening;
using KW.Core;
using KW.Core.Settings;
using KW.Core.Constants;
using KW.Managers;
using static KW.Core.Settings.GameplayEnums;

namespace KW.UI
{
    public class StageCellManager : MonoBehaviour
    {
        private const int CELL_COUNT = 20;
        private const int STAR_COUNT = 5;
        private const string DISABLED_CLASS = "stage-cell--disabled";

        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        [Header("Animation Settings")]
        [SerializeField] private float cellAnimationDelay = 0.05f;
        [SerializeField] private float cellAnimationDuration = 0.3f;
        [SerializeField] private bool enablePageTransitionAnimation = true;

        private VisualElement[] cellElements;
        private VisualElement[] cellTabs;
        private Label[] cellNumberLabels;
        private VisualElement[][] cellStars;
        private Button prevPageButton;
        private Button nextPageButton;
        private Button backButton;
        private Label subtitleLabel;

        private int curStagePage = 1;
        private int maxStagePage;
        private GameType currentGameType;
        private bool isInitialized = false;

        private void Start()
        {
            if (document == null)
            {
                KW.Utility.KWDebug.LogError("[StageCellManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            BindElements(document.rootVisualElement);

            maxStagePage = SettingsManager.Gameplay.MaxStages;

            backButton?.RegisterCallback<ClickEvent>(_ => SceneManager.LoadScene(SceneNames.MAIN));

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == SceneNames.BLOCKED_MAIN)
            {
                currentGameType = GameType.Blocked;
                if (subtitleLabel != null) subtitleLabel.text = "모아쓰기";
                curStagePage = GameManager.Instance.Data.GetLastStagePage(GameType.Blocked);
                prevPageButton?.RegisterCallback<ClickEvent>(_ => PrevPage());
                nextPageButton?.RegisterCallback<ClickEvent>(_ => NextPage());
                RefreshStageDisplay(playAnimation: true);
            }
            else if (sceneName == SceneNames.DEBLOCKED_MAIN)
            {
                currentGameType = GameType.Deblocked;
                if (subtitleLabel != null) subtitleLabel.text = "풀어쓰기";
                curStagePage = GameManager.Instance.Data.GetLastStagePage(GameType.Deblocked);
                prevPageButton?.RegisterCallback<ClickEvent>(_ => PrevPage());
                nextPageButton?.RegisterCallback<ClickEvent>(_ => NextPage());
                RefreshStageDisplay(playAnimation: true);
            }

            isInitialized = true;
        }

        private void OnEnable()
        {
            if (isInitialized && GameManager.Exists)
                RefreshStageDisplay(playAnimation: false);
        }

        private void BindElements(VisualElement root)
        {
            cellElements = new VisualElement[CELL_COUNT];
            cellTabs = new VisualElement[CELL_COUNT];
            cellNumberLabels = new Label[CELL_COUNT];
            cellStars = new VisualElement[CELL_COUNT][];

            for (int i = 0; i < CELL_COUNT; i++)
            {
                VisualElement cell = root.Q<VisualElement>($"stage-cell-{i}");
                cellElements[i] = cell;
                cellTabs[i] = root.Q<VisualElement>($"stage-cell-{i}-tab");
                cellNumberLabels[i] = root.Q<Label>($"stage-cell-{i}-number");

                var stars = new VisualElement[STAR_COUNT];
                for (int s = 0; s < STAR_COUNT; s++)
                    stars[s] = root.Q<VisualElement>($"stage-cell-{i}-star-{s}");
                cellStars[i] = stars;

                if (cell != null)
                {
                    int stageIndex = i;
                    cell.RegisterCallback<ClickEvent>(_ => OnCellClicked(stageIndex));
                }
            }

            prevPageButton = root.Q<Button>("prev-page-button");
            nextPageButton = root.Q<Button>("next-page-button");
            backButton = root.Q<Button>("back-button");
            subtitleLabel = root.Q<Label>("stage-subtitle");
        }

        private void OnCellClicked(int cellIndex)
        {
            int stageIndex = cellIndex + (curStagePage - 1) * CELL_COUNT;

            if (stageIndex > 0 && !GameManager.Instance.Data.GetClearStages(currentGameType, stageIndex - 1).isCleared)
            {
                // TODO: "이전 스테이지를 먼저 클리어하세요" 메시지 표시
                return;
            }

            GameManager.Instance.CurrentStage = stageIndex + 1;
            SceneManager.LoadScene(currentGameType == GameType.Blocked ? SceneNames.BLOCKED_STAGE : SceneNames.DEBLOCKED_STAGE);
        }

        private void RefreshStageDisplay(bool playAnimation)
        {
            if (playAnimation)
                PlayCellAppearAnimation();

            for (int i = 0; i < CELL_COUNT; i++)
            {
                int stageNum = i + (curStagePage - 1) * CELL_COUNT;

                if (cellNumberLabels[i] != null) cellNumberLabels[i].text = (stageNum + 1).ToString();

                var (isStageCleared, attemptToClear) = GameManager.Instance.Data.GetClearStages(currentGameType, stageNum);
                bool isLocked = stageNum > 0 && !GameManager.Instance.Data.GetClearStages(currentGameType, stageNum - 1).isCleared;

                cellElements[i]?.EnableInClassList(DISABLED_CLASS, isLocked);

                if (isStageCleared)
                {
                    Color cellColor = attemptToClear == 1
                        ? SettingsManager.UI.fullCompleteCellColor
                        : SettingsManager.UI.completeCellColor;
                    if (cellElements[i] != null) cellElements[i].style.backgroundColor = cellColor;

                    UpdateStageStars(i, attemptToClear);
                }
                else
                {
                    if (cellElements[i] != null) cellElements[i].style.backgroundColor = SettingsManager.UI.uncompleteCellColor;
                    SetStageStarsVisible(i, false);
                }
            }
        }

        public void PrevPage()
        {
            curStagePage = curStagePage > 1 ? curStagePage - 1 : 1;
            GameManager.Instance.Data.SetLastStagePage(currentGameType, curStagePage);
            RefreshStageDisplay(playAnimation: enablePageTransitionAnimation);
        }

        public void NextPage()
        {
            curStagePage = curStagePage < maxStagePage ? curStagePage + 1 : maxStagePage;
            GameManager.Instance.Data.SetLastStagePage(currentGameType, curStagePage);
            RefreshStageDisplay(playAnimation: enablePageTransitionAnimation);
        }

        #region Helper Methods

        private void UpdateStageStars(int cellIndex, int attemptToClear)
        {
            var stars = cellStars[cellIndex];
            if (stars == null) return;

            if (cellTabs[cellIndex] != null) cellTabs[cellIndex].style.display = DisplayStyle.Flex;

            int activatedStars = Mathf.Clamp(STAR_COUNT + 1 - attemptToClear, 0, STAR_COUNT);
            Color starColor = attemptToClear == 1
                ? SettingsManager.UI.fullCompleteTryColor
                : SettingsManager.UI.completeTryColor;

            for (int s = 0; s < STAR_COUNT; s++)
            {
                if (stars[s] == null) continue;
                stars[s].style.display = DisplayStyle.Flex;
                stars[s].style.backgroundColor = s < activatedStars ? starColor : Color.white;
            }
        }

        private void SetStageStarsVisible(int cellIndex, bool visible)
        {
            if (cellTabs[cellIndex] != null)
                cellTabs[cellIndex].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

            var stars = cellStars[cellIndex];
            if (stars == null) return;

            for (int s = 0; s < STAR_COUNT; s++)
            {
                if (stars[s] != null)
                    stars[s].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void PlayCellAppearAnimation()
        {
            for (int i = 0; i < CELL_COUNT; i++)
            {
                VisualElement cell = cellElements[i];
                if (cell == null) continue;

                cell.transform.scale = Vector3.zero;
                DOTween.To(() => cell.transform.scale, v => cell.transform.scale = v, Vector3.one, cellAnimationDuration)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * cellAnimationDelay);
            }
        }

        #endregion
    }
}
