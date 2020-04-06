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
                AbilityType = AbilityTypes.B,
            });

            Assert.AreEqual("Hearthstone", ability.Name);
            Assert.AreEqual(AbilityTypes.B, ability.AbilityTalentId.AbilityType);
        }

        [TestMethod]
        public void CaptureMacGuffinTwoShouldExistTest()
        {
            Assert.IsTrue(DVaMechPlacementDummy.ContainsAbility("CaptureMacGuffinTwo", StringComparison.OrdinalIgnoreCase));
        }
    }
}
