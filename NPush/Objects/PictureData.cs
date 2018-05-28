using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace NoelPush.Objects
{
    public class PictureData
    {
        private int PngSize;
        private int JpegSize;
        private string Format;

        public byte[] DataBytes { get; private set; }
        public Bitmap Picture { get; private set; }

        public PictureData(Bitmap img)
        {
            this.Picture = img;

            this.Initialize();
        }

        private void Initialize()
        {
            var pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NoelPush\";
            var fileName = DateTime.Now.ToString(@"yyyy-MM-dd-HH-mm-ss");

            var pathPng = pathFolder + fileName + ".png";
            var pathJpeg = pathFolder + fileName + ".jpeg";

            this.CreateImage(pathPng, pathJpeg);

            this.PngSize = (int)(new FileInfo(pathPng)).Length;
            this.JpegSize = (int)(new FileInfo(pathJpeg)).Length;
            this.Format = this.GetSmallerFormat();

            var smallestPath = this.GetSmallerPicture(pathPng, pathJpeg);

            this.DataBytes = File.ReadAllBytes(smallestPath);

            this.DeleteImage(pathPng, pathJpeg);
        }

        private void CreateImage(string pathPng, string pathJpeg)
        {
            // Jpg
            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 90L);

            this.Picture.Save(pathJpeg, jpgEncoder, myEncoderParameters);

            // Png
            this.Picture.Save(pathPng, ImageFormat.Png);
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

        public string GetSmallerPicture(string pathPng, string pathJpeg)
        {
            if (this.PngSize <= this.JpegSize)
                return pathPng;

            return pathJpeg;
        }

        public string GetSmallerFormat()
        {
            if (this.PngSize <= this.JpegSize)
                return "png";

            return "jpeg";
        }
    }
}
