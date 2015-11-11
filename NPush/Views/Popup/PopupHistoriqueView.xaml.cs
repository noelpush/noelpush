using System.Windows.Input;
using NoelPush.ViewModels.Popup;

namespace NoelPush.Views.Popup
{
    public partial class PopupHistoriqueView
    {
        public PopupHistoriqueView()
        {
            InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupHistoriqueViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
