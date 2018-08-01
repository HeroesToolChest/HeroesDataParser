using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    public class AbathurTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradeSpikeAbilityThingy";

        protected override string CHeroId => Hero;

        [Fact]
        public void TooltipCustomOverrideTest()
        {
            Assert.Equal("Zip Zap", TestTalent.Tooltip.Custom);
        }

        [Fact]
        public void TooltipCooldownValueOverrideTest()
        {
            Assert.Equal(3, TestTalent.Tooltip.Cooldown.CooldownValue);
        }

        [Fact]
        public void TooltipEnergyCostOverrideTest()
        {
            Assert.Equal(153, TestTalent.Tooltip.Energy.EnergyCost);
        }

        [Fact]
        public void TooltipEnergyPerCostOverrideTest()
        {
            Assert.True(TestTalent.Tooltip.Energy.IsPerCost);
        }

        [Fact]
        public void TooltipIsLifePercentageOverrideTest()
        {
            Assert.True(TestTalent.Tooltip.Life.IsLifePercentage);
        }

        [Fact]
        public void TooltipLifeCostOverrideTest()
        {
            Assert.Equal(150, TestTalent.Tooltip.Life.LifeCost);
        }
    }
}
