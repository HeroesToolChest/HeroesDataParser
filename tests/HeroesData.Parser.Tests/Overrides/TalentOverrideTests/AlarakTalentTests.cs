using Xunit;

namespace HeroesData.Parser.Tests.Overrides.TalentOverrideTests
{
    public class AlarakTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradeZapiptyZap";

        protected override string CHeroId => Hero;

        [Fact]
        public void TooltipCustomOverrideTest()
        {
            Assert.Null(TestTalent.Tooltip.Custom);
        }

        [Fact]
        public void TooltipCooldownValueOverrideTest()
        {
            Assert.Null(TestTalent.Tooltip.Cooldown.CooldownValue);
        }

        [Fact]
        public void TooltipEnergyCostOverrideTest()
        {
            Assert.Equal(500, TestTalent.Tooltip.Energy.EnergyCost);
        }

        [Fact]
        public void TooltipEnergyPerCostOverrideTest()
        {
            Assert.False(TestTalent.Tooltip.Energy.IsPerCost);
        }

        [Fact]
        public void TooltipIsLifePercentageOverrideTest()
        {
            Assert.False(TestTalent.Tooltip.Life.IsLifePercentage);
        }

        [Fact]
        public void TooltipLifeCostOverrideTest()
        {
            Assert.Null(TestTalent.Tooltip.Life.LifeCost);
        }
    }
}
