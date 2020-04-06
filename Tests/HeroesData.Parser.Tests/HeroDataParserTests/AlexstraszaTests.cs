using Heroes.Models.AbilityTalents;
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

        [TestMethod]
        public void TalentAbilityLinkIdsTest()
        {
            Talent talent = HeroAlexstrasza.GetTalent("AlexstraszaCleansingFlame");
            Assert.AreEqual(2, talent.AbilityTalentLinkIds.Count);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AlexstraszaCleansingFlame"));
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("AlexstraszaCleansingFlameDragonqueen"));
        }
    }
}
