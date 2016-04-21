using System;
using System.Drawing;
using System.Drawing.Imaging;

using NoelPush.Helpers;
using NoelPush.Objects;
using NoelPush.Views;


namespace NoelPush.Services
{
    public static class CaptureService
    {
        static CaptureService()
        {
            SelectorView.Initialize();
        }

        public static Bitmap CaptureRegion(ref ScreenshotData screenshotData)
        {
            SelectorView.Initialize();

            var background = CaptureScreen(ref screenshotData);
            screenshotData.ImgSize = SelectorView.Showing(background);
            return BuildImg(ref screenshotData, background);
        }

        public static Bitmap CaptureScreen(ref ScreenshotData screenshotData)
        {
            screenshotData.ImgSize = new Rectangle(0, 0, MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            return BuildImg(ref screenshotData);
        }

        public static Bitmap CaptureFullScreen(ref ScreenshotData screenshotData)
        {
            screenshotData.ImgSize = new Rectangle(0, 0, MonitorService.CurrentWidth, MonitorService.CurrentHeight);
            return BuildImgFullScreen(ref screenshotData);
        }

        private static Bitmap BuildImg(ref ScreenshotData screenshotData, Bitmap background = null)
        {
            if (screenshotData.ImgSize.Width <= 0 || screenshotData.ImgSize.Height <= 0)
                return null;

            screenshotData.StartDate = DateTime.Now;

            var screen = background ?? new Bitmap(MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            var selection = new Bitmap(screenshotData.ImgSize.Width, screenshotData.ImgSize.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(selection);
            g.CopyFromScreen(screenshotData.ImgSize.Left + MonitorService.VirtualLeft, screenshotData.ImgSize.Top + MonitorService.VirtualTop, 0, 0, new Size(screenshotData.ImgSize.Width, screenshotData.ImgSize.Height), CopyPixelOperation.SourceCopy);
            g.DrawImage(screen, 0, 0, screenshotData.ImgSize, GraphicsUnit.Pixel);

            return selection;
        }

        private static Bitmap BuildImgFullScreen(ref ScreenshotData screenshotData)
        {
            if (screenshotData.ImgSize.Width <= 0 || screenshotData.ImgSize.Height <= 0)
                return null;

            screenshotData.StartDate = DateTime.Now;

            Win32Helper.SIZE size;

            var hDC = Win32Helper.GetDC(Win32Helper.GetDesktopWindow());
            var hMemDC = Win32Helper.CreateCompatibleDC(hDC);

            size.x = Win32Helper.GetSystemMetrics(Win32Helper.SM_CXSCREEN);
            size.y = Win32Helper.GetSystemMetrics(Win32Helper.SM_CYSCREEN);

            var m_HBitmap = Win32Helper.CreateCompatibleBitmap(hDC, size.x, size.y);

            if (m_HBitmap == IntPtr.Zero) 
                return null;

            var hOld = Win32Helper.SelectObject(hMemDC, m_HBitmap);
            Win32Helper.BitBlt(hMemDC, 0, 0, size.x, size.y, hDC, 0, 0, Win32Helper.SRCCOPY);
            Win32Helper.SelectObject(hMemDC, hOld);
            Win32Helper.DeleteDC(hMemDC);
            Win32Helper.ReleaseDC(Win32Helper.GetDesktopWindow(), hDC);

            return Image.FromHbitmap(m_HBitmap);
        }

        public static void CancelCapture()
        {
            SelectorView.Hiding();
        }
    }
}