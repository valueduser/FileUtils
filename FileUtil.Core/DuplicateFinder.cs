using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using FileUtil.Models;
using File = FileUtil.Models.File;

namespace FileUtil.Core
{
	public interface IDuplicateFinder
	{
		void ValidateJob(FindDuplicatesJob job);
		FindDuplicatesResult FindDuplicates(FindDuplicatesJob job);
		void FindDuplicateFiles(FindDuplicatesJob job);
	}

	public class DuplicateFinder
	{
		private IFileHelpers fileSystemHelper;

		public DuplicateFinder(IFileHelpers fileSystemHelper)
		{
			this.fileSystemHelper = fileSystemHelper;
		}

		public void FindDuplicateFiles(FindDuplicatesJob job)
		{
			List<File> knownFiles = new List<File>();
			if (job.Options.IsLocalFileSystem)
			{
				//FindDuplicates(job);
				knownFiles = GetListOfKnownFiles(job);
				PopulateDuplicateResult(knownFiles);
			}
			else
			{
				using (UNCAccessWithCredentials.UNCAccessWithCredentials unc =
					new UNCAccessWithCredentials.UNCAccessWithCredentials())
				{
					if (unc.NetUseWithCredentials(job.Options.Path, job.Options.User, job.Options.Domain, job.Options.Pass)
						|| unc.LastError == 1219) // Already connected
					{
						knownFiles = GetListOfKnownFiles(job);
						PopulateDuplicateResult(knownFiles);
						//FindDuplicates(job);
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
								Console.WriteLine("Unknown error.");
								break;
						}
						Console.ReadKey();
					}
				}
			}
		}

		public List<File> GetListOfKnownFiles(FindDuplicatesJob job)
		{
			List<File> knownFiles = new List<File>();
			string[] files = fileSystemHelper.WalkFilePaths(job);

			foreach (string filePath in files)
			{
				if (String.IsNullOrEmpty(filePath))
				{
					long fileSize = fileSystemHelper.GetFileSize(filePath);
					knownFiles.Add(new File
					{
						FullPath = filePath,
						Filename = fileSystemHelper.GetFileName(filePath),
						SizeInMB = fileSize,
						Hash = fileSystemHelper.GetHashedValue(filePath, fileSize)
					});
				}	
			}
			return knownFiles;
		}

		public FindDuplicatesResult PopulateDuplicateResult(List<File> knownFiles)
		{
			FindDuplicatesResult results = new FindDuplicatesResult();
			foreach (File file in knownFiles)
			{
				if (!results.Duplicates.ContainsKey(file.Hash)) //new entry
				{
					results.Duplicates.Add(file.Hash, file);
					//write results to the duplicates table
					//write results to the file table
				}
				else if (results.Duplicates.ContainsKey(file.Hash) && file.SizeInMB == results.Duplicates[file.Hash].SizeInMB) //duplicate
				{
					//results.Duplicates[file.Hash]
					//results.Duplicates.Add();
					//write results to the duplicates table
					//write results to the file table
				}
				else
				{
					//results.Duplicates[file.Hash].HashCollisions.Add(file);
				}
			}

			return results;
		}

