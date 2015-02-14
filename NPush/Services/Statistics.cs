using NPush.Objects;


namespace NPush.Services
{
    using System.Net.Http;

    class Statistics
    {

        public void StatsStart(string uniqueID, string version, string dotnets)
        {
            var url = "http://npush.noelpush.com/stats.php?startup&";
            url += "uid=" + uniqueID;
            url += "&ver=" + version;
            url += "&dotnets=" + dotnets;

            return;
            var response = new HttpClient().GetStringAsync(url);
        }

        public void StatsUpload(ScreenshotData screenData)
        {
            var url = "http://npush.noelpush.com/stats.php?";
            url += "uid=" + screenData.uniqueID;
            url += "&ver=" + screenData.version;
            url += "&size_png=" + screenData.sizePng;
            url += "&size_jpg=" + screenData.sizeJpg;
            url += "&timing=" + screenData.timing;
            url += "&mode=" + screenData.mode;

            return;
            var response = new HttpClient().GetStringAsync(url);
        }

        internal void StatsFail()
        {
            var url = "http://npush.noelpush.com/stats.php?fail";
            
            return;
            var response = new HttpClient().GetStringAsync(url);
        }
    }
}
