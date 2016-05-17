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
            // Upload only if a jpeg has been uploaded
            if (screenshotData.Format == "png")
                return;

            this.userId = screenshotData.UserId;
            this.firstUrl = screenshotData.uRL;

            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NoelPush\Sending\";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += DateTime.Now.ToString(@"yyyy-MM-dd-HH\hmm\mss\s") + ".png";
            await CreatePng(picture, path);

            var dataBytes = File.ReadAllBytes(path);

            new UploaderService().Upload(this, path, dataBytes, path.Split('.').Last());
        }

        private async Task CreatePng(Image picture, string path)
        {
            picture.Save(path, ImageFormat.Png);
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
