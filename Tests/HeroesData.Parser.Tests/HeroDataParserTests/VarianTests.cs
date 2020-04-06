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
                AbilityType = AbilityTypes.Trait,
            });
            Assert.IsFalse(ability.IsActive);
        }

        [TestMethod]
        public void AbilityShortTooltipTest()
        {
            Ability ability = HeroVarian.GetAbility(new AbilityTalentId("VarianColossusSmash", "VarianColossusSmash")
            {
                AbilityType = AbilityTypes.Heroic,
            });

            Assert.AreEqual("Gain Damage, Lose Health<n/>Smash enemies and lower their Armor", ability.Tooltip.ShortTooltip!.RawDescription);
        }
    }
}
