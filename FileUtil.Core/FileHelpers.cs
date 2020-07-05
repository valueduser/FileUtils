using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Text;
using FileUtil.Models;

namespace FileUtil.Core
{
	public interface IFileHelpers
	{
		string GetFileName(string pathToFile);
		long GetFileSize(string pathToFile);
		string[] WalkFilePaths(FindDuplicatesJob job);
		string GetHashedValue(string pathToFile, long fileSize, long hashLimit = 0);
	}

	public class FileHelpers : IFileHelpers
	{
		private readonly IFileSystem fileSystem;

		public FileHelpers(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		private string ToHex(byte[] bytes)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			foreach (byte singleByte in bytes)
				result.Append(
					singleByte.ToString("x2")
				);

			return result.ToString();
		}

		public string GetFileName(string pathToFile)
		{
			return fileSystem.Path.GetFileName(pathToFile);
		}

		/// <summary>
		/// Reports the size of a file as understood by the File System
		/// </summary>
		/// <param name="pathToFile">Path to the file</param>
		/// <returns>Size in KiloBytes</returns>
		public long GetFileSize(string pathToFile)
		{
			return fileSystem.FileInfo.FromFileName(pathToFile).Length / 1024;
		}

		public string GetHashedValue(string pathToFile, long fileSize, long hashLimit = 0)
		{
			return ToHex(HashFile(pathToFile, fileSize, hashLimit));
		}

		private byte[] HashFile(string filename, long filesize, long hashLimit = 0)
		{
			if (hashLimit < filesize && hashLimit != 0)
			{
				//Console.WriteLine($"Hashing the first {hashLimit} bytes of {filename}...");
				using (var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open))
				{
					using (var stream = mmf.CreateViewStream(0, hashLimit * 1024))
					{
						using (var md5 = MD5.Create())
						{
							try
							{
								byte[] retval = md5.ComputeHash(stream);
								//Console.WriteLine("done.");
								return retval;
							}
							catch (Exception e)
							{
								Console.WriteLine($"Error hashing {filename}: {e}");
								return new byte[] { };
							}
						}
					}
				}

			}
			else
			{
				//Console.WriteLine($"Hashing {filename}...");
				using (var md5 = MD5.Create())
				{
					try
					{
						using (var stream = System.IO.File.OpenRead(filename))
						{
							byte[] retval = md5.ComputeHash(stream);
							//Console.WriteLine("done.");
							return retval;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error hashing {filename}: {e}");
						return new byte[] { };
					}
				}
			}
		}

		public string[] WalkFilePaths(FindDuplicatesJob job)
		{

			//Console.WriteLine("Walking file system paths...");
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
			int filesFound = fileSystemList.Length;
			Console.WriteLine($"Found {filesFound} files.");

			return fileSystemList;
		}
	}
}