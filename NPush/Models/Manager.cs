using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;
using NLog;
using NoelPush.Objects;
using NoelPush.Properties;
using NoelPush.Services;
using NoelPush.ViewModels;
using Application = System.Windows.Application;

namespace NoelPush.Models
{
    public class Manager
    {
        private readonly Logger logger;
        private readonly bool noUpload;
        private readonly NotifyIconViewModel notifyIconViewModel;
        private readonly ScreenCapture screenCapture;
        private readonly UpdatesManager updatesManager;

        public string UserId { get; private set; }
        private Task captureScreenTask;
        private CancellationTokenSource captureScreenTaskToken;

        private int pressCounter;
        private DateTime pressDateTime;
        public ScreenshotData screenshotData;

        public Manager(NotifyIconViewModel notifyIconViewModel)
        {
            logger = LogManager.GetCurrentClassLogger();

            this.UserId = GetUserIdInRegistry();

            screenCapture = new ScreenCapture(this);
            updatesManager = new UpdatesManager();
            this.notifyIconViewModel = notifyIconViewModel;

            string[] args = Environment.GetCommandLineArgs();
            noUpload = (args.Count() >= 2 && args[1] == Resources.CommandLineNoUp);

            Shortcuts.OnKeyPress += Capture;

            updatesManager.CheckUpdate();

            if (updatesManager.FirstRun)
            {
                ShowPopupFirstRun();
            }
        }

        private string GetUserIdInRegistry()
        {
            const string REGISTRY_KEY = @"HKEY_CURRENT_USER\SOFTWARE\NoelPush";
            const string REGISTY_VALUE = "ID";

            try
            {
                var Key = Registry.GetValue(REGISTRY_KEY, REGISTY_VALUE, 0) as string;
                if (Key != null)
                {
                    return Key;
                }
                else
                {
                    return this.WriteUserIdInRegistry();
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return "Undefined";
        }


        private string WriteUserIdInRegistry()
        {
            const string REGISTRY_FIRST_KEY = @"HKEY_CURRENT_USER\SOFTWARE\";
            string REGISTY_FIRST_VALUE = "NoelPush";

            const string REGISTRY_SECOND_KEY = @"HKEY_CURRENT_USER\SOFTWARE\NoelPush";
            string REGISTY_SECOND_VALUE = "ID";

            string REGISTY_STRING = GenerateID();

            if (Convert.ToInt32(Registry.GetValue(REGISTRY_FIRST_KEY, REGISTY_FIRST_VALUE, 0)) != 0)
                return "Undefined";

            Registry.SetValue(REGISTRY_FIRST_KEY, REGISTY_FIRST_VALUE, 0, RegistryValueKind.String);
            Registry.SetValue(REGISTRY_SECOND_KEY, REGISTY_SECOND_VALUE, REGISTY_STRING, RegistryValueKind.String);

            return REGISTY_STRING;
        }

        private void CancelScreen()
        {
            notifyIconViewModel.EnableCommands(true);
            screenCapture.Canceled();
            StopTask();
        }

        private void StopTask()
        {
            if (captureScreenTask == null)
                return;

            captureScreenTaskToken.Cancel();
            try
            {
                captureScreenTask.Wait();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            finally
            {
                captureScreenTaskToken.Dispose();
                captureScreenTaskToken = null;
                captureScreenTask = null;
            }
        }

        public void Capture()
        {
            DateTime date = DateTime.Now;
            this.pressCounter++;

            // First press or bad time
            if (this.pressCounter <= 1 || (this.pressCounter > 1 && date > this.pressDateTime.AddMilliseconds(400)))
            {
                this.screenshotData = new ScreenshotData(this.UserId);

                this.pressCounter = 1;
                this.CancelScreen();
                this.pressDateTime = DateTime.Now;
            }

                // Second press
            else if (this.pressCounter == 2)
            {
                this.screenshotData.mode = 1;
                this.screenshotData.second_press_date = DateTime.Now;
                this.screenshotData.third_press_date = DateTime.MinValue;

                this.pressDateTime = DateTime.Now;
                this.CaptureRegion(this.screenshotData);
            }

            // Third press
            if (this.pressCounter == 3)
            {
                this.screenshotData.mode = 2;
                this.screenshotData.third_press_date = DateTime.Now;

                this.CancelScreen();
                this.CaptureScreen(screenshotData);
                this.pressCounter = 0;
            }
        }

        public void CaptureScreen(ScreenshotData data)
        {
            screenCapture.CaptureScreen(data);
        }

        public void CaptureRegion(ScreenshotData data)
        {
            screenCapture.CaptureRegion(data);
        }

        public void Captured(Bitmap img, ScreenshotData data)
        {
            pressCounter = 0;

            PictureData pictureData = screenCapture.GetPictureSize(img);

            data.png_size = pictureData.sizePng;
            data.jpeg_size = pictureData.sizeJpeg;

            Bitmap smallBitmap = pictureData.GetSmallestPicture();
            string format = pictureData.GetPictureType();

            // Disable buttons during uploading
            notifyIconViewModel.EnableCommands(false);

            if (noUpload)
                new Uploader(this, format).Upload(img);
            else
            {
                if (data.first_press_date != DateTime.MinValue)
                    new Uploader(this, format).Upload(smallBitmap, ImageToByte(smallBitmap), data);
                else
                    new Uploader(this, format).Upload(smallBitmap, ImageToByte(smallBitmap), data);
            }
        }

        public void Uploaded(Bitmap img, string url, ScreenshotData screenshotData)
        {
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(url));

            screenshotData.url = url;
            Statistics.Send(screenshotData);

            notifyIconViewModel.EnableCommands(true);
            notifyIconViewModel.ShowPopupUpload(img);
        }

        public void Uploaded(Bitmap img)
        {
            string pathPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\NoelPush\\";

            if (!Directory.Exists(pathPictures))
            {
                Directory.CreateDirectory(pathPictures);
            }

            string filename = pathPictures + DateTime.Now.ToString("dd-mm-yyyy HHhmmmsss") + ".png";
            img.Save(filename, ImageFormat.Png);

            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(filename));

            notifyIconViewModel.EnableCommands(true);
            notifyIconViewModel.ShowPopupUpload(img);
        }

        private void ShowPopupFirstRun()
        {
            // Task + Dispatcher because it doesn't want to start without.......
            Task.Factory.StartNew(() =>
                Dispatcher.CurrentDispatcher.Invoke(() =>
                    notifyIconViewModel.ShowPopupMessage()));
        }

        internal void UploadFailed()
        {
            notifyIconViewModel.ShowPopupUploadFailed();
        }

        private static byte[] ImageToByte(Bitmap img)
        {
            return (byte[]) new ImageConverter().ConvertTo(img, typeof (byte[]));
        }

        public string GenerateID()
        {
            var random = new Random();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string password = string.Empty;

            for (int i = 0; i < 32; i++)
            {
                password += alphabet[random.Next(0, alphabet.Length) - 1];
            }

            return password;
        }
    }
}