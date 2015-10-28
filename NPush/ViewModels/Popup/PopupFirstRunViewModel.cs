using System.Windows;
using System.Windows.Forms;
using NPush.Objects.ViewModel;

namespace NPush.ViewModels.Popup
{
    public class PopupFirstRunViewModel
    {
        public ViewElement<bool> IsOpen { get; private set; }
        public ViewElement<Rect> Position { get; private set; }

        public PopupFirstRunViewModel()
        {
            this.Position = new ViewElement<Rect>();

            this.IsOpen = new ViewElement<bool> { Value = false };
        }

        public void ShowPopup()
        {
            this.UpdatePosition();
            this.IsOpen.Value = true;
        }

        public void HidePopup()
        {
            this.IsOpen.Value = false;
        }

        private void UpdatePosition()
        {
            var height = Screen.PrimaryScreen.Bounds.Height - 138;
            var width = Screen.PrimaryScreen.Bounds.Width - 323;

            this.Position.Value = new Rect(width, height, 0, 0);
        }
    }
}
