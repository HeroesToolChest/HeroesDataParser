using Xunit;

namespace HeroesData.Parser.Tests.Overrides.PortraitOverrideTests
{
    public class AlexstraszaPortraitTests : OverrideBaseTests, IPortraitOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaPortraitTests()
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
            Assert.Equal("StraightToTheMoon.dds", TestPortrait.LoadingScreenPortraitFileName);
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
