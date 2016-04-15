// ReSharper disable RedundantArgumentName

using System;
using System.Threading;
using System.Windows;
using NLog;
using NoelPush.Services;
using Squirrel;

namespace NoelPush
{
    public partial class App
    {
        private Logger logger;
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CommandService.IsShellMode && !IsSingleInstance())
            {
                Environment.Exit(1);
            }

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: v => UpdatesService.InstallEvent(),
                onAppUpdate: v => UpdatesService.UpdateEvent(),
                onAppUninstall: v => UpdatesService.UninstallEvent()
            );

            this.logger = LogManager.GetCurrentClassLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.logger.Error(e.ExceptionObject.ToString);
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
