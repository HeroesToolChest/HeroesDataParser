using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ThrallTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroThrall.Talents["ThrallMasteryManaTide"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 1);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("ThrallFrostwolfResilience"));

            talent = HeroThrall.Talents["ThrallMasteryFrostwolfsGrace"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 1);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("ThrallFrostwolfResilience"));
        }
    }
}
