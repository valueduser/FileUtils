using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using FileUtil.Core;
using FileUtil.Models;
using FileUtil.Models.Interfaces;
using FileUtil.Service.ServiceInterfaces;
using Moq;
using File = FileUtil.Models.File;

namespace FileUtils
{
	[TestFixture()]
	public class DuplicateFinderTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test()]
		public void Canary()
		{
			Assert.IsTrue(true);
		}

		[Test()]
		public void CanCallFindDuplicates()
		{
			FindDuplicatesResult expectedResult = new FindDuplicatesResult();
			File expectedDuplicateFile = new File()
			{
				Filename = "BOOTNXT",
				SizeInMB = 626,
				FullPath = @"C:\BOOTNXT",
				Hash = "bbaaddbbeeeeff",
				Duplicates = new List<string>(),
				HashCollisions = new List<string>()
			};
			Dictionary<string, File> expectedDuplicates = new Dictionary<string, File>();
			expectedDuplicates.Add(expectedDuplicateFile.Hash, expectedDuplicateFile);
			expectedResult.Duplicates = expectedDuplicates;
			expectedResult.ReportOrderPreference = "Alphabetical";

			FindDuplicateOptions options = new FindDuplicateOptions()
			{
				Domain = "",
				HashLimit = 0,
				IsLocalFileSystem = true,
				Pass = "",
				User = "",
				Path = @".\"
			};
			FindDuplicatesJob testJob = new FindDuplicatesJob(options);

			string[] testFiles = new[] { @"C:\BOOTNXT" };

			Mock<IFileHelpers> fileHelpers = new Moq.Mock<IFileHelpers>();
			fileHelpers.Setup(a => a.GetFileName(It.IsAny<string>())).Returns("BOOTNXT");
			fileHelpers.Setup(a => a.GetFileSize(It.IsAny<string>())).Returns(626);
			fileHelpers.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(testFiles);

			byte[] testBytes = new byte[] { 0xBB, 0xAA, 0xDD, 0xBB, 0xEE, 0xEE, 0xFF };
			fileHelpers.Setup(h => h.HashFile(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(testBytes);
			fileHelpers.Setup(x => x.ToHex(It.IsAny<byte[]>(), It.IsAny<bool>())).Returns("bbaaddbbeeeeff");

			DuplicateFinder sut = new DuplicateFinder(fileHelpers.Object);
			FindDuplicatesResult actualResult = sut.FindDuplicates(testJob);
			Assert.IsTrue(expectedResult.Equals(actualResult));
		}
	}
}
