using System.Drawing;

namespace NoelPush.Objects
{
    public class PictureData
    {
        public Bitmap bitmapPng { get; private set; }
        public Bitmap bitmapJpeg { get; private set; }

        public int sizePng { get; private set; }
        public int sizeJpeg { get; private set; }

        public PictureData(Bitmap bitmapPng, Bitmap bitmapJpeg, int sizePng, int sizeJpeg)
        {
            this.bitmapPng = bitmapPng;
            this.bitmapJpeg = bitmapJpeg;

            this.sizePng = sizePng;
            this.sizeJpeg = sizeJpeg;
        }

        public Bitmap GetSmallestPicture()
        {
            if (this.sizePng < 500000)
                return this.bitmapPng;
            else if (this.sizeJpeg < this.sizePng)
                return this.bitmapJpeg;
            else 
                return this.bitmapPng;
        }

        public string GetPictureType()
        {
            if (this.sizePng < 500000)
                return "png";

            else if (this.sizeJpeg < this.sizePng)
                return "jpeg";
            else
                return "png";
        }
    }
}
