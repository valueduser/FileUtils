using System;
using System.Collections.Specialized;
using System.Configuration;
using FileUtil.Core;
using FileUtil.Models;

namespace FileUtil.Service.ServiceInterfaces
{
	public interface IFindDuplicateFilesService
	{
		void FindDuplicates();
	}

	public class FindDuplicateFilesService
	{
		public void FindDuplicates()
		{
			//Read app config options or //todo console selections
			FindDuplicatesJob job = new FindDuplicatesJob(SafeGetAppConfigs());
			
			//Call into core
			FileUtil.Core.DuplicateFinder dupeFinder = new DuplicateFinder();
			dupeFinder.ValidateJob(job);
			dupeFinder.FindDuplicateFiles(job);
			Console.ReadKey();
		}

		private NameValueCollection SafeGetAppConfigs()
		{
			NameValueCollection appSettings = new NameValueCollection();
			try
			{
				appSettings = ConfigurationManager.AppSettings;
			}
			catch (ConfigurationErrorsException ex)
			{
				Console.WriteLine($"Unable to read from configurations. {ex.Message}");
			}

			if (appSettings == null || appSettings.Count == 0)
			{
				throw new System.ArgumentException("Unable to retrieve configuration values from the app.config file. ");
			}

			return appSettings;
		}
	}
}
