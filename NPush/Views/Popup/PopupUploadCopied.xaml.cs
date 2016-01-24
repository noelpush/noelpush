﻿using System.Windows.Input;
using NoelPush.ViewModels;

namespace NoelPush.Views.Popup
{
    public partial class PopupCopyView
    {
        public PopupCopyView()
        {
            this.InitializeComponent();
        }

        private void PopupView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = this.DataContext as PopupViewModel;

            if (dataContext != null)
                dataContext.IsOpen.Value = false;
        }
    }
}
