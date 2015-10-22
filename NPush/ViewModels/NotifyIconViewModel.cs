using System;
using System.Drawing;
using System.Threading;
using NPush.Models;


namespace NPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly bool canScreen;

        private readonly Manager manager;
        public PopupViewModel PopupDataContext { get; private set; }

        public event TooltipMessageEventHandler ShowUpdateMessageEvent;
        public delegate void TooltipMessageEventHandler(Bitmap img);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel()
        {
            this.manager = new Manager(this);
            this.PopupDataContext = new PopupViewModel();
            this.canScreen = true;
        }

        public void SubscribeToEvent(EnableCommandsEventHandler eventHandler)
        {
            this.EnableCommandsEvent += eventHandler;
        }

        public void ShowMessage(Bitmap img)
        {
            this.PopupDataContext.ShowPopup(img);
            Thread.Sleep(3000);
            this.PopupDataContext.HidePopup();
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
