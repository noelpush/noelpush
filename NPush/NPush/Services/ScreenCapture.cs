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
            WidthScreen = getWidthScreens();
            HeightScreen = getHeightScreens();

            this.manager = manager;
            this.selector = new SelectorView(this);
        }

        public void CaptureScreen()
        {
            var rectangle = new Rectangle(0, 0, WidthScreen, HeightScreen);
            this.BuildImg(rectangle);
        }

        public void CaptureRegion()
        {
            this.selector.Showing();
        }

        public void BuildImg(Rectangle rec)
        {
            var bitmap = new Bitmap(WidthScreen, HeightScreen);
            var bmp = new Bitmap(rec.Width, rec.Height);

            var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(new Point(rec.Left, rec.Top), Point.Empty, new Size(rec.Width, rec.Height));
                g.DrawImage(bitmap, 0, 0, rec, GraphicsUnit.Pixel);

            Task.Factory.StartNew(() => manager.Captured(bmp, 2));
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

        // dual monitor http://www.codeproject.com/Articles/330837/ScreenCap-CSharp-Screen-Capture-Application

        internal void Canceled()
        {
            this.manager.isScreenCapturing = false;
            this.selector.Hiding();
        }
    }
}