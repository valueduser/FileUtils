using System.Collections.Generic;

namespace FileUtil.Models
{
	public class File
	{
		public string Filename { get; set; }
		public string FullPath { get; set; }
		public long SizeInMB { get; set; }
		public string Hash { get; set; }
		public List<string> Duplicates { get; set; }
		public List<string> HashCollisions { get; set; }
	}
}
