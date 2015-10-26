using System;
using System.Windows.Forms;
using System.Drawing;

namespace NPush.Views.Tools
{
    sealed class SelectorForm : Form
    {
        public bool CleanDraw;
        public Point Start { get; set; }
        public Point End { get; set; }
        private readonly Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1);
        private readonly SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

        public SelectorForm()
        {
            this.DoubleBuffered = true;
            this.Paint += OnPaint;
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
