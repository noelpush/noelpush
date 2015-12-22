using System;
using Microsoft.Win32;
using NLog;

namespace NoelPush.Services
{
    static class Registry
    {
        public static string GetUserIdInRegistry()
        {
            const string REGISTRY_KEY = @"HKEY_CURRENT_USER\SOFTWARE\NoelPush";
            const string REGISTY_VALUE = "ID";

            try
            {
                var Key = Microsoft.Win32.Registry.GetValue(REGISTRY_KEY, REGISTY_VALUE, 0) as string;
                if (Key != null)
                {
                    return Key;
                }
                else
                {
                    return WriteUserIdInRegistry();
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }

            return "Undefined";
        }

        private static string WriteUserIdInRegistry()
        {
            const string REGISTRY_FIRST_KEY = @"HKEY_CURRENT_USER\SOFTWARE\";
            string REGISTY_FIRST_VALUE = "NoelPush";

            const string REGISTRY_SECOND_KEY = @"HKEY_CURRENT_USER\SOFTWARE\NoelPush";
            string REGISTY_SECOND_VALUE = "ID";

            string REGISTY_STRING = GenerateID();

            if (Convert.ToInt32(Microsoft.Win32.Registry.GetValue(REGISTRY_FIRST_KEY, REGISTY_FIRST_VALUE, 0)) != 0)
                return "Undefined";

            Microsoft.Win32.Registry.SetValue(REGISTRY_FIRST_KEY, REGISTY_FIRST_VALUE, 0, RegistryValueKind.String);
            Microsoft.Win32.Registry.SetValue(REGISTRY_SECOND_KEY, REGISTY_SECOND_VALUE, REGISTY_STRING, RegistryValueKind.String);

            return REGISTY_STRING;
        }

        public static string GenerateID()
        {
            var random = new Random();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string password = string.Empty;

            for (int i = 0; i < 32; i++)
            {
                password += alphabet[random.Next(0, alphabet.Length) - 1];
            }

            return password;
        }
    }
}
