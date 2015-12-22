using System;
using System.Windows.Forms;
using System.Drawing;

namespace NoelPush.Views.Tools
{
    sealed class SelectorForm : Form
    {
        public bool CleanDraw;
        public Point Start { get; set; }
        public Point End { get; set; }
        private readonly Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1);
        private readonly SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

        private Label LabelX;
        private Label LabelY;
        
        public SelectorForm()
        {
            this.DoubleBuffered = true;
            this.Paint += OnPaint;

            this.LabelX = new Label();
            this.LabelX.ForeColor = Color.Black;
            this.LabelX.BackColor = Color.Transparent;
            this.LabelX.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            this.LabelY = new Label();
            this.LabelY.ForeColor = Color.Black;
            this.LabelY.BackColor = Color.Transparent;
            this.LabelY.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            this.Controls.Add(this.LabelX);
            this.Controls.Add(this.LabelY);
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

        public void UpdateCursor(int x, int y)
        {
            this.LabelX.Text = x.ToString();
            this.LabelX.Location = new Point(x + 10, y + 20);

            this.LabelY.Text = y.ToString();
            this.LabelY.Location = new Point(x + 10, y);
        }
    }
}
