using System;
using System.Windows.Forms;
using System.Drawing;

namespace NoelPush.Views.Tools
{
    public sealed class SelectorForm : Form
    {
        public bool CleanDraw;
        public Point Start { get; set; }
        public Point End { get; set; }
        private readonly Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1);
        private readonly SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

        private static object mutex = new object();
        private static SelectorForm instance;

        private SelectorForm()
        {
            this.DoubleBuffered = true;
            this.Paint += OnPaint;

            this.ShowInTaskbar = false;
            this.Size = new Size(1, 1);
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Cursor = Cursors.Cross;
            this.TopMost = true;
            this.Opacity = 0.2f;
            this.BackColor = Color.FromArgb(255, 255, 254);
            this.TransparencyKey = Color.FromArgb(255, 255, 254);
            this.StartPosition = FormStartPosition.Manual;
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
                    lock (mutex)
                    {
                        if (instance == null)
                            instance = new SelectorForm();
                    }
                }

                return instance;
            }
        }

        internal void Initialize()
        {
            this.Start = Point.Empty;
            this.End = Point.Empty;

            this.CreateGraphics().Clear(this.BackColor);

            this.CleanDraw = true;
            this.Refresh();
            this.CleanDraw = false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
            e.Graphics.FillRectangle(this.brush, this.getRectangle());
            e.Graphics.DrawRectangle(this.pen, this.getRectangle());

            if (this.CleanDraw)
                e.Graphics.Clear(this.BackColor);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle(
                Math.Min(this.Start.X, this.End.X),
                Math.Min(this.Start.Y, this.End.Y),
                Math.Abs(this.End.X - this.Start.X),
                Math.Abs(this.End.Y - this.Start.Y)
            );
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }
}
