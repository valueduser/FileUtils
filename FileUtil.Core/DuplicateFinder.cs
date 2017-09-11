using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileUtil.Models;
using File = FileUtil.Models.File;

namespace FileUtil.Core
{
	public interface IFindDuplicates
	{
		void ValidateJob(FindDuplicatesJob job);
		void RunJob(FindDuplicatesJob job);
		void WalkFilePaths(FindDuplicatesJob job);
		void FindDuplicates(FindDuplicatesJob job);
		void Execute(FindDuplicatesJob job);
	}


	public class DuplicateFinder
	{
		private String[] _fileArr;
		private Dictionary<string, File> _fileDictionary;

		public void RunJob(FindDuplicatesJob job)
		{
			ValidateJob(job);

			if (job.Options.IsLocalFileSystem)
			{
				FindDuplicates(job);
			}
			else
			{
				using (UNCAccessWithCredentials.UNCAccessWithCredentials unc =
					new UNCAccessWithCredentials.UNCAccessWithCredentials())
				{
					if (unc.NetUseWithCredentials(job.Options.Path, job.Options.User, job.Options.Domain, job.Options.Pass)
					    || unc.LastError == 1219) // Already connected
					{
						FindDuplicates(job);
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
							default:
								Console.WriteLine("Unknown error.");
								break;
						}
						Console.ReadKey();
					}
				}
			}
		}

		public void Execute(FindDuplicatesJob job)
		{
			Console.Write("Looking for duplicates...");
			//todo: progress

			int numberOfFilesFound = _fileArr.Length;
			_fileDictionary = new Dictionary<string, File>();

			for (int i = 0; i < numberOfFilesFound; i++)
			{
				FileHelpers.UpdateProgress(i, numberOfFilesFound);
				string file = _fileArr[i];
				string filename;
				long fileSize;

				try
				{
					filename = Path.GetFileName(file);
					fileSize = (new System.IO.FileInfo(file).Length) / 1024;
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
				string hash = FileHelpers.ToHex(FileHelpers.HashFile(file, fileSize, limit), false);

				if (!_fileDictionary.ContainsKey(hash))
				{
					_fileDictionary.Add(hash, new File()
					{
						Filename = filename,
						SizeInMB = fileSize,
						FullPath = file,
						Hash = hash,
						Duplicates = new List<string>(),
						HashCollisions = new List<string>()
					});
				}
				else if (_fileDictionary.ContainsKey(hash) && fileSize == _fileDictionary[hash].SizeInMB)
				{
					_fileDictionary[hash].Duplicates.Add(file);
				}
				else
				{
					_fileDictionary[hash].HashCollisions.Add(file);
				}
			}
			Console.WriteLine("done.");
		}

		//todo: reorder the methods in this file in a sensible way
		public void FindDuplicates(FindDuplicatesJob job)
		{
			_fileArr = FileHelpers.WalkFilePaths(job);
			Execute(job);
			//ReportResults();
			Console.ReadKey();
		}

		internal void ReportResults()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("========= DUPLICATE FILE RESULTS =========\n\n");

			foreach (var file in _fileDictionary)
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
			Console.ReadKey();
		}

		public void ValidateJob(FindDuplicatesJob job)
		{
			if(!job.Options.IsLocalFileSystem && (String.IsNullOrEmpty(job.Options.User)|| String.IsNullOrEmpty(job.Options.Pass)))
			{
				throw new ArgumentException("Remote Filesystem selected but credentials were invalid.");
			}

			//if (!FileHelpers.ValidateFileNames(job.Path))
			//{
			//	throw new ArgumentException();
			//}
		}
	}
}
