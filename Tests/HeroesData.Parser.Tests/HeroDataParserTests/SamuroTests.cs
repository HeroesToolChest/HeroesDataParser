using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class SamuroTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BasicAbilitiesTests()
        {
            Assert.IsTrue(HeroSamuro.ContainsAbility("SamuroMirrorImageTargeted", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroSamuro.ContainsAbility("SamuroCriticalStrikeDummy", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroSamuro.ContainsAbility("SamuroWindwalk", StringComparison.OrdinalIgnoreCase));
        }
    }
}
