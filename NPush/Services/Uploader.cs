using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
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

        private string namePicture;
        private string boundary;
        private byte[] boundaryBytes;
        private byte[] headerBytes;

        public Uploader(Manager manager)
        {
            this.logger = LogManager.GetCurrentClassLogger();

            this.manager = manager;

            this.namePicture = new Random().Next(0, 9999) .ToString("0000") + "-" + Properties.Resources.NamePicture;
            this.boundary = "------WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            this.boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            this.headerBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"fichier\"; filename=\"" + this.namePicture + ".png\"\r\nContent-Type: image/png\r\n\r\n");
        }

        public void Upload(Bitmap bmp)
        {
            this.manager.Uploaded(bmp);
        }

        public void Upload(Bitmap img, byte[] imgBytes, ScreenshotData data)
        {
            var ChronoUpload = new Stopwatch();
            ChronoUpload.Start();
            var url = this.UploadHttpWebRequest(img, imgBytes);
            data.path = url;
            ChronoUpload.Stop();
            data.upload_delay = (int)ChronoUpload.ElapsedMilliseconds;


            Statistics.Send(data);
        }

        private string UploadHttpWebRequest(Bitmap img, byte[] formBytes)
        {
            try
            {
                var reponse = "Upload failed";

                var request = (HttpWebRequest) WebRequest.Create("http://www.noelshack.com/api.php");
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.GetRequestStream().Write(boundaryBytes, 0, boundaryBytes.Length);
                request.GetRequestStream().Write(headerBytes, 0, headerBytes.Length);
                request.GetRequestStream().Write(formBytes, 0, formBytes.Length);
                request.GetRequestStream().Write(boundaryBytes, 0, boundaryBytes.Length);
                request.GetRequestStream().Close();

                request.BeginGetResponse(r =>
                {
                    try
                    {
                        var httpRequest = (HttpWebRequest) r.AsyncState;
                        var httpResponse = (HttpWebResponse) httpRequest.EndGetResponse(r);
                        reponse = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    }
                    catch (Exception e)
                    {
                        this.logger.Error(e.Message);
                        reponse = e.Message;
                    }
                }, request);

                this.manager.Uploaded(img, this.CustomUrl(reponse));

                return reponse;
            }
            catch (WebException e)
            {
                this.logger.Error(e.Message);
                this.manager.UploadFailed();
            }

            return string.Empty;
        }

        private string CustomUrl(string url)
        {
            /* http://www.noelshack.com/2015-02-1420740001-noelpush.png
             * http://image.noelshack.com/fichiers/2015/02/1420740001-noelpush.png */

            url = url.Replace("www", "image");
            url = url.Replace(".com", ".com/fichiers");
            url = url.Replace("-", "/");
            url = url.Replace("/" + this.namePicture.Replace("-", "/"), "-" + this.namePicture);
            return url;
        }
    }
}
