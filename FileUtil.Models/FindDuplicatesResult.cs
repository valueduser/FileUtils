using System.Collections.Generic;
using FileUtil.Models.Interfaces;

namespace FileUtil.Models
{
	public class FindDuplicatesResult: IResult
	{
		public Dictionary<string, File> Duplicates;
		public string ReportOrderPreference { get; set; }

		public FindDuplicatesResult()
		{
			Duplicates = new System.Collections.Generic.Dictionary<string, File>();
			ReportOrderPreference = "Alphabetical";
		}
	}
}
