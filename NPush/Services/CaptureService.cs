using System.Drawing;
using System.Drawing.Imaging;
using NoelPush.Views;


namespace NoelPush.Services
{
    public static class CaptureService
    {
        public static Bitmap CaptureRegion(ref Rectangle imgSize)
        {
            SelectorView.Initialize();

            var background = CaptureScreen(ref imgSize);
            imgSize = SelectorView.Showing(background);
            return BuildImg(ref imgSize, background);
        }

        public static Bitmap CaptureScreen(ref Rectangle imgSize)
        {
            imgSize = new Rectangle(0, 0, MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            return BuildImg(ref imgSize);
        }

        private static Bitmap BuildImg(ref Rectangle imgSize, Bitmap background = null)
        {
            if (imgSize.Width <= 0 || imgSize.Height <= 0)
                return null;

            var screen = background ?? new Bitmap(MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            var selection = new Bitmap(imgSize.Width, imgSize.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(selection);
            g.CopyFromScreen(imgSize.Left + MonitorService.VirtualLeft, imgSize.Top + MonitorService.VirtualTop, 0, 0, new Size(imgSize.Width, imgSize.Height), CopyPixelOperation.SourceCopy);
            g.DrawImage(screen, 0, 0, imgSize, GraphicsUnit.Pixel);

            return selection;
        }

        public static void CancelCapture()
        {
            SelectorView.Hiding();
        }
    }
}