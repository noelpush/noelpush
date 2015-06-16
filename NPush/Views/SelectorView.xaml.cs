using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using NPush.Services;
using NPush.ViewModels;
using NPush.Views.Tools;

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
                Cursor = Cursors.Cross,
                ToClean = false,
                TopMost = true
            };

            this.selectorForm.MouseDown += OnMouseDown;
            this.selectorForm.MouseMove += OnMouseMove;
            this.selectorForm.MouseUp += OnMouseUp;

            this.InitializeComponent();
        }

        public void Showing()
        {
            this.selectorForm.KeyDown += OnKeyDown;
            this.selectorForm.Initialize();
            this.selectorForm.Show();
        }

        internal void Hiding()
        {
            this.selectorForm.Hide();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.selectorForm.Start = new Point(e.X, e.Y);
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

            this.Hiding();
            this.ScreenCapture.BuildImg(this.selectorForm.getRectangle());
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Hiding();
        }

    }

}

