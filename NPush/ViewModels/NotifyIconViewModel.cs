using System;
using System.Drawing;
using System.Threading;
using NPush.Models;
using NPush.ViewModels.Popup;
using NPush.Views.Popup;

namespace NPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly bool canScreen;

        private readonly Manager manager;

        public PopupUploadView PopupUpload { get; private set; }
        public PopupUploadViewModel PopupUploadDataContext { get; private set; }

        public PopupUploadFailedView PopupUploadFailed { get; private set; }
        public PopupUploadFailedViewModel PopupUploadFailedDataContext { get; private set; }

        public PopupFirstRunView PopupFirstRun { get; private set; }
        public PopupFirstRunViewModel PopupFirstRunDataContext { get; private set; }

        public delegate void TooltipMessageEventHandler(Bitmap img);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel()
        {
            this.canScreen = true;

            this.PopupUploadDataContext = new PopupUploadViewModel();
            this.PopupFirstRunDataContext = new PopupFirstRunViewModel();
            this.PopupUploadFailedDataContext = new PopupUploadFailedViewModel();

            this.PopupUpload = new PopupUploadView { DataContext = this.PopupUploadDataContext };
            this.PopupFirstRun = new PopupFirstRunView { DataContext = this.PopupFirstRunDataContext };
            this.PopupUploadFailed = new PopupUploadFailedView { DataContext = this.PopupUploadFailedDataContext };

            this.manager = new Manager(this);
        }

        public void SubscribeToEvent(EnableCommandsEventHandler eventHandler)
        {
            this.EnableCommandsEvent += eventHandler;
        }

        public void ShowPopupUpload(Bitmap img, int delay = 3000)
        {
            this.PopupUploadDataContext.ShowPopup(img);
            Thread.Sleep(delay);
            this.PopupUploadDataContext.HidePopup();
        }

        public void ShowPopupUploadFailed(int delay = 3000)
        {
            this.PopupUploadFailedDataContext.ShowPopup();
            Thread.Sleep(delay);
            this.PopupUploadFailedDataContext.HidePopup();
        }

        public void ShowPopupMessage(int delay = 7000)
        {
            this.PopupFirstRunDataContext.ShowPopup();
            Thread.Sleep(delay);
            this.PopupFirstRunDataContext.HidePopup();
        }

        public void EnableCommands(bool enabled)
        {
            this.EnableCommandsEvent(enabled);
        }

        private bool CanScreen
        {
            get { return this.canScreen; }
        }

        public void CaptureScreen()
        {
            if (this.CanScreen) 
                this.manager.CaptureScreen();
        }

        public void CaptureRegion()
        {
            if (this.CanScreen) 
                this.manager.CaptureRegion();
        }

        public void Exit()
        {
            Environment.Exit(1);
        }
    }
}
