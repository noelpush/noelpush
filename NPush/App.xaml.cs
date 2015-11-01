using System;
using System.Threading;
using System.Windows;
using NLog;

namespace NoelPush
{
    public partial class App
    {
        private Logger logger;
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            this.logger = LogManager.GetCurrentClassLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (!IsSingleInstance())
            {
                Environment.Exit(1);
            }

            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.logger.Error(e.ExceptionObject.ToString);
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
                Mutex.OpenExisting("NOELPUSH");
            }
            catch
            {
                mutex = new Mutex(true, "NOELPUSH");
                return true;
            }

            return false;
        }
    }
}
