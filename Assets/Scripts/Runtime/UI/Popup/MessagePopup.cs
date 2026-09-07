using UnityEngine.UIElements;

namespace KW.UI.Popup
{
    public class MessagePopup : BasePopup
    {
        private Label messageLabel;
        private IVisualElementScheduledItem autoCloseTask;

        protected override void OnAttach()
        {
            messageLabel = Root.Q<Label>("message-label");
        }

        public void ShowMessage(string message, float duration = 1.5f)
        {
            if (messageLabel != null) messageLabel.text = message;

            Show();

            autoCloseTask?.Pause();
            autoCloseTask = Root.schedule.Execute(Close).StartingIn((long)(duration * 1000));
        }

        public override void Hide()
        {
            autoCloseTask?.Pause();
            base.Hide();
        }

        public override void ResetState()
        {
            base.ResetState();
            autoCloseTask?.Pause();
            if (messageLabel != null) messageLabel.text = string.Empty;
        }
    }
}
