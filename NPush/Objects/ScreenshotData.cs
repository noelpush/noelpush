using System;
using System.Drawing;
using NoelPush.Properties;

namespace NoelPush.Objects
{
    public class ScreenshotData
    {
        public string version { get; set; }
        public DateTime start_date { get; set; }
        public int second_press_delay { get; set; }
        public int third_press_delay { get; set; }
        public int upload_delay { get; set; }
        public string path { get; set; }
        public Rectangle img_size { get; set; }
        public int png_size { get; set; }
        public int jpg_size { get; set; }
        public int mode { get; set; }

        public ScreenshotData()
        {
            this.version = Resources.Version;
        }
    }
}
