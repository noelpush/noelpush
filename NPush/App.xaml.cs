// ReSharper disable RedundantArgumentName

using System;
using System.Threading;
using System.Windows;
using NoelPush.Services;

namespace NoelPush
{
    public partial class App
    {
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CommandService.IsShellMode && !IsSingleInstance())
            {
                Environment.Exit(1);
            }

            CleanFilesService.RemoveOldVersion();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
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
