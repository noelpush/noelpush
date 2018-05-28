using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
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

                            this.manager.Uploaded(pictureData.Picture, this.CustomUrl(reponse), false);
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
            /* http://www.noelshack.com/2017-23-5-1497015462-a.png
             * http://image.noelshack.com/fichiers/2017/23/5/1497015462-a.png */

            var Pattern = new Regex(@"www\.noelshack\.com\/(\d+)-(\d+)-(\d+)-(.+)");
            return Pattern.Replace(url, "image.noelshack.com/fichiers/$1/$2/$3/$4");
        }
    }
}
