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
            Assert.Equal(40, talent.Tooltip.Charges.CooldownValue);
        }

        [Fact]
        public void TalentCooldownTest()
        {
            Talent talent = HeroTestHero.Talents["TestHeroBattleRage"];
            Assert.Equal(10, talent.Tooltip.Cooldown.CooldownValue);
        }

        [Fact]
        public void AbilityTooltipOverrideTest()
        {
            Ability ability = HeroTestHero.Abilities["TestHeroNerazimDummy"];
            Assert.Equal("Nerazim v2", ability.Name);
            Assert.Equal("storm_ui_icon_testhero_nerazim.dds", ability.IconFileName);
            Assert.Equal("TestHeroNerazimTalent", ability.ShortTooltipNameId);
            Assert.Equal("TestHeroNerazimPassive", ability.FullTooltipNameId);
            Assert.Equal("Gain an extra ability", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Gain an extra ability three times", ability.Tooltip.FullTooltip.RawDescription);
        }
    }
}
