using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPush.Objects;
using NPush.Properties;
using NPush.Services;
using NPush.ViewModels;
namespace NPush.Models
{
    public class Manager
    {
        private string uniqueID;
        private readonly string version;

        private Shortcuts shortcutImprEcr;

        private Task captureScreenTask;
        private CancellationTokenSource captureScreenTaskToken;

        private readonly Update update;
        private readonly ScreenCapture screenCapture;
        private readonly NotifyIconViewModel notifyIconViewModel;

        private bool noUpload;
        private int pressCounter;
        private DateTime pressDateTime;

        public Manager(NotifyIconViewModel notifyIconViewModel)
        {
            Settings.Default.TimePopup = Settings.Default.TimePopup;
            Settings.Default.Save();

            //this.update = new Update();
            this.screenCapture = new ScreenCapture(this);
            this.notifyIconViewModel = notifyIconViewModel;

            var args = Environment.GetCommandLineArgs();
            this.noUpload = (args.Count() >= 2 && args[1] == Resources.CommandLineNoUp);

            if (Settings.Default.uniqueID.Count() != 32)
            {
                Settings.Default.uniqueID = this.GenerateID();
                Settings.Default.Save();
            }

            this.version = Settings.Default.version;
            this.uniqueID = Settings.Default.uniqueID;

            this.shortcutImprEcr = new Shortcuts();
            Shortcuts.OnKeyPress += Capture;
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

            }
            finally
            {
                this.captureScreenTaskToken.Dispose();
                this.captureScreenTaskToken = null;
                this.captureScreenTask = null;
            }
        }

        public void Capture()
        {
            var date = DateTime.Now;
            this.pressCounter++;

            // First press or bad time
            if (this.pressCounter <= 1 || (this.pressCounter > 1 && date > this.pressDateTime.AddMilliseconds(400)))
            {
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
                this.pressDateTime = DateTime.Now;
                this.CaptureRegion();
            }

            // Third press
            if (this.pressCounter == 3)
            {
                this.CancelScreen();
                this.CaptureScreen();
                this.pressCounter = 0;
            }
        }

        public void CaptureScreen()
        {
            this.screenCapture.CaptureScreen();
        }

        public void CaptureRegion()
        {
            this.screenCapture.CaptureRegion();
        }

        public void Captured(Bitmap img)
        {
            this.pressCounter = 0;

            /*
             * var sizePicture = this.screenCapture.SaveImage(img);
                if (sizePicture.Count() > 0) this.screenshotData.sizePng = sizePicture[0];
                if (sizePicture.Count() > 1) this.screenshotData.sizeJpg = sizePicture[1];
            */

            // Disable buttons during uploading
            this.notifyIconViewModel.EnableCommands(false);

            if (this.noUpload)
                new Uploader(this).Upload(img);
            else
                new Uploader(this).Upload(img, ImageToByte(img));
        }

        public void Uploaded(Bitmap img, string url, long timing)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(url));

            this.notifyIconViewModel.EnableCommands(true);
            this.notifyIconViewModel.ShowPopup(img);
        }

        public void Uploaded(Bitmap img, long timing)
        {
            var pathPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\NPush\\";

            if (!Directory.Exists(pathPictures))
            {
                Directory.CreateDirectory(pathPictures);
            }

            var filename = pathPictures + DateTime.Now.ToString("dd-mm-yyyy HHhmmmsss") + ".png";
            img.Save(filename, ImageFormat.Png);

            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(filename));

            this.notifyIconViewModel.EnableCommands(true);
            this.notifyIconViewModel.ShowPopup(img);
        }

        public void Screened(Bitmap bmp)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetImage(bmp));
        }

        internal void UploadFailed()
        {

        }

        private void NotifSound()
        {
            new SoundPlayer(Resources.notif).Play();
        }

        private string getDotnets()
        {
            return "4.0";
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
