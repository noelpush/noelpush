﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using NLog;
using NoelPush.Objects;
using NoelPush.Properties;
using NoelPush.Services;
using NoelPush.ViewModels;

namespace NoelPush.Models
{
    public class Manager
    {
        private readonly Logger logger;

        private Task captureScreenTask;
        private CancellationTokenSource captureScreenTaskToken;

        private readonly ScreenCapture screenCapture;
        private readonly NotifyIconViewModel notifyIconViewModel;
        private readonly UpdatesManager updatesManager;

        private readonly bool noUpload;
        private int pressCounter;
        private DateTime pressDateTime;

        public Manager(NotifyIconViewModel notifyIconViewModel)
        {
            this.logger = LogManager.GetCurrentClassLogger();

            this.screenCapture = new ScreenCapture(this);
            this.updatesManager = new UpdatesManager();
            this.notifyIconViewModel = notifyIconViewModel;

            var args = Environment.GetCommandLineArgs();
            this.noUpload = (args.Count() >= 2 && args[1] == Resources.CommandLineNoUp);

            if (Settings.Default.uniqueID.Count() != 32)
            {
                Settings.Default.uniqueID = this.GenerateID();
                Settings.Default.Save();
            }

            Shortcuts.OnKeyPress += Capture;

            this.updatesManager.CheckUpdate();

            if (this.updatesManager.FirstRun)
            {
                this.ShowPopupFirstRun();
            }
        }

        private void CancelScreen()
        {
            this.notifyIconViewModel.EnableCommands(true);
            this.screenCapture.Canceled();
            this.StopTask();
        }

        private void StopTask()
        {
            if (this.captureScreenTask == null)
                return;

            this.captureScreenTaskToken.Cancel();
            try
            {
                this.captureScreenTask.Wait();
            }
            catch (Exception e)
            {
                this.logger.Error(e.Message);
            }
            finally
            {
                this.captureScreenTaskToken.Dispose();
                this.captureScreenTaskToken = null;
                this.captureScreenTask = null;
            }
        }

        public ScreenshotData screenshotData;
        public void Capture()
        {
            var date = DateTime.Now;
            this.pressCounter++;

            // First press or bad time
            if (this.pressCounter <= 1 || (this.pressCounter > 1 && date > this.pressDateTime.AddMilliseconds(400)))
            {
                this.screenshotData = new ScreenshotData();
                screenshotData.start_date = date;

                this.pressCounter = 1;
                this.CancelScreen();
                this.pressDateTime = DateTime.Now;

                if (this.pressCounter <= 1)
                {
                    var worker = new BackgroundWorker();
                    worker.DoWork += screenCapture.CaptureSimpleScreen;
                    worker.RunWorkerAsync();
                }
            }

            // Second press
            else if (this.pressCounter == 2)
            {
                this.screenshotData.mode = 1;
                this.screenshotData.second_press_delay = (int)(screenshotData.start_date - date).TotalMilliseconds;

                this.pressDateTime = DateTime.Now;
                this.CaptureRegion(screenshotData);
            }

            // Third press
            if (this.pressCounter == 3)
            {
                this.screenshotData.mode = 2;
                screenshotData.third_press_delay = (int)(screenshotData.start_date - date).TotalMilliseconds;

                this.CancelScreen();
                this.CaptureScreen(screenshotData);
                this.pressCounter = 0;
            }
        }

        public void CaptureScreen(ScreenshotData data)
        {
            this.screenCapture.CaptureScreen(data);
        }

        public void CaptureRegion(ScreenshotData data)
        {
            this.screenCapture.CaptureRegion(data);
        }

        public void Captured(Bitmap img, ScreenshotData data)
        {
            this.pressCounter = 0;

            var pictureData = this.screenCapture.GetPictureSize(img);

            data.png_size = pictureData.sizePng;
            data.jpg_size = pictureData.sizeJpeg;

            var smallBitmap = pictureData.GetSmallestPicture();
            var format = pictureData.GetPictureType();

            // Disable buttons during uploading
            this.notifyIconViewModel.EnableCommands(false);

            if (this.noUpload)
                new Uploader(this, format).Upload(img);
            else
                new Uploader(this, format).Upload(img, ImageToByte(smallBitmap), data);
        }

        public void Uploaded(Bitmap img, string url)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(url));

            this.notifyIconViewModel.EnableCommands(true);
            this.notifyIconViewModel.ShowPopupUpload(img);
        }

        public void Uploaded(Bitmap img)
        {
            var pathPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\NoelPush\\";

            if (!Directory.Exists(pathPictures))
            {
                Directory.CreateDirectory(pathPictures);
            }

            var filename = pathPictures + DateTime.Now.ToString("dd-mm-yyyy HHhmmmsss") + ".png";
            img.Save(filename, ImageFormat.Png);

            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(filename));

            this.notifyIconViewModel.EnableCommands(true);
            this.notifyIconViewModel.ShowPopupUpload(img);
        }

        public void Screened(Bitmap bmp)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetImage(bmp));
        }

        private void ShowPopupFirstRun()
        {
            // Task + Dispatcher because it doesn't want to start without.......
            Task.Factory.StartNew(() =>
                Dispatcher.CurrentDispatcher.Invoke(() =>
                    this.notifyIconViewModel.ShowPopupMessage()));
        }

        internal void UploadFailed()
        {
            this.notifyIconViewModel.ShowPopupUploadFailed();
        }

        private static byte[] ImageToByte(Bitmap img)
        {
            return (byte[])new ImageConverter().ConvertTo(img, typeof(byte[]));
        }

        public string GenerateID()
        {
            var rand = new Random().Next(99999, 999999999).ToString();
            var inputBytes = Encoding.ASCII.GetBytes(rand);
            var hash = MD5.Create().ComputeHash(inputBytes);

            var id = new StringBuilder();
            foreach (var t in hash)
                id.Append(t.ToString("X2"));

            return id.ToString();
        }
    }
}
