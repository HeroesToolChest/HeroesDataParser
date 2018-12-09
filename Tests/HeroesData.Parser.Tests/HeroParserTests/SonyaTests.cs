using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class SonyaTests : HeroDataBaseTest
    {
        [Fact]
        public void AbilityTests()
        {
            Ability ability = HeroSonya.Abilities["BarbarianSeismicSlam"];
            Assert.Equal("<s val=\"StandardTooltipDetails\">Fury: 25</s>", ability.Tooltip.Energy.EnergyTooltip.RawDescription);
        }
    }
}
