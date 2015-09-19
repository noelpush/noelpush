using System;

using NPush.Models;


namespace NPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly Manager manager;
        private readonly bool canScreen;

        public event EventHandler EventShowUpdateMessage;
        public delegate void EventHandler(object sender);

        public NotifyIconViewModel()
        {
            this.manager = new Manager(this);
            this.canScreen = true;
        }

        public void SubscribeToEvent(EventHandler eventHandler)
        {
            this.EventShowUpdateMessage += eventHandler;
        }

        public void ShowMessage(string text)
        {
            this.EventShowUpdateMessage(text);
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
