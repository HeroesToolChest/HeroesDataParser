using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class DVaMechPlacementDummyTests : UnitParserBaseTest
    {
        [TestMethod]
        public void HearthStoneAbilityTests()
        {
            Ability ability = DVaMechPlacementDummy.GetAbility(new AbilityTalentId("Hearthstone", "Hearthstone")
            {
                AbilityType = AbilityType.B,
            });

            Assert.AreEqual("Hearthstone", ability.Name);
            Assert.AreEqual(AbilityType.B, ability.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void CaptureMacGuffinTwoShouldExistTest()
        {
            Assert.IsTrue(DVaMechPlacementDummy.ContainsAbility("CaptureMacGuffinTwo", StringComparison.OrdinalIgnoreCase));
        }
    }
}
