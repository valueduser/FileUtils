using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using FileUtil.Models;
using NUnit.Framework;
using Moq;

namespace FileUtil.Core.Nunit
{
	[TestFixture()]
	public class FileHelperTests
	{
		private string longText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus id porta justo. Aliquam quis mattis odio, feugiat imperdiet nisi. Cras quis bibendum ligula, non feugiat erat. Fusce sed condimentum ipsum. Quisque eleifend sed odio non tempor. Phasellus nec turpis nisi. Vivamus vel est nulla. Phasellus tristique neque consectetur neque placerat luctus. Sed interdum lacus eget dui laoreet, sit amet posuere nisl rhoncus. Donec vitae ipsum eu elit vehicula placerat. Etiam eget nunc a neque pulvinar lacinia et sit amet nibh. Sed ut libero vehicula, tincidunt eros eu, consequat ipsum. Etiam et venenatis odio, quis mattis lorem. Suspendisse quis tortor eu elit aliquet feugiat. Vivamus sem augue, malesuada id leo a, pellentesque aliquam sem. Donec eu ex nunc. Cras efficitur dapibus tempor. Pellentesque sed condimentum nulla. Nunc id enim nec mi vulputate venenatis quis quis risus. Ut fringilla aliquet urna at mattis. Pellentesque iaculis, urna ac egestas aliquet, lectus quam interdum enim, eget pharetra mauris leo sit amet libero. In pharetra nunc eget massa iaculis, non dictum leo tristique. Suspendisse pretium nunc rhoncus quam interdum, nec iaculis dui sollicitudin.Proin commodo felis vitae felis venenatis, quis pulvinar metus congue.Aliquam et felis dolor.Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Ut porta efficitur nunc, vel eleifend nunc ultrices et. Sed imperdiet ante at vestibulum consectetur. Aenean lobortis gravida sapien quis ultrices. Vestibulum sollicitudin ligula ex, et venenatis nisi pharetra eget. Donec pharetra massa arcu, quis varius dui dapibus ut. Aliquam non congue leo. Proin consectetur, ante et scelerisque dapibus, dui ante semper sapien, vitae faucibus magna arcu sit amet ex. Cras consequat dui vel posuere ullamcorper. Nullam elementum lorem in dui tempor, in imperdiet erat ornare.Nullam nec nunc in magna aliquet molestie ac quis tortor. In accumsan mattis massa nec scelerisque.Duis elementum, nibh quis pellentesque aliquet, purus elit gravida dui, quis iaculis odio quam vel sapien. Vestibulum pharetra lacinia nisl eget consequat. Nunc tempus tincidunt urna. Vestibulum lectus diam, vestibulum vel luctus vitae, congue non ex. Donec gravida egestas pulvinar. Maecenas enim est, convallis a augue quis, interdum pulvinar quam. Integer maximus, turpis in iaculis tincidunt, massa risus bibendum ante, in sagittis lectus ipsum ut magna. Nam a laoreet lorem. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Pellentesque tempor faucibus arcu, at sagittis orci fermentum a. Proin elementum nisl eu mi vestibulum pretium. In odio metus, condimentum eget congue feugiat, semper auctor augue. Vivamus tincidunt semper ex, et vehicula tellus scelerisque venenatis. Sed eu ligula ut nunc gravida consequat.";

		[SetUp]
		public void Setup()
		{

		}

		[Test()]
		public void WalkFilePaths_ReturnsExpectedArray()
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
		public void WalkFilePaths_ReturnsExpectedEmptyArray()
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

			string[] testFiles = new string[0];

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);

			filesystemMock.Setup(x => x.Directory.GetFiles(testJob.Options.Path, It.IsAny<string>(), It.IsAny<System.IO.SearchOption>())).Returns(testFiles);

