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
			Console.WriteLine("============MENU============");
			string filename = @"C:\Users\valueduser\Downloads";
			DuplicateFind.FindDuplicates();

			Console.ReadKey();
		}

	}
}
