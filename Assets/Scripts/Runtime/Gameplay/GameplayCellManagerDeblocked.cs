using UnityEngine;
using UnityEngine.UIElements;
using KW.Core;
using KW.Managers;
using KW.Gameplay;
using KW.Core.Settings;
using KW.Utility;
using static KW.Core.Settings.GameplayEnums;

namespace KW.UI
{
    public class GameplayCellManagerDeblocked : GameplayCellManager
    {
        public override void Init()
        {
            base.Init();
            InitialAssign();
            CreateCell(wordSelector.DecomposedWordLength);
        }

        public override void InitialAssign()
        {
            base.InitialAssign();
        }

        public override void SetCurrentCellText(string text) { base.SetCurrentCellText(text); }

        public void SetCellState(GameplayCell gameplayCell, CellState state)
        {
            GameplayCellDeblocked gameplayCellDeblocked = gameplayCell as GameplayCellDeblocked;
            gameplayCellDeblocked.SetCellState(state);
        }

        public override void CreateCell(int wordLen)
        {
            if (wordLen < 1 || wordLen > maxRow)
            {
                KWDebug.LogError($"[GameplayCellManagerDeblocked] Invalid word length: {wordLen}. Must be between 1 and {maxRow}.");
                return;
            }

            if (cellGrid == null || cellTemplate == null)
            {
                KWDebug.LogError("[GameplayCellManagerDeblocked] Required references are null!");
                return;
            }

            GameplayCells = new GameplayCell[maxCol, maxRow];
            Vector2 cellSize = GetCellSize(wordLen);

            for (int i = 0; i < maxCol; i++)
            {
                VisualElement row = CreateRow();
                for (int j = 0; j < wordLen; j++)
                {
                    GameplayCells[i, j] = InstantiateCell<GameplayCellDeblocked>(row, cellSize);
                }
            }
        }
    }
}
