using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using NoelPush.Models;
using NoelPush.Objects;
using NoelPush.Views;


namespace NoelPush.Services
{
    public class ScreenCapture
    {
        private readonly Manager manager;
        private readonly SelectorView selector;
        
        public ScreenCapture(Manager manager)
        {
            var area = new Rectangle(this.Left, this.Top, this.Width, this.Height);

            this.manager = manager;
            this.selector = new SelectorView(this, area);
        }

        public void CaptureRegion(ScreenshotData data)
        {
            this.selector.Showing(data);
        }

        public void CaptureScreen(ScreenshotData data)
        {
            var rectangle = new Rectangle(0, 0, this.Width, this.Height);
            this.BuildImg(rectangle, data);
        }

        public void BuildImg(Rectangle rec, ScreenshotData screenshotData)
        {
            screenshotData.start_date = DateTime.Now;

            if (rec.Width <= 1 || rec.Height <= 1)
                return;

            screenshotData.img_size = rec;

            var screen = new Bitmap(this.Width, this.Height);
            var selection = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(selection);
                g.CopyFromScreen(rec.Left + this.Left, rec.Top + this.Top, 0, 0, new System.Drawing.Size(rec.Width, rec.Height), CopyPixelOperation.SourceCopy);
                g.DrawImage(screen, 0, 0, rec, GraphicsUnit.Pixel);

            System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Clipboard.SetImage(CreateBitmapSourceFromBitmap(selection)));

            Task.Factory.StartNew(() => manager.Captured(selection, screenshotData));
        }

        public BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private int Height
        {
            get { return SystemInformation.VirtualScreen.Height; }
        }

        private int Width
        {
            get { return SystemInformation.VirtualScreen.Width; }
        }

        private int Top
        {
            get { return SystemInformation.VirtualScreen.Top; }
        }

        private int Left
        {
            get { return SystemInformation.VirtualScreen.Left; }
        }

        internal void Canceled()
        {
            this.selector.Hiding();
        }
    }
}