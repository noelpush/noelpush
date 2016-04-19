using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using NoelPush.Properties;
using NoelPush.Services;

using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;


namespace NoelPush.Views.Tools
{
    public sealed class SelectorForm : Form
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        private readonly Pen pen = new Pen(Color.FromArgb(255, Color.DimGray), 1);
        private readonly SolidBrush brush = new SolidBrush(Color.FromArgb(130, Color.Black));

        private Bitmap background;
        private static BufferedGraphics buffer;

        private static readonly object Mutex = new object();
        private static SelectorForm instance;

        private SelectorForm()
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Top = 0;
            this.Left = 0;
            this.TopMost = true;
            using (var cursorStream = new MemoryStream(Resources.crosscustom))
                this.Cursor = new Cursor(cursorStream);

            this.Shown += this.Draw;
        }

        public void Initialize()
        {
            this.Left = MonitorService.VirtualLeft;
            this.Top = MonitorService.VirtualTop;
            this.Width = MonitorService.VirtualWidth;
            this.Height = MonitorService.VirtualHeight;
        }

        public static SelectorForm Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (Mutex)
                    {
                        if (instance == null)
                            instance = new SelectorForm();
                    }
                }

                return instance;
            }
        }

        internal void Initialize(Bitmap background)
        {
            this.background = background;

            var rectangle = new Rectangle(0, MonitorService.VirtualTop, MonitorService.VirtualWidth, MonitorService.VirtualHeight);
            buffer = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), rectangle);

            this.Start = Point.Empty;
            this.End = Point.Empty;

            this.Focus();
        }

        public void Draw()
        {
            var rectangle = this.GetRectangle();

            buffer.Graphics.Clear(Color.Transparent);
            buffer.Graphics.DrawImage(this.background, Point.Empty);

            var region = new Region();

            region.Xor(rectangle);

            buffer.Graphics.FillRegion(brush, region);
            buffer.Graphics.DrawRectangle(pen, rectangle);

            //DrawCoordinates();

            buffer.Render();
        }

        private void DrawCoordinates()
        {
            var position = MousePosition;

            if (position.Y > Height - 30) position.Y -= 50;

            buffer.Graphics.DrawString("X: " + position.X, new Font("Verdana", 9), Brushes.Black, position.X + 1 - 40, position.Y + 20 + 1);
            buffer.Graphics.DrawString("X: " + position.X, new Font("Verdana", 9), Brushes.White, position.X - 40, position.Y + 20);
            buffer.Graphics.DrawString("Y: " + position.Y, new Font("Verdana", 9), Brushes.Black, position.X + 15 + 1, position.Y + 20 + 1);
            buffer.Graphics.DrawString("Y: " + position.Y, new Font("Verdana", 9), Brushes.White, position.X + 15, position.Y + 20);

            var brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            buffer.Graphics.FillRectangle(brush, position.X - 60, position.Y + 15, 150, 20);
        }

        private void Draw(object sender, EventArgs eventArgs)
        {
            this.Draw();
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle(
                Math.Min(Start.X, End.X),
                Math.Min(Start.Y, End.Y),
                Math.Abs(End.X - Start.X),
                Math.Abs(End.Y - Start.Y)
            );
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        // Tempo colorize
        public void Colorize(int alpha)
        {
            alpha += this.brush.Color.A;

            if (alpha < 0) alpha = 0;
            if (alpha > 255) alpha = 255;

            this.brush.Color = Color.FromArgb(alpha, 0, 0, 0);
            this.Draw();

            var label = new Label();
            label.Text = alpha.ToString();
            label.ForeColor = Color.FromArgb(255, 190, 0, 0);
            label.Width = 30;
            label.Height = 15;

            this.Controls.Clear();
            this.Controls.Add(label);
        }
    }
}
