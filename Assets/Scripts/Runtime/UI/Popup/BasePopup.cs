using UnityEngine.UIElements;

namespace KW.UI.Popup
{
    public abstract class BasePopup
    {
        protected VisualElement Root { get; private set; }

        private const string HIDDEN_CLASS = "popup--hidden";

        public void Attach(VisualElement root)
        {
            Root = root;

            // asset.Instantiate()가 반환하는 TemplateContainer는 스타일이 없어 popup-layer 안에서
            // 일반 플로우로 배치되므로, UXML 루트(.popup)의 전체화면 absolute 배치가 먹히도록 직접 늘려준다.
            Root.style.position = Position.Absolute;
            Root.style.top = 0;
            Root.style.left = 0;
            Root.style.right = 0;
            Root.style.bottom = 0;

            Root.AddToClassList(HIDDEN_CLASS);
            Root.pickingMode = PickingMode.Ignore;
            Root.style.display = DisplayStyle.None;

            Button closeButton = Root.Q<Button>("close-button");
            closeButton?.RegisterCallback<ClickEvent>(_ => Close());

            OnAttach();
        }

        protected virtual void OnAttach() { }

        public virtual void Show()
        {
            Root.style.display = DisplayStyle.Flex;
            Root.RemoveFromClassList(HIDDEN_CLASS);
            Root.pickingMode = PickingMode.Position;
            Root.BringToFront();
        }

        public virtual void Hide()
        {
            Root.AddToClassList(HIDDEN_CLASS);
            Root.pickingMode = PickingMode.Ignore;
            // ponytail: display:none이 opacity/scale 페이드아웃 트랜지션보다 먼저 적용돼 닫힘 애니메이션이 즉시 끊김.
            // 클릭 차단(풀스크린 유령 오버레이) 방지가 우선이라 트레이드오프로 감수. 필요하면 스케줄러로 트랜지션 끝난 뒤 None 적용.
            Root.style.display = DisplayStyle.None;
        }

        public virtual void Close()
        {
            Hide();
            PopupManager.Instance?.ReturnToPool(this);
        }

        public virtual void ResetState() { }
    }
}
