using UnityEngine.UIElements;
using KW.Utility;

using static KW.Core.Settings.GameplayEnums;

namespace KW.Gameplay
{
    public sealed class GameplayCellBlocked : GameplayCell
    {
        private VisualElement[] cellStates;

        public override void Attach(VisualElement cellRoot)
        {
            base.Attach(cellRoot);

            cellStates = new[]
            {
                root.Q<VisualElement>("cell-cho"),
                root.Q<VisualElement>("cell-jung"),
                root.Q<VisualElement>("cell-jong"),
            };
        }

        public void SetCellState(CellState state, HangulComponentPosition position)
        {
            int positionIndex = (int)position;

            if (positionIndex < 0 || positionIndex >= cellStates.Length)
            {
                KWDebug.LogWarning($"[GameplayCellBlocked] Invalid position: {position}.");
                return;
            }

            VisualElement stateElement = cellStates[positionIndex];
            if (stateElement == null)
            {
                KWDebug.LogError($"[GameplayCellBlocked] CellState element at position {position} not found in template!");
                return;
            }

            stateElement.style.visibility = Visibility.Visible;
            stateElement.style.backgroundColor = GetStateColor(state);
        }
    }
}
