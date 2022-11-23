using System;
using System.Collections.Specialized;
using System.Configuration;

namespace AdminServer
{
    public class SettingsManager 
    {
        public static string ReadSetting(string key)
        {
            try
            {
                NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;
                return appSettings[key] ?? string.Empty;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                return string.Empty;
            }
        }
    }
}