using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using NoelPush.Models;
using NoelPush.Objects;

namespace NoelPush.Services
{
    internal class UploaderService
    {
        private readonly Logger logger;
        private readonly Manager manager;

        public UploaderService()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }

        public UploaderService(Manager manager) : this()
        {
            this.manager = manager;
        }

        // Upload first picture
        public void Upload(PictureData pictureData)
        {
            var namePicture = new Random().Next(0, 9999).ToString("0000") + "-" + Properties.Resources.NamePicture + "." + pictureData.GetSmallerFormat();
            this.UploadHttpClient(pictureData, namePicture);
        }

        private async void UploadHttpClient(PictureData pictureData, string namePicture, bool retry = true)
        {
            pictureData.screenshotData.StartUpload = DateTime.Now;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);

                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(new MemoryStream(pictureData.DataBytes)), "fichier", namePicture);

                        using (var message = await client.PostAsync("http://www.noelshack.com/api.php", content))
                        {
                            var reponse = await message.Content.ReadAsStringAsync();

                            if (!reponse.Contains("http://www.noelshack.com"))
                            {
                                if (retry)
                                    this.UploadHttpClient(pictureData, namePicture, false);
                                else
                                    this.manager.UploadFailed();

                                return;
                            }

                            pictureData.screenshotData.StopUpload = DateTime.Now;
                            this.manager.Uploaded(pictureData.Picture, this.CustomUrl(reponse), pictureData.screenshotData, false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.Error(e.Message);

                if (e.Message == "A task was canceled.")
                {
                    this.manager.UploadPictureTooLarge();
                }
                else
                {
                    if (retry)
                        this.UploadHttpClient(pictureData, namePicture, false);
                    else
                        this.manager.ConnexionFailed();
                }
            }
        }

        // Upload second picture
        public void Upload(DualFormatService dualFormatService, string path, byte[] pictureData, string format)
        {
            var namePicture = new Random().Next(0, 9999).ToString("0000") + "-" + Properties.Resources.NamePicture + "." + format;
            this.UploadHttpClient(dualFormatService, path, pictureData, namePicture);
        }

        private async void UploadHttpClient(DualFormatService dualFormatService, string path, byte[] pictureData, string namePicture)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);

                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(new MemoryStream(pictureData)), "fichier", namePicture);

                        using (var message = await client.PostAsync("http://www.noelshack.com/api.php", content))
                        {
                            var reponse = await message.Content.ReadAsStringAsync();

                            if (!reponse.Contains("http://www.noelshack.com"))
                            {
                                return;
                            }

                            dualFormatService.Uploaded(path, this.CustomUrl(reponse));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.Error(e.Message);
            }
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        private string CustomUrl(string url)
        {
            /* http://www.noelshack.com/2017-23-5-1497015462-a.png
             * http://image.noelshack.com/fichiers/2017/23/5/1497015462-a.png */

            var Pattern = new Regex(@"www\.noelshack\.com\/(\d+)-(\d+)-(\d+)-(.+)");
            return Pattern.Replace(url, "image.noelshack.com/fichiers/$1/$2/$3/$4");
        }
    }
}
