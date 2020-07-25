using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO.Abstractions;
using FileUtil.Core;
using FileUtil.Models;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace FileUtil.Service.ServiceInterfaces
{
	public interface IFindDuplicateFilesService
	{
		void FindDuplicates();
	}

	public class FindDuplicateFilesService
	{
		private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
		private static IServiceProvider _serviceProvider;

		public void FindDuplicates()
		{
			//Read app config options or //todo console selections
			FindDuplicatesJob job = new FindDuplicatesJob(SafeGetAppConfigs());

			//FileHelpers fileHelpers = new FileHelpers(new FileSystem());
			//DuplicateFinder dupeFinder = new DuplicateFinder(fileHelpers);
			//dupeFinder.ValidateJob(job);
			//ConcurrentDictionary<string, List<File>> duplicates =  dupeFinder.FindDuplicateFiles(job);
			//dupeFinder.ReportResults(duplicates);


			RegisterServices();
			IServiceScope scope = _serviceProvider.CreateScope();
			scope.ServiceProvider.GetRequiredService<DuplicateFinder>().ValidateJob(job);
			ConcurrentDictionary<string, List<File>> duplicates = scope.ServiceProvider.GetRequiredService<DuplicateFinder>().FindDuplicateFiles(job);
			scope.ServiceProvider.GetRequiredService<DuplicateFinder>().ReportResults(duplicates);
			DisposeServices();


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

		private static void RegisterServices()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IFileSystem, FileSystem>();
			services.AddSingleton<IFileHelpers, FileHelpers>();
			services.AddSingleton<DuplicateFinder>();

			_serviceProvider = services.BuildServiceProvider(true);
		}

		private static void DisposeServices()
		{
			if (_serviceProvider == null)
			{
				return;
			}

			if (_serviceProvider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}
