using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
