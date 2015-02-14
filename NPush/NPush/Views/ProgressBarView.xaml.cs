using System.Linq;
using System.Windows;
using System.Windows.Forms;

using NPush.ViewModels;


namespace NPush.Views
{
    internal partial class ProgressBarView
    {
        public ProgressBarView()
        {
            this.InitializeComponent();
            this.DataContext = ProgressBarViewModel.Instance;

            this.Width = Screen.PrimaryScreen.Bounds.Width;
        }

        public void SetValue(int value)
        {
            this.ProgressUpload.Value = value;

            //if (!IsActive) this.Show();
            this.Visibility = value < 100 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
