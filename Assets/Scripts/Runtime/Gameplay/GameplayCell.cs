using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using KW.Core.Settings;

using static KW.Core.Settings.GameplayEnums;

namespace KW.Gameplay
{
    public class GameplayCell
    {
        private const float POP_HALF_DURATION = 0.1f;
        private const float POP_SCALE = 1.1f;

        protected VisualElement root;
        private Label label;
        private Tween popTween;

        protected GameplaySettings gameplaySettings;

        public string Text => label?.text ?? string.Empty;

        public virtual void Attach(VisualElement cellRoot)
        {
            // cellRoot는 VisualTreeAsset.Instantiate()가 반환하는 래퍼(TemplateContainer)라
            // 템플릿 안의 실제 "cell-root" 엘리먼트(배경/텍스트를 담는)를 다시 찾아서 사용한다.
            root = cellRoot.Q<VisualElement>("cell-root") ?? cellRoot;
            label = root.Q<Label>("cell-text");
            gameplaySettings = SettingsManager.Gameplay;
        }

        public virtual void SetCellText(string cellText)
        {
            if (label == null) return;

            label.text = cellText;
            PlayPopAnimation();
        }

        protected virtual Color GetStateColor(CellState state)
        {
            return state switch
            {
                CellState.Empty => gameplaySettings.EmptyColor,
                CellState.Correct => gameplaySettings.CorrectColor,
                CellState.WrongPosition => gameplaySettings.WrongPositionColor,
                CellState.NotInWord => gameplaySettings.NotInWordColor,
                _ => gameplaySettings.DefaultCellColor,
            };
        }

        private void PlayPopAnimation()
        {
            if (root == null) return;

            popTween?.Kill();
            root.transform.scale = Vector3.one;
            popTween = DOTween.Sequence()
                .Append(DOTween.To(() => root.transform.scale, v => root.transform.scale = v, Vector3.one * POP_SCALE, POP_HALF_DURATION).SetEase(Ease.OutQuad))
                .Append(DOTween.To(() => root.transform.scale, v => root.transform.scale = v, Vector3.one, POP_HALF_DURATION).SetEase(Ease.InQuad));
        }
    }
}
