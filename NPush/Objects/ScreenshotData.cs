using System;
using System.Drawing;


namespace NoelPush.Objects
{
    public class ScreenshotData
    {
        public string UserId { get; set; }
        public string Version { get; set; }

        public string uRL { get; set; }
        public int Mode { get; set; }
        public string SentFormat { get; set; }

        public int PngSize { get; set; }
        public int JpegSize { get; set; }
        public Rectangle ImgSize { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime StartUpload { get; set; }
        public DateTime StopUpload { get; set; }
        public DateTime FirstPressDate { get; set; }
        public DateTime SecondPressDate { get; set; }
        public DateTime ThirdPressDate { get; set; }

        public ScreenshotData(string userId)
        {
            this.UserId = userId;
            this.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();
            this.FirstPressDate = DateTime.Now;
        }
    }
}
