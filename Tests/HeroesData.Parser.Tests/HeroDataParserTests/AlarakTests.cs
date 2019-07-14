using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AlarakTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void MockingStrikesTalentIsPassiveTest()
        {
            Assert.IsTrue(HeroAlarak.ContainsTalent("AlarakMockingStrikes"));
            Assert.AreEqual(AbilityType.Passive, HeroAlarak.GetTalent("AlarakMockingStrikes").AbilityType);
        }

        [TestMethod]
        public void AlarakSustainingPowerIconTest()
        {
            Assert.IsTrue(HeroAlarak.ContainsTalent("AlarakSustainingPower"));
            Assert.AreEqual("storm_ui_icon_alarak_lightningsurge_a.dds", HeroAlarak.GetTalent("AlarakSustainingPower").IconFileName);
        }
    }
}
