using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
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

            this.namePicture = Properties.Resources.NamePicture;
            this.boundary = "------WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            this.boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            this.headerBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"fichier\"; filename=\"" + this.namePicture + ".png\"\r\nContent-Type: image/png\r\n\r\n");
        }

        public void Upload(Bitmap bmp)
        {
            this.manager.Uploaded(bmp, 0);
        }

        public void Upload(byte[] imgBytes)
        {
            ChronoUpload.Restart();

          //this.UploadWebClient(imgBytes);
          this.UploadHttpWebRequest(imgBytes);
        }

        private void UploadHttpWebRequest(byte[] formBytes)
        {
            var reponse = "Upload failed";

            var request = (HttpWebRequest)WebRequest.Create("http://www.noelshack.com/api.php");
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
                    var httpRequest = (HttpWebRequest)r.AsyncState;
                    var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                    reponse = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                }
                catch (Exception e)
                {
                    reponse = e.Message;
                }
            }, request);
            
            ChronoUpload.Stop();
            this.manager.Uploaded(this.CustomUrl(reponse), ChronoUpload.ElapsedMilliseconds);
        }

        private void UploadWebClient(byte[] formBytes)
        {
            byte[] param = new byte[boundaryBytes.Length + headerBytes.Length + formBytes.Length + boundaryBytes.Length];
            Buffer.BlockCopy(boundaryBytes, 0, param, 0, boundaryBytes.Length);
            Buffer.BlockCopy(headerBytes, 0, param, boundaryBytes.Length, headerBytes.Length);
            Buffer.BlockCopy(formBytes, 0, param, boundaryBytes.Length + headerBytes.Length, formBytes.Length);
            Buffer.BlockCopy(boundaryBytes, 0, param, boundaryBytes.Length + headerBytes.Length + formBytes.Length, boundaryBytes.Length);

            var webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "multipart/form-data; boundary=" + boundary);
            webClient.UploadDataAsync(new Uri("http://www.noelshack.com/api.php"), param);
            webClient.UploadDataCompleted += WebClientOnUploadDataCompleted;
        }

        private void WebClientOnUploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            ChronoUpload.Stop();
            var url = CustomUrl(Encoding.UTF8.GetString(e.Result));

            if (!string.IsNullOrEmpty(url))
                this.manager.Uploaded(url, ChronoUpload.ElapsedMilliseconds);
            else
                this.manager.UploadFailed();
        }

        private string CustomUrl(string url)
        {
            /* Faut que je me trouve une regex lulz
             * http://www.noelshack.com/2015-02-1420740001-jvpush.png
             * http://image.noelshack.com/fichiers/2015/02/1420740001-jvpush.png */

            url = url.Replace("www", "image");
            url = url.Replace(".com", ".com/fichiers");
            url = url.Replace("-", "/");
            url = url.Replace("/" + this.namePicture, "-" + this.namePicture);
            return url;
        }
    }
}
