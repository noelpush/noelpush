using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NoelPush.Objects;
using NoelPush.Properties;
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
        public ScreenshotData ScreenData;

        public Manager(NotifyIconViewModel notifyIconViewModel)
        {
            this.UserId = RegistryService.GetUserId();
            this.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();

            this.notifyIconViewModel = notifyIconViewModel;

            ShortcutService.OnKeyPress += Capture;

            UpdatesService.Initialize(this.UserId, this.Version);
            if (UpdatesService.FirstRun)
                this.ShowPopupFirstRun();

            if (CommandService.IsShellMode)
            {
                var file = CommandService.GetFileName;
                if (!string.IsNullOrEmpty(file))
                    this.Captured(new Bitmap(Image.FromFile(file)), new ScreenshotData(this.UserId) { StartDate = DateTime.Now }, true);
            }
        }

        public void Capture(bool upload = true)
        {
            DateTime date = DateTime.Now;
            this.pressCounter++;

            // First press or bad time
            if (this.pressCounter <= 1 || (this.pressCounter > 1 && date > this.pressDateTime.AddMilliseconds(400)))
            {
                this.ScreenData = new ScreenshotData(this.UserId);

                this.pressCounter = 1;
                this.CancelCapture();
                this.pressDateTime = DateTime.Now;
            }

            // Second press
            else if (this.pressCounter == 2)
            {
                this.ScreenData.Mode = 1;
                this.ScreenData.SecondPressDate = DateTime.Now;
                this.ScreenData.ThirdPressDate = DateTime.MinValue;

                this.pressDateTime = DateTime.Now;
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => this.CaptureRegion(this.ScreenData, upload)));
            }

            // Third press
            if (this.pressCounter == 3)
            {
                this.ScreenData.Mode = 2;
                this.ScreenData.ThirdPressDate = DateTime.Now;

                this.CancelCapture();
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => this.CaptureScreen(this.ScreenData, upload)));
                this.pressCounter = 0;
            }
        }

        public void CaptureScreen(ScreenshotData data, bool upload)
        {
            var capture = CaptureService.CaptureScreen(ref data);

            if (capture != null)
                this.Captured(capture, data, upload);
        }

        public void CaptureRegion(ScreenshotData data, bool upload)
        {
            var capture = CaptureService.CaptureRegion(ref data);

            if (capture != null)
                this.Captured(capture, data, upload);
        }

        private void CancelCapture()
        {
            notifyIconViewModel.EnableCommands(true);
            CaptureService.CancelCapture();
        }

        public void Captured(Bitmap img, ScreenshotData screenshotData, bool upload)
        {
            this.pressCounter = 0;

            if (upload)
            {
                this.notifyIconViewModel.EnableCommands(false);
                var pictureData = new PictureData(img, screenshotData);
                new UploaderService(this).Upload(pictureData);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => Clipboard.SetImage(CreateBitmapSourceFromBitmap(img)));
                this.notifyIconViewModel.ShowPopupCopy(img);
            }
        }

        public void Uploaded(Bitmap img, string url, ScreenshotData screenshotData, bool error)
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

            screenshotData.uRL = url;
            StatisticsService.StatUpload(screenshotData);
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