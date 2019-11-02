using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class VarianTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTests()
        {
            Ability ability = HeroVarian.GetAbility(new AbilityTalentId("VarianHeroicStrike", "VarianHeroicStrike")
            {
                AbilityType = AbilityType.Trait,
            });
            Assert.IsFalse(ability.IsActive);
        }
    }
}
