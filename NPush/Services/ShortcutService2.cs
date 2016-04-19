using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NoelPush.Services
{
    [Flags]
    public enum ShortcutKeys
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 0x4000
    }

    public static class ShortcutService2
    {
        private static readonly IntPtr Handle;
        private static int id;

        public static event ShortcutEventHandler HotKeyPressed;
        public delegate void ShortcutEventHandler(object sender, ShortcutEventArgs e);

        static ShortcutService2()
        {
            var form = new MessageForm();
            Handle = form.Handle;
            id = form.GetHashCode();
        }

        public static int RegisterShortcut(ShortcutKeys modifiers, Keys key)
        {
            RegisterHotKey(Handle, id++, (uint)modifiers, (uint)key);

            return id;
        }

        public static int RegisterShortcut(Keys key)
        {
            RegisterHotKey(Handle, id++, (uint)ShortcutKeys.None, (uint)key);

            return id;
        }

        public static void UnregisterHotKey(int idKey)
        {
            UnregisterHotKey(Handle, idKey);
        }

        private static void OnHotKeyPressed(ShortcutEventArgs e)
        {
            if (HotKeyPressed == null)
            {
                return;
            }

            HotKeyPressed(null, e);
        }

        internal class MessageForm : Form
        {
            protected const int WmHotkey = 0x312;

            protected override void WndProc(ref Message message)
            {
                if (message.Msg == WmHotkey)
                {
                    var e = new ShortcutEventArgs(message.LParam);
                    OnHotKeyPressed(e);
                }

                base.WndProc(ref message);
            }

            protected override void SetVisibleCore(bool value)
            {
                base.SetVisibleCore(false);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    public class ShortcutEventArgs : EventArgs
    {
        public readonly ShortcutKeys Modifiers;
        public readonly Keys Key;

        public ShortcutEventArgs(ShortcutKeys modifiers, Keys key)
        {
            Modifiers = modifiers;
            Key = key;
        }

        public ShortcutEventArgs(IntPtr param)
        {
            Modifiers = (ShortcutKeys)((int)param & 0xffff);
            Key = (Keys)(((int)param >> 16) & 0xffff);
        }
    }
}
