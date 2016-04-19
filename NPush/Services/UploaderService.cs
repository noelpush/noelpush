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

        public UploaderService(Manager manager)
        {
            this.logger = LogManager.GetCurrentClassLogger();
            this.manager = manager;
        }

        public void Upload(PictureData pictureData)
        {
            var namePicture = new Random().Next(0, 9999).ToString("0000") + "-" + Properties.Resources.NamePicture + "." + pictureData.GetSmallestFormat();
            this.UploadHttpClient(pictureData, namePicture);
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size * nmemb;
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

        private string CustomUrl(string url)
        {
            /* http://www.noelshack.com/2015-02-1420740001-noelpush.png
             * http://image.noelshack.com/fichiers/2015/02/1420740001-noelpush.png */

            var Pattern = new Regex(@"www\.noelshack\.com\/(\d+)-(\d+)-(.+)");
            return Pattern.Replace(url, "image.noelshack.com/fichiers/$1/$2/$3");
        }
    }
}
