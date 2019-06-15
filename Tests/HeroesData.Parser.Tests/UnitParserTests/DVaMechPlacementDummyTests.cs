using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class DVaMechPlacementDummyTests : UnitParserBaseTest
    {
        [TestMethod]
        public void HearthStoneAbilityTests()
        {
            Ability ability = DVaMechPlacementDummy.GetAbilities("Hearthstone").First();

            Assert.AreEqual("Hearthstone", ability.Name);
            Assert.AreEqual(AbilityType.B, ability.AbilityType);
        }

        [TestMethod]
        public void CaptureMacGuffinTwoShouldExistTest()
        {
            Assert.IsTrue(DVaMechPlacementDummy.ContainsAbility("CaptureMacGuffinTwo"));
        }
    }
}
