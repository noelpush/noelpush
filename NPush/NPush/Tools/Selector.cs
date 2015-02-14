using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NPush.Tools
{

    public class Selector : Window
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public bool isMouseDown = false;

        public Canvas cnv = new Canvas();

        public Selector()
        {
            this.PreviewMouseMove += OnMouseMove;
            this.PreviewMouseDown += OnMouseDown;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Title = "Selector";
            this.Opacity = .2;
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = new SolidColorBrush(Colors.Black);
            this.Cursor = Cursors.Cross;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
            this.x = e.GetPosition(null).X;
            this.y = e.GetPosition(null).Y;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isMouseDown) return;

            double curx = e.GetPosition(null).X;
            double cury = e.GetPosition(null).Y;

            var r = new System.Windows.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(145, 0, 0)),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.White),
                Width = Math.Abs(curx - this.x),
                Height = Math.Abs(cury - this.y)
            };

            cnv.Children.Clear();
            cnv.Children.Add(r);
            Canvas.SetLeft(r, this.x);
            Canvas.SetTop(r, this.y);

            if (e.LeftButton == MouseButtonState.Released)
            {
                cnv.Children.Clear();
                this.width = e.GetPosition(null).X - this.x;
                this.height = e.GetPosition(null).Y - this.y;
                this.CaptureScreen(this.x, this.y, this.width, this.height);
                this.x = this.y = 0;
                this.isMouseDown = false;
                this.Hide();
            }
        }

        public void CaptureScreen(double x, double y, double width, double height)
        {
            int ix, iy, iw, ih;
            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);
            var image = new Bitmap(iw, ih, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(ix, iy, ix, iy, new System.Drawing.Size(iw, ih), CopyPixelOperation.SourceCopy);

            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            image.Save(pathDesktop + @"\screen.png", ImageFormat.Png);
        }
    }
}
