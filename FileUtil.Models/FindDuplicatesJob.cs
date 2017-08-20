using System.Collections.Specialized;

namespace FileUtil.Models
{
	public class FindDuplicatesJob
	{
		public string Path;
		public string[] FilesArr;
		public FindDuplicateOptions Options;
		private NameValueCollection _appSettings;

		public FindDuplicatesJob(NameValueCollection appSettings)
		{
			this._appSettings = appSettings;
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
