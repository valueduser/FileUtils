using System;
using System.Collections.Specialized;
using System.Configuration;

namespace FileUtils
{
    public interface IAppConfigManager
    {
        AppConfig SafeGetAppConfigs();
    }

    public class AppConfigManager
    {
        private NameValueCollection _appSettings;
        private ConnectionStringSettingsCollection _dbSettings;
        public AppConfig SafeGetAppConfigs() 
		{
			try
			{
				_appSettings = ConfigurationManager.AppSettings;
                _dbSettings = ConfigurationManager.ConnectionStrings;
			}
			catch (ConfigurationErrorsException ex)
			{
				Console.WriteLine($"Unable to read from configurations - {ex}");
			}

			if (_appSettings == null || _appSettings.Count == 0)
			{
				throw new System.ArgumentException("Unable to retrieve configuration values from the app.config file. ");
			}

			bool isLocal = false;

            return new AppConfig()
            {
                User = _appSettings["NetworkShareUser"] ?? "User Not Found",
                Pass = _appSettings["NetworkSharePassword"] ?? "Password Not Found",
                Domain = _appSettings["NetworkShareDomain"] ?? "Domain Not Found",
                Path = _appSettings["NetworkShareUncPath"] ?? "UNC Path Not Found",
                IsLocalFileSystem = bool.TryParse(_appSettings["IsLocalFileSystem"], out isLocal),
                DBConnectionString = _dbSettings["ConnectionStrings"].ConnectionString
            };
		}
    }
}
