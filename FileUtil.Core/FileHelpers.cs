using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using FileUtil.Models;

namespace FileUtil.Core
{
	public class FileHelpers
	{
		//private static Stopwatch _stopwatch = new Stopwatch();
		//private static long partialTime, fullTime;

		internal static string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			foreach (byte singleByte in bytes)
				result.Append(singleByte.ToString(upperCase ? "X2" : "x2"));

			return result.ToString();
		}

		internal static byte[] HashFile(string filename, long filesize, long hashLimit = 0)
		{
			//_stopwatch.Start();
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
								//_stopwatch.Stop();
								//long partialTime = _stopwatch.ElapsedMilliseconds;
								//Console.WriteLine($"Partial hash time: {filename} -- {partialTime} ms.");
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
							//_stopwatch.Stop();
							//fullTime = _stopwatch.ElapsedMilliseconds;
							//Console.WriteLine($"Full hash time: {fullTime} -- {_stopwatch.ElapsedMilliseconds}.");
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

		internal static bool ValidateFileNames(string rootPath)
		{
			Console.WriteLine("Validating the file system...");
			IEnumerable<string> fileSystemList = new List<string>();
			try
			{
				fileSystemList = Directory.EnumerateFileSystemEntries(rootPath, "*", SearchOption.AllDirectories);
			}
			catch(System.Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.WriteLine($"Found {fileSystemList.Count()} entries.");
			Console.WriteLine("done.");

			if (fileSystemList.Any()) //todo make this more meaningful
			{
				return true;
			}

			return false;
		}

		//internal static string[] SafeGetFilePaths(string rootPath)
		//{
			
		//	Console.WriteLine("SafeGetFilePaths...");
		//	IEnumerable<string> fileSystemList = new List<string>();
		//	try
		//	{
		//		fileSystemList = Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories);
		//	}
		//	catch (System.Exception ex)
		//	{
		//		Console.WriteLine(ex.Message);
		//	}
		//	Console.WriteLine($"Found {fileSystemList.Count()} entries.");
		//	Console.WriteLine("done.");

		//	if (fileSystemList.Any()) //todo make this more meaningful
		//	{
		//		//debug
		//		StringBuilder sb = new StringBuilder();
		//		foreach (string path in fileSystemList)
		//		{
		//			sb.Append(path + "\n");
		//		}
		//		string outputFileName = $"SafeGetFilePaths_Files_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
		//		string pwd = Path.GetFullPath(@".\");

		//		System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
		//		//debug

		//		return fileSystemList.ToArray();
		//	}

		//	return new string[] {};
		//}

		public static string[] WalkFilePaths(FindDuplicatesJob job)
		{
			Console.WriteLine("Walking file system paths...");
			string[] fileSystemList = new string[] { };

			try
			{
				fileSystemList = System.IO.Directory.GetFiles(job.Path, "*.*", System.IO.SearchOption.AllDirectories);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception encountered walking the file tree: {e}");
			}
			Console.WriteLine("done.");
			int filesFound = fileSystemList.Length;
			Console.WriteLine($"Found {filesFound} files.");
			
			return fileSystemList;
		}
	}
}
