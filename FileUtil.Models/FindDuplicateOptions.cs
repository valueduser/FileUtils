﻿using System;
using System.Collections.Specialized;

namespace FileUtil.Models
{
	public class FindDuplicateOptions
	{
		public bool IsLocalFileSystem { get; set; }
		public string User { get; set; }
		public string Pass { get; set; }
		public string Domain { get; set; }
		public string Path { get; set; }
		public int HashLimit { get; set; }

		public FindDuplicateOptions()
		{
			IsLocalFileSystem = true;
			User = "";
			Pass = "";
			Domain = "";
			Path = "";
			HashLimit = 0;
		}

		public FindDuplicateOptions(NameValueCollection appSettings)
		{
			Boolean.TryParse(appSettings["isLocalFileSystem"], out bool isLocalFileSystem);
			IsLocalFileSystem = isLocalFileSystem;

			User = appSettings["NetworkShareUser"] ?? "User Not Found";
			Pass = appSettings["NetworkSharePassword"] ?? "Password Not Found";
			Domain = appSettings["NetworkShareDomain"] ?? "Domain Not Found";
			Path = appSettings["NetworkShareUncPath"] ?? "UNC Path Not Found";

			Int32.TryParse(appSettings["HashSizeLimitInKB"], out int hashLimit);
			if(hashLimit != null && hashLimit > 0)
				HashLimit = hashLimit;
		}
	}
}
