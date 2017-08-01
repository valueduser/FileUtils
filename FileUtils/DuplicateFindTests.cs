using NUnit.Framework;
using System;
namespace FileUtils
{
    [TestFixture()]
    public class DuplicateFindTests
    {
        [Test()]
        public void Canary()
        {
            Assert.IsTrue(true);
        }

        [Test()]
        public void CanCallFindDuplicates()
        {
            string path = "testPath";
            var test = new DuplicateFind();
            test.FindDuplicates(path);

            Assert.IsTrue(true);
        }
    }
}
