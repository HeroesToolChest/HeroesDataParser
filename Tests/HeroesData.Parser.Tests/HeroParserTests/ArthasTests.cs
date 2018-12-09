using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class ArthasTests : HeroDataBaseTest
    {
        [Fact]
        public void BasicPropertiesTests()
        {
            Assert.Equal(2782, HeroArthas.Life.LifeMax);
        }

        [Fact]
        public void AbilityTests()
        {
            Ability ability = HeroArthas.Abilities["ArthasDeathCoil"];
            Assert.True(!string.IsNullOrEmpty(ability.Tooltip?.FullTooltip?.RawDescription));
            Assert.True(!string.IsNullOrEmpty(ability.Tooltip?.ShortTooltip?.RawDescription));
        }

        [Fact]
        public void TalentTests()
        {
            Talent talent = HeroArthas.Talents["ArthasAntiMagicShell"];
            Assert.True(!string.IsNullOrEmpty(talent.Tooltip?.FullTooltip?.RawDescription));
            Assert.True(!string.IsNullOrEmpty(talent.Tooltip?.ShortTooltip?.RawDescription));
        }
    }
}
