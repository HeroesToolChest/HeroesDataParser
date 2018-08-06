using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    public class AbathurAbilityTests : OverrideBaseTests, IAbilityOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurAbilityTests()
            : base()
        {
            LoadOverrideIntoTestAbility(AbilityName);
        }

        public string AbilityName => "SpikeAbilityThingy";

        protected override string CHeroId => Hero;

        [Fact]
        public void ParentLinkOverrideTest()
        {
            Assert.Equal("Symbiote", TestAbility.ParentLink);
        }

        [Fact]
        public void AbilityTierOverrideTest()
        {
            Assert.Equal(AbilityTier.Activable, TestAbility.Tier);
        }

        [Fact]
        public void TooltipIsLifePercentageOverrideTest()
        {
            Assert.False(TestAbility.Tooltip.Life.IsLifePercentage);
        }

        [Fact]
        public void AbilityTypeOverrideTest()
        {
            Assert.Equal(AbilityType.W, TestAbility.AbilityType);
        }
    }
}
