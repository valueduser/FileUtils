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
		private NameValueCollection _appSettings;
		private Dictionary<string, FileUtils.File> _fileDictionary;

		public void DisplayMenu()
		{
			Console.WriteLine("============ File Utilities ============");

			SafeGetAppConfigs();
			string user = _appSettings["NetworkShareUser"] ?? "User Not Found";
			string pass = _appSettings["NetworkSharePassword"] ?? "Password Not Found";
			string domain = _appSettings["NetworkShareDomain"] ?? "Domain Not Found";
			string path = _appSettings["NetworkShareUncPath"] ?? "UNC Path Not Found";

			using (UNCAccessWithCredentials.UNCAccessWithCredentials unc = new UNCAccessWithCredentials.UNCAccessWithCredentials())
			{
				if (unc.NetUseWithCredentials(path, user, domain, pass))
				{
					FindDuplicates(path);
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
						default:
							Console.WriteLine("Unknown error.");
							break;
					}
				}
			}
			Console.ReadKey();
		}

		internal void FindDuplicates(string path)
		{
			Console.Write("Building a list of files...");
			String[] fileArr = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
			Console.WriteLine("done.");
			int filesFound = fileArr.Length;
			Console.WriteLine($"Found {filesFound} files.");

			_fileDictionary = new Dictionary<string, File>();
			Console.Write("Looking for duplicates...");

			for (int i = 0; i < fileArr.Length; i++)
			{
				string file = fileArr[i];
				string filename;
				float fileSize;
				try
				{
					filename = Path.GetFileName(file);
					fileSize = (new System.IO.FileInfo(file).Length) / 1048576f; //match what the OS reports ¯\_(ツ)_/¯
				}
				catch (Exception e)
				{
					Console.WriteLine($"Error in path: {file}. {e}");
					continue;
				}
				if (fileSize > 2097152)
				{
					Console.WriteLine($"File {filename} too large to hash. {fileSize}");
					continue;
				}

				string hash = ToHex(HashFile(file), false);

				if (!_fileDictionary.ContainsKey(hash))
				{
					_fileDictionary.Add(hash, new FileUtils.File() { Filename = filename, SizeInMB = fileSize, FullPath = file, Hash = hash, Duplicates = new List<string>() });
				}
				else
				{
					_fileDictionary[hash].Duplicates.Add(file);
				}
			}

			ReportResults();
			Console.WriteLine("\nDONE.");
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
			}

			string pwd = Path.GetFullPath(@".\");
			string outputFileName = $"Dupes_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
			Console.WriteLine($"Writing report file to {pwd + outputFileName}.txt");
			sb.Append("\n========= END =========");
			System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
		}

		private byte[] HashFile(string filename)
		{
			using (var md5 = MD5.Create())
			{
				try
				{
					using (var stream = System.IO.File.OpenRead(filename))
					{
						return md5.ComputeHash(stream);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Error hasing {filename}: {e}");
					return new byte[] {};
				}
			}
		}

		private void SafeGetAppConfigs()
		{
			_appSettings = ConfigurationManager.AppSettings;
		}

		private string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			foreach (byte singleByte in bytes)
				result.Append(singleByte.ToString(upperCase ? "X2" : "x2"));

			return result.ToString();
		}
	}
}
