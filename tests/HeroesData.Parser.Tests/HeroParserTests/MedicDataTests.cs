using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class MedicDataTests : HeroDataBaseTest
    {
        [Fact]
        public void AbilityEnergyTooltipTextTest()
        {
            Ability ability = HeroMedic.Abilities["MedicHealingBeam"];
            Assert.Equal("<s val=\"StandardTooltipDetails\">Energy: 6 per second</s>", ability.Tooltip.Energy?.EnergyTooltip.RawDescription);
        }

        [Fact]
        public void AbilityTalentShowUsageOffTest()
        {
            Talent talent = HeroMedic.Talents["MedicCellularReactor"];
            Assert.Equal("Cooldown: 45 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
            Assert.Equal("Consueme energy to heal", talent.Tooltip.FullTooltip.RawDescription);
        }
    }
}
