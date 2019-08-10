using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AbathurLocustNestPlaceholderDummyTests : UnitParserBaseTest
    {
        [TestMethod]
        public void AbilitiesTests()
        {
            Ability ability1 = AbathurLocustNestPlaceholderDummy.GetFirstAbility("AbathurSpawnLocusts");

            Assert.AreEqual(AbilityType.Q, ability1.AbilityType);
            Assert.AreEqual(AbilityTier.Basic, ability1.Tier);
        }
    }
}
