using System;
using System.Drawing;
using System.Drawing.Imaging;
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
            SelectorView.Initialize();
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

            var rectangle = new Rectangle(0, 0, MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            return BuildImg(ref data, rectangle);
        }

        private static Bitmap BuildImg(ref ScreenshotData screenshotData, Rectangle rec, Bitmap background = null)
        {
            screenshotData.StartDate = DateTime.Now;

            if (rec.Width <= 1 && rec.Height <= 1)
                return null;

            screenshotData.ImgSize = rec;

            var screen = background ?? new Bitmap(MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            var selection = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(selection);
            g.CopyFromScreen(rec.Left + MonitorService.VirtualLeft, rec.Top + MonitorService.VirtualTop, 0, 0, new Size(rec.Width, rec.Height), CopyPixelOperation.SourceCopy);
            g.DrawImage(screen, 0, 0, rec, GraphicsUnit.Pixel);

            return selection;
        }

        public static void CancelCapture()
        {
            SelectorView.Hiding();
        }
    }
}