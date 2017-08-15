using System;
using System.Security.Cryptography;
using System.Text;

namespace FileUtil.Core
{
	public class FileHelpers
	{
		internal static string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			foreach (byte singleByte in bytes)
				result.Append(singleByte.ToString(upperCase ? "X2" : "x2"));

			return result.ToString();
		}

		internal static byte[] HashFile(string filename)
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
					return new byte[] { };
				}
			}
		}
	}
}
