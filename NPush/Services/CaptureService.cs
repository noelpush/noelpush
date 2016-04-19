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

        public static Bitmap CaptureRegion(ref ScreenshotData screenshotData)
        {
            InitializeSelector();

            var background = CaptureScreen(ref screenshotData);
            screenshotData.ImgSize = SelectorView.Showing(background);
            return BuildImg(ref screenshotData, background);
        }

        public static Bitmap CaptureScreen(ref ScreenshotData screenshotData)
        {
            InitializeSelector();

            screenshotData.ImgSize = new Rectangle(0, 0, MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            return BuildImg(ref screenshotData);
        }

        private static Bitmap BuildImg(ref ScreenshotData screenshotData, Bitmap background = null)
        {
            screenshotData.StartDate = DateTime.Now;

            if (screenshotData.ImgSize.Width <= 0 || screenshotData.ImgSize.Height <= 0)
                return null;

            var screen = background ?? new Bitmap(MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            var selection = new Bitmap(screenshotData.ImgSize.Width, screenshotData.ImgSize.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(selection);
            g.CopyFromScreen(screenshotData.ImgSize.Left + MonitorService.VirtualLeft, screenshotData.ImgSize.Top + MonitorService.VirtualTop, 0, 0, new Size(screenshotData.ImgSize.Width, screenshotData.ImgSize.Height), CopyPixelOperation.SourceCopy);
            g.DrawImage(screen, 0, 0, screenshotData.ImgSize, GraphicsUnit.Pixel);

            return selection;
        }

        public static void CancelCapture()
        {
            SelectorView.Hiding();
        }
    }
}