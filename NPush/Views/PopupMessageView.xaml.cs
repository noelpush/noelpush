using System.Windows.Input;
using NPush.ViewModels;

namespace NPush.Views
{
    public partial class PopupMessageView
    {
        public PopupMessageView()
        {
            InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupMessageViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
