using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class ZaryaDataTests : HeroDataBaseTest
    {
        [Fact]
        public void EnergyTests()
        {
            Assert.Equal(100, HeroZarya.Energy.EnergyMax);
            Assert.Equal("Energy", HeroZarya.Energy.EnergyType);
        }

        [Fact]
        public void AbilityTalentVitalNameOverrideEmptyTest()
        {
            Talent talent = HeroZarya.Talents["ZaryaPainIsTemporary"];
            Assert.True(string.IsNullOrEmpty(talent.Tooltip?.Energy?.EnergyTooltip?.RawDescription));
        }
    }
}
