using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class AurielDataTests : HeroParserBaseTest
    {
        [Fact]
        public void EnergyTests()
        {
            Assert.Equal(475, HeroAuriel.Energy.EnergyMax);
            Assert.Equal("Stored Energy", HeroAuriel.Energy.EnergyType);
        }

        [Fact]
        public void LifeTests()
        {
            Assert.Equal(1700, HeroAuriel.Life.LifeMax);
            Assert.Equal(3.539, HeroAuriel.Life.LifeRegenerationRate);
        }

        [Fact]
        public void RolesTests()
        {
            Assert.Equal(1, HeroAuriel.Roles.Count);
            Assert.Equal(HeroRole.Support, HeroAuriel.Roles[0]);
        }

        [Fact]
        public void AbilityEnergyTooltipTextTest()
        {
            Ability ability = HeroAuriel.Abilities["AurielSacredSweep"];
            Assert.True(string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyText.RawDescription));
        }
    }
}
