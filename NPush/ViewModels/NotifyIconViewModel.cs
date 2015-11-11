using System;
using System.Drawing;
using System.Threading;
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

        public PopupUploadView PopupUpload { get; private set; }
        public PopupUploadViewModel PopupUploadDataContext { get; private set; }

        public PopupUploadFailedView PopupUploadFailed { get; private set; }
        public PopupUploadFailedViewModel PopupUploadFailedDataContext { get; private set; }

        public PopupConnexionFailedView PopupConnexionFailed { get; private set; }
        public PopupConnexionFailedViewModel PopupConnexionFailedDataContext { get; private set; }

        public PopupFirstRunView PopupFirstRun { get; private set; }
        public PopupFirstRunViewModel PopupFirstRunDataContext { get; private set; }

        public delegate void TooltipMessageEventHandler(Bitmap img);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel(EnableCommandsEventHandler eventHandler)
        {
            this.EnableCommandsEvent += eventHandler;

            this.canScreen = true;

            this.PopupUploadDataContext = new PopupUploadViewModel();
            this.PopupFirstRunDataContext = new PopupFirstRunViewModel();
            this.PopupUploadFailedDataContext = new PopupUploadFailedViewModel();
            this.PopupConnexionFailedDataContext = new PopupConnexionFailedViewModel();

            this.PopupUpload = new PopupUploadView { DataContext = this.PopupUploadDataContext };
            this.PopupFirstRun = new PopupFirstRunView { DataContext = this.PopupFirstRunDataContext };
            this.PopupUploadFailed = new PopupUploadFailedView { DataContext = this.PopupUploadFailedDataContext };

            this.manager = new Manager(this);
        }

        public void ShowPopupUpload(Bitmap img, int delay = 3000)
        {
            this.PopupUploadDataContext.ShowPopup(img, delay);
            Thread.Sleep(delay);
            this.PopupUploadDataContext.HidePopup();
        }

        public void ShowPopupUploadFailed(int delay = 3000)
        {
            this.PopupUploadFailedDataContext.ShowPopup(delay);
            Thread.Sleep(delay);
            this.PopupUploadFailedDataContext.HidePopup();
        }

        public void ShowPopupMessage(int delay = 11000)
        {
            this.PopupFirstRunDataContext.ShowPopup(delay);
            Thread.Sleep(delay);
            this.PopupFirstRunDataContext.HidePopup();
        }

        public void ShowPopupConnexionFailed(int delay = 4000)
        {
            this.PopupConnexionFailedDataContext.ShowPopup(delay);
            Thread.Sleep(delay);
            this.PopupConnexionFailedDataContext.HidePopup();
        }

        public void EnableCommands(bool enabled)
        {
            this.EnableCommandsEvent(enabled);
        }

        private bool CanScreen
        {
            get { return this.canScreen; }
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
