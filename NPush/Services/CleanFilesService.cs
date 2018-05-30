using System;
using System.IO;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace NoelPush.Services
{
    public static class CleanFilesService
    {
        public static void RemoveOldVersion()
        {
            try
            {
                var pathInstallation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\NoelPush";
                RemoveLastVersion(pathInstallation);

                var pathStartup = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\NoelPush.lnk";
                if (File.Exists(pathStartup))
                    ResolveShortcut(pathStartup, pathInstallation);

                var pathStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.Startup).Replace("Startup", "NoelPush") + @"\NoelPush.lnk";
                if (File.Exists(pathStartMenu))
                    ResolveShortcut(pathStartMenu, pathInstallation);
            }
            catch (Exception e) { }
        }

        private static void RemoveLastVersion(string path)
        {
            for (var i = 1; i < 20; i++)
            {
                if (Directory.Exists(path + @"\app-" + i + ".0.0"))
                    DeleteDirectory(path + @"\app-" + i + ".0.0");

                if (Directory.Exists(path + @"\app-" + i + ".0.0.0"))
                    DeleteDirectory(path + @"\app-" + i + ".0.0.0");
            }

            if (File.Exists(path + @"\SquirrelSetup.log"))
                File.Delete(path + @"\SquirrelSetup.log");

            if (File.Exists(path + @"\Update.exe"))
                File.Delete(path + @"\Update.exe");
        }

        public static void DeleteDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
                DeleteDirectory(dir);

            Directory.Delete(path, false);
        }

        private static void ResolveShortcut(string pathShortcut, string pathInstallation)
        {
            var wsh = new WshShell();
            var shortcut = wsh.CreateShortcut(pathShortcut) as IWshShortcut;
            shortcut.Arguments = string.Empty;
            shortcut.TargetPath = pathInstallation + @"\app-20.0.0\NoelPush.exe";
            shortcut.WorkingDirectory = pathInstallation + @"\app-20.0.0";
            shortcut.IconLocation = pathInstallation + @"\app-20.0.0\NoelPush.exe,0";
            shortcut.Save();
        }
    }
}
