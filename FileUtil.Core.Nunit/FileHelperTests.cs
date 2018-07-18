using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileUtil.Models;
using NUnit.Framework;
using Moq;

namespace FileUtil.Core.Nunit
{
	[TestFixture()]
	public class FileHelperTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test()]
		public void WalkFilePathsReturnsExpectedArray()
		{
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

			string[] testFiles = new[] {@"C:\BOOTNXT",
										@"C:\hiberfil.sys",
										@"C:\pagefile.sys",
										@"C:\windows-version.txt"};

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);

			filesystemMock.Setup(x => x.Directory.GetFiles(testJob.Options.Path, It.IsAny<string>(), It.IsAny<System.IO.SearchOption>())).Returns(testFiles);


			string[] actualResult = sut.WalkFilePaths(testJob);
			Assert.AreEqual(testFiles, actualResult);
		}

		[Test()]
		public void WalkFilePathsReturnsExpectedEmptyArray()
		{
			//todo
		}

		[Test()]
		public void WalkFilePathsThrowsOnInvalidPath()
		{
			//todo
		}

		[Test]
		public void GetHashedValueHandlesMissingFile()
		{
			string pathToFile = @"C:\windows-version.txt"; //todo
			long filesize = 2195L;

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			string result = sut.GetHashedValue(pathToFile, filesize);

			string expectedResult = "";
			//System.IO.FileNotFoundException
			Assert.NotNull(result);
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public void GetHashedValueHandlesIncorrectFilesize()
		{
			string pathToFile = @"C:\windows-version.txt";
			long filesize = 2195L; //todo

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			string result = sut.GetHashedValue(pathToFile, filesize);

			string expectedResult = "";

			Assert.NotNull(result);
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public void GetHashedValueHandlesValidInput()
		{
			string pathToFile = @"C:\windows-version.txt";
			CreateTestFile(pathToFile);
			long filesize = 2195L;

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			string result = sut.GetHashedValue(pathToFile, filesize);

			string expectedResult = "d41d8cd98f00b204e9800998ecf8427e";

			Assert.NotNull(result);
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public void GetHashedValueRespectsLimitOption()
		{
			string pathToFile = @"C:\windows-version.txt";
			long filesize = 2195L;
			long hashLimit = 555L;

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			string result = sut.GetHashedValue(pathToFile, filesize, hashLimit);

			string expectedResult = "";

			Assert.NotNull(result);
			Assert.AreEqual(result, expectedResult);
		}



		//string GetFileName(string pathToFile);
		//long GetFileSize(string pathToFile);

		private void CreateTestFile(string filename)
		{
			if (!System.IO.File.Exists(filename))
			{
				System.IO.File.Create(filename).Dispose();
			}
		}
	}
}
