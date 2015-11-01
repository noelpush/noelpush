﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        internal void CaptureSimpleScreen(object sender, DoWorkEventArgs e)
        {
            var rec = new Rectangle(this.Left, this.Top, this.Width, this.Height);
            var bmp = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppRgb);
            var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(Point.Empty, Point.Empty, rec.Size);

            this.manager.Screened(bmp);
        }

        public void CaptureScreen(ScreenshotData data)
        {
            var rectangle = new Rectangle(0, 0, this.Width, this.Height);
            this.BuildImg(rectangle, data);
        }

        public void BuildImg(Rectangle rec, ScreenshotData data)
        {
            if (rec.Width <= 0 || rec.Height <= 0)
                return;

            data.img_size = rec;
            var selection = new Bitmap(rec.Width, rec.Height, PixelFormat.Format32bppArgb);
            var screen = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);

            var g = Graphics.FromImage(selection);
                g.CopyFromScreen(rec.Left + this.Left, rec.Top + this.Top, 0, 0, new Size(rec.Width, rec.Height), CopyPixelOperation.SourceCopy);
                g.DrawImage(screen, 0, 0, rec, GraphicsUnit.Pixel);

                Task.Factory.StartNew(() => manager.Captured(selection, data));
        }

        public PictureData GetPictureSize(Bitmap img)
        {
            var pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NoelPush\";
            var fileName = DateTime.Now.ToString(@"yyyy-MM-dd-HH-mm-ss-");

            var pathPng = pathFolder + fileName + ".png";
            var pathJpeg = pathFolder + fileName + ".jpeg";

            img.Save(pathPng, ImageFormat.Png);
            img.Save(pathJpeg, ImageFormat.Jpeg);

            var sizePng = (int)(new FileInfo(pathPng)).Length;
            var sizeJpeg = (int)(new FileInfo(pathJpeg)).Length;

            var bmpPng = LoadImage(pathPng);
            var bmpJpeg = LoadImage(pathJpeg);

            File.Delete(pathPng);
            File.Delete(pathJpeg);

            return new PictureData(bmpPng, bmpJpeg, sizePng, sizeJpeg);
        }

        public static Bitmap LoadImage(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path)); // Don't use using!!
            return new Bitmap(Image.FromStream(ms));
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