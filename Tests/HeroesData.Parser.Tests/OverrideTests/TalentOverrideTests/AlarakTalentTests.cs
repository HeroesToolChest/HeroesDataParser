using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
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
        public void AbilityTypeOverrideTest()
        {
            Assert.Equal(AbilityType.Z, TestTalent.AbilityType);
        }
    }
}
