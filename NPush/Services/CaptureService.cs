using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NoelPush.Objects;
using NoelPush.Views;


namespace NoelPush.Services
{
    public static class CaptureService
    {
        static CaptureService()
        {
            InitializeSelector();
        }

        static void InitializeSelector()
        {
            var area = new Rectangle(Left, Top, Width, Height);
            SelectorView.Initialize(area);
        }

        public static Bitmap CaptureRegion(ref ScreenshotData data)
        {
            InitializeSelector();

            var background = CaptureScreen(ref data);
            var rectangle = SelectorView.Showing(background);
            return BuildImg(ref data, rectangle, background);
        }

        public static Bitmap CaptureScreen(ref ScreenshotData data)
        {
            InitializeSelector();

            var rectangle = new Rectangle(0, 0, Width, Height);
            return BuildImg(ref data, rectangle);
        }

        private static Bitmap BuildImg(ref ScreenshotData screenshotData, Rectangle rec, Bitmap background = null)
        {
            screenshotData.StartDate = DateTime.Now;

            if (rec.Width <= 0 || rec.Height <= 0)
                return null;

            screenshotData.ImgSize = rec;

            var screen = background ?? new Bitmap(Width, Height);
            var selection = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(selection);
            g.CopyFromScreen(rec.Left + Left, rec.Top + Top, 0, 0, new Size(rec.Width, rec.Height), CopyPixelOperation.SourceCopy);
            g.DrawImage(screen, 0, 0, rec, GraphicsUnit.Pixel);

            return selection;
        }

        private static int Height
        {
            get { return SystemInformation.VirtualScreen.Height; }
        }

        private static int Width
        {
            get { return SystemInformation.VirtualScreen.Width; }
        }

        private static int Top
        {
            get { return SystemInformation.VirtualScreen.Top; }
        }

        private static int Left
        {
            get { return SystemInformation.VirtualScreen.Left; }
        }

        public static void CancelCapture()
        {
            SelectorView.Hiding();
        }
    }
}