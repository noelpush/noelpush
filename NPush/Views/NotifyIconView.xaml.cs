using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using NPush.ViewModels;

namespace NPush.Views
{
    internal partial class NotifyIconView
    {
        private readonly NotifyIcon NotifIcon;
        private PopupViewModel PopupDataContext;
        private ContextMenuStrip NotifMenu;

        public NotifyIconView()
        {
            this.DataContext = new NotifyIconViewModel();
            this.PopupDataContext = new PopupViewModel();

            var path = System.IO.Directory.GetCurrentDirectory() + @"\icon.ico";
            var icon = new Icon(path);
            //var icon = Properties.Resources.icon;

            this.NotifMenu = new ContextMenuStrip();

            this.NotifMenu.Items.Add(Properties.Resources.ScreenInProgress, null, this.ScreenInProgressAction);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureRegion, null, this.CaptureRegionAction);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureScreen, null, this.CaptureScreenAction);
            this.NotifMenu.Items.Add(Properties.Resources.Exit, null, this.ExitAction);

            this.NotifMenu.AutoClose = false;
            this.NotifMenu.Items[0].Visible = false;

            this.NotifIcon = new NotifyIcon
            {
                Icon = icon,
                Text = Properties.Resources.SoftwareName,
                ContextMenuStrip = this.NotifMenu
            };

            this.NotifIcon.MouseClick += OnClick;

            this.ShowIcon();

            var notifyIconViewModel = DataContext as NotifyIconViewModel;
            notifyIconViewModel.SubscribeToEvent(ShowMessage);
            notifyIconViewModel.SubscribeToEvent(EnableCommands);
        }

        public void ShowIcon()
        {
            this.NotifIcon.Visible = true;
        }

        public void HideIcon()
        {
            this.NotifIcon.Visible = false;
        }

        private void ScreenInProgressAction(object sender, EventArgs e)
        {
            this.OpenNotifyIcon();
        }

        private void CaptureScreenAction(object sender, EventArgs e)
        {
            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;
            if (notifyIconViewModel != null) 
                notifyIconViewModel.CaptureScreen();

            this.NotifMenu.Close();
        }

        private void CaptureRegionAction(object sender, EventArgs e)
        {
            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;
            if (notifyIconViewModel != null) 
                notifyIconViewModel.CaptureRegion();

            this.NotifMenu.Close();
        }

        private void ExitAction(object sender, EventArgs e)
        {
            this.NotifMenu.Close();
            this.HideIcon();

            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;

            if (notifyIconViewModel != null)
                notifyIconViewModel.Exit();
        }

        // Display menu's icon by left click (right click default)
        private void OnClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) 
                return;

            this.OpenNotifyIcon();
        }

        private void OpenNotifyIcon()
        {
            var context = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);

            if (!this.NotifMenu.Visible)
                context.Invoke(this.NotifIcon, null);
            else
                this.NotifMenu.Visible = false;
        }

        private void ShowMessage(string text)
        {
            this.PopupDataContext.ShowPopup();
            Thread.Sleep(3000);
            this.PopupDataContext.HidePopup();

            //this.NotifIcon.BalloonTipText = text;
            //this.NotifIcon.ShowBalloonTip(Properties.Settings.Default.TimePopup);
        }

        public void EnableCommands(bool enabled)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => this.SetEnable(enabled));
            System.Windows.Application.Current.Dispatcher.Invoke(() => this.SetIcon(enabled));
        }

        private void SetIcon(bool enabled)
        {
            var path = System.IO.Directory.GetCurrentDirectory();
            path += enabled ? @"\\icon.ico" : @"\\icon_upload.ico";
            this.NotifIcon.Icon = new Icon(path);
        }

        public void SetEnable(bool enabled)
        {
            this.NotifIcon.ContextMenuStrip.Items[0].Visible = !enabled;
            this.NotifIcon.ContextMenuStrip.Items[1].Enabled = enabled;
            this.NotifIcon.ContextMenuStrip.Items[2].Enabled = enabled;

            // Rename exit button
            this.NotifIcon.ContextMenuStrip.Items[3].Text = enabled ? Properties.Resources.Exit : Properties.Resources.ExitNPush;
        }

        public void Connect(int connectionId, object target){}
    }
}
