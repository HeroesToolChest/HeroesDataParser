using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    public class AlexstraszaAbilityTests : OverrideBaseTests, IAbilityOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaAbilityTests()
            : base()
        {
            LoadOverrideIntoTestAbility(AbilityName);
        }

        public string AbilityName => "PuffPuff";

        protected override string CHeroId => Hero;

        [Fact]
        public void ParentLinkOverrideTest()
        {
            Assert.Null(TestAbility.ParentLink);
        }

        [Fact]
        public void AbilityTierOverrideTest()
        {
            Assert.Equal(AbilityTier.Basic, TestAbility.Tier);
        }

        [Fact]
        public void TooltipCustomOverrideTest()
        {
            Assert.Null(TestAbility.Tooltip.Custom);
        }

        [Fact]
        public void TooltipEnergyCostOverrideTest()
        {
            Assert.Null(TestAbility.Tooltip.Energy.EnergyCost);
        }

        [Fact]
        public void TooltipEnergyPerCostOverrideTest()
        {
            Assert.False(TestAbility.Tooltip.Energy.IsPerCost);
        }

        [Fact]
        public void TooltipCooldownValueOverrideTest()
        {
            Assert.Null(TestAbility.Tooltip.Cooldown.CooldownValue);
        }

        [Fact]
        public void TooltipLifeCostOverrideTest()
        {
            Assert.Equal(10, TestAbility.Tooltip.Life.LifeCost);
        }

        [Fact]
        public void TooltipIsLifePercentageOverrideTest()
        {
            Assert.False(TestAbility.Tooltip.Life.IsLifePercentage);
        }
    }
}
