using System;
using System.IO;
using System.Linq;
using System.Timers;
using NLog;
using Squirrel;


namespace NoelPush.Services
{
    class UpdatesManager
    {
        private readonly Timer timerUpdates;
        private readonly Logger logger;
        public bool FirstRun;

        public UpdatesManager()
        {
            this.FirstRun = false;
            this.logger = LogManager.GetCurrentClassLogger();

            this.timerUpdates = new Timer();
            this.timerUpdates.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
            this.timerUpdates.Elapsed += this.CheckUpdate;
            this.timerUpdates.Enabled = true;
        }

        public async void CheckUpdate(object sender = null, ElapsedEventArgs elapsed = null)
        {
            try 
            {
                using (var mgr = new UpdateManager(@"http://choco.ovh/npush/releases/", "NoelPush"))
                {
                    SquirrelAwareApp.HandleEvents(
                          onFirstRun: () => this.FirstRun = true);

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
            }
            catch (Exception e)
            {
                this.logger.Error(e.Message);
            }
        }
    }
}
