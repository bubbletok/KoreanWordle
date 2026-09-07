using UnityEngine;
using UnityEngine.UIElements;
using KW.Managers;
using KW.Core.Settings;
using KW.Utility;

namespace KW.Gameplay
{
    public class GameplayCellManager : KWLocalSingleton<GameplayCellManager>
    {
        protected int maxCol => SettingsManager.Gameplay.MaxAttempts;
        protected int maxRow => SettingsManager.Gameplay.MaxWordLength;
        protected bool bShowChoHint;
        protected bool bShowPartHint;

        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        [Header("Cell Template")]
        [SerializeField] protected VisualTreeAsset cellTemplate;

        protected VisualElement cellGrid;

        [Header("Game State")]
        public GameplayCell[,] GameplayCells;
        public int NumberOfTry = 0, CurCellPos = 0;

        protected GameWordSelector wordSelector => GameplayManager.Instance.WordSelector;

        public override void Init()
        {
            base.Init();

            if (document == null)
            {
                KWDebug.LogError("[GameplayCellManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            cellGrid = document.rootVisualElement.Q<VisualElement>("cell-grid");
        }

        public virtual void InitialAssign()
        {
            bShowChoHint = false;
            bShowPartHint = false;
        }

        public virtual void SetCurrentCellText(string text)
        {
            if (GameplayCells[NumberOfTry, CurCellPos] != null)
            {
                GameplayCells[NumberOfTry, CurCellPos].SetCellText(text);
            }
            else
            {
                KWDebug.LogError($"GameplayCell at [{NumberOfTry}, {CurCellPos}] is null");
            }
        }

        public virtual void CreateCell(int wordLen) { }

        protected VisualElement CreateRow()
        {
            var row = new VisualElement();
            row.AddToClassList("cell-row");
            cellGrid.Add(row);
            return row;
        }

        protected T InstantiateCell<T>(VisualElement row, Vector2 cellSize) where T : GameplayCell, new()
        {
            VisualElement cellRoot = cellTemplate.Instantiate();
            cellRoot.style.width = cellSize.x;
            cellRoot.style.height = cellSize.y;
            cellRoot.style.marginLeft = 5;
            cellRoot.style.marginRight = 5;
            row.Add(cellRoot);

            T cell = new T();
            cell.Attach(cellRoot);
            return cell;
        }

        protected Vector2 GetCellSize(int wordLen)
        {
            var ui = SettingsManager.UI;
            return wordLen >= 5 ? ui.cellSize5Letter : ui.cellDefaultSize;
        }
    }
}