			string[] actualResult = sut.WalkFilePaths(testJob);
			Assert.AreEqual(testFiles, actualResult);
		}

		[Test()]
		public void WalkFilePaths_ThrowsOnInvalidPath()
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
			string expectedInnerError = "unanticipated error";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);

			filesystemMock
				.Setup(x => x.Directory.GetFiles(testJob.Options.Path, It.IsAny<string>(),
					It.IsAny<System.IO.SearchOption>())).Throws(new Exception(expectedInnerError));

			try
			{
				sut.WalkFilePaths(testJob);
				Assert.Fail("If WalkFilePaths throws, there isn't anything we can do to recover. Should have thrown but did not.");
			}
			catch (Exception ex)
			{
				Assert.True(ex.Message.Contains(expectedInnerError));
				Assert.Pass();
			}

		}

		[Test]
		public void GetHashedValue_HandlesMissingFile()
		{
			string pathToFile = @"C:\windows-version.txt";
			long filesize = 2195L;
			string expectedResult = "";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			string result = sut.GetHashedValue(pathToFile, filesize);

			//System.IO.FileNotFoundException
			Assert.NotNull(result);
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void GetHashedValue_HandlesIncorrectFileSize()
		{
			string pathToFile = @"C:\windows-version.txt";
			long filesize = 2195L;
			string expectedResult = "80a751fde577028640c419000e33eba6";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			Stream mockStream = new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum"));
			filesystemMock.Setup(x => x.File.OpenRead(It.IsAny<string>())).Returns(mockStream);

			string result = sut.GetHashedValue(pathToFile, filesize);

			Assert.NotNull(result);
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public void GetHashedValue_HandlesValidInput()
		{
			string pathToFile = @"C:\test-file.txt";
			long filesize = 3L;
			string expectedResult = "80a751fde577028640c419000e33eba6";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			Stream mockStream = new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum"));
			filesystemMock.Setup(x => x.File.OpenRead(It.IsAny<string>())).Returns(mockStream);

			string result = sut.GetHashedValue(pathToFile, filesize);

			Assert.NotNull(result);
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void GetHashedValue_RespectsLimitOption()
		{
			string pathToFile = @"C:\test-file.txt";
			long filesize = 3L;
			long hashLimit = 1L;
			string expectedResult = "d20caec3b48a1eef164cb4ca81ba2587";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			Stream mockStream = new MemoryStream(Encoding.UTF8.GetBytes(longText));
			filesystemMock.Setup(x => x.FileStream.Create(It.IsAny<string>(), FileMode.Open)).Returns(mockStream);

			string result = sut.GetHashedValue(pathToFile, filesize, hashLimit);

			Assert.NotNull(result);
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void GetHashedValueLimited_HandlesExceptions()
		{
			string pathToFile = @"C:\test-file.txt";
			long filesize = 3L;
			long hashLimit = 1L;
			string expectedException = "surprise!";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			Stream mockStream = new MemoryStream(Encoding.UTF8.GetBytes(longText));
			filesystemMock.Setup(x => x.FileStream.Create(It.IsAny<string>(), FileMode.Open))
				.Throws(new Exception(expectedException));

			string result = sut.GetHashedValue(pathToFile, filesize, hashLimit);

			Assert.IsEmpty(result);
		}

		[Test]
		public void GetFileSize_ReturnsExpectedValue()
		{
			string pathToFile = @"C:\test-file.txt";
			long expectedResult = 30679615L;
			var mockedFileInfo = new Mock<IFileInfo>();
			mockedFileInfo.Setup(f => f.Length).Returns(31415926535L);
			
			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			filesystemMock.Setup(x => x.FileInfo.FromFileName(It.IsAny<string>()))
				.Returns(mockedFileInfo.Object);

			long result = sut.GetFileSize(pathToFile);

			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void GetFileSize_HandlesExceptions()
		{
			string pathToFile = @"C:\test-file.txt";
			long expectedResult = -1L;
			string expectedException = "surprise!";

			var mockedFileInfo = new Mock<IFileInfo>();
			mockedFileInfo.Setup(f => f.Length).Throws(new Exception(expectedException));

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			filesystemMock.Setup(x => x.FileInfo.FromFileName(It.IsAny<string>()))
				.Returns(mockedFileInfo.Object);

			long result = sut.GetFileSize(pathToFile);

			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void GetFileName_ReturnsExpectedValue()
		{
			string pathToFile = @"C:\test-file.txt";
			string fileName = "iamafilename";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			filesystemMock.Setup(x => x.Path.GetFileName(It.IsAny<string>()))
				.Returns(fileName);

			string result = sut.GetFileName(pathToFile);

			Assert.AreEqual(fileName, result);
		}

		[Test]
		public void GetFileName_HandlesExceptions()
		{
			string pathToFile = @"C:\test-file.txt";
			string expectedResult = "UNKNOWN_FILE";
			string expectedException = "surprise!";

			var filesystemMock = new Moq.Mock<IFileSystem>();
			FileHelpers sut = new FileHelpers(filesystemMock.Object);
			filesystemMock.Setup(x => x.Path.GetFileName(It.IsAny<string>()))
				.Throws(new Exception(expectedException));

			string result = sut.GetFileName(pathToFile);

			Assert.AreEqual(expectedResult, result);
		}
	}
}
