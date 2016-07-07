using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using NLog;

using NoelPush.Services;
using NoelPush.ViewModels;

namespace NoelPush.Views
{
    internal partial class NotifyIconView
    {
        private List<char> debug;
        private readonly NotifyIcon NotifIcon;
        private readonly ContextMenuStrip NotifMenu;

        private bool IsOpen;

        public NotifyIconView()
        {
            this.IsOpen = false;
            this.debug = new List<char>();

            var icon = Properties.Resources.icon;

            this.NotifMenu = new ContextMenuStrip();
            this.NotifMenu.KeyDown += NotifMenuOnKeyDown;

            this.NotifMenu.Items.Add(Properties.Resources.ScreenInProgress, null, this.ScreenInProgressAction);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureRegion, null, this.CaptureRegionAction);
            this.NotifMenu.Items.Add(Properties.Resources.CaptureScreen, null, this.CaptureScreenAction);
            this.NotifMenu.Items.Add(Properties.Resources.Historique, null, this.HistoriqueAction);
            this.NotifMenu.Items.Add(Properties.Resources.Exit, null, this.ExitAction);

            //this.NotifMenu.Items[1].Image = Bitmap.FromFile(@"");
            this.NotifMenu.Items[0].Visible = false;
            this.NotifMenu.Items[0].Enabled = false;
            this.NotifMenu.Items[0].Font = new Font(this.NotifMenu.Items[0].Font.FontFamily, this.NotifMenu.Items[0].Font.Size, System.Drawing.FontStyle.Bold);

            this.NotifIcon = new NotifyIcon
            {
                Icon = icon,
                Text = Properties.Resources.SoftwareName,
                ContextMenuStrip = this.NotifMenu,
            };

            this.NotifIcon.MouseClick += OnClick;
            UpdatesService.RestartAppEvent += RestartAppEvent;

            this.ShowIcon();

            this.DataContext = new NotifyIconViewModel(EnableCommands);
        }

        private void NotifMenuOnKeyDown(object sender, KeyEventArgs k)
        {
            var letter = (char)k.KeyCode;

            if (!char.IsLetterOrDigit(letter))
                return;

            debug.Add(letter);

            if (debug.Count > 5)
                debug.RemoveAt(0);

            var concat = string.Join(string.Empty, debug.ToArray());
            if (concat.ToLower() == "debug")
            {
                try
                {
                    var path = Application.StartupPath + "\\noelpush.logs.txt";
                    System.Diagnostics.Process.Start(@path);
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e.Message);
                }
            }
        }

        private void RestartAppEvent()
        {
            this.HideIcon();
        }

        private void HistoriqueAction(object sender, EventArgs e)
        {
            var notifyIconViewModel = this.DataContext as NotifyIconViewModel;
            if (notifyIconViewModel != null)
            {
                notifyIconViewModel.Historique();
            }
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
            this.OpenNotifyIconLeftClick();
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
            if (e.Button == MouseButtons.Left)
                this.OpenNotifyIconLeftClick();
        }

        private void OpenNotifyIconLeftClick()
        {
            var action = this.IsOpen ? "HideContextMenu" : "ShowContextMenu";
            if (this.IsOpen)
                this.IsOpen = false;

            var context = typeof(NotifyIcon).GetMethod(action, BindingFlags.Instance | BindingFlags.NonPublic);
            context.Invoke(this.NotifIcon, null);
        }

        public void EnableCommands(bool enabled)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => this.SetEnable(enabled));
            System.Windows.Application.Current.Dispatcher.Invoke(() => this.SetIcon(enabled));
        }

        private void SetIcon(bool enabled)
        {
            var icon = enabled ? Properties.Resources.icon : Properties.Resources.icon_upload;
            this.NotifIcon.Icon = icon;
        }

        public void SetEnable(bool enabled)
        {
            // Show/Hide the upload info
            this.NotifMenu.Items[0].Visible = !enabled;

            // Enable/Disable the capture buttons
            this.NotifIcon.ContextMenuStrip.Items[1].Enabled = enabled;
            this.NotifIcon.ContextMenuStrip.Items[2].Enabled = enabled;

            // Rename exit button
            this.NotifIcon.ContextMenuStrip.Items[4].Text = enabled ? Properties.Resources.Exit : Properties.Resources.ExitNoelPush;
        }
    }
}
