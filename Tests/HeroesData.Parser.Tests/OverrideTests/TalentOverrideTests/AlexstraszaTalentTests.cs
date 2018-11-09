using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
{
    public class AlexstraszaTalentTests : OverrideBaseTests, ITalentOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaTalentTests()
            : base()
        {
            LoadOverrideIntoTestTalent(TalentName);
        }

        public string TalentName => "UpgradePuffPuff";

        protected override string CHeroId => Hero;

        [Fact]
        public void AbilityTypeOverrideTest()
        {
            Assert.Equal(AbilityType.Q, TestTalent.AbilityType);
        }
    }
}
