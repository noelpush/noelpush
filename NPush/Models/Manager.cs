using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using NPush.Objects;
using NPush.Properties;
using NPush.Services;
using NPush.Views;


namespace NPush.Models
{
    public class Manager
    {
        private string uniqueID;
        private readonly string version;

        private Shortcuts shortcutsScreen;
        private Shortcuts shortcutsRegion;
        private Shortcuts shortcutsEscape;

        public bool isScreenCapturing;
        private ScreenshotData screenshotData;

        private readonly Update update;
        private readonly Uploader uploader;
        private readonly Statistics stats;
        private readonly ScreenCapture screenCapture;

        public Manager()
        {
            this.update = new Update();
            this.screenCapture = new ScreenCapture(this);
            this.uploader = new Uploader(this);
            this.stats = new Statistics();

            if (Settings.Default.uniqueID.Count() != 32)
            {
                Settings.Default.uniqueID = this.GenerateID();
                Settings.Default.Save();
            }
            
            this.version = Settings.Default.version;
            this.uniqueID = Settings.Default.uniqueID;

            var progressBar = new ProgressBarView();
                progressBar.Show();

            this.shortcutsScreen = new Shortcuts();
            this.shortcutsScreen.KeyPressed += CaptureScreen;
            this.shortcutsScreen.RegisterHotKeys(ModifierKeys.Control, Keys.PrintScreen);

            this.shortcutsRegion = new Shortcuts();
            this.shortcutsRegion.KeyPressed += CaptureRegion;
            this.shortcutsRegion.RegisterHotKeys(ModifierKeys.Shift, Keys.PrintScreen);

            this.shortcutsEscape = new Shortcuts();
            this.shortcutsEscape.KeyPressed += CancelScreen;
            this.shortcutsEscape.RegisterHotKey(Keys.Escape);

            this.stats.StatsStart(this.uniqueID, this.version, this.getDotnets());

            this.CheckUpdate();
        }

        private void CheckUpdate()
        {
            bool isUpdated = this.update.CheckVersion();

            if (isUpdated) return;

            MessageBox.Show(String.Format(Resources.NewVersion, this.version));
            this.update.DoUpdate();
        }

        private void CancelScreen(object sender, KeyPressedEventArgs e)
        {
            this.isScreenCapturing = false;
            this.screenCapture.Canceled();
        }

        public void CaptureScreen(object sender = null, KeyPressedEventArgs keyPressedEventArgs = null)
        {
            if (this.isScreenCapturing) return;
            this.isScreenCapturing = true;

            this.screenshotData = new ScreenshotData(this.uniqueID, this.version, 1);
            this.screenCapture.CaptureScreen();
        }

        public void CaptureRegion(object sender = null, KeyPressedEventArgs keyPressedEventArgs = null)
        {
            if (this.isScreenCapturing) return;
            this.isScreenCapturing = true;

            this.screenshotData = new ScreenshotData(this.uniqueID, this.version, 2);
            this.screenCapture.CaptureRegion();
        }

        public void Captured(Bitmap img, int mode)
        {
            var sizePicture = this.screenCapture.SaveImage(img);
                if (sizePicture.Count() > 0) this.screenshotData.sizePng = sizePicture[0];
                if (sizePicture.Count() > 1) this.screenshotData.sizeJpg = sizePicture[1];

            uploader.Upload(ImageToByte(img));
        }

        public void Uploaded(string url, long timing)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(url));
            NotifSound();

            this.isScreenCapturing = false;

            this.screenshotData.timing = timing;
            stats.StatsUpload(this.screenshotData);
            this.screenshotData = null;
        }

        internal void UploadFailed()
        {
            this.isScreenCapturing = false;
            this.stats.StatsFail();
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
