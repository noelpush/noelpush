using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NLog;
using NoelPush.Models;
using NoelPush.Objects;
using NoelPush.Services;
using NoelPush.Views.Popup;

namespace NoelPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly bool canScreen;

        private readonly Manager manager;

        public PopupUploadView PopupUpload { get; private set; }
        public PopupViewModel PopupUploadDataContext { get; private set; }

        public PopupCopyView PopupCopy { get; private set; }
        public PopupViewModel PopupCopyDataContext { get; private set; }

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
            this.EnableCommandsEvent += eventHandler;

            this.canScreen = true;

            this.PopupCopyDataContext = new PopupViewModel(323, 118, 3000);
            this.PopupUploadDataContext = new PopupViewModel(323, 118, 3000);
            this.PopupFirstRunDataContext = new PopupViewModel(323, 138, 3000);
            this.PopupUploadFailedDataContext = new PopupViewModel(323, 118, 4000);
            this.PopupPictureTooLargeDataContext = new PopupViewModel(323, 118, 4000);
            this.PopupConnexionFailedDataContext = new PopupViewModel(323, 118, 4000);

            this.PopupCopy = new PopupCopyView { DataContext = this.PopupCopyDataContext };
            this.PopupUpload = new PopupUploadView { DataContext = this.PopupUploadDataContext };
            this.PopupFirstRun = new PopupFirstRunView { DataContext = this.PopupFirstRunDataContext };
            this.PopupUploadFailed = new PopupUploadFailedView { DataContext = this.PopupUploadFailedDataContext };
            this.PopupConnexionFailed = new PopupConnexionFailedView { DataContext = this.PopupConnexionFailedDataContext };
            this.PopupPictureTooLarge = new PopupPictureTooLargeView { DataContext = this.PopupPictureTooLargeDataContext };

            this.manager = new Manager(this);
        }

        // Task + Dispatcher because it allows and optimises the displaying.......
        private void ShowPopup(PopupViewModel viewModel)
        {
            Task.Factory.StartNew(() =>
                Dispatcher.CurrentDispatcher.Invoke(viewModel.ShowPopup));
        }

        private void ShowPopup(PopupViewModel viewModel, Bitmap img)
        {
            Task.Factory.StartNew(() =>
                Dispatcher.CurrentDispatcher.Invoke(() =>
                    viewModel.ShowPopup(img)));
        }

        public void ShowPopupUpload(Bitmap img)
        {
            this.ShowPopup(PopupUploadDataContext, img);
        }

        public void ShowPopupCopy(Bitmap img)
        {
            this.ShowPopup(PopupCopyDataContext, img);
        }

        public void ShowPopupUploadFailed()
        {
            this.ShowPopup(PopupUploadFailedDataContext);
        }

        public void ShowPopupPictureTooLarge()
        {
            this.ShowPopup(PopupPictureTooLargeDataContext);
        }

        public void ShowPopupMessage()
        {
            this.ShowPopup(PopupFirstRunDataContext);
        }

        public void ShowPopupConnexionFailed()
        {
            this.ShowPopup(PopupConnexionFailedDataContext);
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

                    var answer = await client.PostAsync("https://noelpush.com/generate_login_token?", content);
                    token = await answer.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e.Message);
                    return;
                }

                if (token.Length != 32)
                    return;

                var urlHistorique = "https://noelpush.com/login?token=" + token;
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
            var taskbarPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar";
            var taskbar = Directory.GetFiles(taskbarPath);

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
                Mode = 1,
                FirstPressDate = DateTime.MinValue,
                SecondPressDate = DateTime.MinValue,
                ThirdPressDate = DateTime.MinValue
            };

            if (this.CanScreen)
                this.manager.CaptureRegion(screenshotData, true);
        }

        public void CaptureScreen()
        {
            var screenshotData = new ScreenshotData(this.manager.UserId)
            {
                Mode = 2,
                FirstPressDate = DateTime.MinValue,
                SecondPressDate = DateTime.MinValue,
                ThirdPressDate = DateTime.MinValue
            };

            if (this.CanScreen)
                this.manager.CaptureScreen(screenshotData, true);
        }

        public void Exit()
        {
            Environment.Exit(1);
        }
    }
}
