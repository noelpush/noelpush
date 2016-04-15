using System;
using System.Collections.Generic;
using System.Globalization;
using NoelPush.Objects;

namespace NoelPush.Services
{
    static class StatisticsService
    {
        public static async void StatUpload(ScreenshotData screenData)
        {
            const string url = "https://noelpush.com/add_upload";

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

            await RequestService.SendRequest(url, values);
        }
    }
}
