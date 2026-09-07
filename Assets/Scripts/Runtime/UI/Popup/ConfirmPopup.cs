using System;
using UnityEngine.UIElements;

namespace KW.UI.Popup
{
    public class ConfirmPopup : BasePopup
    {
        private Label messageLabel;
        private Button confirmButton;
        private Button cancelButton;

        private Action onConfirm;
        private Action onCancel;

        protected override void OnAttach()
        {
            messageLabel = Root.Q<Label>("message-label");
            confirmButton = Root.Q<Button>("confirm-button");
            cancelButton = Root.Q<Button>("cancel-button");

            confirmButton?.RegisterCallback<ClickEvent>(_ =>
            {
                onConfirm?.Invoke();
                Close();
            });

            cancelButton?.RegisterCallback<ClickEvent>(_ =>
            {
                onCancel?.Invoke();
                Close();
            });
        }

        public void ShowConfirm(
            string message,
            Action onConfirm,
            Action onCancel = null,
            string confirmText = "확인",
            string cancelText = "취소")
        {
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;

            if (messageLabel != null) messageLabel.text = message;
            if (confirmButton != null) confirmButton.text = confirmText;
            if (cancelButton != null) cancelButton.text = cancelText;

            Show();
        }

        public override void ResetState()
        {
            base.ResetState();

            onConfirm = null;
            onCancel = null;

            if (messageLabel != null) messageLabel.text = string.Empty;
        }
    }
}
