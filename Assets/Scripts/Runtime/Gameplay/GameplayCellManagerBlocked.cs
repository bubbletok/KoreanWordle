using UnityEngine;
using UnityEngine.UIElements;
using KW.Core;
using KW.Managers;
using KW.Gameplay;
using KW.Core.Settings;
using KW.Core.Constants;
using KW.Utility;
using static KW.Core.Settings.GameplayEnums;

namespace KW.UI
{
    [System.Serializable]
    public class GameplayCellManagerBlocked : GameplayCellManager
    {
        protected string[] chosungLetters;
        protected string[] jungsungLetters;
        protected string[] jongsungLetters;
        public CellState[] ChosungStates, JungsungStates, JongsungStates;

        public override void Init()
        {
            base.Init();
            InitialAssign();
            CreateCell(wordSelector.WordLength);
        }

        public override void InitialAssign()
        {
            base.InitialAssign();

            chosungLetters = KoreanInputConstants.ChosungLetters;
            jungsungLetters = KoreanInputConstants.JungsungLetters;
            jongsungLetters = KoreanInputConstants.JongsungLetters;

            int totalCells = maxCol * maxRow * 3;
            ChosungStates = new CellState[totalCells];
            JungsungStates = new CellState[totalCells];
            JongsungStates = new CellState[totalCells];

            for (int i = 0; i < totalCells; i++)
            {
                ChosungStates[i] = 0;
                JungsungStates[i] = 0;
                JongsungStates[i] = 0;
            }
        }

        public override void SetCurrentCellText(string text) { base.SetCurrentCellText(text); }

        public void SetCellState(GameplayCell gameplayCell, CellState state, HangulComponentPosition position)
        {
            GameplayCellBlocked gameplayCellBlocked = gameplayCell as GameplayCellBlocked;
            if (gameplayCellBlocked != null)
            {
                gameplayCellBlocked.SetCellState(state, position);
            }
            else
            {
                KWDebug.LogError("[GameplayCellManagerBlocked] GameplayCell is not GameplayCellBlocked!");
            }
        }

        public override void CreateCell(int wordLen)
        {
            if (wordLen < 1 || wordLen > maxRow)
            {
                KWDebug.LogError($"[GameplayCellManagerBlocked] Invalid word length: {wordLen}. Must be between 1 and {maxRow}.");
                return;
            }

            if (cellGrid == null || cellTemplate == null)
            {
                KWDebug.LogError("[GameplayCellManagerBlocked] Required references are null!");
                return;
            }

            GameplayCells = new GameplayCell[maxCol, maxRow];
            Vector2 cellSize = GetCellSize(wordLen);

            for (int i = 0; i < maxCol; i++)
            {
                VisualElement row = CreateRow();
                for (int j = 0; j < wordLen; j++)
                {
                    GameplayCells[i, j] = InstantiateCell<GameplayCellBlocked>(row, cellSize);
                }
            }
        }
    }
}
