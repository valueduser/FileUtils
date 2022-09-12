using System.Collections.Specialized;
using FileUtil.Models.Interfaces;

namespace FileUtil.Models
{
	public class FindDuplicatesJob : IJob
	{
		public FindDuplicateOptions Options;

		public FindDuplicatesJob()
		{
			Options = new FindDuplicateOptions();
		}
	}
}
