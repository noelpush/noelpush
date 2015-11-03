using System;
using System.Drawing;
using NoelPush.Properties;

namespace NoelPush.Objects
{
    public class ScreenshotData
    {
        public string userId { get; set; }
        public string version { get; set; }

        public string url { get; set; }
        public int mode { get; set; }

        public int png_size { get; set; }
        public int jpeg_size { get; set; }
        public Rectangle img_size { get; set; }

        public DateTime start_date { get; set; }
        public DateTime start_upload { get; set; }
        public DateTime stop_upload { get; set; }
        public DateTime first_press_date { get; set; }
        public DateTime second_press_date { get; set; }
        public DateTime third_press_date { get; set; }

        public ScreenshotData(string userId)
        {
            this.userId = userId;
            this.version = Resources.Version;
            this.first_press_date = DateTime.Now;
        }
    }
}
