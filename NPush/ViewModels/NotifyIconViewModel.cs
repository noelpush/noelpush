using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Threading;
using NoelPush.Models;
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

            this.PopupCopyDataContext = new PopupViewModel(323, 75, 3000);
            this.PopupUploadDataContext = new PopupViewModel(323, 75, 3000);
            this.PopupFirstRunDataContext = new PopupViewModel(323, 95, 3000);
            this.PopupUploadFailedDataContext = new PopupViewModel(323, 75, 4000);
            this.PopupPictureTooLargeDataContext = new PopupViewModel(323, 75, 4000);
            this.PopupConnexionFailedDataContext = new PopupViewModel(323, 75, 4000);

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

        public void CaptureRegion()
        {
            if (this.CanScreen)
                this.manager.CaptureRegion(true);
        }

        public void CaptureScreen()
        {
            if (this.CanScreen)
                this.manager.CaptureScreen(true);
        }

        public void Exit()
        {
            Environment.Exit(1);
        }
    }
}
