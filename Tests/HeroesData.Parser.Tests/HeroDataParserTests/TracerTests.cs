using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class TracerTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroTracer.GetTalent("TracerJumper");
            Assert.IsTrue(talent.AbilityTalentLinkIdsCount == 1);
            Assert.IsTrue(talent.ContainsAbilityTalentLinkId("TracerBlink"));
        }

        [TestMethod]
        public void TracerBlinkAbilityTest()
        {
            Assert.IsTrue(HeroTracer.ContainsAbility("TracerBlink", StringComparison.OrdinalIgnoreCase));
        }
    }
}
