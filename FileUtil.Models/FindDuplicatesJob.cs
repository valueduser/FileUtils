using System.Collections.Specialized;
using FileUtil.Models.Interfaces;

namespace FileUtil.Models
{
	public class FindDuplicatesJob: IJob
	{
		public string Path;
		public string[] FilesArr;
		public FindDuplicateOptions Options;

		public FindDuplicatesJob(NameValueCollection appSettings)
		{
			Options = new FindDuplicateOptions(appSettings);
			Path = Options.Path;
		}

		public FindDuplicatesJob(FindDuplicateOptions options)
		{
			Options = options;
			Path = options.Path;
		}
	}
}
