using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class TestHeroDataTests : HeroParserBaseTest
    {
        [Fact]
        public void AbilityNameOverrideTest()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroBigBoom"];
            Assert.Equal("Big Boomy Boom", ability.Name);
        }

        [Fact]
        public void TalentChargesTest()
        {
            Talent talent = HeroTestHero.Talents["TestHeroBattleRage"];
            Assert.Equal(3, talent.Tooltip.Charges.CountMax);
            Assert.Equal(1, talent.Tooltip.Charges.CountUse);
            Assert.Null(talent.Tooltip.Charges.CountStart);
        }

        [Fact]
        public void TalentCooldownTest()
        {
            Talent talent = HeroTestHero.Talents["TestHeroBattleRage"];
            Assert.Equal(40, talent.Tooltip.Cooldown.CooldownValue);
        }
    }
}
