using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace FileUtil.Models
{
	public class FindDuplicateOptions
	{
		public List<Source> Sources { get; set; }
		public Config Config { get; set; }

		public FindDuplicateOptions()
		{
			Sources = new List<Source>();
			Config = new Config();
			var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

			int numSources = 0;
			for (var i = 0; configuration[$"Sources:{i}:SourceName"] != null; i++)
			{
				numSources++;
				Source source = new Source()
				{
					Name = configuration[$"Sources:{i}:SourceName"],
					NetworkShareUser = configuration[$"Sources:{i}:NetworkShareUser"] ?? "User Not Found",
					NetworkSharePassword = configuration[$"Sources:{i}:NetworkSharePassword"] ?? "Password Not Found",
					NetworkShareDomain = configuration[$"Sources:{i}:NetworkShareDomain"] ?? "Domain Not Found",
					Path = configuration[$"Sources:{i}:Path"] ?? "Path Not Found"
				};
				bool isLocalFs = true;
				Boolean.TryParse(configuration[$"Sources:{i}:IsLocalFileSystem"], out isLocalFs);
				source.IsLocalFileSystem = isLocalFs;
				Sources.Add(source);
			}

			Int32.TryParse(configuration["HashSizeLimitInKB"], out int hashLimit);
			//TODO: Verify null config gets a default
			if (hashLimit != null && hashLimit > 0)
				Config.HashSizeLimitInKB = hashLimit;
			else Config.HashSizeLimitInKB  = 0;
		}
	}
}
