using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NoelPush.Objects;
using NoelPush.Services;

namespace NoelPush.Models
{
    public class DualFormatService
    {
        private string userId;
        private string firstUrl;

        public void Upload(Bitmap picture, ScreenshotData screenshotData)
        {
            UploadTask(picture, screenshotData);
        }

        public async void UploadTask(Bitmap picture, ScreenshotData screenshotData)
        {
            this.userId = screenshotData.UserId;
            this.firstUrl = screenshotData.uRL;

            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NoelPush\Sending\";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (screenshotData.SentFormat == "png")
            {
                path += DateTime.Now.ToString(@"yyyy-MM-dd-HH\hmm\mss\s") + ".jpeg";
                await CreateJpeg(picture, path);
            }
            else
            {
                path += DateTime.Now.ToString(@"yyyy-MM-dd-HH\hmm\mss\s") + ".png";
                await CreatePng(picture, path);
            }

            var dataBytes = File.ReadAllBytes(path);

            new UploaderService().Upload(this, path, dataBytes, path.Split('.').Last());
        }

        private async Task CreateJpeg(Image picture, string path)
        {
            var jpgEncoder = this.GetEncoder(ImageFormat.Jpeg);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 90L);

            picture.Save(path, jpgEncoder, myEncoderParameters);
        }

        private async Task CreatePng(Image picture, string path)
        {
            picture.Save(path, ImageFormat.Png);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        public async void Uploaded(string path, string secondUrl)
        {
            var jpegUrl = this.firstUrl.Contains(".jpeg") ? this.firstUrl : secondUrl;
            var pngUrl = this.firstUrl.Contains(".png") ? this.firstUrl : secondUrl;

            var result = await StatisticsService.AddPngVersion(this.userId, jpegUrl, pngUrl);

            if (File.Exists(path) && result)
                File.Delete(path);
        }
    }
}
