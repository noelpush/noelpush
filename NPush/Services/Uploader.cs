using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using NLog;
using NoelPush.Models;
using NoelPush.Objects;

namespace NoelPush.Services
{
    internal class Uploader
    {
        private readonly Logger logger;
        private Manager manager;

        public Uploader(Manager manager)
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

        private async void UploadHttpClient(PictureData pictureData, string namePicture)
        {
            pictureData.screenshotData.start_upload = DateTime.Now;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);

                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(new MemoryStream(pictureData.dataBytes)), "fichier", namePicture);

                        using (var message = await client.PostAsync("http://www.noelshack.com/api.php", content))
                        {
                            var reponse = await message.Content.ReadAsStringAsync();

                            if (!reponse.Contains("http://www.noelshack.com"))
                            {
                                this.manager.UploadFailed();
                                return;
                            }

                            pictureData.screenshotData.stop_upload = DateTime.Now;
                            this.manager.Uploaded(pictureData.picture, this.CustomUrl(reponse, namePicture), pictureData.screenshotData, false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.Error(e.Message);

                if (e.Message == "A task was canceled.")
                    this.manager.UploadPictureTooLarge();
                else
                    this.manager.ConnexionFailed();
            }
        }

        private string CustomUrl(string url, string namePicture)
        {
            /* http://www.noelshack.com/2015-02-1420740001-noelpush.png
             * http://image.noelshack.com/fichiers/2015/02/1420740001-noelpush.png */

            url = url.Replace("www", "image");
            url = url.Replace(".com", ".com/fichiers");
            url = url.Replace("-", "/");
            url = url.Replace("/" + namePicture.Replace("-", "/"), "-" + namePicture);
            return url;
        }
    }
}
