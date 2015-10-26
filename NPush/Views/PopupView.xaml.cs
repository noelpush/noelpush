using System.Windows;
using System.Windows.Input;
using NPush.ViewModels;

namespace NPush.Views
{
    public partial class PopupView
    {
        public PopupView()
        {
            InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
