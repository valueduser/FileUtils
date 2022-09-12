using System;
using System.Collections.Concurrent;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using FileUtil.Models;
using Moq;
using File = FileUtil.Models.File;

namespace FileUtil.Core.Nunit
{
	[TestFixture()]
	public class DuplicateFinderTests
	{
		//ConcurrentDictionary<string, List<File>> expectedDuplicateFiles = new ConcurrentDictionary<string, List<File>>();
		//private FindDuplicateOptions _options;
		//private FindDuplicatesJob _testJob;

		[SetUp]
		public void Setup()
		{
			//_options = new FindDuplicateOptions()
			//{
			//	Domain = "",
			//	HashLimit = 0,
			//	IsLocalFileSystem = true,
			//	Pass = "",
			//	User = "",
			//	Path = @".\"
			//};
			//_testJob = new FindDuplicatesJob(_options);
		}

		[Test()]
		public void FindDuplicateFiles_GoldFlow()
		{
			//string fileHash = "bbaaddbbeeeeff";
			//long filesize = 626;
			//string filename = "BOOTNXT";
			//int expectedNumberOfResults = 1;

			//File expectedDuplicateFile = new File()
			//{
			//	Filename = filename,
			//	SizeInKiloBytes = filesize,
			//	FullPath = $@"C:\{filename}",
			//	Hash = fileHash,
			//};

			

			//string[] mockedFiles = new[] { $@"C:\{filename}" };

			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//fileHelperMock.Setup(a => a.GetFileName(It.IsAny<string>())).Returns(filename);
			//fileHelperMock.Setup(a => a.GetFileSize(It.IsAny<string>())).Returns(filesize);
			//fileHelperMock.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(mockedFiles);
			//fileHelperMock.Setup(g => g.GetHashedValue(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(fileHash);

			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//ConcurrentDictionary<string, List<File>> results = sut.FindDuplicateFiles(_testJob);

			//Assert.NotNull(results);
			//Assert.AreEqual(expectedNumberOfResults, results.Count);
			//Assert.AreEqual(results.FirstOrDefault().Value[0], expectedDuplicateFile);
		}

		[Test()]
		public void FindDuplicateFiles_IgnoresPlaceholderFiles()
		{
			//string fileHash = "bbaaddbbeeeeff";
			//long filesize = 1;
			//string filename = "_._";
			//int expectedNumberOfResults = 0;

			//string[] mockedFiles = new[] { $@"C:\{filename}" };

			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//fileHelperMock.Setup(a => a.GetFileName(It.IsAny<string>())).Returns(filename);
			//fileHelperMock.Setup(a => a.GetFileSize(It.IsAny<string>())).Returns(filesize);
			//fileHelperMock.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(mockedFiles);
			//fileHelperMock.Setup(g => g.GetHashedValue(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(fileHash);

			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//ConcurrentDictionary<string, List<File>> results = sut.FindDuplicateFiles(_testJob);

			//Assert.NotNull(results);
			//Assert.AreEqual(expectedNumberOfResults, results.Count);
		}

		[Test()]
		public void FindDuplicateFiles_IgnoresEmptyFilenames()
		{
			//string fileHash = "bbaaddbbeeeeff";
			//long filesize = 1;
			//string filename = "somefilename";
			//int expectedNumberOfResults = 0;

			//string[] mockedFiles = new[] { $@"" };

			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//fileHelperMock.Setup(a => a.GetFileName(It.IsAny<string>())).Returns(filename);
			//fileHelperMock.Setup(a => a.GetFileSize(It.IsAny<string>())).Returns(filesize);
			//fileHelperMock.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(mockedFiles);
			//fileHelperMock.Setup(g => g.GetHashedValue(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>())).Returns(fileHash);

			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//ConcurrentDictionary<string, List<File>> results = sut.FindDuplicateFiles(_testJob);

			//Assert.NotNull(results);
			//Assert.AreEqual(expectedNumberOfResults, results.Count);
		}

