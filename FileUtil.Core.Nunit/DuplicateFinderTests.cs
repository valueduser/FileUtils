using NUnit.Framework;
using System;
using FileUtil.Core;
using FileUtil.Models;
using FileUtil.Models.Interfaces;
using FileUtil.Service.ServiceInterfaces;
using Moq;

namespace FileUtils
{
	[TestFixture()]
	public class DuplicateFinderTests
	{
        //Todo mock fileshare, AppConfigs

	    private static FindDuplicateOptions options = new FindDuplicateOptions()
	    {
	        Domain = "",
	        HashLimit = 0,
	        IsLocalFileSystem = true,
	        Pass = "",
	        User = "",
	        Path = @".\"
	    };
	    Mock<FindDuplicatesJob> mockJob = new Mock<FindDuplicatesJob>(options);
        //FindDuplicatesJob(FindDuplicateOptions options)


        private DuplicateFinder sut = new DuplicateFinder();

        [SetUp]
	    public void Setup()
	    {
	        //DuplicateFinder sut = new DuplicateFinder(); //todo mock
            
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
            var mockFileHelpers = new Mock<IFileHelpers>();

            // Doesn't seem to be mocking the call correctly
            mockFileHelpers.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(new string[] {});

            //string path = "testPath";
            //FindDuplicateOptions options = new FindDuplicateOptions()
            //{
            //	Domain = "",
            //	HashLimit = 0,
            //	IsLocalFileSystem = true,
            //	Pass = "",
            //	User = "",
            //	Path = @".\"
            //};
            //FindDuplicatesJob job = new FindDuplicatesJob(options); // todo mock
            //sut.FindDuplicateFiles(job);

            //Assert.IsTrue(true);
		    FindDuplicatesResult actualResult = sut.FindDuplicates(mockJob.Object); //sut.FindDuplicateFiles(mockJob.Object);
            Assert.AreEqual(expectedResult, actualResult);
		}

        //public FindDuplicatesResult FindDuplicates(FindDuplicatesJob job)
    }
}
