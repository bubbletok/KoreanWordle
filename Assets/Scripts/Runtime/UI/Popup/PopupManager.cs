using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using KW.Managers;
using KW.Utility;

namespace KW.UI.Popup
{
    public class PopupManager : KWGlobalSingleton<PopupManager>
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument popupDocument;

        [Header("Popup Templates")]
        [SerializeField] private VisualTreeAsset confirmPopupAsset;
        [SerializeField] private VisualTreeAsset messagePopupAsset;
        [SerializeField] private VisualTreeAsset gameResultPopupAsset;
        [SerializeField] private VisualTreeAsset tutorialPopupBlockedAsset;
        [SerializeField] private VisualTreeAsset tutorialPopupDeblockedAsset;

        [Header("Popup Configuration")]
        [SerializeField] private int initialPoolSize = 3;

        private VisualElement popupLayer;
        private readonly Dictionary<Type, VisualTreeAsset> templateLookup = new Dictionary<Type, VisualTreeAsset>();
        private readonly Dictionary<Type, Queue<BasePopup>> popupPools = new Dictionary<Type, Queue<BasePopup>>();
        private readonly List<BasePopup> activePopups = new List<BasePopup>();

        protected override void OnAwake()
        {
            base.OnAwake();

            if (popupDocument == null)
            {
                KWDebug.LogError("[PopupManager] UIDocument가 지정되지 않았습니다. 씬에 UIDocument를 배치하고 Inspector에 연결하세요.");
                return;
            }

            popupLayer = popupDocument.rootVisualElement.Q("popup-layer") ?? popupDocument.rootVisualElement;

            templateLookup[typeof(ConfirmPopup)] = confirmPopupAsset;
            templateLookup[typeof(MessagePopup)] = messagePopupAsset;
            templateLookup[typeof(GameResultPopup)] = gameResultPopupAsset;

            WarmPool<ConfirmPopup>();
            WarmPool<MessagePopup>();
            WarmPool<GameResultPopup>();
        }

        private void WarmPool<T>() where T : BasePopup, new()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                BasePopup popup = CreatePopup<T>();
                if (popup == null) return;
                popup.Hide();
                ReturnToPool(popup);
            }
        }

        public T GetPopup<T>() where T : BasePopup, new()
        {
            Type type = typeof(T);

            // 같은 타입 팝업이 이미 떠 있으면 새로 쌓지 않고 재사용한다.
            // (연타 시 동일 팝업이 겹겹이 쌓여 앞의 것만 닫히고 뒤의 동일한 팝업이 그대로 남아있는
            //  "닫기 버튼이 안 먹는 것처럼 보이는" 문제의 원인이었음)
            foreach (BasePopup active in activePopups)
            {
                if (active.GetType() == type) return (T)active;
            }

            if (popupPools.TryGetValue(type, out Queue<BasePopup> pool) && pool.Count > 0)
            {
                T popup = (T)pool.Dequeue();
                popup.ResetState();
                activePopups.Add(popup);
                return popup;
            }

            T newPopup = CreatePopup<T>();
            if (newPopup != null)
            {
                activePopups.Add(newPopup);
            }
            return newPopup;
        }

        private T CreatePopup<T>() where T : BasePopup, new()
        {
            if (popupLayer == null) return null;

            if (!templateLookup.TryGetValue(typeof(T), out VisualTreeAsset asset) || asset == null)
            {
                KWDebug.LogError($"[PopupManager] {typeof(T).Name}에 대한 VisualTreeAsset이 지정되지 않았습니다.");
                return null;
            }

            VisualElement root = asset.Instantiate();
            popupLayer.Add(root);

            T popup = new T();
            popup.Attach(root);
            return popup;
        }

        public void ReturnToPool(BasePopup popup)
        {
            if (popup == null) return;

            Type type = popup.GetType();
            if (!popupPools.TryGetValue(type, out Queue<BasePopup> pool))
            {
                pool = new Queue<BasePopup>();
                popupPools[type] = pool;
            }

            popup.ResetState();
            pool.Enqueue(popup);
            activePopups.Remove(popup);
        }

        public void CloseAllPopups()
        {
            List<BasePopup> popupsToClose = new List<BasePopup>(activePopups);
            foreach (BasePopup popup in popupsToClose)
            {
                popup.Close();
            }
        }

        public BasePopup GetTopPopup()
        {
            return activePopups.Count > 0 ? activePopups[activePopups.Count - 1] : null;
        }

        public bool HasActivePopups() => activePopups.Count > 0;

        #region Convenience Methods

        public GameResultPopup ShowGameResult(bool isWin, string answer, string explanation = null)
        {
            GameResultPopup popup = GetPopup<GameResultPopup>();
            popup?.ShowResult(isWin, answer, explanation);
            return popup;
        }

        public MessagePopup ShowMessage(string message, float duration = 1.5f)
        {
            MessagePopup popup = GetPopup<MessagePopup>();
            popup?.ShowMessage(message, duration);
            return popup;
        }

        // TutorialPopup은 Blocked/Deblocked용 내용이 서로 달라 타입 기반 풀 대신
        // 매번 지정된 에셋으로 새로 만들고 Close 시 파괴한다.
        public TutorialPopup ShowTutorial(bool isBlocked)
        {
            if (popupLayer == null) return null;

            VisualTreeAsset asset = isBlocked ? tutorialPopupBlockedAsset : tutorialPopupDeblockedAsset;
            if (asset == null)
            {
                KWDebug.LogError("[PopupManager] TutorialPopup 에셋이 지정되지 않았습니다.");
                return null;
            }

            VisualElement root = asset.Instantiate();
            popupLayer.Add(root);

            TutorialPopup popup = new TutorialPopup();
            popup.Attach(root);
            popup.Show();
            return popup;
        }

        #endregion
    }
}
