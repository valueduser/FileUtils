using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtil.Service
{
	public class FindDuplicatesJob
	{
		public string Path;
		public string[] filesArr;
		public FindDuplicateOptions Options;
		private NameValueCollection _appSettings;

		public FindDuplicatesJob(NameValueCollection appSettings)
		{
			this._appSettings = appSettings;
			Options = new FindDuplicateOptions(appSettings);
		}


	}
}
