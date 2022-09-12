using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FileUtil.Models;
using Konsole;
using File = FileUtil.Models.File;

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
		private readonly IFileHelpers _fileSystemHelper;

		private int hashLimit;

		public DuplicateFinder(IFileHelpers fileSystemHelper)
		{
			this._fileSystemHelper = fileSystemHelper;
		}

		public ConcurrentDictionary<string, List<File>> FindDuplicateFiles(FindDuplicatesJob job)
		{
			string[] filePaths;
			ConcurrentDictionary<string, List<File>> duplicateDictionary = new ConcurrentDictionary<string, List<File>>();
			hashLimit = job.Options.Config.HashSizeLimitInKB;

			foreach (Source source in job.Options.Sources)
			{
				if (source.IsLocalFileSystem)
				{
					filePaths = GetFilePaths(source);
					PopulateFileMetaData(filePaths, duplicateDictionary);
				}
				else
				{
					NetworkCredential networkCred = new NetworkCredential(source.NetworkShareUser, source.NetworkSharePassword, source.NetworkShareDomain);
					CredentialCache netCache = new CredentialCache();
					netCache.Add(new System.Uri(source.Path), "Basic", networkCred);
					filePaths = GetFilePaths(source);
					PopulateFileMetaData(filePaths, duplicateDictionary);
				}
			}
			return duplicateDictionary;
		}

		public string[] GetFilePaths(Source source)
		{
			//Since we don't yet know how many files will be found, progress reporting is not trivial.
			Console.WriteLine($"Traversing {source.Path}...");
			string[] files = _fileSystemHelper.WalkFilePaths(source);
			Console.WriteLine("...done.");
			return files;
		}

		internal ConcurrentDictionary<string, List<File>> PopulateFileMetaData(string[] files, ConcurrentDictionary<string, List<File>> duplicateDictionary)
		{
			Console.WriteLine("Populating metadata for discovered files...");

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

				if (!String.IsNullOrEmpty(filePath))
				{
					long fileSize = _fileSystemHelper.GetFileSize(filePath);
					File tempFile = new File
					{
						FullPath = filePath,
						Filename = _fileSystemHelper.GetFileName(filePath),
						SizeInKiloBytes = fileSize,
						//todo: add option to hash only a portion of the file AND / OR check the files table. if the filename && size && path are the same as an entry in the files table, don't bother hashing (optionally) - just use the value from the table
						Hash = _fileSystemHelper.GetHashedValue(filePath, fileSize, hashLimit)
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
			Console.WriteLine("\n...done.");
			return duplicateDictionary;
		}

		public void ReportResults(ConcurrentDictionary<string, List<File>> duplicateDictionary)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("========= DUPLICATE FILE RESULTS =========\n\n");

			foreach(var entry in duplicateDictionary)
			{
				//Skip hashes without duplicates
				if(entry.Value.Count == 1) continue;

				sb.Append($"MD5 Hash: {entry.Key} is shared by the following files: \n");

				foreach(File file in entry.Value)
				{
					sb.Append($"\t{file.FullPath,-10}\t({file.SizeInKiloBytes} KB)\n");
				}
				sb.Append("\n");
			}

			string pwd = Path.GetFullPath(@".\");
			string outputFileName = $"Duplicates_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
			Console.WriteLine($"Writing report file to {pwd + outputFileName}.txt"); //todo: make configurable
			System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
		}

		public void ValidateJob(List<Source> sources)
		{
			sources.ForEach(delegate (Source source)
			{
				if (!source.IsLocalFileSystem && (String.IsNullOrEmpty(source.NetworkShareUser) || String.IsNullOrEmpty(source.NetworkSharePassword)))
				{
					throw new ArgumentException("Remote Filesystem selected but credentials were missing.");
				}
			});
		}

		// Konsole requires a real Console to work https://github.com/goblinfactory/konsole/issues/58
		// This is disgusting hack to get the TestRunner to work but hopefully a temporary one.
		private bool IsConsoleApplication()
		{
			return System.Diagnostics.Process.GetCurrentProcess().ProcessName == "FileUtils";
		}
	}
}
