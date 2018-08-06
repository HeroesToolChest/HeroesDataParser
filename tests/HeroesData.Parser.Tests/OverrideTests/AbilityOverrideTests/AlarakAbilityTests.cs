using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    public class AlarakAbilityTests : OverrideBaseTests, IAbilityOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakAbilityTests()
            : base()
        {
            LoadOverrideIntoTestAbility(AbilityName);
        }

        public string AbilityName => "ZapiptyZap";

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
        public void AbilityTypeOverrideTest()
        {
            Assert.Equal(AbilityType.Q, TestAbility.AbilityType);
        }
    }
}
