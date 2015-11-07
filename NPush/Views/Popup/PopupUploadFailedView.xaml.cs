using System.Windows.Input;
using NoelPush.ViewModels.Popup;

namespace NoelPush.Views.Popup
{
    public partial class PopupUploadFailedView
    {
        public PopupUploadFailedView()
        {
            this.InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupConnexionFailedViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
