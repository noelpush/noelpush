using System;
using System.Drawing;
using System.Threading;
using System.Windows.Threading;
using NPush.Models;
using NPush.Views;


namespace NPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly bool canScreen;

        private readonly Manager manager;
        public PopupUploadView PopupUpload { get; private set; }
        public PopupUploadViewModel PopupUploadDataContext { get; private set; }

        public PopupMessageView PopupMessage { get; private set; }
        public PopupMessageViewModel PopupMessageDataContext { get; private set; }

        public delegate void TooltipMessageEventHandler(Bitmap img);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel()
        {
            this.canScreen = true;

            this.PopupUploadDataContext = new PopupUploadViewModel();
            this.PopupMessageDataContext = new PopupMessageViewModel();

            this.PopupUpload = new PopupUploadView { DataContext = this.PopupUploadDataContext };
            this.PopupMessage = new PopupMessageView { DataContext = this.PopupMessageDataContext };

            this.manager = new Manager(this);
        }

        public void SubscribeToEvent(EnableCommandsEventHandler eventHandler)
        {
            this.EnableCommandsEvent += eventHandler;
        }

        public void ShowPopupUpload(Bitmap img, int delay = 2000)
        {
            this.PopupUploadDataContext.ShowPopup(img);
            Thread.Sleep(delay);
            this.PopupUploadDataContext.HidePopup();
        }

        public void ShowPopupMessage(string text, int delay = 2000)
        {
            this.PopupMessageDataContext.ShowPopup(text);
            Thread.Sleep(delay);
            this.PopupMessageDataContext.HidePopup();
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
