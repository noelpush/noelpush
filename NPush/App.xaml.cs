using System;
using System.Threading;
using System.Windows;

namespace NPush
{
    public partial class App
    {
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!IsSingleInstance())
            {
                Environment.Exit(1);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mutex.ReleaseMutex();

            base.OnExit(e);
        }

        static bool IsSingleInstance()
        {
            try
            {
                Mutex.OpenExisting("NPUSH");
            }
            catch
            {
                mutex = new Mutex(true, "NPUSH");
                return true;
            }

            return false;
        }
    }
}
