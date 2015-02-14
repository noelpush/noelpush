using System;
using System.Reflection;
using System.Windows.Forms;

using NPush.Properties;
using NPush.Services;


namespace NPush
{
    public class IconMenu
    {
        private static NotifyIcon Icon;
        private static ContextMenuStrip Menu;

        public IconMenu()
        {
            Menu = new ContextMenuStrip();
            Menu.Items.Add(Resources.CaptureScreen, null, new EventHandler(Icon_CaptureScreen_OnClick));
            Menu.Items.Add(Resources.CaptureRegion, null, new EventHandler(Icon_CaptureRegion_OnClick));
            Menu.Items.Add(Resources.Exit, null, new EventHandler(Icon_Exit_OnClick));

            Icon = new NotifyIcon();
            Icon.MouseClick += new MouseEventHandler(this.Icon_MouseClick);

            Icon.Icon = Resources.Icon;
            Icon.ContextMenuStrip = Menu;
        }

        public void Show()
        {
            Icon.Visible = true;
        }

        public void Hide()
        {
            Icon.Visible = false;
        }

        public void ShowBalloonTip(string message, int duration = 0)
        {
            Icon.BalloonTipText = message;
            Icon.ShowBalloonTip(duration);
        }

        private void Icon_CaptureScreen_OnClick(object sender, EventArgs e)
        {
            ScreenCapture screen = new ScreenCapture();
            screen.CaptureScreen();
        }

        private void Icon_CaptureRegion_OnClick(object sender, EventArgs e)
        {
            MessageBox.Show("Screenshot");
        }

        private void Icon_Exit_OnClick(object sender, EventArgs e)
        {
            this.Hide();
            Application.Exit();
        }

        // Display menu's icon by left click
        private void Icon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(Icon, null);
            }
        }
    }
}
