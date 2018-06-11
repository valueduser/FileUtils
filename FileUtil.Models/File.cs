using System.Collections.Generic;

namespace FileUtil.Models
{
	public class File
	{
		public int Id { get; set; }
		public string Filename { get; set; }
		public string FullPath { get; set; }
		public long SizeInMB { get; set; }
		public string Hash { get; set; }
		public List<string> Duplicates { get; set; } //todo remove
		public List<string> HashCollisions { get; set; } //todo remove

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			File objresult = (File)obj;

			if (!Filename.Equals(objresult.Filename) ||
			    !FullPath.Equals(objresult.FullPath) ||
			    !SizeInMB.Equals(objresult.SizeInMB) ||
			    !Hash.Equals(objresult.Hash) ||
			    Duplicates.Count != objresult.Duplicates.Count ||
			    HashCollisions.Count != objresult.HashCollisions.Count)
			{
				return false;
			}

			return (CompareLists(Duplicates, objresult.Duplicates) && CompareLists(HashCollisions, objresult.HashCollisions));
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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
