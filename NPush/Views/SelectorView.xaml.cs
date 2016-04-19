using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

using NoelPush.Views.Tools;

using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;


namespace NoelPush.Views
{
    public partial class SelectorView
    {
        public static void Initialize()
        {
            SelectorForm.Instance.Initialize();
        }

        public static Rectangle Showing(Bitmap background)
        {
            SelectorForm.Instance.MouseDown += OnMouseDown;
            SelectorForm.Instance.MouseMove += OnMouseMove;
            SelectorForm.Instance.MouseUp += OnMouseUp;
            SelectorForm.Instance.KeyDown += OnKeyDown;

            SelectorForm.Instance.Initialize(background);
            SelectorForm.Instance.ShowDialog();

            return SelectorForm.Instance.GetRectangle();
        }

        internal static void Hiding()
        {
            SelectorForm.Instance.MouseDown -= OnMouseDown;
            SelectorForm.Instance.MouseMove -= OnMouseMove;
            SelectorForm.Instance.MouseUp -= OnMouseUp;
            SelectorForm.Instance.KeyDown -= OnKeyDown;

            SelectorForm.Instance.Hide();
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            SelectorForm.Instance.Start = new Point(e.X, e.Y);
            SelectorForm.Instance.End = new Point(e.X, e.Y);
            SelectorForm.Instance.Draw();
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SelectorForm.Instance.End = new Point(e.X, e.Y);
                ClipCursor();
            }

            SelectorForm.Instance.Draw();
        }

        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            SelectorForm.Instance.End = new Point(e.X + 1, e.Y + 1);

            Hiding();
        }

        private static void ClipCursor()
        {
            var currentScreen = Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            System.Windows.Forms.Cursor.Clip = currentScreen.Bounds;
        }

        // Cancel action if escape is pressed
        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                SelectorForm.Instance.Start = Point.Empty;
                SelectorForm.Instance.End = Point.Empty;
                Hiding();
            }

            // Tempo colorize
            else if (e.KeyCode == Keys.Add)
            {
                SelectorForm.Instance.Colorize(5);
            }
            else if (e.KeyCode == Keys.Subtract)
            {
                SelectorForm.Instance.Colorize(-5);
            }
        }
    }
}

