namespace NoelPush.Objects
{
    class ScreenshotData
    {
        public string uniqueID { get; private set; }
        public string version { get; private set; }
        public int mode { get; private set; }
        public long sizePng { get; set; }
        public long sizeJpg { get; set; }
        public long timing { get; set; }

        public ScreenshotData(string uniqueID, string version, int mode)
        {
            this.uniqueID = uniqueID;
            this.version = version;
            this.mode = mode;
        }
    }
}
