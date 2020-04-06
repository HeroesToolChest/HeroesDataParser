using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AlarakTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void MockingStrikesTalentIsPassiveTest()
        {
            Talent talent = HeroAlarak.GetTalent("AlarakMockingStrikes");
            Assert.IsTrue(HeroAlarak.ContainsTalent("AlarakMockingStrikes"));
            Assert.AreEqual(AbilityTypes.Passive, talent.AbilityTalentId.AbilityType);
            Assert.AreEqual(3, talent.AbilityTalentLinkIds.Count);
            Assert.AreEqual("AlarakCounterStrikeTargeted2ndHeroic", talent.AbilityTalentLinkIds.ToList()[1]);
        }

        [TestMethod]
        public void AlarakSustainingPowerIconTest()
        {
            Assert.IsTrue(HeroAlarak.ContainsTalent("AlarakSustainingPower"));
            Assert.AreEqual("storm_ui_icon_alarak_lightningsurge_a.dds", HeroAlarak.GetTalent("AlarakSustainingPower").IconFileName);
        }
    }
}
