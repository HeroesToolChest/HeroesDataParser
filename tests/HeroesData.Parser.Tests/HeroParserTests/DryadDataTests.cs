using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class DryadDataTests : HeroParserBaseTest
    {
        [Fact]
        public void AbilityMountNoCooldownUntilTalentUpgradeTest()
        {
            Ability ability = HeroDryad.Abilities["DryadGallopingGait"];
            Assert.Empty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }

        [Fact]
        public void TalentCooldownTest()
        {
            Talent talent = HeroDryad.Talents["DryadGallopingGait"];
            Assert.Equal("Cooldown: 30 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
