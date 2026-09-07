using UnityEngine;
using UnityEngine.UIElements;
using KW.Core.Settings;

namespace KW.Input
{
    public class KeyboardCell
    {
        private Button button;
        private GameplayEnums.CellState keyCellState = GameplayEnums.CellState.None;

        private KoreanInputHandler koreanInputHandler => KoreanInputHandler.Instance;

        public string KeyName { get; private set; }

        public void Attach(Button keyButton, string keyName)
        {
            button = keyButton;
            KeyName = keyName;
            button.text = keyName;
            button.RegisterCallback<ClickEvent>(_ => koreanInputHandler.EnterChar(KeyName));
        }

        public void SetKeyCellState(string key, GameplayEnums.CellState state)
        {
            if (KeyName != key) return;

            switch (state)
            {
                case GameplayEnums.CellState.Empty:
                    if (keyCellState == GameplayEnums.CellState.Correct) return;
                    if (keyCellState == GameplayEnums.CellState.WrongPosition) return;
                    break;
                case GameplayEnums.CellState.NotInWord:
                    if (keyCellState == GameplayEnums.CellState.Correct) return;
                    if (keyCellState == GameplayEnums.CellState.WrongPosition) return;
                    break;
                case GameplayEnums.CellState.WrongPosition:
                    if (keyCellState == GameplayEnums.CellState.Correct) return;
                    break;
            }

            Color stateColor = state switch
            {
                GameplayEnums.CellState.Empty => SettingsManager.Gameplay.EmptyColor,
                GameplayEnums.CellState.NotInWord => SettingsManager.Gameplay.NotInWordColor,
                GameplayEnums.CellState.WrongPosition => SettingsManager.Gameplay.WrongPositionColor,
                GameplayEnums.CellState.Correct => SettingsManager.Gameplay.CorrectColor,
                _ => Color.white,
            };

            keyCellState = state;
            button.style.backgroundColor = stateColor;
        }
    }
}