		[Test()]
		public void FindDuplicateFiles_GoldFlowManyFiles()
		{
			//int expectedNumberOfResults = 2;

			//Mock<IFileHelpers> fileHelperMock = GenerateMockFileMetaData(10);

			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//ConcurrentDictionary<string, List<File>> results = sut.FindDuplicateFiles(_testJob);

			//Assert.NotNull(results);
			//Assert.AreEqual(expectedNumberOfResults, results.Count);
			
			//Assert.AreEqual(expectedDuplicateFiles, results);
		}

		[Test()]
		public void ValidateJob_ThrowsExceptionBlankUser()
		{
			//FindDuplicateOptions options = new FindDuplicateOptions()
			//{
			//	Domain = "",
			//	HashLimit = 0,
			//	IsLocalFileSystem = false,
			//	Pass = "asdf",
			//	User = "",
			//	Path = @".\"
			//};
			//FindDuplicatesJob testJob = new FindDuplicatesJob(options);
			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//try
			//{
			//	sut.ValidateJob(testJob);
			//	Assert.Fail("Should have thrown an argument exception");

			//}
			//catch (ArgumentException ex)
			//{
			//	Assert.Pass();
			//}
		}

		[Test()]
		public void ValidateJob_ThrowsExceptionBlankPass()
		{
			//string expectedInnerError = "Remote Filesystem selected but credentials were invalid.";
			//FindDuplicateOptions options = new FindDuplicateOptions()
			//{
			//	Domain = "",
			//	HashLimit = 0,
			//	IsLocalFileSystem = false,
			//	Pass = "",
			//	User = "admin",
			//	Path = @".\"
			//};
			//FindDuplicatesJob testJob = new FindDuplicatesJob(options);
			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//try
			//{
			//	sut.ValidateJob(testJob);
			//	Assert.Fail("Should have thrown an argument exception");

			//}
			//catch (ArgumentException ex)
			//{
			//	Assert.True(ex.Message.Contains(expectedInnerError));
			//	Assert.Pass();
			//}
		}

		[Test()]
		public void ValidateJob_IgnoresUserPassLocal()
		{
			//FindDuplicateOptions options = new FindDuplicateOptions()
			//{
			//	Domain = "",
			//	HashLimit = 0,
			//	IsLocalFileSystem = true,
			//	Pass = "",
			//	User = "",
			//	Path = @".\"
			//};
			//FindDuplicatesJob testJob = new FindDuplicatesJob(options);
			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//DuplicateFinder sut = new DuplicateFinder(fileHelperMock.Object);

			//sut.ValidateJob(testJob);
		}

		//TODO: ReportResults

		#region TestHelpers

		//public Mock<IFileHelpers> GenerateMockFileMetaData(int quantity)
		//{
			//string fileNamePrefix = "SuperTest_";
			//string fileExtension = "mp3";
			//Mock<IFileHelpers> fileHelperMock = new Moq.Mock<IFileHelpers>();
			//string[] mockedFileList = new string[quantity];

			//for (int i = 0; i < quantity; i++)
			//{
			//	string filename = $"{fileNamePrefix}{i}.{fileExtension}";
			//	mockedFileList[i] = $@"C:\{filename}";
			//	string fileHash = "hashValue" + i % 2;
			//	fileHelperMock.Setup(a => a.GetFileName(mockedFileList[i])).Returns(filename);
			//	fileHelperMock.Setup(a => a.GetFileSize(mockedFileList[i])).Returns(i);
			//	fileHelperMock.Setup(g => g.GetHashedValue(mockedFileList[i], It.IsAny<long>(), It.IsAny<long>())).Returns(fileHash);
			//	PopulatedExpectedDuplicateFiles(filename, i, fileHash);
			//}
			//fileHelperMock.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(mockedFileList);
			
			//return fileHelperMock;
		//}

		private void PopulatedExpectedDuplicateFiles(string filename, long filesize, string fileHash)
		{
			//File tempFile = new File()
			//{
			//	Name = filename,
			//	SizeInKiloBytes = filesize,
			//	Path = $@"C:\{filename}",
			//	Hash = fileHash,
			//};

			//expectedDuplicateFiles.AddOrUpdate(fileHash, new List<File>() {tempFile},
			//	(key, value) => { value.Add(tempFile); return value; });
		}

		#endregion
	}
}
