using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class HanamuraMercDefenderSentinelTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("hanamura-MercDefenderSentinel", HanamuraMercDefenderSentinel.Id);
            Assert.AreEqual(6500, HanamuraMercDefenderSentinel.Life.LifeMax);
        }
    }
}
