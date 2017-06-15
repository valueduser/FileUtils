using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtils
{
	class Program
	{
		static void Main(string[] args)
		{
			//Todo: Menu
			DisplayMenu();
		}

		public static void DisplayMenu()
		{
			Console.WriteLine("============ File Utilities ============");
			string path = @"C:\Users\valueduser\Downloads\";
			DuplicateFind.FindDuplicates(path);

			Console.ReadKey();
		}

	}
}
