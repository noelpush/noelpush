using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using NPush.Models;
using NPush.Views;


namespace NPush.Services
{
    public class ScreenCapture
    {
        private readonly Manager manager;
        private readonly SelectorView selector;

        private readonly int WidthScreen;
        private readonly int HeightScreen;
        
        public ScreenCapture(Manager manager)
        {
            this.WidthScreen = getWidthScreens();
            this.HeightScreen = getHeightScreens();

            this.manager = manager;
            this.selector = new SelectorView(this);
        }

        public void CaptureRegion()
        {
            this.selector.Showing();
        }

        public void CaptureSimpleScreen()
        {
            var rec = new Rectangle(0, 0, this.WidthScreen, this.HeightScreen);
            var bmp = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppRgb);
            var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(Point.Empty, Point.Empty, rec.Size);

            this.manager.Screened(bmp);
        }

        public void CaptureScreen()
        {
            var rectangle = new Rectangle(0, 0, WidthScreen, HeightScreen);
            this.BuildImg(rectangle);
        }

        public void BuildImg(Rectangle rec)
        {
            if (rec.Width <= 0 || rec.Height <= 0)
                return;

            var bitmap = new Bitmap(WidthScreen, HeightScreen);
            var bmp = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppRgb);

            var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(new Point(rec.Left, rec.Top), Point.Empty, new Size(rec.Width, rec.Height));
                g.DrawImage(bitmap, 0, 0, rec, GraphicsUnit.Pixel);

            Task.Factory.StartNew(() => manager.Captured(bmp));
        }

        public long[] SaveImage(Bitmap img)
        {
            CheckFolder();

            var pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NPush\Historique\";
            var fileName = DateTime.Now.ToString(@"yyyy-MM-dd-HH-mm-ss-");

            img.Save(pathFolder + fileName + ".png", ImageFormat.Png);
            img.Save(pathFolder + fileName + ".jpeg", ImageFormat.Jpeg);

            var sizePng = (new FileInfo(pathFolder + fileName + ".png")).Length;
            var sizeJpg = (new FileInfo(pathFolder + fileName + ".jpeg")).Length;

            File.Delete(pathFolder + fileName + ".png");
            File.Delete(pathFolder + fileName + ".jpeg");

            return new[] { sizePng, sizeJpg };
        }

        private int getHeightScreens()
        {
            return Screen.AllScreens.Select(screen => screen.WorkingArea.Height).Concat(new[] { 0 }).Max();
        }

        private int getWidthScreens()
        {
            return Screen.AllScreens.Sum(screen => screen.WorkingArea.Width);
        }

        private void CheckFolder()
        {
            var pathAppdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathNPush = pathAppdata + @"\NPush";
            var pathHistorique = pathNPush + @"\Historique";

            if (!Directory.Exists(pathNPush))
                Directory.CreateDirectory(pathNPush);

            if (!Directory.Exists(pathHistorique))
                Directory.CreateDirectory(pathHistorique);
        }

        internal void Canceled()
        {
            this.selector.Hiding();
        }
    }
}