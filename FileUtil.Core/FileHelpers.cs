using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FileUtil.Models;

namespace FileUtil.Core
{
	public interface IFileHelpers
	{
		string ToHex(byte[] bytes, bool upperCase);
		string GetFileName(string pathToFile);
		long GetFileSize(string pathToFile);
		byte[] HashFile(string filename, long filesize, long hashlimit = 0);
		string[] WalkFilePaths(FindDuplicatesJob job);
		void UpdateProgress(int currentIteration, int totalIterations);
	}

	public class FileHelpers : IFileHelpers
	{
		private readonly IFileSystem fileSystem;

		static int lastPercentUpdate = 0;

		public FileHelpers(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		public string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			foreach (byte singleByte in bytes)
				result.Append(singleByte.ToString(upperCase ? "X2" : "x2"));

			return result.ToString();
		}

		public string GetFileName(string pathToFile)
		{
			return fileSystem.Path.GetFileName(pathToFile);
		}

		public long GetFileSize(string pathToFile)
		{
			return fileSystem.FileInfo.FromFileName(pathToFile).Length / 1024;
		}

		public byte[] HashFile(string filename, long filesize, long hashLimit = 0)
		{
			if (hashLimit < filesize && hashLimit != 0)
			{
				Console.WriteLine($"Hashing the first {hashLimit} bytes of {filename}...");
				using (var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open))
				{
					using (var stream = mmf.CreateViewStream(0, hashLimit * 1024))
					{
						using (var md5 = MD5.Create())
						{
							try
							{
								byte[] retval = md5.ComputeHash(stream);
								Console.WriteLine("done.");
								return retval;
							}
							catch (Exception e)
							{
								Console.WriteLine($"Error hasing {filename}: {e}");
								return new byte[] { };
							}
						}
					}
				}

			}
			else
			{
				Console.WriteLine($"Hashing {filename}...");
				using (var md5 = MD5.Create())
				{
					try
					{
						using (var stream = System.IO.File.OpenRead(filename))
						{
							byte[] retval = md5.ComputeHash(stream);
							Console.WriteLine("done.");
							return retval;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error hasing {filename}: {e}");
						return new byte[] { };
					}
				}
			}
		}

		public string[] WalkFilePaths(FindDuplicatesJob job)
		{

			Console.WriteLine("Walking file system paths...");
			string[] fileSystemList = new string[] { };

			try
			{
				fileSystemList = fileSystem.Directory.GetFiles(job.Path, "*.*", System.IO.SearchOption.AllDirectories);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception encountered walking the file tree: {e}");
				throw;
			}
			Console.WriteLine("done.");
			int filesFound = fileSystemList.Length;
			Console.WriteLine($"Found {filesFound} files.");

			return fileSystemList;
		}

		public void UpdateProgress(int currentIteration, int totalIterations)
		{
			double dprogress = ((double)currentIteration / totalIterations) * 100;
			int progress = (int)dprogress;
			if (progress > lastPercentUpdate)
			{
				lastPercentUpdate = progress;
				StringBuilder sb = new StringBuilder();
				string hashes = String.Concat(Enumerable.Repeat("#", (progress / 5)));
				string dots = String.Concat(Enumerable.Repeat(".", 20 - (progress / 5)));
				string percentageComplete = $"{lastPercentUpdate}%";
				sb.Append($"[{hashes}{dots}] {percentageComplete} ({currentIteration}/{totalIterations})");
				//Console.Clear();
				Console.WriteLine(sb.ToString());
			}
		}
	}
}