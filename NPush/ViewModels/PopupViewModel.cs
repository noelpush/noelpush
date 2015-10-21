using System.Drawing;
using Itron.Mcn.Utils.Wpf;

namespace NPush.ViewModels
{
    class PopupViewModel
    {
        public ViewElement<Bitmap> Picture { get; private set; }
        public ViewElement<bool> IsOpen { get; private set; }

        public PopupViewModel()
        {
            this.Picture = new ViewElement<Bitmap>();
            this.IsOpen = new ViewElement<bool>();
            //this.IsOpen.Value = false;
        }

        public void ShowPopup()
        {
            //this.IsOpen.Value = true;
        }

        public void HidePopup()
        {
            //this.IsOpen.Value = false;
        }
    }
}
