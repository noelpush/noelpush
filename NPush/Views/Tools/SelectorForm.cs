using System;
using System.Windows.Forms;
using System.Drawing;
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
        private BufferedGraphics buffer;

        private static readonly object Mutex = new object();
        private static SelectorForm instance;

        private SelectorForm()
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Top = 0;
            this.Left = 0;
            this.Cursor = Cursors.Cross;
            this.TopMost = true;

            this.Shown += this.Draw;
        }

        public void Initialize(Rectangle area)
        {
            this.Left = area.Left;
            this.Top = area.Top;
            this.Width = area.Width;
            this.Height = area.Height;
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

            this.buffer = BufferedGraphicsManager.Current.Allocate(
                this.CreateGraphics(),
                new Rectangle(Left, Top, Width, Height)
                );

            this.Start = Point.Empty;
            this.End = Point.Empty;

            this.Focus();
        }

        public void Draw()
        {
            var rectangle = this.GetRectangle();

            this.buffer.Graphics.Clear(Color.Transparent);
            this.buffer.Graphics.DrawImage(this.background, Point.Empty);

            var region = new Region();

            region.Xor(rectangle);

            this.buffer.Graphics.FillRegion(brush, region);
            this.buffer.Graphics.DrawRectangle(pen, rectangle);

            this.buffer.Render();
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

            var label = new System.Windows.Forms.Label();
            label.Text = alpha.ToString();
            label.ForeColor = Color.FromArgb(255, 190, 0, 0);
            label.Width = 30;
            label.Height = 15;

            this.Controls.Clear();
            this.Controls.Add(label);
        }
    }
}
