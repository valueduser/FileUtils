using System;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;

namespace FileUtil.Models
{
	public class FindDuplicateOptions
	{
		public Source[] Sources { get; set; }
		public Config Config { get; set; }

		public bool IsLocalFileSystem { get; set; }
		public string User { get; set; }
		public string Pass { get; set; }
		public string Domain { get; set; }
		public string Path { get; set; }
		public int HashLimit { get; set; }

		public FindDuplicateOptions()
		{
			Sources = new Source[0];
			Config = new Config();
			Config.HashSizeLimitInKB = 0;
			//IsLocalFileSystem = true;
			//User = "";
			//Pass = "";
			//Domain = "";
			//Path = "";
			//HashLimit = 0;
		}

		public FindDuplicateOptions(NameValueCollection appSettings)
		{
			var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

			var test = configuration["Sources"];


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
