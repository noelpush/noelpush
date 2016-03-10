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
            if (!IsSingleInstance())
            {
                Environment.Exit(1);
            }

            this.logger = LogManager.GetCurrentClassLogger();

            try
            {
                using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
                {
                    SquirrelAwareApp.HandleEvents(
                        onInitialInstall: v => this.InstallEvent(mgr),
                        onAppUpdate: v => this.UpdateEvent(mgr),
                        onAppUninstall: v => this.UninstallEvent(mgr));
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
            }

            var userId = RegistryService.GetUserIdFromRegistry();
            var version = NoelPush.Properties.Resources.Version;

            StatisticsService.NewUpdate(userId, version);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

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
        

        private void InstallEvent(UpdateManager mgr)
        {
            mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu, false);
            mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.Startup, false);
            mgr.CreateUninstallerRegistryEntry();
        }

        private void UpdateEvent(UpdateManager mgr)
        {
            mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu, true);
            mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.Startup, true);
            mgr.RemoveUninstallerRegistryEntry();
            mgr.CreateUninstallerRegistryEntry();
        }

        private void UninstallEvent(UpdateManager mgr)
        {
            mgr.RemoveShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu);
            mgr.RemoveShortcutsForExecutable("NoelPush.exe", ShortcutLocation.Startup);
            mgr.RemoveUninstallerRegistryEntry();
        }
    }
}
