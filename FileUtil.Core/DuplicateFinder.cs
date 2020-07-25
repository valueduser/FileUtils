using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileUtil.Models;
using Konsole;
using File = FileUtil.Models.File;
using NLog;

namespace FileUtil.Core
{
	public interface IDuplicateFinder
	{
		// TODO: Update
		ConcurrentDictionary<string, List<File>> FindDuplicateFiles(FindDuplicatesJob job);

		string[] GetFilePaths(FindDuplicatesJob job);

		List<File> PopulateFileMetaData(string[] files);
	}

	public class DuplicateFinder
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
		private readonly IFileHelpers _fileSystemHelper;
		//private readonly Logger _logger;
		private int _hashLimit;

		public DuplicateFinder(IFileHelpers fileSystemHelper)
		{
			_fileSystemHelper = fileSystemHelper;
			//_logger = LogManager.GetCurrentClassLogger();
		}

		public ConcurrentDictionary<string, List<File>> FindDuplicateFiles(FindDuplicatesJob job)
		{
			string[] filePaths;
			ConcurrentDictionary<string, List<File>> duplicateDictionary = new ConcurrentDictionary<string, List<File>>();
			_hashLimit = job.Options.HashLimit;
			if (job.Options.IsLocalFileSystem)
			{
				filePaths = GetFilePaths(job);
				PopulateFileMetaData(filePaths, duplicateDictionary);
			}
			else
			{
				using UNCAccessWithCredentials.UNCAccessWithCredentials unc =
					new UNCAccessWithCredentials.UNCAccessWithCredentials();
				if (unc.NetUseWithCredentials(job.Options.Path, job.Options.User, job.Options.Domain, job.Options.Pass)
					|| unc.LastError == 1219) // Already connected
				{
					filePaths = GetFilePaths(job);
					PopulateFileMetaData(filePaths, duplicateDictionary);

				}
				else
				{
					Logger.Error($"Failed to connect to UNC location. Error: {unc.LastError}.");
					switch (unc.LastError)
					{
						case 1326:
							Logger.Error("Login failure: The user name or password is incorrect.");
							break;
						case 86:
							Logger.Error("Access denied: The specified network password is not correct.");
							break;
						case 87:
							Logger.Error("Invalid parameter.");
							break;
						case 1219:
							Logger.Error("Multiple connections to server.");
							unc.Dispose();
							break;
						case 53:
							Logger.Error("Network path not found.");
							break;
						case 5:
							Logger.Error("Access denied.");
							break;
						default:
							Logger.Error($"Unknown error. {unc.LastError}");
							break;
					}
					Console.ReadKey();
				}
			}

			return duplicateDictionary;
		}

		public string[] GetFilePaths(FindDuplicatesJob job)
		{
			//Since we don't yet know how many files will be found, progress reporting is not trivial.
			Logger.Debug($"Traversing {job.Path}...");
			string[] files = _fileSystemHelper.WalkFilePaths(job);
			Logger.Debug("...done.");
			return files;
		}

		internal ConcurrentDictionary<string, List<File>> PopulateFileMetaData(string[] files, ConcurrentDictionary<string, List<File>> duplicateDictionary)
		{
			Logger.Debug("Populating metadata for discovered files...");

			bool isInConsole = IsConsoleApplication();

			var pb = isInConsole ? new ProgressBar(PbStyle.DoubleLine, files.Length) : null;
			int lastIncrement = 0;
			
			if(isInConsole) pb.Refresh(0, "Initializing...");

			int i = 0;
			foreach (string filePath in files)
			{
				// Only update the progress bar occasionally
				if (isInConsole && Math.Floor(i * 100.0 / files.Length) > lastIncrement || i == files.Length)
				{
					string tempFilePath = filePath;
					if (filePath.Contains("{"))
					{
						tempFilePath = tempFilePath.Replace("{", "");
					}
					if (filePath.Contains("}"))
					{
						tempFilePath = tempFilePath.Replace("}", "");
					}

					try
					{
						pb.Refresh(i, tempFilePath);
						lastIncrement = (i * 100 / files.Length);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}

				if (!string.IsNullOrEmpty(filePath))
				{
					long fileSize = _fileSystemHelper.GetFileSize(filePath);
					File tempFile = new File
					{
						FullPath = filePath,
						Filename = _fileSystemHelper.GetFileName(filePath),
						SizeInKiloBytes = fileSize,
						//todo: add option to hash only a portion of the file AND / OR check the files table. if the filename && size && path are the same as an entry in the files table, don't bother hashing (optionally) - just use the value from the table
						Hash = _fileSystemHelper.GetHashedValue(filePath, fileSize, _hashLimit)
					};

					//Ignore empty directory placeholder
					if (tempFile.Filename == "_._")
					{
						continue;
					}

					duplicateDictionary.AddOrUpdate(tempFile.Hash, new List<File>() { tempFile }, (key, value) => { value.Add(tempFile); return value; });
				}
				i++;
			}
			Logger.Debug("\n...done.");
			return duplicateDictionary;
		}

		public void ReportResults(ConcurrentDictionary<string, List<File>> duplicateDictionary)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("========= DUPLICATE FILE RESULTS =========\n\n");

			foreach(var (key, value) in duplicateDictionary)
			{
				//Skip hashes without duplicates
				if(value.Count == 1) continue;

				sb.Append($"MD5 Hash: {key} is shared by the following files: \n");

				foreach(File file in value)
				{
					sb.Append($"\t{file.FullPath,-10}\t({file.SizeInKiloBytes} KB)\n");
				}
				sb.Append("\n");
			}

			string pwd = Path.GetFullPath(@".\");
			string outputFileName = $"Duplicates_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
			Logger.Debug($"Writing report file to {pwd + outputFileName}.txt"); //todo: make configurable);
			System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
		}

		public void ValidateJob(FindDuplicatesJob job)
		{
			if (job.Options.IsLocalFileSystem ||
			    (!string.IsNullOrEmpty(job.Options.User) && !string.IsNullOrEmpty(job.Options.Pass))) return;
			Logger.Error("Remote Filesystem selected but credentials were invalid.");
			throw new ArgumentException("Remote Filesystem selected but credentials were invalid.");
		}

		// Konsole requires a real Console to work https://github.com/goblinfactory/konsole/issues/58
		// This is disgusting hack to get the TestRunner to work but hopefully a temporary one.
		private bool IsConsoleApplication()
		{
			return System.Diagnostics.Process.GetCurrentProcess().ProcessName == "FileUtils";
		}
	}
}
