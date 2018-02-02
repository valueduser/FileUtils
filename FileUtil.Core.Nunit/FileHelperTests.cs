﻿using System;
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

        //private FileHelpers sut = new FileHelpers();

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

        [Test()]
        public void CanCallFindDuplicates()
        {
            //FindDuplicatesResult expectedResult = new FindDuplicatesResult();
            //var mockFileHelpers = new Mock<IFileHelpers>();

            // Doesn't seem to be mocking the call correctly
           // mockFileHelpers.Setup(f => f.WalkFilePaths(It.IsAny<FindDuplicatesJob>())).Returns(new string[] { });

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
           // FindDuplicatesResult actualResult = sut.FindDuplicates(mockJob.Object); //sut.FindDuplicateFiles(mockJob.Object);
          //  Assert.AreEqual(expectedResult, actualResult);
        }












    }
}
