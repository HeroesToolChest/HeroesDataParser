using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AlexstraszaTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void HeroUnitLifeTest()
        {
            Assert.AreEqual(1698, HeroAlexstrasza.Life.LifeMax);
            Assert.AreEqual("Health", HeroAlexstrasza.Life.LifeType);
        }
    }
}
