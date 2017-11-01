using NUnit.Framework;
using System;
using FileUtil.Core;
using FileUtil.Models;
using FileUtil.Service.ServiceInterfaces;
using Moq;

namespace FileUtils
{
	[TestFixture()]
	public class DuplicateFindTests
	{
		//Todo mock fileshare, AppConfigs

		[Test()]
		public void Canary()
		{
			Assert.IsTrue(true);
		}

		[Test()]
		public void CanCallFindDuplicates()
		{
			string path = "testPath";
			FindDuplicateOptions options = new FindDuplicateOptions()
			{
				Domain = "",
				HashLimit = 0,
				IsLocalFileSystem = true,
				Pass = "",
				User = "",
				Path = @".\"
			};
			FindDuplicatesJob job = new FindDuplicatesJob(options);
			DuplicateFinder sut = new DuplicateFinder();
			sut.FindDuplicateFiles(job);
			
			Assert.IsTrue(true);
		}
	}
}
