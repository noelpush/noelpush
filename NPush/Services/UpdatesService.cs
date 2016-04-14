using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using NLog;
using Squirrel;


namespace NoelPush.Services
{
    public static class UpdatesService
    {
        private static Timer timerUpdates;
        public static bool FirstRun;

        private static string UserId;
        private static string Version;

        public static void Initialize(string userId, string version)
        {
            UserId = userId;
            Version = version;
            
            FirstRun = false;

            timerUpdates = new Timer();
            timerUpdates.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
            timerUpdates.Elapsed += CheckUpdate;
            timerUpdates.Enabled = true;
        }

        public static async void CheckUpdate(object sender = null, ElapsedEventArgs elapsed = null)
        {
            try
            {
                using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
                {
                    SquirrelAwareApp.HandleEvents(
                          onFirstRun: () => FirstRun = true);

                    var updates = await mgr.CheckForUpdate();

                    if (updates.ReleasesToApply.Any())
                    {
                        var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();

                        await mgr.DownloadReleases(updates.ReleasesToApply);
                        await mgr.ApplyReleases(updates);

                        var latestExe = Path.Combine(mgr.RootAppDirectory, string.Concat("app-", lastVersion.Version), "NoelPush.exe");
                        mgr.Dispose();

                        UpdateManager.RestartApp(latestExe);
                    }
                    mgr.Dispose();
                }

                StatisticsService.NewUpdate(UserId, Version);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }
        }

        public static async Task UpdateApp()
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
                }
                mgr.Dispose();
            }
        }

        public static void InstallEvent()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.CreateShortcutsForExecutable(appName, ShortcutLocation.StartMenu | ShortcutLocation.Startup | ShortcutLocation.AppRoot, false);
                mgr.CreateUninstallerRegistryEntry();
            }

        }

        public static void UpdateEvent()
        {
            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu | ShortcutLocation.Startup | ShortcutLocation.AppRoot, false, null, null);
            }
        }

        public static void UninstallEvent()
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
