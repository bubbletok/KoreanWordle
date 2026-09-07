using UnityEngine;

using KW.Core;
using KW.Core.Settings;
using KW.UI;
using KW.Managers;
using KW.Gameplay;
using KW.Utility;

namespace KW.Input
{
    public abstract class KoreanInputHandler : KWLocalSingleton<KoreanInputHandler>
    {
        [SerializeField] protected KeyboardView keyboardView;

        protected GameWordSelector wordSelector;
        protected string answer;
        protected GameManager gameManager;
        protected GameplayManager gameplayManager;
        protected GameplayUIManager gameplayUIManager;
        protected GameplaySettings gameplaySettings;

        public override void Init()
        {
            InitializeCommon();
            InitializeMode();
        }

        private void InitializeCommon()
        {
            gameManager = GameManager.Instance;
            gameplayManager = GameplayManager.Instance;
            gameplayUIManager = GameplayUIManager.Instance;
            gameplaySettings = SettingsManager.Gameplay;
            wordSelector = gameplayManager.WordSelector;

            if (wordSelector == null)
                KWDebug.LogError("[KoreanInputHandler] WordSelector not found!");
        }

        protected abstract void InitializeMode();
        public abstract void EnterChar(string character);
        public abstract void Backspace();
        public abstract void EnterWord();

        public void CapsWord()
        {
            if (gameplayManager.IsGameEnd) return;
            gameplayUIManager.ToggleCapsMode();
        }
    }
}
