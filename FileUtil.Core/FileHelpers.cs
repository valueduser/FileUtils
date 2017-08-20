using System;
//using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Text;

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
			if (hashLimit < filesize)
			{
				Console.WriteLine($"Hashing the first {hashLimit} bytes of {filename}...");
				using (var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open))
				{
					using (var stream = mmf.CreateViewStream(0, hashLimit))
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
		//string pwd = Path.GetFullPath(@".\");
		//string outputFileName = $"Dupes_{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}.{DateTime.UtcNow.Year}-{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}";
		//Console.WriteLine($"Writing report file to {pwd + outputFileName}.txt"); //todo: make configurable
		//sb.Append("\n========= END =========");
		//System.IO.File.WriteAllText(pwd + outputFileName + ".txt", sb.ToString());
		//Console.ReadKey();
	}
}
