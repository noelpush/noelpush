using System;
using NLog;
using NoelPush.Objects;

namespace NoelPush.Services
{
    using System.Net.Http;

    static class Statistics
    {
        public static void Send(ScreenshotData screenData)
        {
            // http://choco.ovh/npush/stats.php?path=&version=&mode=&png=&jpg=

            var url = "http://choco.ovh/npush/stats.php?";

            url += "path=" + screenData.path;
            url += "&version=" + screenData.version;
            url += "&mode=" + screenData.mode;
            url += "&png=" + screenData.png_size;
            url += "&jpg=" + screenData.jpg_size;

            SendRequest(url);
        }

        private static void SendRequest(string url)
        {
            try
            {
                new HttpClient().GetStringAsync(url);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }
        }
    }
}
