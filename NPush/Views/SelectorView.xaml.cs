using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using NPush.Services;
using NPush.ViewModels;

namespace NPush.Views
{
    internal partial class SelectorView
    {
        public ScreenCapture ScreenCapture;
        public SelectorForm selectorForm;

        public SelectorView(ScreenCapture screenCapture)
        {
            this.DataContext = new SelectorViewModel();
            this.ScreenCapture = screenCapture;

            this.selectorForm = new SelectorForm
            {
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                Width = Screen.AllScreens.Sum(screen => screen.WorkingArea.Width),
                Height = Screen.AllScreens.Select(screen => screen.WorkingArea.Height).Concat(new[] { 0 }).Max(),
                Left = 0,
                Top = 0,
                TopMost = true,
                Cursor = Cursors.Cross
            };

            this.selectorForm.MouseDown += OnMouseDown;
            this.selectorForm.MouseMove += OnMouseMove;
            this.selectorForm.MouseUp += OnMouseUp;
            this.selectorForm.KeyDown += OnKeyDown;

            InitializeComponent();
        }

        public void Showing()
        {
            this.selectorForm.Initialize();
            this.selectorForm.Show();
        }

        internal void Hiding()
        {
            this.selectorForm.Hide();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.selectorForm.Start = new Point(e.X,e.Y);
            this.selectorForm.End = new Point(e.X, e.Y);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            this.selectorForm.End = new Point(e.X, e.Y);
            this.selectorForm.Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            this.selectorForm.Hide();
            this.ScreenCapture.BuildImg(this.selectorForm.getRectangle());
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.selectorForm.Hide();
        }

        internal class SelectorForm : Form
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            private readonly Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1);
            private readonly SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

            internal void Initialize()
            {
                this.CreateGraphics().Clear(this.BackColor);
                this.Start = Point.Empty;
                this.End = Point.Empty;
            }

            public SelectorForm()
            {
                this.Opacity = 0.2f;
                this.DoubleBuffered = true;
                this.BackColor = Color.FromArgb(255, 255, 254);
                this.TransparencyKey = this.BackColor;

                this.Paint += OnPaint;
            }

            private void OnPaint(object sender, PaintEventArgs e)
            {
                e.Graphics.Clear(this.BackColor);
                e.Graphics.FillRectangle(brush, this.getRectangle());
                e.Graphics.DrawRectangle(pen, this.getRectangle());
            }

            public Rectangle getRectangle()
            {
                return new Rectangle(Math.Min(this.Start.X, this.End.X), Math.Min(this.Start.Y, this.End.Y), Math.Abs(this.End.X - this.Start.X), Math.Abs(this.End.Y - this.Start.Y));
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
            }
        }
    }

}

