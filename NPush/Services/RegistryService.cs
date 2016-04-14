using System;
using Microsoft.Win32;
using NLog;

namespace NoelPush.Services
{
    static class RegistryService
    {
        public static string GetUserId()
        {
            const string REGISTRY_KEY = @"HKEY_CURRENT_USER\SOFTWARE\NoelPush";
            const string REGISTY_VALUE = "ID";

            try
            {
                var Key = Registry.GetValue(REGISTRY_KEY, REGISTY_VALUE, 0) as string;
                return Key ?? WriteUserId();
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }

            return "Undefined";
        }

        private static string WriteUserId()
        {
            const string REGISTRY_FIRST_KEY = @"HKEY_CURRENT_USER\SOFTWARE\";
            const string REGISTY_FIRST_VALUE = "NoelPush";

            const string REGISTRY_SECOND_KEY = @"HKEY_CURRENT_USER\SOFTWARE\NoelPush";
            const string REGISTY_SECOND_VALUE = "ID";

            var REGISTY_STRING = GenerateID();

            if (Convert.ToInt32(Registry.GetValue(REGISTRY_FIRST_KEY, REGISTY_FIRST_VALUE, 0)) != 0)
                return "Undefined";

            Registry.SetValue(REGISTRY_FIRST_KEY, REGISTY_FIRST_VALUE, 0, RegistryValueKind.String);
            Registry.SetValue(REGISTRY_SECOND_KEY, REGISTY_SECOND_VALUE, REGISTY_STRING, RegistryValueKind.String);

            return REGISTY_STRING;
        }

        public static void WriteShell()
        {
            WriteShell("pngfile"); 
            WriteShell("jpegfile");
        }

        private static void WriteShell(string type)
        {
            var REGISTRY_PATH = string.Format("HKEY_CLASSES_ROOT\\{0}\\shell\\NoelPush", type);

            const string REGISTY_FIRST_NAME = "";
            const string REGISTY_FIRST_VALUE = "Héberger avec NoelPush";

            const string REGISTY_SECOND_NAME = "Icon";
            const string REGISTY_SECOND_VALUE = @"C:\Users\choco\Documents\GitHub\NoelPush\Output\Debug\NoelPush.exe";

            const string REGISTY_THIRD_NAME = "Position";
            const string REGISTY_THIRD_VALUE = "top";

            const string REGISTY_FOURTH_NAME = "";
            const string REGISTY_FOURTH_VALUE = "\"C:\\Users\\Choco\\Documents\\GitHub\\NoelPush\\Output\\Debug\\NoelPush.exe\" --file \"%1\"";

            Registry.SetValue(REGISTRY_PATH, REGISTY_FIRST_NAME, REGISTY_FIRST_VALUE, RegistryValueKind.String);
            Registry.SetValue(REGISTRY_PATH, REGISTY_SECOND_NAME, REGISTY_SECOND_VALUE, RegistryValueKind.String);
            Registry.SetValue(REGISTRY_PATH, REGISTY_THIRD_NAME, REGISTY_THIRD_VALUE, RegistryValueKind.String);
            Registry.SetValue(REGISTRY_PATH + "\\command", REGISTY_FOURTH_NAME, REGISTY_FOURTH_VALUE, RegistryValueKind.String);
        }

        public static string GenerateID()
        {
            var random = new Random();
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var password = string.Empty;

            for (var i = 0; i < 32; i++)
            {
                password += alphabet[random.Next(0, alphabet.Length) - 1];
            }

            return password;
        }
    }
}
