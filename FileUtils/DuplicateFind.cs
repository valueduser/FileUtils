using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace FileUtils
{
	public class DuplicateFind
	{
		private static NameValueCollection _appSettings;
		private static Dictionary<string, FileUtils.File> _fileDictionary;

		internal static void FindDuplicates(string path)
		{
			//todo:
			// 1). Read network share credentials from app settings
			SafeGetAppConfigs();
			string user = _appSettings["NetworkShareUser"] ?? "User Not Found";
			string pass = _appSettings["NetworkSharePassword"] ?? "Password Not Found";

			// 2). Impersonate user or map network drive

			Console.Write("Building a list of files...");
			String[] fileArr = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
			Console.WriteLine("done.");

			_fileDictionary = new Dictionary<string, File>();

			Console.Write("Looking for duplicates...");
			foreach (string file in fileArr)
			{
				string hash = ToHex(HashFile(file), false);
				string filename = Path.GetFileName(file);
				float fileSize = (new System.IO.FileInfo(file).Length) / 1048576f; //match what the OS reports ¯\_(ツ)_/¯
				if (!_fileDictionary.ContainsKey(hash))
				{
					_fileDictionary.Add(hash, new FileUtils.File() { Filename = filename, SizeInMB = fileSize, FullPath = file, Hash = hash, Duplicates = new List<string>() });
				}
				else
				{
					_fileDictionary[hash].Duplicates.Add(file);
				}
			}
			Console.WriteLine("done.");

			ReportResults();
			Console.WriteLine("\nDONE.");
		}

		internal static void ReportResults()
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
			}

			string pwd = Path.GetFullPath(@".\");
			string outputFileName = $"Dupes_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
			Console.WriteLine($"Writing report file to {pwd + outputFileName}.txt");
			sb.Append("\n========= END =========");
			System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
		}

		private static byte[] HashFile(string filename)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = System.IO.File.OpenRead(filename))
				{
					return md5.ComputeHash(stream);
				}
			}
		}

		private static void SafeGetAppConfigs()
		{
			_appSettings = ConfigurationManager.AppSettings;
			
		}

		private static string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			foreach (byte singleByte in bytes)
				result.Append(singleByte.ToString(upperCase ? "X2" : "x2"));

			return result.ToString();
		}
	}
}
