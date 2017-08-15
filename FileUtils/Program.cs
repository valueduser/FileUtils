using FileUtil.Service.ServiceInterfaces;

namespace FileUtils
{
	class Program
	{
		static void Main(string[] args)
		{
			FindDuplicateFilesService dupe = new FindDuplicateFilesService();
			dupe.FindDuplicates();
			//DuplicateFind dupe = new DuplicateFind();
			//dupe.DisplayMenu();
		}
	}
}
