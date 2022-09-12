﻿using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;
using FileUtil.Models;

namespace FileUtil.Core
{
	public interface IFileHelpers
	{
		string GetFileName(string pathToFile);
		long GetFileSize(string pathToFile);
		string[] WalkFilePaths(Source source);
		Hash GetHashedValue(string pathToFile, long fileSize, long hashLimit = 0);
	}

	public class FileHelpers : IFileHelpers
	{
		private readonly IFileSystem _fileSystem;

		public FileHelpers(IFileSystem fileSystem)
		{
			this._fileSystem = fileSystem;
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
			try
			{
				return _fileSystem.Path.GetFileName(pathToFile);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception encountered getting file name: {ex}");
				return "UNKNOWN_FILE";
			}
		}

		/// <summary>
		/// Reports the size of a file as understood by the File System
		/// </summary>
		/// <param name="pathToFile">Path to the file</param>
		/// <returns>Size in KiloBytes</returns>
		public long GetFileSize(string pathToFile)
		{
			try
			{
				return _fileSystem.FileInfo.FromFileName(pathToFile).Length / 1024;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception encountered getting file size: {ex}");
				return -1;
			}
		}

		public Hash GetHashedValue(string pathToFile, long fileSize, long hashLimit = 0)
		{
			return new Hash()
			{
				Value = ToHex(HashFile(pathToFile, fileSize, hashLimit)),
				IsPartial = hashLimit > 0,
				CreatedOn = DateTime.Now
			};
		}

		private byte[] HashFile(string filename, long filesize, long hashLimit = 0)
		{
			if (hashLimit != 0 && hashLimit < filesize)
			{
				try
				{
					byte[] bytes = new byte[hashLimit];
					using (var fs = _fileSystem.FileStream.Create(filename, FileMode.Open))
					{
						fs.Read(bytes, 0, (int) hashLimit);
						using (var md5 = MD5.Create())
						{
							return md5.ComputeHash(bytes);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error hashing {filename}: {ex}");
					return new byte[] { };
				}
			}
			else
			{
				//Console.WriteLine($"Hashing {filename}...");
				using (var md5 = MD5.Create())
				{
					try
					{
						using (var stream = _fileSystem.File.OpenRead(filename))
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

		public string[] WalkFilePaths(Source source)
		{

			//Console.WriteLine("Walking file system paths...");
			string[] fileSystemList = new string[] { };

			try
			{
				fileSystemList = _fileSystem.Directory.GetFiles(source.Path, "*.*", System.IO.SearchOption.AllDirectories);
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