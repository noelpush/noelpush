using System.Drawing;
using NoelPush.Objects.ViewModel;

namespace NoelPush.ViewModels.Popup
{
    public class PopupUploadViewModel : PopupViewModel
    {
        public ViewElement<Bitmap> Picture { get; private set; }

        public PopupUploadViewModel()
        {
            this.Height = 118;
            this.Width = 323;

            this.Picture = new ViewElement<Bitmap>();
        }

        public void ShowPopup(Bitmap img, int delay = 3000)
        {
            this.Picture.Value = img;
            this.ShowPopup(delay);
        }
    }
}
