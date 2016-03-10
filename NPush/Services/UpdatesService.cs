using System;
using System.IO;
using System.Linq;
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
    }
}
