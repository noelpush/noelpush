using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using NLog;
using NoelPush.Models;
using NoelPush.Objects;
using NoelPush.Views.Popup;

namespace NoelPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private Logger logger;

        private readonly bool canScreen;

        private readonly Manager manager;

        public PopupUploadView PopupUpload { get; private set; }
        public PopupViewModel PopupUploadDataContext { get; private set; }

        public PopupUploadFailedView PopupUploadFailed { get; private set; }
        public PopupViewModel PopupUploadFailedDataContext { get; private set; }

        public PopupPictureTooLargeView PopupPictureTooLarge { get; private set; }
        public PopupViewModel PopupPictureTooLargeDataContext { get; private set; }

        public PopupConnexionFailedView PopupConnexionFailed { get; private set; }
        public PopupViewModel PopupConnexionFailedDataContext { get; private set; }

        public PopupFirstRunView PopupFirstRun { get; private set; }
        public PopupViewModel PopupFirstRunDataContext { get; private set; }

        public delegate void TooltipMessageEventHandler(Bitmap img);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel(EnableCommandsEventHandler eventHandler)
        {
            this.logger = LogManager.GetCurrentClassLogger();

            this.EnableCommandsEvent += eventHandler;

            this.canScreen = true;

            this.PopupUploadDataContext = new PopupViewModel(323, 118);
            this.PopupFirstRunDataContext = new PopupViewModel(323, 138);
            this.PopupUploadFailedDataContext = new PopupViewModel(323, 118);
            this.PopupPictureTooLargeDataContext = new PopupViewModel(323, 118);
            this.PopupConnexionFailedDataContext = new PopupViewModel(323, 118);

            this.PopupUpload = new PopupUploadView { DataContext = this.PopupUploadDataContext };
            this.PopupFirstRun = new PopupFirstRunView { DataContext = this.PopupFirstRunDataContext };
            this.PopupUploadFailed = new PopupUploadFailedView { DataContext = this.PopupUploadFailedDataContext };
            this.PopupConnexionFailed = new PopupConnexionFailedView { DataContext = this.PopupConnexionFailedDataContext };
            this.PopupPictureTooLarge = new PopupPictureTooLargeView { DataContext = this.PopupPictureTooLargeDataContext };

            this.manager = new Manager(this);
        }

        public void ShowPopupUpload(Bitmap img, int delay = 3000)
        {
            this.PopupUploadDataContext.ShowPopup(delay, img);
        }

        public void ShowPopupUploadFailed(int delay = 3000)
        {
            this.PopupUploadFailedDataContext.ShowPopup(delay);
        }

        public void ShowPopupPictureTooLarge(int delay = 3000)
        {
            this.PopupPictureTooLargeDataContext.ShowPopup(delay);
        }

        public void ShowPopupMessage(int delay = 11000)
        {
            this.PopupFirstRunDataContext.ShowPopup(delay);
        }

        public void ShowPopupConnexionFailed(int delay = 4000)
        {
            this.PopupConnexionFailedDataContext.ShowPopup(delay);
        }

        public void EnableCommands(bool enabled)
        {
            this.EnableCommandsEvent(enabled);
        }

        private bool CanScreen
        {
            get { return this.canScreen; }
        }

        public async void Historique()
        {
            var id = this.manager.UserId;

            if (id.Length != 32)
                return;

            using (var client = new HttpClient())
            {
                string token = string.Empty;

                try
                {
                    var values = new Dictionary<string, string> { { "uid", id } };
                    var content = new FormUrlEncodedContent(values);

                    var answer = await client.PostAsync("https://www.noelpush.com/generate_login_token?", content);
                    token = await answer.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    this.logger.Error(e.Message);
                    return;
                }

                if (token.Length != 32)
                    return;

                var urlHistorique = "https://www.noelpush.com/login?token=" + token;
                this.OpenInBrowser(urlHistorique);
            }
        }

        private void OpenInBrowser(string url)
        {
            // Open in a navigator if the process is launched

            var processList = Process.GetProcesses();
            var browsers1 = new[] { "chrome", "firefox", "edge", "iexplore", "opera", "safari" };

            for (var i = 0; i < browsers1.Count(); i++)
            {
                if (processList.Any(proc => proc.ProcessName == browsers1[i]))
                {
                    Process.Start(browsers1[i], url);
                    return;
                }
            }

            // Else check if a navigator is pinned in taskbar
            var taskbar = Directory.GetFiles(@"C:\Users\choco\AppData\Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar");

            var browsers2 = new[] { "google chrome", "chromium", "firefox", "edge", "internet explorer", "opera", "safari" };
            var browsers3 = new[] { "chrome", "chrome", "firefox", "edge", "iexplore", "opera", "safari" };

            for (var i = 0; i < browsers2.Count(); i++)
            {
                if (taskbar.Any(t => t.ToLower().Contains(browsers2[i] + ".lnk")))
                {
                    Process.Start(browsers3[i], url);
                    return;
                }
            }

            // Else open with the default navigator
            Process.Start(url);
        }

        public void CaptureRegion()
        {
            var screenshotData = new ScreenshotData(this.manager.UserId)
            {
                mode = 1,
                first_press_date = DateTime.MinValue,
                second_press_date = DateTime.MinValue,
                third_press_date = DateTime.MinValue
            };

            if (this.CanScreen)
                this.manager.CaptureRegion(screenshotData);
        }

        public void CaptureScreen()
        {
            var screenshotData = new ScreenshotData(this.manager.UserId)
            {
                mode = 2,
                first_press_date = DateTime.MinValue,
                second_press_date = DateTime.MinValue,
                third_press_date = DateTime.MinValue
            };

            if (this.CanScreen)
                this.manager.CaptureScreen(screenshotData);
        }

        public void Exit()
        {
            Environment.Exit(1);
        }
    }
}
