using Xunit;

namespace HeroesData.Parser.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void StringsReplaceOnlyFirstTest()
        {
            string test1 = "0Zero";
            Assert.Equal("ZeroZero", test1.ReplaceFirst("0", "Zero"));

            string test2 = "0Two0";
            Assert.Equal("ZeroTwo0", test2.ReplaceFirst("0", "Zero"));

            string test3 = "xyz000";
            Assert.Equal("xyzZero00", test3.ReplaceFirst("0", "Zero"));

            string test4 = "cat";
            Assert.Equal("cat", test4.ReplaceFirst("0", "Zero"));
        }
    }
}
