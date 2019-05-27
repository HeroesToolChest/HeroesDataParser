using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class DVaMechPlacementDummyTests : UnitParserBaseTest
    {
        [TestMethod]
        public void HearthStoneAbilityTests()
        {
            Ability ability = DVaMechPlacementDummy.GetAbility("Hearthstone");

            Assert.AreEqual("Hearthstone", ability.Name);
            Assert.AreEqual("Hearthstone", ability.ShortTooltipNameId);
            Assert.AreEqual("Hearthstone", ability.FullTooltipNameId);
            Assert.AreEqual(AbilityType.B, ability.AbilityType);
        }

        [TestMethod]
        public void CaptureMacGuffinTwoShouldExistTest()
        {
            Assert.IsTrue(DVaMechPlacementDummy.ContainsAbility("CaptureMacGuffinTwo"));
        }
    }
}
