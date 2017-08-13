using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtil.Service
{
	public class FindDuplicateOptions
	{
		public bool IsLocalFileSystem;
		public string User;
		public string Pass;
		public string Domain;
		public string Path;
		public int HashLimit;

		public FindDuplicateOptions(NameValueCollection appSettings)
		{
			Boolean.TryParse(appSettings["isLocalFileSystem"], out IsLocalFileSystem);
			User = appSettings["NetworkShareUser"] ?? "User Not Found";
			Pass = appSettings["NetworkSharePassword"] ?? "Password Not Found";
			Domain = appSettings["NetworkShareDomain"] ?? "Domain Not Found";
			Path = appSettings["NetworkShareUncPath"] ?? "UNC Path Not Found";
			Int32.TryParse(appSettings["NetworkShareUncPath"], out HashLimit);
		}
	}
}
