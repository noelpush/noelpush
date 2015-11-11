using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace NoelPush.Objects
{
    public class PictureData
    {
        public byte[] dataBytes { get; private set; }
        public Bitmap picture { get; private set; }
        public ScreenshotData screenshotData { get; private set; }

        public int sizePng { get; private set; }
        public int sizeJpeg { get; private set; }

        public PictureData(Bitmap img, ScreenshotData screenshotData)
        {
            this.picture = img;
            this.screenshotData = screenshotData;

            this.Initialize();
        }

        private void Initialize()
        {
            var pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NoelPush\";
            var fileName = DateTime.Now.ToString(@"yyyy-MM-dd-HH-mm-ss");

            var pathPng = pathFolder + fileName + ".png";
            var pathJpeg = pathFolder + fileName + ".jpeg";

            this.CreateImage(pathPng, pathJpeg);

            this.screenshotData.png_size = (int)(new FileInfo(pathPng)).Length;
            this.screenshotData.jpeg_size = (int)(new FileInfo(pathJpeg)).Length;

            var smallestPath = this.GetSmallestPicture(pathPng, pathJpeg);

            this.dataBytes = File.ReadAllBytes(smallestPath);

            this.DeleteImage(pathPng, pathJpeg);
        }

        private void CreateImage(string pathPng, string pathJpeg)
        {
            // Jpg
            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 90L);

            this.picture.Save(pathJpeg, jpgEncoder, myEncoderParameters);

            // Png
            this.picture.Save(pathPng, ImageFormat.Png);
        }

        private void DeleteImage(string pathPng, string pathJpeg)
        {
            File.Delete(pathPng);
            File.Delete(pathJpeg);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        public static Bitmap LoadImage(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path));
            return new Bitmap(Image.FromStream(ms));
        }

        public string GetSmallestPicture(string pathPng, string pathJpeg)
        {
            if (this.sizePng < 500000 || this.sizePng <= this.sizeJpeg)
                return pathPng;

            return pathJpeg;
        }

        public string GetSmallestFormat()
        {
            if (this.sizePng < 500000 || this.sizePng <= this.sizeJpeg)
                return "png";

            return "jpeg";
        }

        public string GetPictureType()
        {
            if (this.sizePng < 500000)
                return "png";

            else if (this.sizeJpeg < this.sizePng)
                return "jpeg";
            else
                return "png";
        }
    }
}
