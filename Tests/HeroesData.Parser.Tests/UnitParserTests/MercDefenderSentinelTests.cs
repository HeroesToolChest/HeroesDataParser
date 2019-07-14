using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class MercDefenderSentinelTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("MercDefenderSentinel", MercDefenderSentinel.Id);
            Assert.AreEqual(8000, MercDefenderSentinel.Life.LifeMax);
        }
    }
}
