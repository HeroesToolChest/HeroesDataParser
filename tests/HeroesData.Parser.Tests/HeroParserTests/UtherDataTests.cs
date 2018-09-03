using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class UtherDataTests : HeroParserBaseTest
    {
        [Fact]
        public void TalentCooldownTextOverrideShowUsageOff()
        {
            Talent talent = HeroUther.Talents["UtherMasteryBenediction"];
            Assert.Equal("Cooldown: 60 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }
    }
}
