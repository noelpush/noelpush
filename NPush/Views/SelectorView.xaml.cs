using System.Drawing;
using System.Windows.Forms;
using NoelPush.Objects;
using NoelPush.Services;
using NoelPush.Views.Tools;

namespace NoelPush.Views
{
    internal partial class SelectorView
    {
        private bool upload;
        private ScreenshotData data;
        public ScreenCapture ScreenCapture;
        public SelectorForm selectorForm;
        public CursorForm cursorForm;

        public SelectorView(ScreenCapture screenCapture, Rectangle area)
        {
            this.ScreenCapture = screenCapture;

            this.selectorForm = new SelectorForm
            {
                ShowInTaskbar = false,
                Size = new Size(1, 1),
                WindowState =  FormWindowState.Normal,
                FormBorderStyle = FormBorderStyle.None,
                Left = area.Left,
                Top = area.Top,
                Width = area.Width,
                Height = area.Height,
                Cursor = Cursors.Cross,
                TopMost = true,
                Opacity = 0.2f,
                BackColor = Color.FromArgb(255, 255, 254),
                TransparencyKey = Color.FromArgb(255, 255, 254),
                StartPosition = FormStartPosition.Manual
            };

            this.selectorForm.MouseDown += OnMouseDown;
            this.selectorForm.MouseMove += OnMouseMove;
            this.selectorForm.MouseUp += OnMouseUp;
            this.selectorForm.KeyDown += OnKeyDown;

            this.InitializeComponent();
        }

        public void Showing(ScreenshotData data, bool upload)
        {
            this.upload = upload;
            this.data = data;
            this.selectorForm.Initialize();
            this.selectorForm.Show();
        }

        internal void Hiding()
        {
            this.selectorForm.CleanDraw = true;
            this.selectorForm.Refresh();
            this.selectorForm.CleanDraw = false;

            this.selectorForm.Hide();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.selectorForm.Start = new Point(e.X, e.Y);
            this.selectorForm.End = new Point(e.X, e.Y);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //this.selectorForm.UpdateCursor(e.X, e.Y);

            if (e.Button != MouseButtons.Left) return;

            this.selectorForm.End = new Point(e.X, e.Y);
            this.selectorForm.Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            this.Hiding();
            this.ScreenCapture.BuildImg(this.selectorForm.getRectangle(), this.data, this.upload);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Hiding();
        }

        public void Connect(int connectionId, object target) {}
    }

}

