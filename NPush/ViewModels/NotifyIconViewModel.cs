using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using NoelPush.Models;
using NoelPush.Objects;
using NoelPush.ViewModels.Popup;
using NoelPush.Views.Popup;

namespace NoelPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly bool canScreen;

        private readonly Manager manager;
        private Timer timerHistorique;

        public PopupUploadView PopupUpload { get; private set; }
        public PopupUploadViewModel PopupUploadDataContext { get; private set; }

        public PopupUploadFailedView PopupUploadFailed { get; private set; }
        public PopupUploadFailedViewModel PopupUploadFailedDataContext { get; private set; }

        public PopupConnexionFailedView PopupConnexionFailed { get; private set; }
        public PopupConnexionFailedViewModel PopupConnexionFailedDataContext { get; private set; }

        public PopupFirstRunView PopupFirstRun { get; private set; }
        public PopupFirstRunViewModel PopupFirstRunDataContext { get; private set; }

        public PopupHistoriqueView PopupHistorique { get; private set; }
        public PopupHistoriqueViewModel PopupHistoriqueDataContext { get; private set; }

        public delegate void TooltipMessageEventHandler(Bitmap img);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel(EnableCommandsEventHandler eventHandler)
        {
            this.EnableCommandsEvent += eventHandler;

            this.canScreen = true;

            this.PopupUploadDataContext = new PopupUploadViewModel();
            this.PopupFirstRunDataContext = new PopupFirstRunViewModel();
            this.PopupHistoriqueDataContext = new PopupHistoriqueViewModel();
            this.PopupUploadFailedDataContext = new PopupUploadFailedViewModel();
            this.PopupConnexionFailedDataContext = new PopupConnexionFailedViewModel();

            this.PopupUpload = new PopupUploadView { DataContext = this.PopupUploadDataContext };
            this.PopupFirstRun = new PopupFirstRunView { DataContext = this.PopupFirstRunDataContext };
            this.PopupHistorique = new PopupHistoriqueView { DataContext = this.PopupHistoriqueDataContext };
            this.PopupUploadFailed = new PopupUploadFailedView { DataContext = this.PopupUploadFailedDataContext };
            this.PopupConnexionFailed = new PopupConnexionFailedView { DataContext = this.PopupConnexionFailedDataContext };

            this.manager = new Manager(this);
        }

        public void ShowPopupUpload(Bitmap img, int delay = 3000)
        {
            this.PopupUploadDataContext.ShowPopup(img, delay);
        }

        public void ShowPopupUploadFailed(int delay = 3000)
        {
            this.PopupUploadFailedDataContext.ShowPopup(delay);
        }

        public void ShowPopupMessage(int delay = 11000)
        {
            this.PopupFirstRunDataContext.ShowPopup(delay);
        }

        public void ShowPopupHistorique(int delay = 5000)
        {
            this.PopupHistoriqueDataContext.ShowPopup(delay);
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

        public void Historique()
        {
            var id = this.manager.UserId;

            if (id.Length != 32)
                return;

            using (var client = new HttpClient())
            {
                try
                {
                    var urlToken = "http://www.noelpush.com/generate_login_token?uid=" + id;
                    var token = client.GetStringAsync(urlToken);

                    var urlHistorique = "https://www.noelpush.com/login?token=" + token.Result;
                    Dispatcher.CurrentDispatcher.Invoke(() => Clipboard.SetText(urlHistorique));
                }
                catch (Exception)
                {
                    return;
                }

                this.timerHistorique = new Timer();
                this.timerHistorique.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                this.timerHistorique.Elapsed += this.HistoriqueTimeElapsed;
                this.timerHistorique.Enabled = true;
            }

            // Task + Dispatcher because it doesn't want without
            Task.Factory.StartNew(() => Dispatcher.CurrentDispatcher.Invoke(() => this.ShowPopupHistorique()));
        }

        private void HistoriqueTimeElapsed(object sender, ElapsedEventArgs e)
        {
            const string link = "https://www.noelpush.com/login";

            if (Clipboard.GetText().Contains(link))
                Clipboard.SetText(string.Empty);
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
