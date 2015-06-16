using System;
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

            this.NotifMenu = new ContextMenuStrip();
            this.NotifMenu.Items.Add(Properties.Resources.CaptureScreen, null, this.CaptureScreen);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureRegion, null, this.CaptureRegion);
            this.NotifMenu.Items.Add(Properties.Resources.Exit, null, this.Exit);

            this.NotifIcon = new NotifyIcon { Icon = Properties.Resources.icon, ContextMenuStrip = this.NotifMenu };
            this.NotifIcon.MouseClick += OnClick;

            this.ShowIcon();

            (DataContext as NotifyIconViewModel).SubscribeToEvent(ShowUpdateMessage);
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

        private void ShowUpdateMessage(object sender)
        {
            this.NotifIcon.BalloonTipText = Properties.Resources.NewVersion;
            this.NotifIcon.ShowBalloonTip(20000);
        }
    }
}
