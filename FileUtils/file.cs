using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtils
{
	internal class File
	{
		internal string Filename { get; set; }
		internal string FullPath { get; set; }
		internal long SizeInKB { get; set; }
		internal string Hash { get; set; }

		internal List<FileUtils.File> Duplicates { get; set; }

	}
}
