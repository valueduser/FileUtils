﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileUtil.Models;
using Konsole;
using File = FileUtil.Models.File;

namespace FileUtil.Core
{
	public interface IDuplicateFinder
	{
		void FindDuplicateFiles(FindDuplicatesJob job);

		string[] GetFilePaths(FindDuplicatesJob job);

		List<File> PopulateFileMetaData(string[] files);
	}

	public class DuplicateFinder
	{
		private IFileHelpers fileSystemHelper;

		private int hashLimit;
		private ConcurrentDictionary<string, List<File>> _duplicateDictionary = new ConcurrentDictionary<string, List<File>>();

		public DuplicateFinder(IFileHelpers fileSystemHelper)
		{
			this.fileSystemHelper = fileSystemHelper;
		}

		public void FindDuplicateFiles(FindDuplicatesJob job)
		{
			string[] filePaths;
			hashLimit = job.Options.HashLimit;
			if (job.Options.IsLocalFileSystem)
			{
				filePaths = GetFilePaths(job);
				PopulateFileMetaData(filePaths);
				ReportResults();
			}
			else
			{
				using (UNCAccessWithCredentials.UNCAccessWithCredentials unc =
					new UNCAccessWithCredentials.UNCAccessWithCredentials())
				{
					if (unc.NetUseWithCredentials(job.Options.Path, job.Options.User, job.Options.Domain, job.Options.Pass)
						|| unc.LastError == 1219) // Already connected
					{
						filePaths = GetFilePaths(job);
						PopulateFileMetaData(filePaths);
						ReportResults();
						//PersistFile
					}
					else
					{
						Console.WriteLine($"Failed to connect to UNC location. Error: {unc.LastError}.");
						switch (unc.LastError)
						{
							case 1326:
								Console.WriteLine("Login failure: The user name or password is incorrect.");
								break;
							case 86:
								Console.WriteLine("Access denied: The specified network password is not correct.");
								break;
							case 87:
								Console.WriteLine("Invalid parameter.");
								break;
							case 1219:
								Console.WriteLine("Multiple connections to server.");
								unc.Dispose();
								break;
							case 53:
								Console.WriteLine("Network path not found.");
								break;
							case 5:
								Console.WriteLine("Access denied.");
								break;
							default:
								Console.WriteLine($"Unknown error. {unc.LastError}");
								break;
						}
						Console.ReadKey();
					}
				}
			}
		}

		public string[] GetFilePaths(FindDuplicatesJob job)
		{
			//Since we don't yet know how many files will be found, progress reporting is not trivial.
			Console.WriteLine($"Traversing {job.Path}...");
			string[] files = fileSystemHelper.WalkFilePaths(job);
			Console.WriteLine("...done.");
			return files;
		}

		public List<File> PopulateFileMetaData(string[] files) //todo do I need to return List??
		{
			List<File> knownFiles = new List<File>();

			Console.WriteLine("Populating metadata for discovered files...");
			var pb = new ProgressBar(PbStyle.DoubleLine, files.Length);
			int lastIncrement = 0;
			pb.Refresh(0, "Initializing...");

			int i = 0;
			foreach (string filePath in files)
			{
				if (Math.Floor(i * 100.0 / files.Length) > lastIncrement || i == files.Length)
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
					long fileSize = fileSystemHelper.GetFileSize(filePath);
					File tempFile = new File
					{
						FullPath = filePath,
						Filename = fileSystemHelper.GetFileName(filePath),
						SizeInKiloBytes = fileSize,
						//todo: add option to hash only a portion of the file AND / OR check the files table. if the filename && size && path are the same as an entry in the files table, don't bother hashing (optionally) - just use the value from the table
						Hash = fileSystemHelper.GetHashedValue(filePath, fileSize, hashLimit)
					};

					//Ignore empty directory placeholder
					if (tempFile.Filename == "_._")
					{
						continue;
					}

					AddToHashTable(tempFile);

					knownFiles.Add(tempFile);
				}
				i++;
			}
			Console.WriteLine("\n...done.");
			return knownFiles;
		}

		public bool AddToHashTable(File file)
		{
			//_duplicateDictionary.AddOrUpdate(file.Hash, new List<File>() {file}, (key, value) => { value.Add(file); return value; });
			//if we can determine if it was added, _duplicates.Add(file);

			if (!_duplicateDictionary.ContainsKey(file.Hash))
			{
				_duplicateDictionary.GetOrAdd(file.Hash, new List<File>() { file });

				return false;
			}

			_duplicateDictionary.TryGetValue(file.Hash, out var tempList);

			if (tempList != null)
			{
				tempList.Add(file);
				_duplicateDictionary[file.Hash] = tempList;
			}

			return true;
		}

		internal void ReportResults()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("========= DUPLICATE FILE RESULTS =========\n\n");

			foreach(var entry in _duplicateDictionary)
			{
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

		public void ValidateJob(FindDuplicatesJob job)
		{
			if (!job.Options.IsLocalFileSystem && (String.IsNullOrEmpty(job.Options.User) || String.IsNullOrEmpty(job.Options.Pass)))
			{
				throw new ArgumentException("Remote Filesystem selected but credentials were invalid.");
			}
		}
	}
}
