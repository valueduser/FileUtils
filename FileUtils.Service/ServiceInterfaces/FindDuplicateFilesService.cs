using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO.Abstractions;
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

			FileHelpers fileHelpers = new FileHelpers(new FileSystem());
			DuplicateFinder dupeFinder = new DuplicateFinder(fileHelpers);
			dupeFinder.ValidateJob(job);
			ConcurrentDictionary<string, List<File>> duplicates =  dupeFinder.FindDuplicateFiles(job);
			dupeFinder.ReportResults(duplicates);

			//TODO: Persist

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
