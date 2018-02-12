using System.Collections.Generic;
using System.Linq;
using FileUtil.Models.Interfaces;

namespace FileUtil.Models
{
	public class FindDuplicatesResult : IResult
	{
		public Dictionary<string, File> Duplicates;
		public string ReportOrderPreference { get; set; }

		public FindDuplicatesResult()
		{
			Duplicates = new System.Collections.Generic.Dictionary<string, File>();
			ReportOrderPreference = "Alphabetical";
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			FindDuplicatesResult objresult = (FindDuplicatesResult)obj;

			if (objresult.Duplicates.Count != Duplicates.Count)
			{
				return false;
			}

			return
				Duplicates.Keys.Count == objresult.Duplicates.Keys.Count &&
				Duplicates.Keys.All(k => objresult.Duplicates.ContainsKey(k) &&
				                         object.Equals(objresult.Duplicates[k], Duplicates[k]));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
