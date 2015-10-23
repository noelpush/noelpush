using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using NPush.Objects.ViewModel;

namespace NPush.ViewModels
{
    public class PopupViewModel
    {
        public ViewElement<bool> IsOpen { get; private set; }
        public ViewElement<Rect> Position { get; private set; } 
        public ViewElement<Bitmap> Picture { get; private set; }

        public PopupViewModel()
        {
            this.Position = new ViewElement<Rect>();

            this.Picture = new ViewElement<Bitmap>();
            this.IsOpen = new ViewElement<bool> { Value = false };
        }

        public void ShowPopup(Bitmap img)
        {
            this.UpdatePosition();
            this.Picture.Value = img;
            this.IsOpen.Value = true;
        }

        public void HidePopup()
        {
            this.IsOpen.Value = false;
        }

        private void UpdatePosition()
        {
            var height = Screen.PrimaryScreen.WorkingArea.Height - 65;
            var width = Screen.PrimaryScreen.WorkingArea.Width - 310;

            this.Position.Value = new Rect(width, height, 0, 0);
        }
    }
}
