using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using NPush.Models;

namespace NPush.Services
{
    internal class Uploader
    {
        private Stopwatch ChronoUpload;

        private Manager manager;

        private string namePicture;
        private string boundary;
        private byte[] boundaryBytes;
        private byte[] headerBytes;

        public Uploader(Manager manager)
        {
            this.ChronoUpload = new Stopwatch();

            this.manager = manager;

            this.namePicture = new Random().Next(0, 9999) .ToString("0000") + "-" + Properties.Resources.NamePicture;
            this.boundary = "------WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            this.boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            this.headerBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"fichier\"; filename=\"" + this.namePicture + ".png\"\r\nContent-Type: image/png\r\n\r\n");
        }

        public void Upload(Bitmap bmp)
        {
            this.manager.Uploaded(bmp, 0);
        }

        public void Upload(Bitmap img, byte[] imgBytes)
        {
            ChronoUpload.Restart();

          //this.UploadWebClient(imgBytes);
            this.UploadHttpWebRequest(img, imgBytes);
        }

        private void UploadHttpWebRequest(Bitmap img, byte[] formBytes)
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
                        reponse = e.Message;
                    }
                }, request);

                ChronoUpload.Stop();
                this.manager.Uploaded(img, this.CustomUrl(reponse), ChronoUpload.ElapsedMilliseconds);
            }
            catch (WebException e)
            {
                this.manager.UploadFailed();
            }
        }

        private string CustomUrl(string url)
        {
            /* Faut que je me trouve une regex lulz
             * http://www.noelshack.com/2015-02-1420740001-jvpush.png
             * http://image.noelshack.com/fichiers/2015/43/1445454562-npush.png */

            url = url.Replace("www", "image");
            url = url.Replace(".com", ".com/fichiers");
            url = url.Replace("-", "/");
            url = url.Replace("/" + this.namePicture.Replace("-", "/"), "-" + this.namePicture);
            return url;
        }
    }
}
