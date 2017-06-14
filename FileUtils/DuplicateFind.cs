using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace FileUtils
{
	public class DuplicateFind
	{
		private static NameValueCollection _appSettings;
		private static Dictionary<string, FileUtils.File> _fileDictionary;
		private static Dictionary<string, FileUtils.File> _dupeDictionary;

		internal static void FindDuplicates()
		{
			//todo:
			// 1). Read network share credentials from app settings
			SafeGetAppConfigs();
			string user = _appSettings["NetworkShareUser"] ?? "User Not Found";
			string pass = _appSettings["NetworkSharePassword"] ?? "Password Not Found";

			// 2). Impersonate user or map network drive
			// 3). loop through each file / directory recursively
			//		3a). Hash
			//		3b). Lookup in fileDictionary
			//			If not present, add it
			//			If is present, add to dupeDictionary and add itself to the list of dupes in fileDictionary
			// 4). Report results

			string filename = @"C:\Users\valueduser\Downloads\The Brand New Climbers Training Primer.pdf";
			string hash = ToHex(HashFile(filename), false);
			_dupeDictionary = new Dictionary<string, File>();
			_dupeDictionary.Add(hash, new FileUtils.File() {Filename = filename, SizeInKB = 6, FullPath = filename, Hash = hash, Duplicates = new List<File>()});

			ReportToConsole();
		}

		internal static string ReportToConsole()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("File duplication identification completed");

			if (_dupeDictionary.Count > 0)
			{
				foreach (var dupeFile in _dupeDictionary)
				{

					foreach (var dupe in dupeFile.Value.Duplicates)
					{
						Console.WriteLine(dupe.FullPath);
					}
					Console.WriteLine($"{dupeFile.Value.Filename}: \t {dupeFile.Key} \t {dupeFile.Value.SizeInKB} \t {dupeFile.Value.FullPath}");
				}
			}

			return String.Empty;
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
