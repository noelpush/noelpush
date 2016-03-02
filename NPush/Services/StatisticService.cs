using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using NLog;
using NoelPush.Objects;
using System.Net.Http;

namespace NoelPush.Services
{
    static class StatisticService
    {
        public static void StatUpload(ScreenshotData screenData)
        {
            var url = "https://www.noelpush.com/add_upload";

            var values = new Dictionary<string, string>
            {
                { "uid", screenData.UserId },
                { "version", screenData.Version },
                { "url", screenData.uRL },
                { "mode", screenData.Mode.ToString(CultureInfo.InvariantCulture) },
                { "png_filesize", screenData.PngSize.ToString(CultureInfo.InvariantCulture) },
                { "jpeg_filesize", screenData.JpegSize.ToString(CultureInfo.InvariantCulture) },
                { "width", screenData.ImgSize.Width.ToString(CultureInfo.InvariantCulture) },
                { "height", screenData.ImgSize.Height.ToString(CultureInfo.InvariantCulture) },
                { "upload_delay",  ((int)(screenData.StopUpload - screenData.StartUpload).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
                { "total_delay",  ((int)(screenData.StopUpload - screenData.StartDate).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
                { "second_press_delay", (screenData.SecondPressDate == DateTime.MinValue ? -1 : (int)(screenData.SecondPressDate - screenData.FirstPressDate).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
                { "third_press_delay", (screenData.ThirdPressDate == DateTime.MinValue ? -1 : (int)(screenData.ThirdPressDate - screenData.SecondPressDate).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) },
            };

            SendRequest(url, values);
        }

        public static bool NewUpdate(string userId, string version)
        {
            var url = "https://stats.noelpush.com/check_update";

            var values = new Dictionary<string, string>
            {
                { "uid", userId },
                { "current_version", version }
            };

            return true;
            var answer = SendRequest(url, values);
            return answer.Result == "1";
        }

        private static async Task<string> SendRequest(string url, Dictionary<string, string> values)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(url, content);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }

            return string.Empty;
        }
    }
}
