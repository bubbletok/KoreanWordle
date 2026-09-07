using UnityEngine.UIElements;

using static KW.Core.Settings.GameplayEnums;

namespace KW.Gameplay
{
    public sealed class GameplayCellDeblocked : GameplayCell
    {
        public void SetCellState(CellState state)
        {
            root.style.unityBackgroundImageTintColor = GetStateColor(state);
        }
    }
}
