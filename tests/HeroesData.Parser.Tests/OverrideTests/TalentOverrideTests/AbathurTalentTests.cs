using Heroes.Models.AbilityTalents;
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
        public void AbilityTypeOverrideTest()
        {
            Assert.Equal(AbilityType.W, TestTalent.AbilityType);
        }
    }
}
