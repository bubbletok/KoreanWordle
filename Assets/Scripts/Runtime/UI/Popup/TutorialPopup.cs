namespace KW.UI.Popup
{
    public class TutorialPopup : BasePopup
    {
        public override void Close()
        {
            Hide();
            Root.RemoveFromHierarchy();
        }
    }
}
