using System.Windows.Input;
using NoelPush.ViewModels.Popup;

namespace NoelPush.Views.Popup
{
    public partial class PopupFirstRunView
    {
        public PopupFirstRunView()
        {
            InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupFirstRunViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
