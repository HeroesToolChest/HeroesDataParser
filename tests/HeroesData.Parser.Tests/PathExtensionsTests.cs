using System.Runtime.InteropServices;
using Xunit;

namespace HeroesData.Parser.Tests
{
    public class PathExtensionsTests
    {
        [Fact]
        public void GetWindowsFilePathTest()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Equal(@"test\to\filePath\", PathExtensions.GetFilePath(@"test\to\filePath\"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Assert.Equal("test/to/filePath/", PathExtensions.GetFilePath(@"test\to\filePath\"));
        }
    }
}
