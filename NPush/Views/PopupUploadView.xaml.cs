using System.Windows.Input;
using NPush.ViewModels;

namespace NPush.Views
{
    public partial class PopupUploadView
    {
        public PopupUploadView()
        {
            this.InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupUploadViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
