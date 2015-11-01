using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Squirrel;


namespace NPush.Services
{
    class UpdatesManager
    {
        private readonly Logger logger;
        public bool FirstRun;

        public UpdatesManager()
        {
            this.FirstRun = false;
            this.logger = LogManager.GetCurrentClassLogger();
        }

        public async void CheckUpdate()
        {
            try 
            {
                #if (DEBUG)
                    return;
                #endif

                using (var mgr = new UpdateManager(@"http://choco.ovh/npush/releases/", "NoelPush"))
                {
                    SquirrelAwareApp.HandleEvents(
                          onFirstRun: () => FirstRun = true,
                          onInitialInstall: v => mgr.CreateShortcutForThisExe(),
                          onAppUpdate: v => mgr.CreateShortcutForThisExe(),
                          onAppUninstall: v => mgr.RemoveShortcutForThisExe());

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
