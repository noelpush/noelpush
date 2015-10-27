﻿using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using NPush.Objects.ViewModel;

namespace NPush.ViewModels
{
    public class PopupUploadViewModel
    {
        public ViewElement<bool> IsOpen { get; private set; }
        public ViewElement<Rect> Position { get; private set; } 
        public ViewElement<Bitmap> Picture { get; private set; }

        public PopupUploadViewModel()
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
            var height = Screen.PrimaryScreen.Bounds.Height - 118;
            var width = Screen.PrimaryScreen.Bounds.Width - 323;

            this.Position.Value = new Rect(width, height, 0, 0);
        }
    }
}
