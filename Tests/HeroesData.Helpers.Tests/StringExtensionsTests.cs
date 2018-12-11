using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Helpers.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void StringsReplaceOnlyFirstTest()
        {
            string test1 = "0Zero";
            Assert.AreEqual("ZeroZero", test1.ReplaceFirst("0", "Zero"));

            string test2 = "0Two0";
            Assert.AreEqual("ZeroTwo0", test2.ReplaceFirst("0", "Zero"));

            string test3 = "xyz000";
            Assert.AreEqual("xyzZero00", test3.ReplaceFirst("0", "Zero"));

            string test4 = "cat";
            Assert.AreEqual("cat", test4.ReplaceFirst("0", "Zero"));
        }
    }
}
