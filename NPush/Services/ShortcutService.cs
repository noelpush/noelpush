using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace NoelPush.Services
{

    public delegate void ShortcutEventHandler(object sender, ShortcutEventArgs e);

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

    public static class ShortcutService
    {
        private static MessageForm Form;
        private static IntPtr Handle;
        private static int Id;
        public static event ShortcutEventHandler HotKeyPressed;

        static ShortcutService()
        {
            Form = new MessageForm();
            Handle = Form.Handle;
            Id = Form.GetHashCode();
        }

        public static int RegisterShortcut(ShortcutKeys modifiers, Keys key)
        {
            RegisterHotKey(Handle, Id++, (uint)modifiers, (uint)key);

            return Id;
        }

        public static int RegisterShortcut(Keys key)
        {
            RegisterHotKey(Handle, Id++, (uint)ShortcutKeys.None, (uint)key);

            return Id;
        }

        public static void UnregisterHotKey(int id)
        {
            UnregisterHotKey(Handle, id);
        }

        private static void OnHotKeyPressed(ShortcutEventArgs e)
        {
            if (HotKeyPressed == null)
            {
                return;
            }

            HotKeyPressed(null, e);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        internal class MessageForm : Form
        {
            protected const int WM_HOTKEY = 0x312;

            protected override void WndProc(ref Message message)
            {
                if (message.Msg == WM_HOTKEY)
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
