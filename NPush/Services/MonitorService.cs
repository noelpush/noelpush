using System.Drawing;
using System.Windows.Forms;


namespace NoelPush.Services
{
    static class MonitorService
    {
        // Virtual screens
        
        public static int VirtualHeight
        {
            get { return SystemInformation.VirtualScreen.Height; }
        }

        public static int VirtualWidth
        {
            get { return SystemInformation.VirtualScreen.Width; }
        }

        public static int VirtualTop
        {
            get { return SystemInformation.VirtualScreen.Top; }
        }

        public static int VirtualLeft
        {
            get { return SystemInformation.VirtualScreen.Left; }
        }

        // Current screen
        
        public static int CurrentHeight
        {
            get { return Screen.FromPoint(Cursor.Position).Bounds.Height; }
        }

        public static int CurrentWidth
        {
            get { return Screen.FromPoint(Cursor.Position).Bounds.Width; }
        }

        public static int CurrentTop
        {
            get { return Screen.FromPoint(Cursor.Position).Bounds.Top; }
        }

        public static int CurrentLeft
        {
            get { return Screen.FromPoint(Cursor.Position).Bounds.Left; }
        }
    }
}
