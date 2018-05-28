using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using NoelPush.Objects;
using NoelPush.Services;
using NoelPush.ViewModels;

using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;

namespace NoelPush.Models
{
    public class Manager
    {
        public string UserId { get; private set; }
        public string Version { get; private set; }

        private readonly NotifyIconViewModel notifyIconViewModel;

        private int pressCounter;
        private DateTime pressDateTime;
        public Rectangle ImgSize;

        public Manager(NotifyIconViewModel notifyIconViewModel)
        { 
            this.UserId = string.Empty;
            this.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();

            this.notifyIconViewModel = notifyIconViewModel;

            ShortcutService.OnKeyPress += Capture;

            if (CommandService.IsShellMode)
            {
                var file = CommandService.GetFileName;
                if (!string.IsNullOrEmpty(file))
                    this.Captured(new Bitmap(Image.FromFile(file)), true);
            }
        }

        public void Capture(bool upload = true)
        {
            var date = DateTime.Now;
            this.pressCounter++;

            // First press or bad time
            if (this.pressCounter <= 1 || (this.pressCounter > 1 && date > this.pressDateTime.AddMilliseconds(400)))
            {
                this.ImgSize = new Rectangle();

                this.pressCounter = 1;
                this.CancelCapture();
                this.pressDateTime = DateTime.Now;
            }

            // Second press
            else if (this.pressCounter == 2)
            {
                this.pressDateTime = DateTime.Now;

                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => this.CaptureRegion(upload)));
            }

            // Third press
            if (this.pressCounter == 3)
            {
                this.CancelCapture();
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => this.CaptureScreen(upload)));
                this.pressCounter = 0;
            }
        }

        public void CaptureRegion(bool upload)
        {
            var capture = CaptureService.CaptureRegion(ref this.ImgSize);

            if (capture != null)
                this.Captured(capture, upload);
        }

        public void CaptureScreen(bool upload)
        {
            var capture = CaptureService.CaptureScreen(ref this.ImgSize);

            if (capture != null)
                this.Captured(capture, upload);
        }

        private void CancelCapture()
        {
            notifyIconViewModel.EnableCommands(true);
            CaptureService.CancelCapture();
        }

        public void Captured(Bitmap img, bool upload)
        {
            this.pressCounter = 0;

            if (upload)
            {
                this.notifyIconViewModel.EnableCommands(false);
                var pictureData = new PictureData(img);
                new UploaderService(this).Upload(pictureData);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => Clipboard.SetImage(CreateBitmapSourceFromBitmap(img)));
                this.notifyIconViewModel.ShowPopupCopy(img);
            }
        }

        public void Uploaded(Bitmap img, string url, bool error)
        {
            if (!error)
            {
                Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(url));
                this.notifyIconViewModel.EnableCommands(true);
                this.notifyIconViewModel.ShowPopupUpload(img);
            }
            else
            {
                this.UploadFailed();
            }
        }

        public BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        internal void UploadFailed()
        {
            notifyIconViewModel.EnableCommands(true);
            notifyIconViewModel.ShowPopupUploadFailed();
        }

        internal void UploadPictureTooLarge()
        {
            notifyIconViewModel.EnableCommands(true);
            notifyIconViewModel.ShowPopupPictureTooLarge();
        }

        internal void ConnexionFailed()
        {
            notifyIconViewModel.ShowPopupConnexionFailed();
        }

        private void ShowPopupFirstRun()
        {
            notifyIconViewModel.ShowPopupMessage();
        }
    }
}