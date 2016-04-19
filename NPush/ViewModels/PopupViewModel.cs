using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using NoelPush.Objects.ViewModel;
using NoelPush.Services;

namespace NoelPush.ViewModels
{
    public class PopupViewModel
    {
        protected int Width;
        protected int Height;
        private readonly int delay;

        public ViewElement<bool> IsOpen { get; private set; }
        public ViewElement<Rect> Position { get; private set; }
        public ViewElement<Bitmap> Picture { get; private set; }

        public PopupViewModel(int width, int height, int delay)
        {
            this.Height = height;
            this.Width = width;
            this.delay = delay;

            this.Position = new ViewElement<Rect>();
            this.IsOpen = new ViewElement<bool> { Value = false };
            this.Picture = new ViewElement<Bitmap>();
        }

        public virtual void ShowPopup()
        {
            this.UpdatePosition();

            this.IsOpen.Value = true;
            Thread.Sleep(this.delay);
            this.IsOpen.Value = false;

            if (CommandService.IsShellMode)
            {
                Environment.Exit(1);
            }
        }

        public void ShowPopup(Bitmap img)
        {
            this.Picture.Value = img;
            this.ShowPopup();
        }

        private void UpdatePosition()
        {
            var currentScreen = Screen.FromPoint(Cursor.Position);

            var width = currentScreen.Bounds.X + currentScreen.WorkingArea.Width - this.Width;
            var height = currentScreen.Bounds.Y + currentScreen.WorkingArea.Height - this.Height;

            this.Position.Value = new Rect(width, height, 0, 0);
        }
    }
}
