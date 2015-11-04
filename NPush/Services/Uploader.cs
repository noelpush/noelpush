using System;
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

        public Uploader(Manager manager, string format)
        {
            this.logger = LogManager.GetCurrentClassLogger();

            this.manager = manager;

            this.namePicture = new Random().Next(0, 9999) .ToString("0000") + "-" + Properties.Resources.NamePicture;
            this.boundary = "------WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            this.boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            this.headerBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"fichier\"; filename=\"" + this.namePicture + "." + format + "\"\r\nContent-Type: image/" + format + "\r\n\r\n");
            var a = "Content-Disposition: form-data; name=\"fichier\"; filename=\"" + this.namePicture + "." + format + "\"\r\nContent-Type: image/" + format;
        }

        public void Upload(Bitmap bmp)
        {
            this.manager.Uploaded(bmp);
        }

        public void Upload(Bitmap img, byte[] imgBytes, ScreenshotData screenshotData)
        {
            this.UploadHttpWebRequest(img, imgBytes, screenshotData);
            //this.UploadFile(@"C:\Users\choco\Pictures\png3.png");
        }

        bool UploadFile(string strFileName)
        {
            System.Net.CredentialCache MyCredentialCache;
            MyCredentialCache = new System.Net.CredentialCache();
            HttpWebResponse Response = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://www.noelshack.com/api.php");
                req.Timeout = -1;
                req.Proxy = null;
                req.Method = "POST";
                req.AllowWriteStreamBuffering = true;
                req.ContentType = "multipart/form-data; boundary=" + this.boundary;
                Stream reqStream = req.GetRequestStream();
                FileStream loFile = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
                byte[] lcfile = new byte[loFile.Length];

                reqStream.Write(this.boundaryBytes, 0, this.boundaryBytes.Length);
                reqStream.Write(this.headerBytes, 0, this.headerBytes.Length);
                reqStream.Write(lcfile, 0, (int)loFile.Length);
                reqStream.Write(this.boundaryBytes, 0, this.boundaryBytes.Length);
                reqStream.Close();
                req.Credentials = MyCredentialCache;

                Response = (HttpWebResponse)req.GetResponse();
                Response.Close();
                return true;
            }
            catch (Exception ex)
            {
                    return false;
            } 
        }

        private void UploadHttpWebRequest(Bitmap img, byte[] formBytes, ScreenshotData screenshotData)
        {
            try
            {
                bool error = false;
                var start_upload = new DateTime();
                var stop_upload = new DateTime();

                var reponse = "Upload failed";

                ServicePointManager.DefaultConnectionLimit = int.MaxValue;

                var request = (HttpWebRequest)WebRequest.Create("http://www.noelshack.com/api.php");
                request.Proxy = null;
                request.AllowWriteStreamBuffering = true;
                request.Timeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
                request.ContentType = "multipart/form-data; boundary=" + this.boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.GetRequestStream().Write(this.boundaryBytes, 0, this.boundaryBytes.Length);
                request.GetRequestStream().Write(this.headerBytes, 0, this.headerBytes.Length);
                request.GetRequestStream().Write(formBytes, 0, formBytes.Length);
                request.GetRequestStream().Write(this.boundaryBytes, 0, this.boundaryBytes.Length);
                request.GetRequestStream().Close();

                start_upload = DateTime.Now;

                request.BeginGetResponse(r =>
                {
                    try
                    {
                        var httpRequest = (HttpWebRequest) r.AsyncState;
                        var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                        reponse = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                        stop_upload = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        stop_upload = DateTime.Now;
                        this.logger.Error(e.Message);
                        reponse = e.Message;
                        error = true;
                    }
                }, request);


                screenshotData.start_upload = start_upload;
                screenshotData.stop_upload = stop_upload;

                this.manager.Uploaded(img, this.CustomUrl(reponse), screenshotData, error);
            }
            catch (WebException e)
            {
                this.logger.Error(e.Message);
                this.manager.ConnexionFailed();
            }
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
