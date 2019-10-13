using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ThrallTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroThrall.GetTalent("ThrallMasteryManaTide");
            Assert.IsTrue(talent.AbilityTalentLinkIdsCount == 2);
            Assert.IsTrue(talent.ContainsAbilityTalentLinkId("ThrallFrostwolfResilience"));
            Assert.AreEqual(AbilityType.Trait, talent.AbilityTalentId.AbilityType);

            talent = HeroThrall.GetTalent("ThrallMasteryFrostwolfsGrace");
            Assert.IsTrue(talent.AbilityTalentLinkIdsCount == 2);
            Assert.IsTrue(talent.ContainsAbilityTalentLinkId("ThrallFrostwolfResilience"));
            Assert.AreEqual(AbilityType.Trait, talent.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void AbilitiesListTests()
        {
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallSundering", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallEarthquake", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallChainLightning", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallFeralSpirit", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallWindfury", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallFrostwolfResilience", StringComparison.OrdinalIgnoreCase));

            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallCancelSundering", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(HeroThrall.ContainsAbility("ThrallFrostwolfsGrace", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ThrallMasteryGraceOfAirTalentAbilityTypeTest()
        {
            Talent talent = HeroThrall.GetTalent("ThrallMasteryGraceOfAir");
            Assert.AreEqual(AbilityType.E, talent.AbilityTalentId.AbilityType);
        }
    }
}
