using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using NPush.ViewModels;

namespace NPush.Views
{
    internal partial class NotifyIconView
    {
        private readonly NotifyIcon NotifIcon;
        private ContextMenuStrip NotifMenu;

        public NotifyIconView()
        {
            this.DataContext = new NotifyIconViewModel();

            var path = System.IO.Directory.GetCurrentDirectory() + @"\\icon.ico";
            var icon = new Icon(path);
            //var icon = Properties.Resources.icon;

            this.NotifMenu = new ContextMenuStrip();
            this.NotifMenu.Items.Add(Properties.Resources.ScreenInProgress, null);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureScreen, null, this.CaptureScreen);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureRegion, null, this.CaptureRegion);
            this.NotifMenu.Items.Add(Properties.Resources.Exit, null, this.Exit);
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

        private void CaptureScreen(object sender, EventArgs e)
        {
            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;
            if (notifyIconViewModel != null) notifyIconViewModel.CaptureScreen();
        }

        private void CaptureRegion(object sender, EventArgs e)
        {
            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;
            if (notifyIconViewModel != null) notifyIconViewModel.CaptureRegion();
        }

        private void Exit(object sender, EventArgs e)
        {
            this.HideIcon();

            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;
            if (notifyIconViewModel != null) notifyIconViewModel.Exit();
        }

        // Display menu's icon by left click (right click default)
        private void OnClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);

            if (!this.NotifMenu.Visible)
                mi.Invoke(this.NotifIcon, null);
            else
                this.NotifMenu.Visible = false;
        }

        private void ShowMessage(string text)
        {
            this.NotifIcon.BalloonTipClicked += NotifIcon_BalloonTipClicked;
            this.NotifIcon.BalloonTipText = text;
            this.NotifIcon.ShowBalloonTip(Properties.Settings.Default.TimePopup);
        }

        // Open url in browser if tooltip is clicked
        private void NotifIcon_BalloonTipClicked(object sender, EventArgs eventArgs)
        {
            var url = this.NotifIcon.BalloonTipText.Split('\n').First();
            Process.Start(url);
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
        }

        public void Connect(int connectionId, object target){}
    }
}
