// ReSharper disable RedundantArgumentName

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => InstallEvent(),
                    onAppUpdate: v => UpdateEvent(),
                    onAppUninstall: v => UninstallEvent()
                );

                UpdateApp();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
            }

            var userId = RegistryService.GetUserId();
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

        public async Task UpdateApp()
        {
            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                var updates = await mgr.CheckForUpdate();
                if (updates.ReleasesToApply.Any())
                {
                    var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();
                    await mgr.DownloadReleases(new[] { lastVersion });
                    await mgr.ApplyReleases(updates);
                    await mgr.UpdateApp();

                    MessageBox.Show("The application has been updated - please close and restart.");
                }
                else
                {
                    MessageBox.Show("No Updates are available at this time.");
                }
            }
        }

        private static void InstallEvent()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.CreateShortcutsForExecutable(appName, ShortcutLocation.StartMenu | ShortcutLocation.Startup | ShortcutLocation.AppRoot, false);
                mgr.CreateUninstallerRegistryEntry();
            }

        }

        private static void UpdateEvent()
        {
            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu | ShortcutLocation.Startup | ShortcutLocation.AppRoot, false, null, null);
            }
        }

        private static void UninstallEvent()
        {
            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.RemoveShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu);
                mgr.RemoveShortcutsForExecutable("NoelPush.exe", ShortcutLocation.Startup);
                mgr.RemoveUninstallerRegistryEntry();
            }
        }
    }
}
