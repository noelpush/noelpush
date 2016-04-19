using System;
using System.Collections.Generic;
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

        private static string userId;
        private static string version;

        public static Action RestartAppEvent;

        public static void Initialize(string id, string v)
        {
            userId = id;
            version = v;

            FirstRun = false;

            timerUpdates = new Timer();
            timerUpdates.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            timerUpdates.Elapsed += CheckUpdate;
            timerUpdates.Enabled = true;

            CheckUpdate();
        }

        public static void CheckUpdate(object sender = null, ElapsedEventArgs elapsed = null)
        {
            const string url = "https://noelpush.com/check_update";

            var values = new Dictionary<string, string>
            {
                { "uid", userId },
                { "current_version", version }
            };

            RequestService.SendRequest(url, values).ContinueWith(Update);
        }

        private static async void Update(Task<string> result)
        {

          if (result.Result == null || result.Result != "1")
                return;

            try
            {
                using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
                {
                    SquirrelAwareApp.HandleEvents(onFirstRun: () => FirstRun = true);

                    var updates = await mgr.CheckForUpdate();

                    if (updates.ReleasesToApply.Any())
                    {
                        var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();

                        await mgr.DownloadReleases(updates.ReleasesToApply);
                        await mgr.ApplyReleases(updates);

                        var latestExe = Path.Combine(mgr.RootAppDirectory, string.Concat("app-", lastVersion.Version), "NoelPush.exe");
                        mgr.Dispose();

                        RestartAppEvent();
                        UpdateManager.RestartApp(latestExe);
                    }
                    mgr.Dispose();
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }
        }

        public static void InstallEvent()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            var appName = Path.GetFileName(exePath);

            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.CreateShortcutsForExecutable(appName, ShortcutLocation.StartMenu | ShortcutLocation.Startup, false);
                mgr.CreateUninstallerRegistryEntry();
                mgr.Dispose();
            }
        }

        public static void UpdateEvent()
        {
            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.CreateShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu | ShortcutLocation.Startup, false);
                mgr.Dispose();
            }
        }

        public static void UninstallEvent()
        {
            using (var mgr = new UpdateManager(@"https://releases.noelpush.com/", "NoelPush"))
            {
                mgr.RemoveShortcutsForExecutable("NoelPush.exe", ShortcutLocation.StartMenu);
                mgr.RemoveShortcutsForExecutable("NoelPush.exe", ShortcutLocation.Startup);
                mgr.RemoveUninstallerRegistryEntry();
                mgr.Dispose();
            }
        }
    }
}
