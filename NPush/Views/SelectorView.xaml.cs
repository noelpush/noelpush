using System.Drawing;
using System.Windows.Forms;
using NoelPush.Views.Tools;

namespace NoelPush.Views
{
    public static partial class SelectorView
    {
        public static void Initialize(Rectangle area)
        {
            SelectorForm.Instance.Initialize(area);

            SelectorForm.Instance.MouseDown += OnMouseDown;
            SelectorForm.Instance.MouseMove += OnMouseMove;
            SelectorForm.Instance.MouseUp += OnMouseUp;
            SelectorForm.Instance.KeyDown += OnKeyDown;
        }

        public static Rectangle Showing(Bitmap background)
        {
            SelectorForm.Instance.Initialize(background);
            SelectorForm.Instance.ShowDialog();

            return SelectorForm.Instance.GetRectangle();
        }

        internal static void Hiding()
        {
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
            if (e.Button != MouseButtons.Left) return;

            SelectorForm.Instance.End = new Point(e.X, e.Y);
            SelectorForm.Instance.Draw();
        }

        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Hiding();
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
        }

        public static void Connect(int connectionId, object target) { }
    }

}