		public FindDuplicatesResult FindDuplicates(FindDuplicatesJob job)
		{
			FindDuplicatesResult results = new FindDuplicatesResult();
			results.ReportOrderPreference = job.Options.ReportOrderPreference;

			Console.Write("Looking for duplicates...");
			job.FilesArr = fileSystemHelper.WalkFilePaths(job);

			int numberOfFilesFound = job.FilesArr.Length;

			for (int i = 0; i < numberOfFilesFound; i++)
			{
				fileSystemHelper.UpdateProgress(i, numberOfFilesFound);
				string file = job.FilesArr[i];
				string filename;
				long fileSize;

				try
				{
					filename = fileSystemHelper.GetFileName(file);//Path.GetFileName(file);
					fileSize = fileSystemHelper.GetFileSize(file); //(new System.IO.FileInfo(file).Length) / 1024;
				}
				catch (Exception e)
				{
					Console.WriteLine($"Error in path: {file}. {e}");
					continue;
				}
				//if (fileSize > 2097152) //todo: make configurable
				//{
				//	Console.WriteLine($"File {filename} too large to hash. {fileSize}");
				//	continue;
				//}

				long limit = job.Options.HashLimit; //todo
				string hash = fileSystemHelper.ToHex(fileSystemHelper.HashFile(file, fileSize, limit), false);

				//Todo: at this point the entry could be a duplicate or not. 
				if (!results.Duplicates.ContainsKey(hash))
				{
					results.Duplicates.Add(hash, new File()
					{
						Filename = filename,
						SizeInMB = fileSize,
						FullPath = file,
						Hash = hash,
						Duplicates = new List<string>(),
						HashCollisions = new List<string>()
					});
				}
				else if (results.Duplicates.ContainsKey(hash) && fileSize == results.Duplicates[hash].SizeInMB)
				{
					results.Duplicates[hash].Duplicates.Add(file);
				}
				else
				{
					results.Duplicates[hash].HashCollisions.Add(file);
				}
			}
			Console.WriteLine("done.");
			//ReportResults(results);
			return results;
		}

		internal void ReportResults(FindDuplicatesResult results)
		{
			//Todo pull the file printing work out into a separate class
			//Todo print number of duplicates to the console and only bother with a text file if dupes > 0

			var reportDictionary = new Dictionary<string, File>();
			if (results.ReportOrderPreference.Equals(Enums.ReportOrder.FileSizeDesc.ToString()))
			{
				foreach (var duplicate in results.Duplicates.OrderByDescending(i => i.Value.SizeInMB))
				{
					reportDictionary.Add(duplicate.Key, duplicate.Value);
				}
			}
			else if (results.ReportOrderPreference.Equals(Enums.ReportOrder.FileSizeAsc.ToString()))
			{
				foreach (var duplicate in results.Duplicates.OrderBy(i => i.Value.SizeInMB))
				{
					reportDictionary.Add(duplicate.Key, duplicate.Value);
				}
			}
			else if (results.ReportOrderPreference.Equals(Enums.ReportOrder.Alphabetical.ToString()))
			{
				foreach (var duplicate in results.Duplicates.OrderBy(i => i.Value.Filename))
				{
					reportDictionary.Add(duplicate.Key, duplicate.Value);
				}
			}
			else
			{
				reportDictionary = results.Duplicates;
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("========= DUPLICATE FILE RESULTS =========\n\n");

			foreach (var file in reportDictionary)
			{
				if (file.Value.Duplicates.Count > 0)
				{
					sb.Append($"MD5 Hash: {file.Key} ({file.Value.SizeInMB:0.00} MB) is shared by the following files: \n");
					sb.Append($"{file.Value.FullPath,-10}\n");
					foreach (var dupe in file.Value.Duplicates)
					{
						sb.Append($"\t{dupe,-10}\n");
					}
					sb.Append("\n");
				}
				if (file.Value.HashCollisions.Count > 0)
				{
					sb.Append($"MD5 Hash: {file.Key} is shared by the following files with differing file sizes: \n");
					sb.Append($"{file.Value.FullPath,-10}\n");
					foreach (var collision in file.Value.HashCollisions)
					{
						sb.Append($"\t{collision,-10}\n : {file.Value.SizeInMB:0.00} MB");
					}
					sb.Append("\n");
				}
			}

			string pwd = Path.GetFullPath(@".\");
			string outputFileName =
				$"Dupes_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
			Console.WriteLine($"Writing report file to {pwd + outputFileName}.txt"); //todo: make configurable
			sb.Append("\n========= END =========");
			System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
			Console.WriteLine("Done.");
			Console.ReadKey();
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
