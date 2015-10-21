using System;
using NPush.Models;


namespace NPush.ViewModels
{
    public class NotifyIconViewModel
    {
        private readonly Manager manager;
        private readonly bool canScreen;

        public event TooltipMessageEventHandler ShowUpdateMessageEvent;
        public delegate void TooltipMessageEventHandler(string text);

        public event EnableCommandsEventHandler EnableCommandsEvent;
        public delegate void EnableCommandsEventHandler(bool enabled);

        public NotifyIconViewModel()
        {
            this.manager = new Manager(this);
            this.canScreen = true;
        }

        public void SubscribeToEvent(TooltipMessageEventHandler eventHandler)
        {
            this.ShowUpdateMessageEvent += eventHandler;
        }

        public void SubscribeToEvent(EnableCommandsEventHandler eventHandler)
        {
            this.EnableCommandsEvent += eventHandler;
        }

        public void ShowMessage(string text)
        {
            this.ShowUpdateMessageEvent(text);
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
