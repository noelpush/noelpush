using System;
using System.IO;

using Microsoft.Win32;
using NLog;

using Squirrel;


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
            var executablePath = AppDomain.CurrentDomain.BaseDirectory + "\\NoelPush.exe";
            if (!File.Exists(executablePath))
                return;

            var registryPath = string.Format("HKEY_CURRENT_USER\\Software\\Classes\\{0}\\shell\\NoelPush", type);

            string registyFirstName = "";
            string registyFirstValue = "Envoyer sur NoelShack";

            string registySecondName = "Icon";
            string registySecondValue = executablePath;

            string registyFourthName = "";
            string registyFourthValue = executablePath + " --file \"%1\"";

            Registry.SetValue(registryPath, registyFirstName, registyFirstValue, RegistryValueKind.String);
            Registry.SetValue(registryPath, registySecondName, registySecondValue, RegistryValueKind.String);
            Registry.SetValue(registryPath + "\\command", registyFourthName, registyFourthValue, RegistryValueKind.String);
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
