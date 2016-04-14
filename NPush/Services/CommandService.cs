using System;
using System.IO;


namespace NoelPush.Services
{
    public static class CommandService
    {
        public static bool IsShellMode
        {
            get
            {
                var args = Environment.GetCommandLineArgs();
                return args.Length == 3 && args[1] == "--file" && !string.IsNullOrEmpty(args[2]);
            }
        }

        public static string GetFileName
        {
            get
            {
                var args = Environment.GetCommandLineArgs();
                return File.Exists(args[2]) ? args[2] : string.Empty;
            }
        }
    }
}
