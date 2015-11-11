using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using NLog;
using NoelPush.Objects;
using NoelPush.Properties;
using NoelPush.Services;
using NoelPush.ViewModels;
using Clipboard = System.Windows.Forms.Clipboard;

namespace NoelPush.Models
{
    public class Manager
    {
        private readonly Logger logger;
        private readonly bool CommandNoUpload;

        private readonly NotifyIconViewModel notifyIconViewModel;
        private readonly ScreenCapture screenCapture;
        private readonly UpdatesManager updatesManager;

        public string UserId { get; private set; }
        private Task captureScreenTask;

        private int pressCounter;
        private DateTime pressDateTime;
        public ScreenshotData screenshotData;

        public Manager(NotifyIconViewModel notifyIconViewModel)
        {
            this.logger = LogManager.GetCurrentClassLogger();

            this.UserId = GetUserIdInRegistry();

            this.screenCapture = new ScreenCapture(this);
            this.notifyIconViewModel = notifyIconViewModel;

            string[] args = Environment.GetCommandLineArgs();
            this.CommandNoUpload = (args.Count() >= 2 && args[1] == Resources.CommandLineNoUp);

            Shortcuts.OnKeyPress += Capture;

            this.updatesManager = new UpdatesManager();
            this.updatesManager.CheckUpdate();

            if (this.updatesManager.FirstRun)
            {
                this.ShowPopupFirstRun();
            }

            if ((args.Count() >= 2 && args[1] == Resources.CommandFileName && !string.IsNullOrEmpty(args[2])))
                this.Captured(new Bitmap(Image.FromFile(args[2])), new ScreenshotData(this.UserId) { start_date = DateTime.Now });
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
            this.screenCapture.CaptureScreen(data);
        }

        public void CaptureRegion(ScreenshotData data)
        {
            this.screenCapture.CaptureRegion(data);
        }

        public void Captured(Bitmap img, ScreenshotData screenshotData)
        {
            this.pressCounter = 0;
            this.notifyIconViewModel.EnableCommands(false);

            var pictureData = new PictureData(img, screenshotData);
            new Uploader(this).Upload(pictureData);
        }

        public void Uploaded(Bitmap img, string url, ScreenshotData screenshotData, bool error)
        {
            if (!error)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(url));
                this.notifyIconViewModel.EnableCommands(true);
                this.notifyIconViewModel.ShowPopupUpload(img);
            }
            else
            {
                this.UploadFailed();
            }

            screenshotData.url = url;
            Statistics.Send(screenshotData);
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

            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(filename));

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
            notifyIconViewModel.EnableCommands(true);
            notifyIconViewModel.ShowPopupUploadFailed();
        }

        internal void ConnexionFailed()
        {
            notifyIconViewModel.ShowPopupConnexionFailed();
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