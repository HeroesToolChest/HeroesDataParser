using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HeroesData.Helpers.Tests
{
    [TestClass]
    public class PathExtensionsTests
    {
        [TestMethod]
        public void GetWindowsFilePathTest()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.AreEqual(@"test\to\filePath\", PathHelper.GetFilePath(@"test\to\filePath\"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Assert.AreEqual("test/to/filePath/", PathHelper.GetFilePath(@"test\to\filePath\"));
        }

        [TestMethod]
        public void FileNameToLowerTest()
        {
            string expectedName = Path.Combine("this", "is", "a", "path", "orphea_portrait.dds");
            string fileName = Path.Combine("this", "is", "a", "path", "Orphea_Portrait.dds");
            PathHelper.FileNameToLower(fileName.AsMemory());

            Assert.AreEqual(expectedName, fileName);
        }
    }
}
