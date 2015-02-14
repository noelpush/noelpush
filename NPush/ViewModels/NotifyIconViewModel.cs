using System;

using NPush.Models;


namespace NPush.ViewModels
{
    class NotifyIconViewModel
    {
        private readonly Manager process;
        private readonly bool canScreen;

        public NotifyIconViewModel()
        {
            this.process = new Manager();
            this.canScreen = true;
        }

        private bool CanScreen
        {
            get { return this.canScreen; }
        }

        public void CaptureScreen()
        {
            if (this.CanScreen) process.CaptureScreen();
        }

        public void CaptureRegion()
        {
            if (this.CanScreen) process.CaptureRegion();
        }

        public void Exit()
        {
            Environment.Exit(1);
        }

    }
}
