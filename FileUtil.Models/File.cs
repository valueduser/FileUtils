using System.Collections.Generic;

namespace FileUtil.Models
{
	public class File
	{
		public int Id { get; set; }
		public string Filename { get; set; }
		public string FullPath { get; set; }
		public long SizeInMegaBytes { get; set; }
		public string Hash { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			File objresult = (File)obj;

			if (!Filename.Equals(objresult.Filename) ||
			    !FullPath.Equals(objresult.FullPath) ||
			    !SizeInMegaBytes.Equals(objresult.SizeInMegaBytes) ||
			    !Hash.Equals(objresult.Hash))
			{
				return false;
			}

			return true;
		}

		private bool CompareLists(List<string> list1, List<string> list2)
		{
			for (int i = 0; i < list1.Count; i++)
			{
				if (list1[i] != list2[i])
					return false;
			}
			return true;
		}

		public override int GetHashCode() => base.GetHashCode();
	}
}
