using System.Drawing;
using NPush.Objects.ViewModel;

namespace NPush.ViewModels
{
    public class PopupViewModel
    {
        public ViewElement<Bitmap> Picture { get; private set; }
        public ViewElement<bool> IsOpen { get; private set; }

        public PopupViewModel()
        {
            this.Picture = new ViewElement<Bitmap>();
            this.IsOpen = new ViewElement<bool> { Value = false };
        }

        public void ShowPopup(Bitmap img)
        {
            this.Picture.Value = img;
            this.IsOpen.Value = true;
        }

        public void HidePopup()
        {
            this.IsOpen.Value = false;
        }
    }
}
