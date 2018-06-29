using Xunit;

namespace HeroesData.Parser.Tests.Overrides.PortraitOverrideTests
{
    public class AlarakPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakPortraitTests()
            : base()
        {
            LoadOverrideIntoTestPortrait(CHeroIdName);
        }

        public string CHeroIdName => Hero;

        protected override string CHeroId => Hero;

        [Fact]
        public void HeroSelectPortraitOverrideTest()
        {
            Assert.Equal("Boom.dds", TestPortrait.HeroSelectPortraitFileName);
        }

        [Fact]
        public void LeaderboardPortraitOverrideTest()
        {
            Assert.Equal("Zoom.dds", TestPortrait.LeaderboardPortraitFileName);
        }

        [Fact]
        public void LoadingScreenPortraitOverrideTest()
        {
            Assert.Equal("someOtherImage", TestPortrait.LoadingScreenPortraitFileName);
        }

        [Fact]
        public void PartyPanelPortraitOverrideTest()
        {
            Assert.Null(TestPortrait.PartyPanelPortraitFileName);
        }

        [Fact]
        public void TargetPortraitOverrideTest()
        {
            Assert.Null(TestPortrait.TargetPortraitFileName);
        }
    }
}
