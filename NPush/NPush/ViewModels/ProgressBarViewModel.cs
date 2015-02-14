
namespace NPush.ViewModels
{
    class ProgressBarViewModel
    {
        private static readonly object locker = new object();
        private static ProgressBarViewModel instance;

        //public ViewElement<int> ProgressBarValue { get; set; }
    
        private ProgressBarViewModel()
        {
            //ProgressBarValue = new ViewElement<int>();
        }

        public static ProgressBarViewModel Instance
        {
            get
            {
                if (instance != null) return instance;

                lock (locker)
                {
                    if (instance == null)
                        instance = new ProgressBarViewModel();
                }

                return instance;
            }
        }
    }
}
