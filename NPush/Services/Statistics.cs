using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using NoelPush.Objects;

namespace NoelPush.Services
{
    using System.Net.Http;

    static class Statistics
    {
        public static void Send(ScreenshotData screenData)
        {
            var url = "http://stats.noelpush.com/upload";

            var values = new Dictionary<string, string>
            {
                { "uid", screenData.userId },
                { "version", screenData.version },
                { "url", screenData.url },
                { "mode", screenData.mode.ToString(CultureInfo.InvariantCulture) },
                { "png_filesize", screenData.png_size.ToString(CultureInfo.InvariantCulture) },
                { "jpeg_filesize", screenData.jpeg_size.ToString(CultureInfo.InvariantCulture) },
                { "width", screenData.img_size.Width.ToString(CultureInfo.InvariantCulture) },
                { "height", screenData.img_size.Height.ToString(CultureInfo.InvariantCulture) },
                { "upload_delay",  ((int)(screenData.stop_upload - screenData.start_upload).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
                { "total_delay",  ((int)(screenData.stop_upload - screenData.start_date).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
                { "second_press_delay", (screenData.second_press_date == DateTime.MinValue ? -1 : (int)(screenData.second_press_date - screenData.first_press_date).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
                { "third_press_delay", (screenData.third_press_date == DateTime.MinValue ? -1 : (int)(screenData.third_press_date - screenData.second_press_date).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
            };

            SendRequest(url, values);
        }

        private static async void SendRequest(string url, Dictionary<string, string> values)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }
        }
    }
}
