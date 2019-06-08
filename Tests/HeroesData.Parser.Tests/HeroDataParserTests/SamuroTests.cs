using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class SamuroTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BasicAbilitiesTests()
        {
            Assert.IsTrue(HeroSamuro.ContainsAbility("SamuroMirrorImageTargeted"));
            Assert.IsTrue(HeroSamuro.ContainsAbility("SamuroCriticalStrikeDummy"));
            Assert.IsTrue(HeroSamuro.ContainsAbility("SamuroWindwalk"));
        }
    }
}
