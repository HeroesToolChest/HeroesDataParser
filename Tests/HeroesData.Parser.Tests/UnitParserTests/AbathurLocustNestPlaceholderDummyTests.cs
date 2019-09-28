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
            Ability ability1 = AbathurLocustNestPlaceholderDummy.GetAbility(new AbilityTalentId("AbathurSpawnLocusts", "AbathurLocustStrain")
            {
                AbilityType = AbilityType.Q,
            });

            Assert.AreEqual(AbilityType.Q, ability1.AbilityTalentId.AbilityType);
            Assert.AreEqual(AbilityTier.Basic, ability1.Tier);
        }
    }
}
