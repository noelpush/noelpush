using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using NoelPush.Objects.ViewModel;

namespace NoelPush.ViewModels
{
    public class PopupViewModel
    {
        protected int Height;
        protected int Width;

        public ViewElement<bool> IsOpen { get; private set; }
        public ViewElement<Rect> Position { get; private set; }
        public ViewElement<Bitmap> Picture { get; private set; }

        public PopupViewModel(int width, int height)
        {
            this.Height = height;
            this.Width = width;

            this.Position = new ViewElement<Rect>();
            this.IsOpen = new ViewElement<bool> { Value = false };
            this.Picture = new ViewElement<Bitmap>();
        }

        public virtual void ShowPopup(int delay = 3000)
        {
            this.UpdatePosition();

            this.IsOpen.Value = true;
            Thread.Sleep(delay);
            this.IsOpen.Value = false;
        }

        public void ShowPopup(int delay, Bitmap img)
        {
            this.Picture.Value = img;
            this.ShowPopup(delay);
        }

        private void UpdatePosition()
        {
            var height = Screen.PrimaryScreen.Bounds.Height - this.Height;
            var width = Screen.PrimaryScreen.Bounds.Width - this.Width;

            this.Position.Value = new Rect(width, height, 0, 0);
        }
    }
}
